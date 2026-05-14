namespace PTDA_Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PurchaseReceiptDetail
    {
        [Key]
        public int ReceiptDetailID { get; set; }

        public int ReceiptID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal CostPrice { get; set; }

        public virtual Product Product { get; set; }

        public virtual PurchaseReceipt PurchaseReceipt { get; set; }
    }
}
