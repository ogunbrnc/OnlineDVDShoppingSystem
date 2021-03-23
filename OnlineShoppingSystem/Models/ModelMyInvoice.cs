using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelMyInvoice
    {
        public String InvoiceDate { get; set; }
        public String BarcodeNumbers { get; set; }
        public double Amount { get; set; }
        public int InvoiceID { get; set; }
        public List<ModelMyInvoice> listMyInvoice { get; set; }
        public bool Empty
        {
            get
            {
                return (listMyInvoice.Count == 0);

            }
        }
    }
}