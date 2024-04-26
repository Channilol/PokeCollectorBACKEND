using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Ajax.Utilities;
using PokeCollector.Models;

namespace PokeCollector.Controllers
{
    public class DatabaseController : Controller
    {
        private DBContext db = new DBContext();
        // GET: Database
        public ActionResult Index()
        {
            return View();
        }

        //MANAGEMENT USER

        [HttpGet]
        public ActionResult GetUser(string email, string password)
        {
            if (email != "")
            {
                var user = db.Users.Where(u => u.Email == email && u.Password == password).Select(u => new
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Password = u.Password,
                    Name = u.Name,
                    Surname = u.Surname,
                    Role = u.Role,
                    Image = u.Image
                }).FirstOrDefault();
                if (user != null)
                {
                    var checkCart = db.Cart.Where(c => c.UserId == user.UserId && c.State == "NON ORDINATO").FirstOrDefault();
                    if (checkCart == null)
                    {
                        var newCart = new Cart();
                        newCart.UserId = user.UserId;
                        newCart.State = "NON ORDINATO";
                        newCart.TotalPrice = 0;
                        newCart.Date = DateTime.Now;
                        db.Cart.Add(newCart);
                        db.SaveChanges();
                    }
                    return Json(user, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "L'utente non esiste");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Email non valida");
            }
        }

        [HttpGet]
        public ActionResult CheckUser(int id, string password)
        {
            if (id > 0)
            {
                var user = db.Users.Where(u => u.UserId == id && u.Password == password).Select(u => new
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Password = u.Password,
                    Name = u.Name,
                    Surname = u.Surname,
                    Role = u.Role,
                    Image = u.Image
                }).FirstOrDefault();
                if (user != null)
                {
                    var checkCart = db.Cart.Where(c => c.UserId == user.UserId && c.State == "NON ORDINATO").FirstOrDefault();
                    if (checkCart == null)
                    {
                        var newCart = new Cart();
                        newCart.UserId = user.UserId;
                        newCart.State = "NON ORDINATO";
                        newCart.TotalPrice = 0;
                        newCart.Date = DateTime.Now;
                        db.Cart.Add(newCart);
                        db.SaveChanges();
                    }
                    return Json(user, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "L'utente non esiste");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UserId non valido");
            }
        }

        [HttpPost]
        public ActionResult RegisterUser(Users user)
        {
            if (user != null)
            {
                user.Role = "User";
                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Utente registrato con successo");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel model state");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dati per la registrazione mancanti");
            }
        }

