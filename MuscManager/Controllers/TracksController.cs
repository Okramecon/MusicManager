using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicManager.Data;
using MusicManager.Data.Model;
using MusicManager.Data.Model.ManyToManyRelationModels;
using MusicManager.ViewModels;

namespace MusicManager.Controllers
{
    public class TracksController : Controller
    {
        private readonly AppDbContext _context;

        public TracksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tracks
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres.ToDictionaryAsync(g => g.Id, g => g.Name);
            var tracks  = await _context.Tracks.Select(t => new TrackViewModel(t)).ToListAsync();
            tracks.ForEach(t => t.TrackGenres = t.Model.TrackGenres.Select(tg => tg.GenreId).ToList());
            return View(new TrackListViewModel(tracks, genres));
        }
        

        // GET: Tracks/Create
        public async Task<IActionResult> Create()
        {
            var track = new TrackViewModel();
            await LoadSelectListItems(track);
            return View(track);
        }

        // POST: Tracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind("Id,Name,ReleaseDate")]
        public async Task<IActionResult> Create(TrackViewModel track)
        {
            if (ModelState.IsValid)
            {
                if (_context.Tracks.Any(p => p.Name == track.Name))
                {
                    ModelState.AddModelError(nameof(Track.Name), "Трек с таким именем уже существует");
                    await LoadSelectListItems(track);
                    return View(track);
                }
                _context.Add(track.Model);
                foreach (int genreId in track.TrackGenres ?? Enumerable.Empty<int>())
                {
                    _context.Add(new TrackGenre()
                        {GenreId = genreId, TrackId = track.Model.Id, Track = track.Model});
                }
                foreach (int playlistId in track.TrackPlaylists ?? Enumerable.Empty<int>())
                {
                    _context.Add(new PlaylistTrack()
                        { PlaylistId = playlistId, TrackId = track.Model.Id, Track = track.Model });
                }

                await UploadTrackAsync(track);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectListItems(track);
            return View(track);
        }
        public async Task UploadTrackAsync(TrackViewModel track)
        {
            var file = Path.Combine(Environment.CurrentDirectory, "wwwroot/Locals", track.Upload.FileName);
            track.Url = "Locals/" + track.Upload.FileName;
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await track.Upload.CopyToAsync(fileStream);
            }
        }

        private async Task LoadSelectListItems(TrackViewModel track)
        {
            var availableAuthors = await _context.Authors.Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToListAsync();
            var availableAlbums = await _context.Albums.Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToListAsync();
            var availablePlaylists = await _context.Playlists.Where(p => p.Owner.Login == User.Identity.Name).Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToListAsync();
            var availableGenres = await _context.Genres.Select(g => new SelectListItem(g.Name, g.Id.ToString())).ToListAsync();
            track.AvailableAuthors = availableAuthors;
            track.AvailableAlbums = availableAlbums;
            track.AvalilablePlaylists = availablePlaylists;
            track.AvailableGenres = availableGenres;
        }

        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            var trackViewModel = await TrackViewModel.LoadAsync(id.Value, _context);
            await LoadSelectListItems(trackViewModel);
            return View(trackViewModel);
        }

        

        // POST: Tracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] TrackViewModel track)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_context.Tracks.Any(p => p.Name == track.Name && p.Id != track.Id))
                {
                    ModelState.AddModelError(nameof(Track.Name), "Трек с таким именем уже существует");
                    await LoadSelectListItems(track);
                    return View(track);
                }
                try
                {
                    var dbModel = await _context.Tracks.FindAsync(id);
                    if (dbModel is null)
                        return NotFound();

                    //добавляем новый жанр
                    _context.AddRange(track.TrackGenres
                                          ?.Where(tg => !dbModel.TrackGenres.Select(s => s.GenreId).Contains(tg))
                                          ?.Select(genreId => new TrackGenre {GenreId = genreId, TrackId = track.Id}) ??
                                      Enumerable.Empty<TrackGenre>());
                    //удаление жанров
                    _context.RemoveRange(dbModel.TrackGenres?.Where(tg => !track.TrackGenres.Contains(tg.GenreId)) ?? Enumerable.Empty<TrackGenre>());

                    //добавляем в новые плейлисты
                    _context.AddRange(track.TrackPlaylists
                        ?.Where(pt => !dbModel.PlaylistTracks.Select(s => s.PlaylistId).Contains(pt))
                        ?.Select(tp => new PlaylistTrack {PlaylistId = tp, TrackId = track.Id}) ?? Enumerable.Empty<PlaylistTrack>());
                    //удаление из плейлистов
                    _context.RemoveRange(
                        dbModel.PlaylistTracks?.Where(pt => !track.TrackPlaylists.Contains(pt.PlaylistId)) ??
                        Enumerable.Empty<PlaylistTrack>());
                    dbModel.Name = track.Model.Name;
                    dbModel.AuthorId = track.Model.AuthorId;
                    dbModel.AlbumId = track.Model.AlbumId;
                    dbModel.ReleaseDate = track.Model.ReleaseDate;

                    _context.Update(dbModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackExists(track.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectListItems(track);
            return View(track);
        }

        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackExists(int id)
        {
            return _context.Tracks.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> Index(TrackListViewModel trackListViewModel)
        {
            var filters = trackListViewModel.Filter;
            var genres = await _context.Genres.ToDictionaryAsync(g => g.Id, g => g.Name);
            var query = _context.Tracks.Where(t =>
                t.ReleaseDate <= filters.ReleaseDateTo
                && t.ReleaseDate >= filters.ReleaseDateFrom);
            if (!string.IsNullOrWhiteSpace(filters.TrackName))
            {
                query = query.Where(track => track.Name.ToLower().Contains(filters.TrackName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filters.AlbumName))
            {
                query = query.Where(track => track.Album.Name.ToLower().Contains(filters.AlbumName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filters.AuthorName))
            {
                query = query.Where(track => track.Author.Name.ToLower().Contains(filters.AuthorName.ToLower()));
            }

            if (filters.Genres?.Any() ?? false)
            {
                query = query.Where(track => track.TrackGenres.Any(t => filters.Genres.Contains(t.GenreId)));
            }
            var tracks = await query.Select(t => new TrackViewModel(t)).ToListAsync();
            tracks.ForEach(t => t.TrackGenres = t.Model.TrackGenres.Select(tg => tg.GenreId).ToList());
            trackListViewModel.Tracks = tracks;
            trackListViewModel.Genres = genres;
            return View(trackListViewModel);
        }
    }
}
