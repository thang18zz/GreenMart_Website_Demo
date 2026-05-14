namespace PTDA_Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PurchaseReceipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseReceipt()
        {
            PurchaseReceiptDetails = new HashSet<PurchaseReceiptDetail>();
        }

        [Key]
        public int ReceiptID { get; set; }

        public int SupplierID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseReceiptDetail> PurchaseReceiptDetails { get; set; }

        public virtual User User { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
