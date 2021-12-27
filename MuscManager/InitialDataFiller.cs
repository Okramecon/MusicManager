using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using MusicManager.Data;
using MusicManager.Data.Model;
using MusicManager.Data.Model.ManyToManyRelationModels;

namespace MusicManager
{
    public class InitialDataFiller
    {
        private readonly AppDbContext _context;

        public InitialDataFiller(AppDbContext context)
        {
            _context = context;
        }

        public void FillDatabase()
        {
            var genres = new List<Genre>()
            {
                new Genre(){Name = "Рок"},
                new Genre(){Name = "Рэп"},
                new Genre(){Name = "Классика"},
                new Genre(){Name = "Джаз"},
                new Genre(){Name = "Фольк"},
                new Genre(){Name = "Индастриал рок"},
                new Genre(){Name = "Поп-музыка"},
            };
            var albums = new List<Album>()
            {
                new Album(){Name = "The Slim Shady LP"},
                new Album(){Name = "Hybrid Theory"}
            };
            var authors = new List<Author>()
            {
                new Author(){Name = "Eminem"},
                new Author(){Name = "Linking Park"}
            };
            var tracks = new List<Track>()
            {
                new Track(){Name = "With You", Album = albums.Last(), Author = authors.Last()},
                new Track(){Name = "Points of Authorit", Album = albums.Last(), Author = authors.Last()},
                new Track(){Name = "In the End", Album = albums.Last(), Author = authors.Last()},
                new Track(){Name = "My Name Is", Album = albums.First(), Author = authors.First()},
                new Track(){Name = "Rock Bottom", Album = albums.First(), Author = authors.First()},
                new Track(){Name = "I’m Shady", Album = albums.First(), Author = authors.First()},
            };
            var trackGenres = new List<TrackGenre>()
            {
                new TrackGenre(){Genre = genres[0], Track = tracks[0]},
                new TrackGenre(){Genre = genres[0], Track = tracks[1]},
                new TrackGenre(){Genre = genres[0], Track = tracks[2]},
                new TrackGenre(){Genre = genres[1], Track = tracks[2]},
                new TrackGenre(){Genre = genres[1], Track = tracks[3]},
                new TrackGenre(){Genre = genres[1], Track = tracks[4]},
                new TrackGenre(){Genre = genres[1], Track = tracks[5]},
            };
           
            var defaultUser = new User(){Login = "user", Password = "123", Role = _context.Roles.FirstOrDefault(r=>r.Name == "user")};

            var playlists = new List<Playlist>()
            {
                new Playlist(){Name = "Избранное", Owner = defaultUser}
            };

            var playlistTracks = new List<PlaylistTrack>()
            {
                new PlaylistTrack(){Playlist =  playlists[0], Track = tracks[0]},
                new PlaylistTrack(){Playlist =  playlists[0], Track = tracks[1]},
                new PlaylistTrack(){Playlist =  playlists[0], Track = tracks[4]},
            };

            if (!_context.Users.Any(u=>u.Role.Name == "user"))
            {
                _context.AddRange(genres);
                _context.AddRange(albums);
                _context.AddRange(authors);
                _context.AddRange(tracks);
                _context.AddRange(trackGenres);
                _context.AddRange(defaultUser);
                _context.AddRange(playlists);
                _context.AddRange(playlistTracks);
                _context.SaveChanges();
                Console.WriteLine("Database initialized");
            }
        }
    }
}
