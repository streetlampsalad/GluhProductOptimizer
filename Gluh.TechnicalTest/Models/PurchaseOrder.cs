using Gluh.TechnicalTest.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gluh.TechnicalTest.Models
{
    public class PurchaseOrder
    {
        public Product Product { get; set; }
        public Supplier Supplier { get; set; }
        public int Quantity { get; set; }
    }
}
