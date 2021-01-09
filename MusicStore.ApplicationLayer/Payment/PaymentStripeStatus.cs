using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.Payment
{
    public static class PaymentStripeStatus
    {
        //payment status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "Delayed";



        //order status
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "In Process";
        public const string StatusShipping = "Shipping";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefund = "Refund";
    }
}
