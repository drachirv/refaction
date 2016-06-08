using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using refactor_me.Models;
using refactor_me.Controllers;

namespace refactor_me.Tests
{
  
    [TestClass]
    public class TestProductsController
    {
        private DatabaseEntities _context;
        private ProductsController _controller;

        public TestProductsController()
        {
            _context = new DatabaseEntities();
            _controller = new ProductsController(_context);
        }

      
        [TestMethod]
        public void GetProducts_ShouldReturnAllProducts()
        {
            // Arrange   
            ClearDb();
            _context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Demo Product 1",
                Description = "Desc1", DeliveryPrice = 10, Price = 20 });
            _context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Demo Product 2",
                Description = "Desc2", DeliveryPrice = 0, Price = 30 });
            _context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Demo Product 3",
                Description = "Desc 3", DeliveryPrice = 100, Price = 40 });
            _context.SaveChanges();

            // Act
            var result = _controller.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Items.Count());
        }

        [TestMethod]
        public void GetProductByName_ShouldReturnTheRightProducts()
        {

            // Arrange
            Guid newGuid = Guid.NewGuid();
            _context.Products.Add(new Product { Id = newGuid, Name = "This is a test with the name sponge bob",
                Description = "Desc", DeliveryPrice = 100, Price = 40 });
            _context.SaveChanges();

            // Act
            var result = _controller.SearchByName("sponge");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual(newGuid, result.Items.First().Id);
        }

        [TestMethod]
        public void GetProduct_ShouldFailIfProductDoesNotExist()
        {
           
            try
            {
                _controller.GetProduct(Guid.NewGuid());
                Assert.Fail("No exception thrown");
            }
            catch (System.Web.Http.HttpResponseException e)
            {
                if (e.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Error is not not found");
                }
            }
            
        }


        [TestMethod]
        public void CreateProduct_ShouldInsertTheRightProduct()
        {
            // Arrange
            var testProduct = GenerateTestProduct();
            
            // Act
            _controller.Create(testProduct);

            // Assert
            var result = _controller.GetProduct(testProduct.Id);
            Assert.AreEqual(result.Id, testProduct.Id);
            Assert.AreEqual(result.Name, testProduct.Name);
            Assert.AreEqual(result.Description, testProduct.Description);
            Assert.AreEqual(result.DeliveryPrice, testProduct.DeliveryPrice);
        }

        [TestMethod]
        public void CreateProduct_ShouldFailIfAlreadyExisting()
        {
            var testProduct = GenerateTestProduct();
            _controller.Create(testProduct);

            try
            {
                _controller.Create(testProduct);
                Assert.Fail("No exception thrown");
            }
            catch (System.Web.Http.HttpResponseException e)
            {
                if (e.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    Assert.Fail("Error is not not found");
                }
            }
        }
        
        [TestMethod]
        public void UpdateProduct_ShouldReturnTheNewProduct()
        {

            // Arrange
            var testProduct = GenerateTestProduct();
            _controller.Create(testProduct);

            testProduct.Description = "Update description";
            testProduct.Name = "Update name";
            testProduct.Price = 12345;
            testProduct.DeliveryPrice = 9876;

            // Act
            _controller.Update(testProduct.Id, testProduct);

            // Assert
            var result = _controller.GetProduct(testProduct.Id);
            Assert.AreEqual(result.Id, testProduct.Id);
            Assert.AreEqual(result.Name, testProduct.Name);
            Assert.AreEqual(result.Description, testProduct.Description);
            Assert.AreEqual(result.DeliveryPrice, testProduct.DeliveryPrice);
        }

        [TestMethod]
        public void DeleteProduct_ProductShouldNotBeInGetAll()
        {
            
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            _controller.Create(testProduct);

            // Act
            _controller.Delete(testProduct.Id);

            // Assert
            var result = _controller.GetAll();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Items.Count());
        }

        [TestMethod]
        public void DeleteProduct_ProductShouldNotBeGetById()
        {
            var testProduct = GenerateTestProduct();
            _controller.Create(testProduct);


            _controller.Delete(testProduct.Id);
            try
            {
                var result = _controller.GetProduct(testProduct.Id);
                Assert.Fail("No exception thrown");
            }
            catch (System.Web.Http.HttpResponseException e)
            {
                if (e.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Error is not not found");
                }
            }
        }

        [TestMethod]
        public void CreateProductOption_ShouldInsertTheRightProductOption()
        {
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            var productOption = GenerateTestProductOption();
            _controller.Create(testProduct);

            // Act
            _controller.CreateOption(testProduct.Id,productOption);

            // Assert
            var result = _controller.GetOption(testProduct.Id,productOption.Id);

            Assert.AreEqual(result.Id, productOption.Id);
            Assert.AreEqual(result.Name, productOption.Name);
            Assert.AreEqual(result.Description, productOption.Description);
        }


        [TestMethod]
        public void UpdateProductOption_ShouldReturnTheUpdatedProductOption()
        {
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            var productOption = GenerateTestProductOption();
            _controller.Create(testProduct);
            _controller.CreateOption(testProduct.Id, productOption);
            productOption.Name = "NEW NAME";
            productOption.Description = "NEW Description";

            // Act
            _controller.UpdateOption(testProduct.Id, productOption.Id,productOption);

            // Assert
            var result = _controller.GetOption(testProduct.Id, productOption.Id);
            Assert.AreEqual(result.Id, productOption.Id);
            Assert.AreEqual(result.Name, productOption.Name);
            Assert.AreEqual(result.Description, productOption.Description);
        }


        [TestMethod]
        public void DeleteProductOption_ProductOptionShouldNotBeGetById()
        {
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            var productOption = GenerateTestProductOption();
            _controller.Create(testProduct);
            _controller.CreateOption(testProduct.Id, productOption);
      
            _controller.DeleteOption( productOption.Id);
            
            try
            {
                var result = _controller.GetOption(testProduct.Id, productOption.Id);
                Assert.Fail("No exception thrown");
            }
            catch (System.Web.Http.HttpResponseException e)
            {
                if (e.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Error is not not found");
                }
            }
        }


        [TestMethod]
        public void DeleteProduct_ProductOptionShouldNotBeGetById()
        {
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            var productOption = GenerateTestProductOption();
            _controller.Create(testProduct);
            _controller.CreateOption(testProduct.Id, productOption);
            
            _controller.Delete(testProduct.Id);
            
            try
            {
                var result = _controller.GetOption(testProduct.Id, productOption.Id);
                Assert.Fail("No exception thrown");
            }
            catch (System.Web.Http.HttpResponseException e)
            {
                if (e.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Error is not not found");
                }
            }
        }


        [TestMethod]
        public void DeleteProductOption_ProductOptionShouldNotBeInTheProductOptionsList()
        {
            // Arrange
            ClearDb();
            var testProduct = GenerateTestProduct();
            var productOption = GenerateTestProductOption();
            _controller.Create(testProduct);
            _controller.CreateOption(testProduct.Id, productOption);

            // Act
            _controller.DeleteOption(productOption.Id);

            // Assert
            var result = _controller.GetOptions(testProduct.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Items.Count());
        }

        #region Arrange Helpers
        
        /// <summary>
        /// Truncates ProductOptions and Products tables
        /// </summary>
        private void ClearDb()
        {
            _context.ProductOptions.RemoveRange(_context.ProductOptions);
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();
        }

        private Product GenerateTestProduct()
        {
            return new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Demo name",
                Price = 5,
                Description = "Demo product description",
                DeliveryPrice = 1
            };
        }

        private ProductOption GenerateTestProductOption()
        {
            return new ProductOption()
            {
                Id = Guid.NewGuid(),
                Name = "Demo Product Option name",
                Description = "Demo productoption description"
              
            };
        }
        #endregion
    }
}