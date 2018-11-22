using System;
using System.Collections.Generic;
using System.Text;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Database;
using System.Linq;

namespace Gluh.TechnicalTest
{
    // Assumptions:
    // This method just optimizes the order, it does not need to manipulate the stockOnHand quantity
    // The shipping cost is per unit not the entire order
    // The same product can not be in the list of purchase requirements multiple times
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

            return CreateProductPurchaseOrders(purchaseRequirements, new List<ProductPurchaseOrder>());
        }

        private List<ProductPurchaseOrder> CreateProductPurchaseOrders(List<PurchaseRequirement> purchaseRequirements, List<ProductPurchaseOrder> currentProductOrders)
        {
            // Using recursion to process each purchase requirement individually 
            if(!purchaseRequirements.Any())
            {
                return currentProductOrders;
            }

            var purchaseRequirement = purchaseRequirements.First();

            // create a new product purchase order with the purchase requirement
            var currentProductOrder = new ProductPurchaseOrder
            {
                Product = purchaseRequirement.Product,
                PurchaseOrders = new List<PurchaseOrder>()
            };

            // Create the purchase orders for the new product purchase order above and add it to the list
            currentProductOrders.Add(CreatePurchaseOrders(purchaseRequirement, currentProductOrder));

            // Remove the processed purchase requirement from the list once its done
            purchaseRequirements.Remove(purchaseRequirement);

            // Call the method again to process the next purchase requirement
            return CreateProductPurchaseOrders(purchaseRequirements, currentProductOrders);                        
        }

        private ProductPurchaseOrder CreatePurchaseOrders(PurchaseRequirement purchaseRequirement, ProductPurchaseOrder currentProductOrder)
        {
            // Using recursion to process each purchase order individually
            if(!purchaseRequirement.Product.Stock.Any())
            {                
                return FilterPurchaseOrders(purchaseRequirement, currentProductOrder, new List<PurchaseOrder>());
            }            

            var stock = purchaseRequirement.Product.Stock.First();

            // Check if the stock on hand per supplier can fulfill the requirement qty
            if(stock.StockOnHand >= purchaseRequirement.Quantity)
            {
                currentProductOrder.PurchaseOrders.Add(new PurchaseOrder
                {                    
                    Supplier = stock.Supplier,
                    Quantity = purchaseRequirement.Quantity,
                    ProductCost = stock.Cost,
                    ShippingCost = stock.Supplier.ShippingCost,
                    ShippingCostMinOrderValue = stock.Supplier.ShippingCostMinOrderValue,
                    ShippingCostMaxOrderValue = stock.Supplier.ShippingCostMaxOrderValue,
                    Type = purchaseRequirement.Product.Type
                });                
            }
            else if(stock.StockOnHand > 0)
            {
                currentProductOrder.PurchaseOrders.Add(new PurchaseOrder
                {                    
                    Supplier = stock.Supplier,
                    Quantity = stock.StockOnHand,
                    ProductCost = stock.Cost,
                    ShippingCost = stock.Supplier.ShippingCost,
                    ShippingCostMinOrderValue = stock.Supplier.ShippingCostMinOrderValue,
                    ShippingCostMaxOrderValue = stock.Supplier.ShippingCostMaxOrderValue,
                    Type = purchaseRequirement.Product.Type
                });                
            }

            // Remove the processed purchase order from the list once its done
            purchaseRequirement.Product.Stock.Remove(stock);

            // Call the method again to process the next purchase order
            return CreatePurchaseOrders(purchaseRequirement, currentProductOrder);
        }

        private ProductPurchaseOrder FilterPurchaseOrders(PurchaseRequirement purchaseRequirement, ProductPurchaseOrder currentProductOrder, List<PurchaseOrder> filteredOrders) 
        {
            // Using recursion to process each purchase order individually
            if(purchaseRequirement.Quantity <= 0 || !currentProductOrder.PurchaseOrders.Any())
            {
                currentProductOrder.PurchaseOrders = filteredOrders;
                return currentProductOrder;
            }

            // Check is the overall required qty has gone down and update the remaining suppliers qty
            foreach(var purchaseOrder in currentProductOrder.PurchaseOrders.Where(x => x.Quantity > purchaseRequirement.Quantity))
            {
                purchaseOrder.Quantity = purchaseRequirement.Quantity;
            }

            // Order the orders by total price
            currentProductOrder.PurchaseOrders = currentProductOrder.PurchaseOrders.OrderBy(x => x.TotalCost).ToList();

            var currentOrder = currentProductOrder.PurchaseOrders.First();

            // Check if this order can fulfill the requirement qty if not create a purchase order for the stock on hand qty and reduce the overall requirement qty
            if(currentOrder.Quantity == purchaseRequirement.Quantity)
            {
                filteredOrders.Add(currentOrder);

                // Override the old purchase orders with the new filtered orders and return everything 
                currentProductOrder.PurchaseOrders = filteredOrders;
                return currentProductOrder;
            }
            else if(currentOrder.Quantity < purchaseRequirement.Quantity)
            {
                filteredOrders.Add(currentOrder);
                purchaseRequirement.Quantity -= currentOrder.Quantity;
            }
            else if(currentOrder.Quantity > purchaseRequirement.Quantity)
            {
                currentOrder.Quantity = purchaseRequirement.Quantity;
                filteredOrders.Add(currentOrder);

                // Override the old purchase orders with the new filtered orders and return everything
                currentProductOrder.PurchaseOrders = filteredOrders;
                return currentProductOrder;
            }

            // Remove the processed purchase order from the list once its done
            currentProductOrder.PurchaseOrders.Remove(currentOrder);

            // Call the method again to process the next purchase order
            return FilterPurchaseOrders(purchaseRequirement, currentProductOrder, filteredOrders);
        }
    }
}
