using BLL.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace MVC.Controllers
{
    public class DbController : Controller
    {
        private readonly Db _db;

        public DbController(Db db)
        {
            _db = db;
        }

        public IActionResult Seed()
        {
            _db.ProductStores.RemoveRange(_db.ProductStores.ToList());
            _db.Stores.RemoveRange(_db.Stores.ToList());
            _db.Products.RemoveRange(_db.Products.ToList());
            _db.Categories.RemoveRange(_db.Categories.ToList());

            _db.Stores.Add(new Store()
            {
                Name = "Hepsiburada",
                IsVirtual = true
            });
            _db.Stores.Add(new Store()
            {
                Name = "Vatan",
                IsVirtual = false
            });
            _db.Stores.Add(new Store()
            {
                Name = "Migros"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Teknosa"
            });
            _db.Stores.Add(new Store()
            {
                Name = "İtopya"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Sahibinden",
                IsVirtual = true
            });

            _db.SaveChanges();

            _db.Categories.Add(new Category()
            {
                Name = "Computer",
                Description = "Laptops, desktops and computer peripherals",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Laptop",
                        UnitPrice = 3000.5m,
                        StockAmount = 10,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Mouse",
                        UnitPrice = 20.5M,
                        StockAmount = null,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Keyboard",
                        UnitPrice = 40,
                        StockAmount = 45,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "İtopya").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Monitor",
                        UnitPrice = 2500,
                        StockAmount = 20,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    }
                }
            });
            _db.Categories.Add(new Category()
            {
                Name = "Home Theater System",
                Description = null,
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Speaker",
                        UnitPrice = 2500,
                        StockAmount = 70,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Receiver",
                        UnitPrice = 5000,
                        StockAmount = 30,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Equalizer",
                        UnitPrice = 1000,
                        StockAmount = 40
                    }
                }
            });
            _db.Categories.Add(new Category()
            {
                Name = "Phone",
                Description = "IOS and Android Phones",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "iPhone",
                        UnitPrice = 10000,
                        StockAmount = 20,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
                            }
                        }
                    }
                }
            });
            _db.Categories.Add(new Category()
            {
                Name = "Food",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Apple",
                        UnitPrice = 10.5m,
                        StockAmount = 500,
                        ExpirationDate = new DateTime(2024, 12, 31),
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Chocolate",
                        UnitPrice = 2.5M,
                        StockAmount = 125,
                        ExpirationDate = DateTime.Parse("09/18/2025", new CultureInfo("en-US")),
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
                            }
                        }
                    }
                }
            });
            _db.Categories.Add(new Category()
            {
                Name = "Medicine",
                Description = "Antibiotics, Vitamins, Pain Killers, etc.",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Antibiotic",
                        UnitPrice = 35,
                        StockAmount = 5,
                        ExpirationDate = DateTime.Parse("19.05.2027", new CultureInfo("tr-TR")),
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
                            }
                        }
                    }
                }
            });
            _db.Categories.Add(new Category()
            {
                Name = "Software",
                Description = "Operating Systems, Antivirus Software, Office Software and Video Games"
            });

            _db.SaveChanges();

            TempData["Message"] = "Database seed successful.";

            return RedirectToAction("Index", "Products");
        }
    }
}
