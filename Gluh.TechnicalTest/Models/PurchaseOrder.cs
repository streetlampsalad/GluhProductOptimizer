using Gluh.TechnicalTest.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gluh.TechnicalTest.Models
{
    public class PurchaseOrder
    {        
        public Supplier Supplier { get; set; }
        public int Quantity { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
