﻿using Gluh.TechnicalTest.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gluh.TechnicalTest.Models
{
    public class ProductPurchaseOrder
    {
        public Product Product { get; set; }        
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public decimal TotalProductCost
        {
            get
            {
                return PurchaseOrders.Sum(x => x.TotalProductCost);
            }
        }
        public decimal TotalShippingCost
        {
            get
            {
                return PurchaseOrders.Sum(x => x.TotalShippingCost);
            }
        }
        public decimal TotalCost 
        { 
            get {
                return TotalProductCost + TotalShippingCost;
            } 
        }           
    }
}
