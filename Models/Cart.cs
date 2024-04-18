namespace PokeCollector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cart")]
    public partial class Cart
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

        public virtual Orders Orders { get; set; }

        public virtual Users Users { get; set; }
    }
}
