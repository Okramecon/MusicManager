using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MusicManager.Data;
using MusicManager.Data.Model;
using MusicManager.Data.Model.ManyToManyRelationModels;
using SQLitePCL;

namespace MusicManager.ViewModels
{
    public class TrackViewModel : ViewModelBase<Track>
    {
        //private AuthorViewModel Author { get; set; }
        //private AlbumViewModel Album { get; set; }

        public TrackViewModel(Track model) : base(model)
        {
            
        }

        public TrackViewModel()
        {
        }

        public static async Task<TrackViewModel> LoadAsync(int id, AppDbContext context)
        {
            var model = await context.Tracks.FindAsync(id);
            if (model is null)
                return null;
            var result = new TrackViewModel(model);
            result.TrackGenres = model.TrackGenres.Select(s => s.GenreId).ToList();
            result.TrackPlaylists = model.PlaylistTracks.Select(s => s.PlaylistId).ToList();
            return result;
        }

        public static TrackViewModel Load(int id, AppDbContext context)
        {
            var loadTask = LoadAsync(id, context);
            loadTask.Wait();
            return loadTask.Result;
        }

        public int Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Название трека не может быть пустым")]
        [Display(Name = "Название трека")]
        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Дата релиза трека не может быть пустой")]
        [Display(Name = "Дата релиза трека")]
        public DateTime ReleaseDate
        {
            get => Model.ReleaseDate;
            set => Model.ReleaseDate = value;
        }

        public string Url
        { 
            get => Model.Url;
            set => Model.Url = value; 
        }

        [Required(ErrorMessage = "Укажите автора песни.")]
        [Display(Name = "Автор")]
        public int AuthorId
        {
            get => Model.AuthorId;
            set => Model.AuthorId = value;
        }

        public string AuthorName => Model.Author?.Name;

        [Display(Name = "Альбом")]
        public int? AlbumId
        {
            get => Model.AlbumId;
            set => Model.AlbumId = value;
        }

        public string AlbumName => Model.Album?.Name;

        [Display(Name = "Жанры")]
        public ICollection<int> TrackGenres { get; set; }

        [Display(Name = "Добавить в плейлисты")]
        public ICollection<int> TrackPlaylists { get; set; }

        public ICollection<SelectListItem> AvailableGenres { get; set; }
        public ICollection<SelectListItem> AvailableAuthors { get; set; }
        public ICollection<SelectListItem> AvailableAlbums { get; set; }
        public ICollection<SelectListItem> AvalilablePlaylists { get; set; }

    }
}