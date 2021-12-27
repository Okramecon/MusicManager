using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicManager.Data;
using MusicManager.Data.Model;
using MusicManager.Data.Model.ManyToManyRelationModels;
using MusicManager.ViewModels;

namespace MusicManager.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class PlaylistsController : Controller
    {
        private readonly AppDbContext _context;

        public PlaylistsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Playlists
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlistModels = await _context.Playlists.Include(p => p.PlaylistTracks).ThenInclude(p => p.Track)
                .Where(p => p.Owner.Login == User.Identity.Name).ToListAsync();
            var playlistViewModels = playlistModels.Select(p =>
                new PlaylistViewModel(p,
                    p.PlaylistTracks.Select(p =>TrackViewModel.Load(p.TrackId, _context)).ToList()));
            return View(playlistViewModels);
        }

        // GET: Playlists?userId=
        [Authorize(Roles = "admin")]
        
        public async Task<IActionResult> UserPlaylists([FromQuery]int userId)
        {
            var user = _context.Users.Find(userId);
            if (user is null)
                return NotFound();

            var playlistModels = await _context.Playlists.Include(p => p.PlaylistTracks).ThenInclude(p => p.Track)
                .Where(p => p.OwnerId == userId).ToListAsync();
            var playlistViewModels = playlistModels.Select(p =>
                new PlaylistViewModel(p,
                    p.PlaylistTracks.Select(p => TrackViewModel.Load(p.TrackId, _context)).ToList()));
            ViewData["userName"] = user.Login;

            return View(playlistViewModels);
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

                if (_context.Playlists.Any(p => p.Name == playlist.Name && p.OwnerId == currentUser.Id))
                {
                    ModelState.AddModelError(nameof(Playlist.Name), "Такой плейлист уже существует");
                    return View(playlist);
                }

                playlist.Owner = currentUser;
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user =
                    (await _context.Playlists.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == playlist.Id))
                    .Owner;

                if (_context.Playlists.Any(p => p.Name == playlist.Name && p.OwnerId == user.Id && p.Id != playlist.Id))
                {
                    ModelState.AddModelError(nameof(Playlist.Name), "Такой плейлист уже существует");
                    return View(playlist);
                }

                var playlistDbModel = await _context.Playlists.FindAsync(playlist.Id);
                playlistDbModel.Name = playlist.Name;
                try
                {
                    _context.Update(playlistDbModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (user.Login != User.Identity.Name)
                {
                    return RedirectToAction(nameof(UserPlaylists), new {userId = user.Id});
                }
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = (await _context.Playlists.Include(u=>u.Owner).FirstOrDefaultAsync(u=>u.Id == id));
            var owner = playlist.Owner;
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            if (owner.Login != User.Identity.Name)
            {
                return RedirectToAction(nameof(UserPlaylists), new {userId = playlist.OwnerId});
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }


        public async Task<IActionResult> Remove(int id, int playlistId)
        {
            var owner = (await _context.Playlists.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == playlistId))
                ?.Owner;
            try
            {
                _context.PlaylistTracks.Remove(new PlaylistTrack() { PlaylistId = playlistId, TrackId = id });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return NotFound();
            }

            if (owner.Login != User.Identity.Name)
            {
                return RedirectToAction(nameof(UserPlaylists), new {userId = owner.Id});
            }            
            return RedirectToAction(nameof(Index));
        }
    }
}
