namespace PokeCollector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Products()
        {
            Orders = new HashSet<Orders>();
            WishList = new HashSet<WishList>();
        }

        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        public decimal PricePerUnit { get; set; }

        public int CategoryId { get; set; }

        public int Discount { get; set; }

        [Required]
        [StringLength(50)]
        public string Language { get; set; }

        public string Image { get; set; }

        [StringLength(2)]
        public string Disponibilita { get; set; }

        public string Descrizione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }

        public virtual ProductCategories ProductCategories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WishList> WishList { get; set; }
    }
}
