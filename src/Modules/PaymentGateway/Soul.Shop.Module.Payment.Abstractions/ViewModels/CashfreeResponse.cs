﻿namespace Soul.Shop.Module.Payment.Abstractions.ViewModels
{
    public class CashfreeResponse
    {
        public string OrderId { get; set; }

        public string OrderAmount { get; set; }

        public string ReferenceId { get; set; }

        public string TxStatus { get; set; }

        public string PaymentMode { get; set; }

        public string TxMsg { get; set; }

        public string TxTime { get; set; }

        public string Signature { get; set; }
    }
}
