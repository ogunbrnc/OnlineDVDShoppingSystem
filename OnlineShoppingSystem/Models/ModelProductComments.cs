using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelProductComments
    {
        public Product product { get; set; }
        public ProductComment productComment { get; set; }
    }
}