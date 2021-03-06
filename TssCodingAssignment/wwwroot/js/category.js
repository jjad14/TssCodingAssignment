var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Category/Upsert/${data}" class="btn btn-success text-white" style="cursor: pointer;">
                                <i class="fas fa-edit"></i>
                            </a>    
                            <a onclick=Delete("/Admin/Category/Delete/${data}") class="btn btn-danger text-white" style="cursor: pointer;">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                        `;
                }, "width": "40%"
            }
        ]
    });
}

function Delete(url) {

    toastr.options = {
        "debug": false,
        "positionClass": "toast-bottom-right",
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 1000,
        "timeOut": 5000,
        "extendedTimeOut": 1000
    }

    Swal.fire({
        title: 'Are you sure you want to delete this category?',
        text: "This action is permanent.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    })










    //    .then((willDelete) => {
    //    if (willDelete) {
    //        $.ajax({
    //           type: "DELETE",
    //            url: url,
    //            success: function (data) {
    //                if (data.success) {

    //                    toastr.success(data.message);
    //                    dataTable.ajax.reload();
    //                } else {
    //                    toastr.error(data.message);
    //                }
    //            }
    //        });
    //    }
    //})
}