        public ActionResult CheckWishList(int userId, int productId)
        {
            if (userId > 0 && productId > 0)
            {
                var wishProduct = db.WishList.Where(w => w.UserId == userId && w.ProductId == productId).FirstOrDefault();

                if (wishProduct != null)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel userId/productId");
        }

        public ActionResult GetWishlist(int userId)
        {
            if (userId > 0)
            {
                var wishlist = db.WishList.Join(db.Products,
                    w => w.ProductId,
                    p => p.ProductId,
                    (w, p) => new
                    {
                        WishId = w.WishId,
                        UserId = w.UserId,
                        ProductId = p.ProductId,
                        Name = p.Name,
                        PricePerUnit = p.PricePerUnit,
                        CategoryId = p.CategoryId,
                        Discount = p.Discount,
                        Language = p.Language,
                        Image = p.Image,
                        Disponibilita = p.Disponibilita,
                        Descrizione = p.Descrizione
                    }).Where(w => w.UserId == userId).ToList();
                if (wishlist != null)
                {
                    return Json(wishlist, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nello userId");
        }

        public ActionResult AddToWishList(int userId, int productId)
        {
            if (userId > 0 && productId > 0)
            {
                WishList newWish = new WishList();
                newWish.UserId = userId;
                newWish.ProductId = productId;

                db.WishList.Add(newWish);
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK, "WishList aggiornata con successo");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel userId/productId");
        }

        [HttpDelete]
        public ActionResult RemoveFromWishList(int userId, int productId)
        {
            if (userId > 0 && productId > 0)
            {
                var wishProduct = db.WishList.Where(w => w.UserId == userId && w.ProductId == productId).FirstOrDefault();

                if (wishProduct != null)
                {
                    db.WishList.Remove(wishProduct);
                    db.SaveChanges();

                    return new HttpStatusCodeResult(HttpStatusCode.OK, "WishList aggiornata con successo");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Non esiste questo prodotto nella wishlist");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel userId/productId");
        }

        // MANAGEMENT USER SHIPPING INFO

        [HttpPost]
        public ActionResult CreateShipmentInfo(UserShipmentInfo shipmentInfo)
        {
            if (shipmentInfo != null)
            {
                if (ModelState.IsValid)
                {
                    db.UserShipmentInfo.Add(shipmentInfo);
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Informazioni utente registrate con successo");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel model state");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dati per la registrazione mancanti");
            }
        }

        public ActionResult CheckShipmentInfo(int userId)
        {
            if (userId > 0)
            {
                var shipmentInfo = db.UserShipmentInfo.Where(u => u.UserId == userId && u.IsActive == "SI").OrderByDescending(u => u.ShipmentId).Select(u => new
                {
                    ShipmentId = u.ShipmentId,
                    UserId = u.UserId,
                    Address = u.Address,
                    ZipCode = u.ZipCode,
                    City = u.City,
                    Province = u.Province,
                    CardNumber = u.CardNumber,
                    CardExpiringDate = u.CardExpiringDate,
                    CardCCV = u.CardCCV,
                    IsActive = u.IsActive,
                }).FirstOrDefault();
                if (shipmentInfo != null)
                {
                    return Json(shipmentInfo, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "IdUser non esiste");
        }

        // MANAGEMERT CARRELLO

        public ActionResult CheckCart(int userId)
        {
            if(userId > 0)
            {
                var checkCart = db.Cart.Where(c => c.UserId == userId && c.State == "NON ORDINATO").Select(c => new
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    State = c.State,
                    TotalPrice = c.TotalPrice,
                    Date = c.Date,
            }).FirstOrDefault();
                return Json(checkCart, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "L'utente non esiste");
            }
        }

        public ActionResult GetCartItems(int userId)
        { 
            if(userId > 0)
            {
                var cart = db.Cart.Where(c => c.UserId == userId && c.State == "NON ORDINATO").Select(c => new
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    State = c.State,
                    TotalPrice = c.TotalPrice,
                    Date = c.Date,
                }).FirstOrDefault();
                if (cart != null)
                {
                    var cartItems = db.Orders.Join(db.Products,
                        o => o.ProductId,
                        p => p.ProductId,
                        (o, p) => new
                        {
                            OrderId = o.OrderId,
                            ProductId = o.ProductId,
                            Quantity = o.Quantity,
                            Price = o.Price,
                            CartId = o.CartId,
                            Name = p.Name,
                            PricePerUnit = p.PricePerUnit,
                            Language = p.Language,
                            Image = p.Image,
                            Descrizione = p.Descrizione,
                        }).Where(or => or.CartId == cart.CartId).ToList();
                    return Json(cartItems, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "L'utente non esiste");
            }
        }

        public ActionResult GetTotalPrice(int userId)
        {
            if (userId > 0)
            {
                var lastCart = db.Cart.Where(c => c.UserId == userId && c.State == "NON ORDINATO").FirstOrDefault();
                if (lastCart != null)
                {
                    var order = db.Orders.Where(o => o.CartId == lastCart.CartId).FirstOrDefault();
                    if (order != null)
                    {
                        var totalPrice = db.Orders.Where(o => o.CartId == lastCart.CartId).Sum(o => o.Price);
                        return Json(totalPrice, JsonRequestBehavior.AllowGet);
                    }
                }
                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "L'utente non esiste");
            }
        }

        [HttpPut]
        public ActionResult ChangeCart(Cart cart)
        {
            var existingCart = db.Cart.Where(c => c.CartId == cart.CartId).FirstOrDefault();
            var cartSum = db.Orders.Where(o => o.CartId == cart.CartId).Sum(o => o.Price);
            if (cartSum < 90)
            {
                cartSum += 10;
            }

            if (existingCart != null && cartSum > 0)
            {
                existingCart.State = "IN ATTESA";
                existingCart.TotalPrice = cartSum;
                existingCart.Date = DateTime.Now;

                try
                {
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Ordine aggiornato con successo");
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Errore durante il salvataggio delle modifiche: {ex.Message}");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Il carrello non esiste");
        }

        [HttpGet]
        public ActionResult GetUserCarts(int userId)
        {
            if (userId > 0)
            {
                var userCarts = db.Cart.Where(c => c.UserId == userId).Select(c => new
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    State = c.State,
                    TotalPrice = c.TotalPrice,
                    Date = c.Date,
                }).ToList();
                if(userCarts.Count > 0)
                {
                    return Json(userCarts, JsonRequestBehavior.AllowGet);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Questo user non ha ordini");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Questo user non esiste");
        }

        [HttpGet]
        public ActionResult GetAllCarts()
        {
            var CartsToSend = db.Cart.Select(c => new
            {
                CartId = c.CartId,
                UserId = c.UserId,
                State = c.State,
                TotalPrice = c.TotalPrice,
                Date = c.Date,
            }).ToList();
            if(CartsToSend.Any())
            {
                return Json(CartsToSend, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Non ci sono ordini");
        }

        public ActionResult SeeCartOrders(int cartId)
        {
            if (cartId > 0)
            {
                var cart = db.Cart.Where(c => c.CartId == cartId).FirstOrDefault();
                if (cart != null)
                {
                    var orders = db.Orders.Join(db.Products,
                                  o => o.ProductId,
                                  p => p.ProductId,
                                  (o, p) => new
                                  {
                                      OrderId = o.OrderId,
                                      Quantity = o.Quantity,
                                      Price = o.Price,
                                      CartId = o.CartId,
                                      ProductId = p.ProductId,
                                      Name = p.Name,
                                      PricePerUnit = p.PricePerUnit,
                                      CategoryId = p.CategoryId,
                                      Discount = p.Discount,
                                      Language = p.Language,
                                      Image = p.Image,
                                      Disponibilita = p.Disponibilita,
                                      Descrizione = p.Descrizione
                                  })
                        .Where(o => o.CartId == cartId).ToList();
                    if (orders.Any())
                    {
                        return Json(orders, JsonRequestBehavior.AllowGet);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Non ci sono ordini");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Carrello vuoto");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id carrello non valido");
        }

        [HttpPut]
        public ActionResult EditCart(Cart cart)
        {
            if(cart != null)
            {
                if (ModelState.IsValid)
                {
                    var existingCart = db.Cart.Where(c => c.CartId == cart.CartId).FirstOrDefault();

                    if (existingCart != null)
                    {
                        existingCart.State = cart.State;
                        existingCart.Date = DateTime.Now;

                        db.SaveChanges();
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Non esiste questo carrello nel database");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel model state");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel body della fetch");
        }

        [HttpDelete]
        public ActionResult DeleteCart(int cartId)
        {
            if (cartId > 0)
            {
                var existingCart = db.Cart.Where(c => c.CartId == cartId).FirstOrDefault();

                if (existingCart != null )
                {
                    db.Cart.Remove(existingCart);
                    db.SaveChanges();

                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Non esiste questo carrello nel database");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel cartId");
        }

        // MANAGEMENT ORDINI

        [HttpPost]
        public ActionResult AddOrder(Orders order)
        {
            if (order != null)
            {
                var product = db.Products.Where(p => p.ProductId == order.ProductId).Select(p => new
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    PricePerUnit = p.PricePerUnit,
                    CategoryId = p.CategoryId,
                    Discount = p.Discount,
                    Language = p.Language,
                    Image = p.Image,
                    Disponibilita = p.Disponibilita,
                    Descrizione = p.Descrizione
                }).FirstOrDefault();

                var discountedPrice = Math.Round((product.PricePerUnit - ((product.PricePerUnit * product.Discount) / 100)) * 100) / 100;

                if (ModelState.IsValid)
                {
                    var cartItems = db.Orders.Select(o => new
                    {
                        OrderId = o.OrderId,
                        ProductId = o.ProductId,
                        Quantity = o.Quantity,
                        Price = o.Price,
                        CartId = o.CartId,
                    });
                    bool isProductInCart = cartItems.Any(p => p.ProductId == order.ProductId && p.CartId == order.CartId);

                    if (isProductInCart)
                    {
                        var existingOrder = db.Orders.Where(o => o.ProductId == order.ProductId).FirstOrDefault();

                        existingOrder.Quantity = order.Quantity;
                        existingOrder.Price = discountedPrice * order.Quantity;

                        db.SaveChanges();
                    }
                    else
                    {
                        order.Price = discountedPrice * order.Quantity;
                        db.Orders.Add(order);
                        db.SaveChanges();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Ordine registrato con successo");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel model state");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dati per la registrazione mancanti");
            }
        }

        [HttpDelete]
        public ActionResult DeleteOrder(int orderId)
        {
            if (orderId > 0)
            {
                var orderToRemove = db.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();
                if (orderToRemove != null)
                {
                    db.Orders.Remove(orderToRemove);
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Ordine eliminato con successo");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non esistente");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dati ordine errati");
        }

        // MANAGAMENT PRODOTTI

        public ActionResult GetAllProducts()
        {
            var products = db.Products
                            .Join(db.ProductCategories,
                                  p => p.CategoryId,
                                  c => c.CategoryId,
                                  (p, c) => new
                                  {
                                      ProductId = p.ProductId,
                                      Name = p.Name,
                                      PricePerUnit = p.PricePerUnit,
                                      CategoryId = p.CategoryId,
                                      Discount = p.Discount,
                                      Language = p.Language,
                                      Image = p.Image,
                                      Type = c.Type,
                                      Disponibilita = p.Disponibilita,
                                      Descrizione = p.Descrizione
                                  })
                            .Where(p => p.PricePerUnit > 0)
                            .ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProduct(int productId)
        {
            if (productId > 0)
            {
                var product = db.Products.Where(p => p.ProductId == productId).Select(p => new
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    PricePerUnit = p.PricePerUnit,
                    CategoryId = p.CategoryId,
                    Discount = p.Discount,
                    Language = p.Language,
                    Image = p.Image,
                    Disponibilita = p.Disponibilita,
                    Descrizione = p.Descrizione
                }).FirstOrDefault();
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Inserisci un ProductId valido" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProductByLan(string type)
        {
            if (type != null)
            {
                var products = db.Products.Where(p => p.Language == type).Select(p => new
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    PricePerUnit = p.PricePerUnit,
                    CategoryId = p.CategoryId,
                    Discount = p.Discount,
                    Language = p.Language,
                    Image = p.Image,
                    Disponibilita = p.Disponibilita,
                    Descrizione = p.Descrizione
                }).ToList();
                if (products.Any())
                {
                    return Json(products, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Lingua non esistente nel database");
        }

        public ActionResult GetProductBySearch(string input)
        {
            if(input != null && input != "")
            {
                var products = db.Products.Where(p => p.Name.Contains(input) && p.PricePerUnit > 0).Select(p => new
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    PricePerUnit = p.PricePerUnit,
                    CategoryId = p.CategoryId,
                    Discount = p.Discount,
                    Language = p.Language,
                    Image = p.Image,
                    Disponibilita = p.Disponibilita,
                    Descrizione = p.Descrizione
                }).ToList();
                if (products.Any())
                {
                    return Json(products, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "input errato");
        }

        public ActionResult GetNews()
        {
            var products = db.Products
                .Join(db.ProductCategories,
                      p => p.CategoryId,
                      c => c.CategoryId,
                      (p, c) => new
                      {
                          ProductId = p.ProductId,
                          Name = p.Name,
                          PricePerUnit = p.PricePerUnit,
                          CategoryId = p.CategoryId,
                          Discount = p.Discount,
                          Language = p.Language,
                          Image = p.Image,
                          Type = c.Type,
                          Disponibilita = p.Disponibilita,
                          Descrizione = p.Descrizione,
                      })
                .OrderByDescending(p => p.ProductId)
                .Where(p => p.PricePerUnit > 0)
                .Take(5)
                .ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddProduct(Products product)
        {
            if (product != null)
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Prodotto registrato con successo");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Errore nel model state");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dati per la registrazione mancanti");
            }
        }

        public class ProductDTO
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal PricePerUnit { get; set; }
            public int CategoryId { get; set; }
            public int Discount { get; set; }
            public string Language { get; set; }
            public string Image { get; set; }
            public string Disponibilita { get; set; }
            public string Descrizione { get; set; }
        }

        [HttpPut]
        public ActionResult EditProduct(Products product)
        {
            if (product != null)
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = db.Products.FirstOrDefault(p => p.ProductId == product.ProductId);

                    if (existingProduct != null)
                    {
                        existingProduct.Name = product.Name;
                        existingProduct.PricePerUnit = product.PricePerUnit;
                        existingProduct.CategoryId = product.CategoryId;
                        existingProduct.Discount = product.Discount;
                        existingProduct.Language = product.Language;
                        existingProduct.Image = product.Image;
                        existingProduct.Disponibilita = product.Disponibilita;
                        existingProduct.Descrizione = product.Descrizione;

                        db.SaveChanges();

                        var modifiedProductDTO = new ProductDTO
                        {
                            ProductId = existingProduct.ProductId,
                            Name = existingProduct.Name,
                            PricePerUnit = existingProduct.PricePerUnit,
                            CategoryId = existingProduct.CategoryId,
                            Discount = existingProduct.Discount,
                            Language = existingProduct.Language,
                            Image = existingProduct.Image,
                            Disponibilita = existingProduct.Disponibilita,
                            Descrizione = existingProduct.Descrizione,
                        };

                        return Json(modifiedProductDTO, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return HttpNotFound("Prodotto non trovato");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ModelState non valido");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Il prodotto non può essere nullo");
            }
        }

        [HttpDelete]
        public ActionResult DeleteProduct(int id)
        {
            if (id > 0)
            {
                var product = db.Products.Where(p => p.ProductId == id).FirstOrDefault();
                if(product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return Json(new { message = $"Prodotto n.{id} è stato eliminato" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = $"Il prodotto n.{id} non esiste" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "Inserisci un id prodotto valido" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}