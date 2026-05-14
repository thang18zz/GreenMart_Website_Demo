namespace PTDA_Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            OrderDetails = new HashSet<OrderDetail>();
            PurchaseReceiptDetails = new HashSet<PurchaseReceiptDetail>();
        }

        public int ProductID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [StringLength(255)]
        public string ImageURL { get; set; }

        public int? StockQuantity { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItems { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseReceiptDetail> PurchaseReceiptDetails { get; set; }
    }
}
