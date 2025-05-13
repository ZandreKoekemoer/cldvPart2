using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenuesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            //Reference Part 7, add search to an ASP.NET Core MVC app

            // According to Microsoft Learn (2025), LINQ queries in ASP.NET Core allow for filtering data from the database context.
            // This helpsed me to use the search function in my code
            ViewData["CurrentFilter"] = searchString;
            var venues = from v in _context.Venues select v;

            if (!string.IsNullOrEmpty(searchString))
            {
                venues = venues.Where(v => v.Name.Contains(searchString) || v.Location.Contains(searchString));
            }

            return View(await venues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageUrl = await UploadImageToBlobAsync(imageFile);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        venue.ImageUrl = await UploadImageToBlobAsync(imageFile);
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venues.Any(e => e.VenueId == venue.VenueId)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Prevents deleting if Venue is assiosiated with a booking
            var venue = await _context.Venues.FindAsync(id);
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has associated bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            //Reference Quickstart: Upload, download, and list blobs with Azure Blob Storage v12 SDK in .NET

            // According to Microsoft (2024), Blob Storage is used to upload and store unstructured data like images and documents.
            // I used this reference to implement secure, scalable image uploads for venues in my system.

            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            string containerName = _configuration["AzureBlobStorage:VenueContainerName"];

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString();
        }
    }
}
/* Microsoft. 2024. Quickstart: Upload, download, and list blobs with Azure Blob Storage v12 SDK in .NET. (Version 2.0) [Source code]. Available at: <https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet> [Accessed 13 May 2025].
*/
/*
Reece Waving. 2025. Getting started with MVC (Version 2.0) [Source code].
Available at:
<https://www.youtube.com/watch?v=eXGo2-nZnzk&list=PL480DYS-b_kevhFsiTpPIB2RzhKPig4iK&index=4>
[Accessed 16 March 2025]. 
*/
/*Microsoft. 2025. Part 7, add search to an ASP.NET Core MVC app. (Version 2.0) [Source code]. Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-9.0> [Accessed 13 May 2025].

*/