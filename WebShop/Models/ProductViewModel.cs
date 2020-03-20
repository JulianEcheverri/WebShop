using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Caching;
using WebShop.Entities;

namespace WebShop.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required"), StringLength(100)]
        public string Title { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} is required")]
        public int Number { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} is required")]
        public int Price { get; set; }

        // For getting all the products from db
        public List<Product> ProductsInDb { get; set; }

        // This will save all the products in memory
        public List<Product> ProductsInMemory { get; set; }

        // Allows you to decide where we'll save de data. Sent from view
        public bool SaveInMemory { get; set; }

        // We'll save the data in this one, if the user wants to save it in memory
        protected ObjectCache memoryCache = MemoryCache.Default;

        // The "memoryCache" object must have a key
        protected string CacheKey = "ProductsInMemory";

        // The "memoryCache" object must have policies to save data. In this case, time expiration
        protected CacheItemPolicy CacheItemPolicy { get; set; }

        public ProductViewModel()
        {
            // Adding time expiration like policy for memoryCache object
            // For each time that we create/edit a product, this one will exist in memory 30 min since current time
            CacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMinutes(30) };

            // Initialize the lists
            ProductsInMemory = memoryCache[CacheKey] != null ? memoryCache[CacheKey] as List<Product> : new List<Product>();
            ProductsInDb = GetProducts();
        }

        // For getting the products saved in Data base
        public List<Product> GetProducts()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Product> products = db.Products.ToList();
                return products;
            }
        }

        // For getting the products saved in memory
        public void GetProduct()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Adding ProductsInMemory in ProductsInDb for finding the product to edit (manually mapped)
                ProductsInDb.AddRange(ProductsInMemory);
                Product product = ProductsInDb.FirstOrDefault(x => x.Id == Id);
                if (product != null)
                {
                    Title = product.Title;
                    Number = product.Number;
                    Price = product.Price;
                }
            }
        }

        // Return an object that contains these properties:
        // Success: tells to Js that the process was did it correctly
        // ProductId: the productid from created/edited product object
        // ValidationCode: only if an error happens: 0 for the index break, -1 for the catch
        // Exception: messsage from catch exception
        // SaveInMemory: to tell the listing how the product was saved
        public object CreateOrEditProduct()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    int? productId = default;
                    if (SaveInMemory)
                    {
                        productId = CreateOrEditProductInMemory();
                        if (productId == default(int) || productId == null) return new { Success = default(bool), ValidationCode = productId };
                    }
                    else
                    {
                        if (IsProductRepeated()) return new { Success = default(bool), ValidationCode = default(int) };
                        Product productInDb = db.Products.FirstOrDefault(x => x.Id == Id);

                        if (productInDb != null)
                        {
                            productInDb.Title = Title;
                            productInDb.Number = Number;
                            productInDb.Price = Price;
                            db.Products.AddOrUpdate(productInDb);
                            db.SaveChanges();
                            productId = productInDb.Id;
                        }
                        else
                        {
                            Product newProduct = new Product()
                            {
                                Title = Title,
                                Number = Number,
                                Price = Price
                            };
                            db.Products.AddOrUpdate(newProduct);
                            db.SaveChanges();
                            productId = newProduct.Id;
                        }
                    }
                    return new { Success = true, ProductId = productId, SaveInMemory };
                }
                catch (Exception ex)
                {
                    return new { Success = default(bool), ValidationCode = -1, Exception = ex.Message };
                }
            }
        }

        public int? CreateOrEditProductInMemory()
        {
            try
            {
                int productId = default;
                if (IsProductRepeated()) return default(int);
                Product productSavedInMemory = ProductsInMemory.FirstOrDefault(x => x.Id == Id);

                if (productSavedInMemory != null)
                {
                    productSavedInMemory.Title = Title;
                    productSavedInMemory.Number = Number;
                    productSavedInMemory.Price = Price;
                    // Replacing the object edited in memory according his position
                    ProductsInMemory[ProductsInMemory.IndexOf(productSavedInMemory)] = productSavedInMemory;
                    productId = productSavedInMemory.Id;
                }
                else
                {
                    // Getting products amount for Id assignment and put them negative for avoid inconsistencies against Id from db when the user is editing
                    Product newProduct = new Product()
                    {
                        Id = (ProductsInMemory.Count() + 1) * -1,
                        Title = Title,
                        Number = Number,
                        Price = Price
                    };
                    ProductsInMemory.Add(newProduct);
                    productId = newProduct.Id;
                }
                memoryCache.Set(CacheKey, ProductsInMemory, CacheItemPolicy);
                return productId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Preventing index break
        public bool IsProductRepeated()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Product productInDbRepeated = db.Products.FirstOrDefault(x => x.Title == Title && x.Id != Id);
                // Preventing index break in memory. It doesn't exist there, but we're preventing as well
                Product productInMemoryRepeated = ProductsInMemory.FirstOrDefault(x => x.Title == Title && x.Id != Id);
                return productInDbRepeated != null || productInMemoryRepeated != null;
            }
        }
    }
}