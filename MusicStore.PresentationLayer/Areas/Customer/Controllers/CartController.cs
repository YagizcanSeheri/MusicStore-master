using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MusicStore.ApplicationLayer;
using MusicStore.ApplicationLayer.ViewModels;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MusicStore.PresentationLayer.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender,UserManager<IdentityUser> userManager)
        {
            _uow = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            CartVM cart = new CartVM()
            {
                OrderHeader = new OrderHeader(),

                CartList = _uow.ShoppingCart.GetAll(x => x.AppUserId == claims.Value, includeProperties: "Product")
            };

            cart.OrderHeader.OrderTotal = 0;
            cart.OrderHeader.AppUser = _uow.AppUser.GetFirstOrDefault(x => x.Id == claims.Value, includeProperties: "Company");
            foreach (var shoppingCart in cart.CartList)
            {
                shoppingCart.Price =ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(shoppingCart.Count,shoppingCart.Product.Price,shoppingCart.Product.Price50,shoppingCart.Product.Price100);
                cart.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
                shoppingCart.Product.Description = ApplicationLayer.Extensions.CartExtension.ConvertToRawHtml(shoppingCart.Product.Description);


                if (shoppingCart.Product.Description.Length>50)
                {
                    shoppingCart.Product.Description = shoppingCart.Product.Description.Substring(0, 49) + "...";
                }
            }
            return View(cart);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _uow.AppUser.GetFirstOrDefault(x => x.Id == claims.Value);
            if (user == null)
                 ModelState.AddModelError(string.Empty, "Verification is empty!");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Please check your email..!!");
            return RedirectToAction("Index");
        }


        public IActionResult Plus(int cartId)
        {
            var cart = _uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            if (cart == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                cart.Count += 1;
                cart.Price = MusicStore.ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(cart.Count,cart.Product.Price,cart.Product.Price50,cart.Product.Price100);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            if (cart.Count == 1)
            {
                var cnt = _uow.ShoppingCart.GetAll(x => x.AppUserId == cart.AppUserId).ToList().Count();
                _uow.ShoppingCart.Remove(cart);
                _uow.Commit();
                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price= MusicStore.ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                _uow.Commit();
                
            }
            return RedirectToAction("Index");
        }


        public IActionResult Remove(int cartId)
        {
            var cart = _uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            var cnt = _uow.ShoppingCart.GetAll(x => x.AppUserId == cart.AppUserId).ToList().Count();
            _uow.ShoppingCart.Remove(cart);
            _uow.Commit();
            HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, cnt - 1);

            return RedirectToAction("Index");
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM cart = new CartVM()
            {
                OrderHeader = new OrderHeader(),

                CartList = _uow.ShoppingCart.GetAll(x => x.AppUserId == claims.Value, includeProperties: "Product")
            };

            cart.OrderHeader.AppUser = _uow.AppUser.GetFirstOrDefault(x => x.Id == claims.Value, includeProperties: "Company");

            foreach (var item in cart.CartList)
            {
                item.Price = MusicStore.ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

                cart.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            cart.OrderHeader.Name = cart.OrderHeader.AppUser.Name;
            cart.OrderHeader.PhoneNumber = cart.OrderHeader.AppUser.PhoneNumber;
            cart.OrderHeader.Address = cart.OrderHeader.AppUser.Address;
            cart.OrderHeader.City = cart.OrderHeader.AppUser.City;
            cart.OrderHeader.Country = cart.OrderHeader.AppUser.Country;
            cart.OrderHeader.PostCode = cart.OrderHeader.AppUser.PostCode;

            return View(cart);
        }





    }
}
