using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Tests
{
    public class PurchaseOptimizerTests
    {
        private PurchaseOptimizer _purchaseOptimizer;               

        [SetUp]
        public void Setup()
        {            
            _purchaseOptimizer = new PurchaseOptimizer();
        }             

        [Test]
        public void Optimize_EmptyRequirements()
        {
            var ex = Assert.Throws<ArgumentException>(() => _purchaseOptimizer.Optimize(new List<PurchaseRequirement>()));
            Assert.That(ex.Message, Is.EqualTo("Purchase requirements cannot be empty"));
        }

        [Test]
        public void Optimize_1Product1Supplier()
        {
            var _testSupplier = new Supplier
            {
                ID = 1,
                Name = "_testSupplier",
                ShippingCost = 0,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testProduct = new Product
            {
                ID = 1,
                Name = "_testProduct",
                Type = ProductType.Physical                
            };

            _testProduct.Stock = new List<ProductStock>
            {
                new ProductStock
                {
                    ID = 1,
                    Cost = 940.56m,
                    StockOnHand = 105,
                    Supplier = _testSupplier,
                    Product = _testProduct
                }
            };

            var _testData = new List<PurchaseRequirement>
            {
                new PurchaseRequirement
                {
                    Product = _testProduct,                    
                    Quantity = 10
                }
            };

            var result = _purchaseOptimizer.Optimize(_testData);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("_testProduct", result[0].Product.Name);
            Assert.AreEqual(1, result[0].PurchaseOrders.Count());
            Assert.AreEqual("_testSupplier", result[0].PurchaseOrders[0].Supplier.Name);
            Assert.AreEqual(10, result[0].PurchaseOrders[0].Quantity);
        }

        [Test]
        public void Optimize_1Product2Suppliers()
        {
            var _testSupplier = new Supplier
            {
                ID = 1,
                Name = "_testSupplier",
                ShippingCost = 0,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testSupplier2 = new Supplier
            {
                ID = 2,
                Name = "_testSupplier2",
                ShippingCost = 0,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testProduct = new Product
            {
                ID = 1,
                Name = "_testProduct",
                Type = ProductType.Physical
            };

            _testProduct.Stock = new List<ProductStock>
            {
                new ProductStock
                {
                    ID = 1,
                    Cost = 940.56m,
                    StockOnHand = 5,
                    Supplier = _testSupplier,
                    Product = _testProduct
                },
                new ProductStock
                {
                    ID = 2,
                    Cost = 940.56m,
                    StockOnHand = 105,
                    Supplier = _testSupplier2,
                    Product = _testProduct
                }
            };

            var _testData = new List<PurchaseRequirement>
            {
                new PurchaseRequirement
                {
                    Product = _testProduct,
                    Quantity = 15
                }
            };

            var result = _purchaseOptimizer.Optimize(_testData);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("_testProduct", result[0].Product.Name);
            Assert.AreEqual(2, result[0].PurchaseOrders.Count());
            Assert.AreEqual("_testSupplier", result[0].PurchaseOrders[0].Supplier.Name);
            Assert.AreEqual(5, result[0].PurchaseOrders[0].Quantity);            
            Assert.AreEqual("_testSupplier2", result[0].PurchaseOrders[1].Supplier.Name);
            Assert.AreEqual(10, result[0].PurchaseOrders[1].Quantity);
        }

        [Test]
        public void Optimize_DifferentCost()
        {
            var _testSupplier = new Supplier
            {
                ID = 1,
                Name = "_testSupplier",
                ShippingCost = 0,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testSupplier2 = new Supplier
            {
                ID = 2,
                Name = "_testSupplier2",
                ShippingCost = 0,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testProduct = new Product
            {
                ID = 1,
                Name = "_testProduct",
                Type = ProductType.Physical
            };

            _testProduct.Stock = new List<ProductStock>
            {
                new ProductStock
                {
                    ID = 1,
                    Cost = 940.56m,
                    StockOnHand = 25,
                    Supplier = _testSupplier,
                    Product = _testProduct
                },
                new ProductStock
                {
                    ID = 2,
                    Cost = 540.56m,
                    StockOnHand = 5,
                    Supplier = _testSupplier2,
                    Product = _testProduct
                }
            };

            var _testData = new List<PurchaseRequirement>
            {
                new PurchaseRequirement
                {
                    Product = _testProduct,
                    Quantity = 15
                }
            };

            var result = _purchaseOptimizer.Optimize(_testData);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("_testProduct", result[0].Product.Name);
            Assert.AreEqual(2, result[0].PurchaseOrders.Count());
            Assert.AreEqual("_testSupplier2", result[0].PurchaseOrders[0].Supplier.Name);
            Assert.AreEqual(5, result[0].PurchaseOrders[0].Quantity);
            Assert.AreEqual("_testSupplier", result[0].PurchaseOrders[1].Supplier.Name);
            Assert.AreEqual(10, result[0].PurchaseOrders[1].Quantity);
            Assert.AreEqual(12108.4, result[0].TotalCost);
        }

        [Test]
        public void Optimize_ShippingCosts()
        {
            var _testSupplier = new Supplier
            {
                ID = 1,
                Name = "_testSupplier",
                ShippingCost = 100,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testSupplier2 = new Supplier
            {
                ID = 2,
                Name = "_testSupplier2",
                ShippingCost = 5,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testProduct = new Product
            {
                ID = 1,
                Name = "_testProduct",
                Type = ProductType.Physical
            };

            _testProduct.Stock = new List<ProductStock>
            {
                new ProductStock
                {
                    ID = 1,
                    Cost = 1m,
                    StockOnHand = 25,
                    Supplier = _testSupplier,
                    Product = _testProduct
                },
                new ProductStock
                {
                    ID = 2,
                    Cost = 5m,
                    StockOnHand = 5,
                    Supplier = _testSupplier2,
                    Product = _testProduct
                }
            };

            var _testData = new List<PurchaseRequirement>
            {
                new PurchaseRequirement
                {
                    Product = _testProduct,
                    Quantity = 5
                }
            };

            var result = _purchaseOptimizer.Optimize(_testData);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("_testProduct", result[0].Product.Name);
            Assert.AreEqual(1, result[0].PurchaseOrders.Count());
            Assert.AreEqual("_testSupplier2", result[0].PurchaseOrders[0].Supplier.Name);
            Assert.AreEqual(5, result[0].PurchaseOrders[0].Quantity);            
            Assert.AreEqual(50, result[0].TotalCost);
        }

        [Test]
        public void Optimize_ShippingCostsMaxMin()
        {
            var _testSupplier = new Supplier
            {
                ID = 1,
                Name = "_testSupplier",
                ShippingCost = 100,
                ShippingCostMinOrderValue = 5,
                ShippingCostMaxOrderValue = 100
            };

            var _testSupplier2 = new Supplier
            {
                ID = 2,
                Name = "_testSupplier2",
                ShippingCost = 5,
                ShippingCostMinOrderValue = 0,
                ShippingCostMaxOrderValue = 0
            };

            var _testSupplier3 = new Supplier
            {
                ID = 3,
                Name = "_testSupplier3",
                ShippingCost = 15,
                ShippingCostMinOrderValue = 25,
                ShippingCostMaxOrderValue = 50
            };

            var _testProduct = new Product
            {
                ID = 1,
                Name = "_testProduct",
                Type = ProductType.Physical
            };

            _testProduct.Stock = new List<ProductStock>
            {
                new ProductStock
                {
                    ID = 1,
                    Cost = 5m,
                    StockOnHand = 25,
                    Supplier = _testSupplier,
                    Product = _testProduct
                },
                new ProductStock
                {
                    ID = 2,
                    Cost = 10m,
                    StockOnHand = 5,
                    Supplier = _testSupplier2,
                    Product = _testProduct
                },
                new ProductStock
                {
                    ID = 3,
                    Cost = 15m,
                    StockOnHand = 25,
                    Supplier = _testSupplier3,
                    Product = _testProduct
                }
            };

            var _testData = new List<PurchaseRequirement>
            {
                new PurchaseRequirement
                {
                    Product = _testProduct,
                    Quantity = 10
                }
            };

            var result = _purchaseOptimizer.Optimize(_testData);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("_testProduct", result[0].Product.Name);
            Assert.AreEqual(2, result[0].PurchaseOrders.Count());
            Assert.AreEqual("_testSupplier2", result[0].PurchaseOrders[0].Supplier.Name);
            Assert.AreEqual(5, result[0].PurchaseOrders[0].Quantity);
            Assert.AreEqual(50, result[0].PurchaseOrders[0].TotalCost);
            Assert.AreEqual("_testSupplier3", result[0].PurchaseOrders[1].Supplier.Name);
            Assert.AreEqual(5, result[0].PurchaseOrders[1].Quantity);
            Assert.AreEqual(75, result[0].PurchaseOrders[1].TotalCost);
            Assert.AreEqual(125, result[0].TotalCost);
        }
    }
}