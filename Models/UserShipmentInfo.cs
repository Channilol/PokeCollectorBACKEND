namespace PokeCollector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserShipmentInfo")]
    public partial class UserShipmentInfo
    {
        [Key]
        public int ShipmentId { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string Province { get; set; }

        public string CardNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string CardExpiringDate { get; set; }

        public int CardCCV { get; set; }

        [Required]
        [StringLength(2)]
        public string IsActive { get; set; }

        public virtual Users Users { get; set; }
    }
}
