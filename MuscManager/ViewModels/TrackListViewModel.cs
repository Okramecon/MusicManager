using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicManager.ViewModels
{
    public class TrackListViewModel
    {
        public TrackListViewModel(IEnumerable<TrackViewModel> tracks, Dictionary<int, string> genres)
        {
            Tracks = tracks;
            Genres = genres;
        }

        public TrackListViewModel()
        {
        }

        public IEnumerable<TrackViewModel> Tracks { get; set; }
        public Dictionary<int, string> Genres { get; set; }
        public IEnumerable<SelectListItem> GenresSelectListItems => Genres?.Select(g=>new SelectListItem(g.Value, g.Key.ToString()));
        public Filters Filter { get; } = new Filters();

        public class Filters
        {
            [Display(Name = "Название трека")]
            public string TrackName { get; set; }

            [Required(ErrorMessage = "Укажите период выхода трека")]
            [Display(Name = "Время выхода от: ")]
            public DateTime ReleaseDateFrom { get; set; } = DateTime.MinValue;
            [Required(ErrorMessage = "Укажите период выхода трека")]
            [Display(Name = "Время выхода до: ")]
            public DateTime ReleaseDateTo { get; set; } = DateTime.Now;
            [Display(Name = "Автор/Испольнитель")]
            public string AuthorName { get; set; }
            [Display(Name = "Название альбома")]
            public string AlbumName { get; set; }
            [Display(Name = "Жанры")]
            public IEnumerable<int> Genres { get; set; }
        }
    }
}