using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class SubCategoryProducts
    {
        public int ProductCategoryID { get; set; }
        public int ProductSubCategoryID { get; set; }
        public List<Product> myProductList { get; set; }


    }
}