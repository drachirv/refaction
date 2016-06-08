using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using refactor_me.Models;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private readonly DatabaseEntities _db;

        public ProductsController(DatabaseEntities context)
        {
            _db = context;
        }


        public ProductsController()
        {
            _db = new DatabaseEntities(); 
        }


        /// <summary>
        /// Gets all products
        /// </summary>
        [Route]
        [HttpGet]
        public Products GetAll()
        {
            return new Products(_db.Products);
        }

        /// <summary>
        /// Finds all products matching the specified name.
        /// </summary>
        [Route]
        [HttpGet]
        public Products SearchByName(string name)
        {
            return new Products(_db.Products.Where(p => p.Name.ToLower().Contains(name)));
        }

        /// <summary>
        /// Gets the product that matches the specified ID
        /// </summary>
        /// <param name="id">GUID of the product</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public Product GetProduct(Guid id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return product;
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        [Route]
        [HttpPost]
        public void Create(Product product)
        {
            _db.Products.Add(product);
            try
            {
                _db.SaveChanges();
            }
            // If we cannot store a product, it is likely it is already here
            catch (DbUpdateException)
            {
                if (_db.Products.Count(p => p.Id == product.Id) > 0)
                {
                    throw new HttpResponseException(HttpStatusCode.Conflict);
                }
                throw;
            }
        }

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="id">Id of the product to update</param>
        /// <param name="product">Product with updated field</param>
        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, Product product)
        {
            if (id != product.Id)
            {
                // We do not let the client changing the product ID
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
                        
            _db.Entry(product).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
             catch (DbUpdateException)
            {
                // Are we trying to update a non existing product?
                if (_db.Products.Count(p => p.Id == id) == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                throw;
            }
        }

        /// <summary>
        /// Deletes a product and its options.
        /// </summary>
        /// <param name="id">ID of the product to delete</param>
        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            
            _db.ProductOptions.RemoveRange(product.ProductOptions);
            _db.Products.Remove(product);
            _db.SaveChanges();
        }

        /// <summary>
        /// Finds all options for a specified product
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <returns></returns>
        [Route("{productId}/options")]
        [HttpGet]
        public ProductOptions GetOptions(Guid productId)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // For that specific usage we need to tell entity to load ProductOptions related to our Product
            _db.Entry(product).Collection(p => p.ProductOptions).Load();

            return new ProductOptions(product.ProductOptions);
        }

        /// <summary>
        /// Finds the specified product option for the specified product
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <param name="id">ID of the product option</param>
        /// <returns></returns>
        [Route("{productId}/options/{id}")]
        [HttpGet]
        public ProductOption GetOption(Guid productId, Guid id)
        {
            var productOption = _db.ProductOptions.Find(id);
            if (productOption == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return productOption;
        }

        /// <summary>
        /// Adds a new product option to the specified product
        /// </summary>       
        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, ProductOption option)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // as the ProductOption representation is not expected to contain the productId it is safer to add it here
            option.ProductId = productId;

            product.ProductOptions.Add(option);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (_db.ProductOptions.Count(po => po.Id == option.Id) > 0)
                {
                    throw new HttpResponseException(HttpStatusCode.Conflict);
                }
                throw;
            }
        }

        /// <summary>
        /// Updates the specified product option.
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <param name="id">ID of the product option</param>
        /// <param name="option">Updated product option</param>
        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid productId, Guid id, ProductOption option)
        {
            if (id != option.Id)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // as the ProductOption representation is not expected to contain the productId it is safer to add it here
            option.ProductId = productId;

            _db.Entry(option).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // are we sure the option already exist?
                if (_db.ProductOptions.Count(po => po.Id == option.Id) == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                throw;
            }
        }

        /// <summary>
        /// deletes the specified product option
        /// </summary>
        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
        {
            var productOption = _db.ProductOptions.Find(id);
            if (productOption == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _db.ProductOptions.Remove(productOption);
            _db.SaveChanges();
        }
    }
}