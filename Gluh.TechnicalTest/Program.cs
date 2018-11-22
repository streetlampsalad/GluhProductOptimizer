using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var purchaseRequirements = new TestData().Create();
            var purchaseOptimizer = new PurchaseOptimizer();

            var productPurchaseOrders = purchaseOptimizer.Optimize(purchaseRequirements);

            foreach(var productPurchaseOrder in productPurchaseOrders)
            {
                Console.WriteLine("Product: " + productPurchaseOrder.Product.Name);
                if(productPurchaseOrder.PurchaseOrders.Any())
                {
                    foreach(var order in productPurchaseOrder.PurchaseOrders)
                    {
                        Console.WriteLine("     Supplier: " + order.Supplier.Name + " | Qty: " + order.Quantity + " | Cost: " + order.TotalCost.ToString("C"));
                    }
                }
                else
                {
                    Console.WriteLine("     No Supplier Stock");
                }
                
                Console.WriteLine("Total Cost : " + productPurchaseOrder.TotalCost.ToString("C"));
                Console.WriteLine("==============================================");
            }

            Console.ReadLine();
        }
    }
}
