using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ProductDetail
    {
        public Product product { get; set; }
        public List<ProductComment> productComments { get; set; }
    }
}