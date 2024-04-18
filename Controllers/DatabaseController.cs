using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
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
                                  })
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
                    Image = p.Image
                }).FirstOrDefault();
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Inserisci un ProductId valido" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetUser(string email, string password)
        {
            if (email != "")
            {
                var user = db.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
                if (user != null)
                {
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
        public ActionResult CheckUser(int id)
        {
            if (id > 0)
            {
                var user = db.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (user != null)
                {
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