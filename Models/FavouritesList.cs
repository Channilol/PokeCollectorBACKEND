namespace PokeCollector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FavouritesList")]
    public partial class FavouritesList
    {
        [Key]
        public int FavId { get; set; }

        public int UserId { get; set; }

        public int PokemonId { get; set; }

        public virtual Users Users { get; set; }
    }
}
