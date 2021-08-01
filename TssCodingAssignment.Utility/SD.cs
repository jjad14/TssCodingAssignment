using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TssCodingAssignment.Utility
{
    public static class SD
    {

        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";

        public const string ssShoppingCart = "Shopping Cart Session";


        // convert all of the html inside the raw data
        // What this method will do is it will get all of the string and it will convert it into html format
        // so we can directly display that it is almost the same functionality as @Html.Raw()
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
