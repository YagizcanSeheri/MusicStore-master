using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicStore.ApplicationLayer;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using MusicStore.PresentationLayer.Models;

namespace MusicStore.PresentationLayer.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _uow.Product.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim!= null)
            {
                var shoppingCount = _uow.ShoppingCart.GetAll(x => x.AppUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, shoppingCount);
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _uow.Product.GetFirstOrDefault(x=> x.Id ==id, includeProperties:"Category,CoverType");

            ShoppingCart cart = new ShoppingCart() 
            {
                Product = product,
                ProductId = product.Id,

            };
            return View(cart);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.AppUserId = claim.Value;

                ShoppingCart fromDb = _uow.ShoppingCart.GetFirstOrDefault(
                    x=> x.AppUserId == shoppingCart.AppUserId && x.ProductId ==shoppingCart.ProductId,includeProperties:"Product");
                if (fromDb == null)
                {
                    //Ekleme
                    _uow.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    //Guncelleme
                    fromDb.Count += shoppingCart.Count;
                }

                _uow.Commit();
                var shoppingCount = _uow.ShoppingCart.GetAll(x => x.AppUserId == shoppingCart.AppUserId).ToList().Count();
                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart,shoppingCount);
                return RedirectToAction("Index");
            }
            else
            {
                var product = _uow.Product.GetFirstOrDefault(x => x.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id,

                };
                return View(cart);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
