using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MusicManager.Data;
using MusicManager.Data.Model;
using MusicManager.ViewModels;

namespace MusicManager.Controllers
{
    /* public class AccountController : Controller
     {
         private readonly AppDbContext _dbContext;

         public AccountController(AppDbContext dbContextContext)
         {
             _dbContext = dbContextContext;
         }

         public IActionResult Login()
         {
             return View();
         }

         [HttpPost]
         public async Task<IActionResult> Login(LoginViewModel model)
         {
             if (ModelState.IsValid)
             {
                 User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                 if (user != null)
                 {
                     await Authenticate(model.Login); // аутентификация

                     return RedirectToAction("Index", "Home");
                 }
                 ModelState.AddModelError("", "Некорректные логин и(или) пароль");
             }
             return View(model);
         }



         [HttpGet]
         public IActionResult Register()
         {
             return View();
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Register(RegisterViewModel model)
         {
             if (ModelState.IsValid)
             {
                 User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                 if (user == null)
                 {
                     _dbContext.Users.Add(model.Model);
                     await _dbContext.SaveChangesAsync();

                     await Authenticate(model.Login);

                     return RedirectToAction("Index", "Home");
                 }
                 else
                     ModelState.AddModelError("", "Некорректные логин и(или) пароль");
             }
             return View(model);
         }

         private async Task Authenticate(string userName)
         {
             var claims = new List<Claim>
             {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
             };
             ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
             var x = User.Identity.IsAuthenticated;
             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));


         }

         public async Task<IActionResult> Logout()
         {
             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
             return RedirectToAction("Login", "Account");
         }
     }*/
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        public AccountController(AppDbContext dbContextContext)
        {
            _dbContext = dbContextContext;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _dbContext.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    var success = await Authenticate(user);
                    if (!success)
                    {
                        return Content("Ваш пользователь был заблокирован");
                    }// аутентификация
                    if (user.Role.Name == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Playlists");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    Role userRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "user");
                    model.Model.Role = userRole;
                    _dbContext.Users.Add(model.Model);
                    await _dbContext.SaveChangesAsync();

                    await Authenticate(model.Model); // аутентификация

                    return RedirectToAction("Index", "Playlists");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task<bool> Authenticate(User user)
        {
            if (user.IsBlocked)
                return false;
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),

                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            return true;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }

}
