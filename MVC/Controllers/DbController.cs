using BLL.DAL;
using BLL.Models;
using EZcore.Controllers;
using EZcore.DAL.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MVC.Controllers
{
    public class DbController : MvcController
    {
        const int PIN = 0905;

        private readonly Db _db;

        public DbController(Db db)
        {
            _db = db;
        }

        public IActionResult Seed(int pin)
        {
            if (pin == PIN)
            {
                if (_db.Stores.Any() || _db.Products.Any() || _db.Categories.Any()
                    || _db.Roles.Any() || _db.Users.Any())
                {
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Stores', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Products', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ProductStores', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Categories', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Roles', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Users', RESEED, 0)");
                    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('UserRoles', RESEED, 0)");
                }

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
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                        }
                    },
                    new Product()
                    {
                        Name = "Mouse",
                        UnitPrice = 20.5M,
                        StockAmount = null,
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                        }
                    },
                    new Product()
                    {
                        Name = "Keyboard",
                        UnitPrice = 40,
                        StockAmount = 45,
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "İtopya").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
                        }
                    },
                    new Product()
                    {
                        Name = "Monitor",
                        UnitPrice = 2500,
                        StockAmount = 20,
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
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
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id
                        }
                    },
                    new Product()
                    {
                        Name = "Receiver",
                        UnitPrice = 5000,
                        StockAmount = 30,
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
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
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Teknosa").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id,
                            _db.Stores.SingleOrDefault(s => s.Name == "Sahibinden").Id
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
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
                        }
                    },
                    new Product()
                    {
                        Name = "Chocolate",
                        UnitPrice = 2.5M,
                        StockAmount = 125,
                        ExpirationDate = DateTime.Parse("09/18/2025", new CultureInfo("en-US")),
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
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
                        Stores = new List<int>()
                        {
                            _db.Stores.SingleOrDefault(s => s.Name == "Migros").Id
                        }
                    }
                }
                });
                _db.Categories.Add(new Category()
                {
                    Name = "Software",
                    Description = "Operating Systems, Antivirus Software, Office Software and Video Games"
                });

                _db.UserRoles.RemoveRange(_db.UserRoles.ToList());
                _db.Roles.RemoveRange(_db.Roles.ToList());
                _db.Users.RemoveRange(_db.Users.ToList());

                _db.Roles.Add(new Role()
                {
                    Name = Roles.EZcodeAdmin.ToString(),
                });
                _db.Roles.Add(new Role()
                {
                    Name = Roles.EZcodeUser.ToString(),
                });

                _db.SaveChanges();

                _db.Users.Add(new User()
                {
                    UserName = "admin",
                    Password = "admin",
                    IsActive = true,
                    Roles = new List<int>()
                {
                    _db.Roles.SingleOrDefault(r => r.Name == Roles.EZcodeAdmin.ToString()).Id
                }
                });
                _db.Users.Add(new User()
                {
                    UserName = "user",
                    Password = "user",
                    IsActive = true,
                    Roles = new List<int>()
                {
                    _db.Roles.SingleOrDefault(r => r.Name == Roles.EZcodeUser.ToString()).Id
                }
                });

                _db.SaveChanges();

                var files = Directory.GetFiles(Path.Combine("wwwroot", "files"));
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }

                Message = "Database seed successful.";
            }
            else
            {
                Message = "Invalid pin!";
            }

            return RedirectToAction("Index", "Products");
        }
    }
}
