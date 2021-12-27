using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using MusicManager.Data.Model.ManyToManyRelationModels;

namespace MusicManager.Data.Model
{
    public class Playlist
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
        
    }
}