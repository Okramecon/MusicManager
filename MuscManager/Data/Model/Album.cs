using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicManager.Data.Model
{
    public class Album
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}