using System.ComponentModel.DataAnnotations;

namespace MusicManager.Data.Model.ManyToManyRelationModels
{
    public class PlaylistTrack
    {
        [Required]
        public int PlaylistId { get; set; }

        [Required]
        public int TrackId { get; set; }

        public virtual Playlist Playlist { get; set; }
        public virtual Track Track { get; set; }
    }
}