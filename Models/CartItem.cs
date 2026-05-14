namespace PTDA_Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CartItem
    {
        public int CartItemID { get; set; }

        public int UserID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public DateTime? AddedAt { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
