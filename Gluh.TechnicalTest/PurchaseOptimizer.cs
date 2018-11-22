using System;
using System.Collections.Generic;
using System.Text;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Database;
using System.Linq;

namespace Gluh.TechnicalTest
{
    //Assumption: This method just optimizes the order, it does not need to reduce or reserve the stockOnHand quantity
    public class PurchaseOptimizer
    {        
        /// <summary>
        /// Calculates the optimal set of supplier to purchase products from.
        /// ### Complete this method
        /// </summary>
        public List<ProductPurchaseOrder> Optimize(List<PurchaseRequirement> purchaseRequirements)
        {
            if(!purchaseRequirements.Any())
            {
                throw new ArgumentException("Purchase requirements cannot be empty");
            }

            return CreatePurchaseOrders(purchaseRequirements, new List<ProductPurchaseOrder>());
        }

        private List<ProductPurchaseOrder> CreatePurchaseOrders(List<PurchaseRequirement> purchaseRequirements, List<ProductPurchaseOrder> currentOrders)
        {
            if(!purchaseRequirements.Any())
            {
                return currentOrders;
            }

            var purchaseRequirement = purchaseRequirements.First();
            purchaseRequirement.Product.Stock = purchaseRequirement.Product.Stock.OrderBy(x => x.Cost).ToList();

            var currentOrder = new ProductPurchaseOrder
            {
                Product = purchaseRequirement.Product,
                PurchaseOrders = new List<PurchaseOrder>()
            };

            currentOrders.Add(CreatePurchaseOrders(purchaseRequirement, currentOrder));

            purchaseRequirements.Remove(purchaseRequirement);
            return CreatePurchaseOrders(purchaseRequirements, currentOrders);                        
        }

        private ProductPurchaseOrder CreatePurchaseOrders(PurchaseRequirement purchaseRequirement, ProductPurchaseOrder currentOrder)
        {
            if(!purchaseRequirement.Product.Stock.Any() || purchaseRequirement.Quantity == 0)
            {
                return currentOrder;
            }            

            var stock = purchaseRequirement.Product.Stock.First();

            if(stock.StockOnHand >= purchaseRequirement.Quantity)
            {
                currentOrder.PurchaseOrders.Add(new PurchaseOrder
                {                    
                    Supplier = stock.Supplier,
                    Quantity = purchaseRequirement.Quantity,
                    ProductCost = stock.Cost * purchaseRequirement.Quantity,
                    ShippingCost = 0
                });
            }
            else if(stock.StockOnHand > 0)
            {
                currentOrder.PurchaseOrders.Add(new PurchaseOrder
                {                    
                    Supplier = stock.Supplier,
                    Quantity = stock.StockOnHand,
                    ProductCost = stock.Cost * purchaseRequirement.Quantity,
                    ShippingCost = 0
                });

                purchaseRequirement.Quantity -= stock.StockOnHand;
            }
            
            purchaseRequirement.Product.Stock.Remove(stock);
            return CreatePurchaseOrders(purchaseRequirement, currentOrder);
        }
    }
}
