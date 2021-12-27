using System.ComponentModel.DataAnnotations;

namespace MusicManager.Data.Model.ManyToManyRelationModels
{
    public class TrackGenre
    {
        [Required] 
        public int TrackId { get; set; }

        [Required]
        public int GenreId { get; set; }

        public virtual Track Track { get; set; }
        public virtual Genre Genre { get; set; }
    }
}