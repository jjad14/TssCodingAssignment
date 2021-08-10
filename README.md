# TssCodingAssignment

Using Visual Studio, C#, and front-end technologies (such as bootstrap, jquery, css, html, etc.), create an MVC web app with the following suggested features:
*	Data is provided via a backing database
*	Display a listing of products
*	Allow the user to Create, Edit, or Delete a product (control access to this if you wish)
*	Allow the user to select any product in the list and display details for the product
*	Allow the user to add any product to a cart / wish list
*	Allow the user to view the contents of the cart / wish list
* Allow the user to run a few different reports to the screen against the products, for example: 
  * Products grouped by ?Type?, with subtotals/totals on value
  * Products that were created in the last 5 days
  * Products that were deleted in the last 10 days


## My Solution

* Back end
  * Unit of Work
  * Repository Pattern
  * N-tier architecture 
  * Authentication (Admin and Customers)
  * Authorization (Admins can create, update and delete products and categories)
  * Code First approach (EF Core)
  * Sql Server

* Client Side (JavaScript Libraries): 
  * Jquery
  * Bootstrap 4
  * dataTables.net
  * toastr.js
  * font awesome
  * sweatalert2
  * moment.js

Default Admin User Credentials found in DbInitializer.cs

Email: admin@gmail.com
Password: Admin@123


