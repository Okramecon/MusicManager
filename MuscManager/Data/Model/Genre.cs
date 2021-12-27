using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MusicManager.Data.Model.ManyToManyRelationModels;

namespace MusicManager.Data.Model
{
    public class Genre
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<TrackGenre> Tracks { get; set; }
    }
}