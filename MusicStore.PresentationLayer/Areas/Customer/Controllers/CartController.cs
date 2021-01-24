using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MusicStore.ApplicationLayer;
using MusicStore.ApplicationLayer.Extensions;
using MusicStore.ApplicationLayer.Payment;
using MusicStore.ApplicationLayer.ViewModels;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using Stripe;
using System;
using System.Collections.Generic;
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


        public IActionResult Plus(int id)
        {
            #region Other option
            //var cart = _uow.ShoppingCart.GetFirstOrDefault(x => x.Id == id, includeProperties: "Product");
            //if (cart == null)
            //{
            //    return Json(false);
            //    //return RedirectToAction("Index");
            //}
            //else
            //{
            //    cart.Count += 1;
            //    cart.Price = MusicStore.ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(cart.Count,cart.Product.Price,cart.Product.Price50,cart.Product.Price100);
            //    _uow.Commit();
            //    //return RedirectToAction("Index");
            //    //var allShoppingCart = _uow.ShoppingCart.GetAll();

            //    return Json(false);
            //    //return Json(allShoppingCart);
            //}
            #endregion
            try
            {
                var cart = _uow.ShoppingCart.GetFirstOrDefault(x => x.Id == id, includeProperties: "Product");

                if (cart == null)
                    return Json(false);
                

                cart.Count += 1;
                cart.Price = CartExtension.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                _uow.Commit();
                

                return Json(true);
              
            }
            catch (Exception ex)
            {
                return Json(false);
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public  IActionResult SummaryPost(CartVM cart,string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            cart.OrderHeader.AppUser = _uow.AppUser.GetFirstOrDefault(x => x.Id == claims.Value, includeProperties: "Company");

            cart.CartList = _uow.ShoppingCart.GetAll(x => x.AppUserId == claims.Value, includeProperties:"Product");
            
            cart.OrderHeader.PaymentStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.PaymentStatusPending;
            cart.OrderHeader.OrderStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.StatusPending;
            cart.OrderHeader.AppUserId = claims.Value;
            cart.OrderHeader.OrderDate = DateTime.Now;
            _uow.OrderHeader.Add(cart.OrderHeader);
            _uow.Commit();

            List<OrderDetails> orderDetails = new List<OrderDetails>();
            foreach (var orderDetail in cart.CartList)
            {
                orderDetail.Price = MusicStore.ApplicationLayer.Extensions.CartExtension.GetPriceBaseOnQuantity(orderDetail.Count, orderDetail.Product.Price, orderDetail.Product.Price50, orderDetail.Product.Price100);

                OrderDetails oDetails = new OrderDetails() 
                {
                    ProductId = orderDetail.ProductId,
                    OrderId = cart.OrderHeader.Id,
                    Price = orderDetail.Price,
                    Count = orderDetail.Count
                };
                cart.OrderHeader.OrderTotal += oDetails.Count * oDetails.Price;
                _uow.OrderDetails.Add(oDetails);
            }

            _uow.ShoppingCart.RemoveRange(cart.CartList);
            
            HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, 0);

            if (stripeToken == null)
            {
                cart.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                cart.OrderHeader.PaymentStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.PaymentStatusDelayedPayment;
                cart.OrderHeader.OrderStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.StatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(cart.OrderHeader.OrderTotal*100),
                    Currency= "usd",
                    Description="Order Id : " + cart.OrderHeader.Id,
                    Source= stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    cart.OrderHeader.PaymentStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.PaymentStatusRejected;
                
                else
                    cart.OrderHeader.TransactionId = charge.BalanceTransactionId;

                if (charge.Status.ToLower() == "succeeded")
                {
                    cart.OrderHeader.PaymentStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.PaymentStatusApproved;
                    cart.OrderHeader.OrderStatus = MusicStore.ApplicationLayer.Payment.PaymentStripeStatus.StatusApproved;
                    cart.OrderHeader.PaymentDate = DateTime.Now;

                }
            }
            _uow.Commit();

            return RedirectToAction("OrderConfirmation", "Cart", new { id = cart.OrderHeader.Id });


        }


        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }





    }
}
