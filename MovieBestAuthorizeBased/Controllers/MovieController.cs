using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBestAuthorizeBased.Constant;
using MovieBestAuthorizeBased.Data;
using MovieBestAuthorizeBased.Models;
using MovieBestAuthorizeBased.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace MovieBestAuthorizeBased.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _maxAllowedPosterSize = 1048576;
        private readonly List<string> _allowedExtenstions = new() { ".jpg", ".png" };
        public MovieController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationDbContext Context { get; }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Movie> movies = await _context.Movies.Where(m => m.IsConfirmed).OrderByDescending(m => m.Rate).ToListAsync();
            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Create";
            MovieViewModel movie = new ()
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };
            return View("MovieForm", movie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            ViewData["Title"] = "Create";
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("MovieForm", model);
            }

            var poster = files.FirstOrDefault();

            if (!CkeckFilesExtentions(poster))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("MovieForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("MovieForm", model);
            }
            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Movie movie = new ()
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                Storeline = model.Storeline,
                Poster = dataStream.ToArray(),
                UserId = user.Id,
            };
            if (User.IsInRole(DefaultRoles.Admin.ToString()))
                movie.IsConfirmed = true;

            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Edit";

            if (id == null)
                return BadRequest();

            Movie movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            ApplicationUser user =await _userManager.GetUserAsync(HttpContext.User);
            if (!CanAcces(movie.UserId,user))
                return View("AccessDenied");


            MovieViewModel model = new()
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Year = movie.Year,
                Rate = movie.Rate,
                Storeline = movie.Storeline,
                Poster = movie.Poster,
                Genres = _context.Genres.OrderBy(m => m.Name),
                IsConfirmed = false
            };
            return View("MovieForm", model);

        }
        private bool CanAcces(string userId,ApplicationUser user)
        {
            string logged = user.Id.ToUpper();
            string movieUserId =userId.ToUpper();


            return logged ==movieUserId || (logged != movieUserId && User.IsInRole(DefaultRoles.Admin.ToString())) ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieViewModel model)
        {
            ViewData["Title"] = "Edit";
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            Movie movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();

            IFormFileCollection files = Request.Form.Files;
            if (files.Any())
            {
                IFormFile poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);
                model.Poster = dataStream.ToArray();

                if (!CkeckFilesExtentions(poster))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("MovieForm", model);
                }
                if (!ckeckFileLengthIsAllowed(poster))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("MovieForm", model);
                }
                movie.Poster = model.Poster;

            }
            movie.Title = model.Title;
            movie.Storeline = model.Storeline;
            movie.Rate = model.Rate;
            movie.Year = model.Year;
            movie.GenreId = model.GenreId;

            if(User.IsInRole(DefaultRoles.Admin.ToString()))
                movie.IsConfirmed = true;
            else
                 movie.IsConfirmed = false;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            Movie movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            int genreId = movie.GenreId;
            Genre x = await _context.Genres.FindAsync(genreId);
            ViewBag.Genre = x.Name;
            return View(movie);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            Movie movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (!CanAcces(movie.UserId, user))
                return View("AccessDenied");

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles ="Admin")]
        public async Task< IActionResult> GetPaindingMovies()
        {
            return View(await _context.Movies.Where(m => !m.IsConfirmed).ToListAsync());
        }
        public async Task<IActionResult> ApproveMovie(int? id)
        {
            if (id == null)
                return BadRequest();

            Movie movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            movie.IsConfirmed = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(GetPaindingMovies));
        }
        private bool CkeckFilesExtentions(IFormFile file)
        {
            return (_allowedExtenstions.Contains(Path.GetExtension(file.FileName).ToLower()));
        }

        public bool ckeckFileLengthIsAllowed(IFormFile file)
        {
            return (file.Length <= _maxAllowedPosterSize);
        }
       // [HttpPost]
        public async Task<IActionResult> MyMovies(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"No User With ID = {id}");
            //List<Movie> movies =await _context.Movies.Where(m => m.UserId.Equals(id)).ToListAsync();

            return View(await _context.Movies.Where(m => m.UserId.Equals(id)).ToListAsync());
        } 

    }
}
