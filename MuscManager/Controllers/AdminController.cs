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
using MusicManager.ViewModels;

namespace MusicManager.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Users.Include(u => u.Role).Where(u=>u.Login != User.Identity.Name).Select(u=>new UserViewModel(u));
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/CreateAdmin
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // POST: Admin/CreateAdmin
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin([Bind("Id,Login,Password,RoleId")] User user)
        {
            if (ModelState.IsValid)
            {

                if (_context.Users.Any(p => p.Login == p.Login))
                {
                    ModelState.AddModelError(nameof(Data.Model.User.Login), "Такой пользователь уже существует");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                    return View(user);
                }
                user.Role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = new UserViewModel(await _context.Users.FindAsync(id));
            if (user.Model == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userDbModel = await _context.Users.FindAsync(id);
                
                try
                {
                    userDbModel.RoleId = user.RoleId;   
                    _context.Update(userDbModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Admin/Block/5
        public async Task<IActionResult> Block(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserViewModel(user));
        }

        // POST: Admin/Block/5
        [HttpPost, ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            user.IsBlocked = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public async Task<IActionResult> UnBlock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsBlocked);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserViewModel(user));
        }

        // POST: Admin/Block/5
        [HttpPost, ActionName("UnBlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBlockConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            user.IsBlocked = false;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
