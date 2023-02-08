using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBestAuthorizeBased.Data;
using MovieBestAuthorizeBased.Models;
using MovieBestAuthorizeBased.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased.Controllers
{
    //[Authorize (Roles = "SuperAdmin")]
    [Authorize (Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task< IActionResult > Index()
        {
            List<Genre> allGenres =await _context.Genres.ToListAsync();
            return View(allGenres);
        }
        public  async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> Add(GenreViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            List<GenreViewModel> genres = await _context.Genres.Select(g => new GenreViewModel { Name = g.Name}).ToListAsync();

            if (genres.Any(g => g.Name == model.Name))
            {
                ModelState.AddModelError("Name", "This Genre is Already Exists");
                return View(model);
            }
            Genre genre = new ()
            {
                Name = model.Name
            };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(byte? id)
        {
            Genre genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if(genre == null)
                return NotFound();

            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
