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
        public decimal ShippingCostMinOrderValue { get; set; }
        public decimal ShippingCostMaxOrderValue { get; set; }
        public decimal TotalProductCost
        {
            get
            {
                return ProductCost * Quantity;
            }
        }
        public decimal TotalShippingCost
        {
            get
            {
                if(TotalProductCost >= ShippingCostMinOrderValue && TotalProductCost <= ShippingCostMaxOrderValue)
                {
                    return ShippingCost * Quantity;
                }
                return 0;
            }
        }
        public decimal TotalCost
        {
            get
            {
                return TotalProductCost + TotalShippingCost;
            }
        }
    }
}
