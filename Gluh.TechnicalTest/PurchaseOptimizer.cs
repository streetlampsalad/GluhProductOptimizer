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
    // The same product can be in the list of purchase requirements multiple times
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
            if(!purchaseRequirements.Any())
            {
                return currentProductOrders;
            }

            var purchaseRequirement = purchaseRequirements.First();
            purchaseRequirement.Product.Stock = purchaseRequirement.Product.Stock.OrderBy(x => x.Cost).ToList();

            var currentProductOrder = new ProductPurchaseOrder
            {
                Product = purchaseRequirement.Product,
                PurchaseOrders = new List<PurchaseOrder>()
            };

            currentProductOrders.Add(CreateProductPurchaseOrder(purchaseRequirement, currentProductOrder));

            purchaseRequirements.Remove(purchaseRequirement);
            return CreateProductPurchaseOrders(purchaseRequirements, currentProductOrders);                        
        }

        private ProductPurchaseOrder CreateProductPurchaseOrder(PurchaseRequirement purchaseRequirement, ProductPurchaseOrder currentProductOrder)
        {
            if(!purchaseRequirement.Product.Stock.Any())
            {                
                return FilterPurchaseOrders(purchaseRequirement, currentProductOrder, new List<PurchaseOrder>());
            }            

            var stock = purchaseRequirement.Product.Stock.First();

            if(stock.StockOnHand >= purchaseRequirement.Quantity)
            {
                currentProductOrder.PurchaseOrders.Add(new PurchaseOrder
                {                    
                    Supplier = stock.Supplier,
                    Quantity = purchaseRequirement.Quantity,
                    ProductCost = stock.Cost,
                    ShippingCost = stock.Supplier.ShippingCost,
                    ShippingCostMinOrderValue = stock.Supplier.ShippingCostMinOrderValue,
                    ShippingCostMaxOrderValue = stock.Supplier.ShippingCostMaxOrderValue
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
                    ShippingCostMaxOrderValue = stock.Supplier.ShippingCostMaxOrderValue
                });                
            }
            
            purchaseRequirement.Product.Stock.Remove(stock);
            return CreateProductPurchaseOrder(purchaseRequirement, currentProductOrder);
        }

        private ProductPurchaseOrder FilterPurchaseOrders(PurchaseRequirement purchaseRequirement, ProductPurchaseOrder currentProductOrder, List<PurchaseOrder> filteredOrders) 
        {                        
            if(purchaseRequirement.Quantity <= 0 || !currentProductOrder.PurchaseOrders.Any())
            {
                currentProductOrder.PurchaseOrders = filteredOrders;
                return currentProductOrder;
            }

            foreach(var purchaseOrder in currentProductOrder.PurchaseOrders.Where(x => x.Quantity > purchaseRequirement.Quantity))
            {
                purchaseOrder.Quantity = purchaseRequirement.Quantity;
            }

            currentProductOrder.PurchaseOrders = currentProductOrder.PurchaseOrders.OrderBy(x => x.TotalCost).ToList();

            var currentOrder = currentProductOrder.PurchaseOrders.First();

            if(currentOrder.Quantity == purchaseRequirement.Quantity)
            {
                filteredOrders.Add(currentOrder);
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
                currentProductOrder.PurchaseOrders = filteredOrders;
                return currentProductOrder;
            }

            currentProductOrder.PurchaseOrders.Remove(currentOrder);
            return FilterPurchaseOrders(purchaseRequirement, currentProductOrder, filteredOrders);
        }
    }
}
