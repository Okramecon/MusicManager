using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicManager.Data.Model;

namespace MusicManager.ViewModels
{
    public class PlaylistViewModel : ViewModelBase<Playlist>
    {
        public PlaylistViewModel(Playlist model, IEnumerable<TrackViewModel> tracks) : base(model)
        {
            Tracks = tracks;
        }

        public PlaylistViewModel()
        {
            Tracks = new List<TrackViewModel>();
        }

        public IEnumerable<TrackViewModel> Tracks { get; set; }

        public int Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }
    }
}
