using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class PaymentInformation
    {
        public int CustomerID { get; set; }
        public int CartID { get; set; }
        public double Amount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Product> myProductsList { get; set; }

    }
}