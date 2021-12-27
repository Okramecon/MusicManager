using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MusicManager.Data.Model.ManyToManyRelationModels;

namespace MusicManager.Data.Model
{
    public class Track
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        [DefaultValue(1)]
        public int AuthorId { get; set; }

        public int? AlbumId { get; set; }


        public virtual Author Author { get; set; }
        public virtual Album Album { get; set; }
        public virtual ICollection<TrackGenre> TrackGenres { get; set; }
        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    }
}