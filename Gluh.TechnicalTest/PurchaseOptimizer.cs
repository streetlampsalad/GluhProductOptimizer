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
        public List<PurchaseOrder> Optimize(List<PurchaseRequirement> purchaseRequirements)
        {
            if(!purchaseRequirements.Any())
            {
                throw new ArgumentException("Purchase requirements cannot be empty");
            }

            return CreatePurchaseOrders(purchaseRequirements, new List<PurchaseOrder>());
        }

        private List<PurchaseOrder> CreatePurchaseOrders(List<PurchaseRequirement> purchaseRequirements, List<PurchaseOrder> currentOrders)
        {
            if(!purchaseRequirements.Any())
            {
                return currentOrders;
            }

            var purchaseRequirement = purchaseRequirements.First();

            currentOrders = CreatePurchaseOrders(purchaseRequirement, currentOrders);

            purchaseRequirements.Remove(purchaseRequirement);
            return CreatePurchaseOrders(purchaseRequirements, currentOrders);                        
        }

        private List<PurchaseOrder> CreatePurchaseOrders(PurchaseRequirement purchaseRequirement, List<PurchaseOrder> currentOrders)
        {
            if(!purchaseRequirement.Product.Stock.Any() || purchaseRequirement.Quantity == 0)
            {
                return currentOrders;
            }

            var stock = purchaseRequirement.Product.Stock.First();

            if(stock.StockOnHand >= purchaseRequirement.Quantity)
            {
                currentOrders.Add(new PurchaseOrder
                {
                    Product = purchaseRequirement.Product,
                    Supplier = stock.Supplier,
                    Quantity = purchaseRequirement.Quantity
                });
            }
            else if(stock.StockOnHand > 0)
            {
                currentOrders.Add(new PurchaseOrder
                {
                    Product = purchaseRequirement.Product,
                    Supplier = stock.Supplier,
                    Quantity = stock.StockOnHand
                });

                purchaseRequirement.Quantity -= stock.StockOnHand;
            }

            purchaseRequirement.Product.Stock.Remove(stock);
            return CreatePurchaseOrders(purchaseRequirement, currentOrders);
        }
    }
}
