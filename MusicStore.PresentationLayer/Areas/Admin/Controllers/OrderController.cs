using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.ApplicationLayer;
using MusicStore.ApplicationLayer.Payment;
using MusicStore.ApplicationLayer.ViewModels;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MusicStore.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _uow;

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(ProjectConstant.Role_Admin) || User.IsInRole(ProjectConstant.Role_Emp))
                orderHeaderList = _uow.OrderHeader.GetAll(includeProperties: "AppUser");
            else
                orderHeaderList = _uow.OrderHeader.GetAll(u => u.AppUserId == claim.Value, includeProperties: "AppUser");

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(x => x.PaymentStatus == PaymentStripeStatus.PaymentStatusDelayedPayment);
                    break;

                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(x =>x.OrderStatus == PaymentStripeStatus.StatusApproved
                                        || x.OrderStatus == PaymentStripeStatus.StatusInProcess
                                        || x.OrderStatus == PaymentStripeStatus.StatusPending);
                    break;

                case "completed":
                    orderHeaderList = orderHeaderList.Where(x => x.OrderStatus == PaymentStripeStatus.StatusShipping);
                    break;

                case "rejected":
                    orderHeaderList = orderHeaderList.Where(x => x.OrderStatus == PaymentStripeStatus.StatusCancelled || x.OrderStatus == PaymentStripeStatus.StatusRefund || x.OrderStatus == PaymentStripeStatus.PaymentStatusRejected);
                    break;

                default:
                    break;
            }


            return Json(new { data = orderHeaderList });
        }


        public IActionResult Details(int id)
        {
            OrderDetailsVM = new OrderDetailsVM
            {
                OrderHeader = _uow.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: "AppUser"),
                OrderDetails = _uow.OrderDetails.GetAll(x => x.OrderId == id, includeProperties: "Product")

            };
            return View(OrderDetailsVM);
        }


        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Emp)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            orderHeader.OrderStatus = PaymentStripeStatus.StatusInProcess;
            _uow.Commit();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Emp)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(x => x.Id == OrderDetailsVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderDetailsVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailsVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = PaymentStripeStatus.StatusShipping;
            orderHeader.ShippingDate = DateTime.Now;

            _uow.Commit();


            return RedirectToAction("Index");
        }

        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            if (orderHeader.PaymentStatus == PaymentStripeStatus.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                orderHeader.OrderStatus = PaymentStripeStatus.StatusRefund;
                orderHeader.PaymentStatus = PaymentStripeStatus.StatusRefund;
            }
            else
            {
                orderHeader.OrderStatus = PaymentStripeStatus.StatusCancelled;
                orderHeader.PaymentStatus = PaymentStripeStatus.StatusCancelled;
            }

            _uow.Commit();
            return RedirectToAction("Index");
        }
    }

}
