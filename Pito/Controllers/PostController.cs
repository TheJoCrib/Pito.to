using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pito.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pito.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly LoginContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostController(LoginContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.libraries.FindAsync(id);

            if (library == null)
            {
                return NotFound();
            }

            var libraryViewModel = new Libraryviewsmodel
            {
                Id = library.Id,
                Description = library.Description,
                Title = library.Title,
            };

            return View(libraryViewModel); // Ensure you are passing the correct model type here
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Title,Fiyat,photo")] Libraryviewsmodel libraryViewModel)
        {
            if (id != libraryViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingLibrary = await _context.libraries.FindAsync(id);

                    if (existingLibrary == null)
                    {
                        return NotFound();
                    }

                    // Handle file upload
                    if (libraryViewModel.photo != null)
                    {
                        string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                        existingLibrary.Image = $"{Guid.NewGuid()}_{libraryViewModel.photo.FileName}";
                        string filePath = Path.Combine(uploadFolder, existingLibrary.Image);
                        libraryViewModel.photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }

                    // Update other properties
                    existingLibrary.Description = libraryViewModel.Description;
                    existingLibrary.Title = libraryViewModel.Title;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(libraryViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libraryviewsmodel lib)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                if (lib.photo != null)
                {
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    filename = $"{Guid.NewGuid()}_{lib.photo.FileName}";
                    string filepath = Path.Combine(uploadFolder, filename);
                    lib.photo.CopyTo(new FileStream(filepath, FileMode.Create));
                }

                Library l = new Library
                {
                    Id = lib.Id,
                    Description = lib.Description,
                    Title = lib.Title,
                    Image = filename
                };

                _context.libraries.Add(l);
                _context.SaveChanges();
                ViewBag.success = "Eklendi";
                return View();
            }
            return View(lib);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.libraries.FindAsync(id);

            if (library == null)
            {
                return NotFound();
            }

            var libraryViewModel = new Libraryviewsmodel
            {
                Id = library.Id,
                Description = library.Description,
                Title = library.Title,
            };

            return View(libraryViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var library = await _context.libraries.FindAsync(id);

            if (library != null)
            {
                _context.libraries.Remove(library);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LibraryExists(int id)
        {
            return _context.libraries.Any(e => e.Id == id);
        }
    }
}