namespace PokeCollector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WishList")]
    public partial class WishList
    {
        [Key]
        public int WishId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public virtual Products Products { get; set; }

        public virtual Users Users { get; set; }
    }
}
