
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EventEaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.Event.EventName.Contains(searchString) ||
                    b.Venue.Name.Contains(searchString) ||
                    b.BookingDate.ToString().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await bookings.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingDate,EventId,VenueId")] Booking booking)
        {
            DateTime selectedDate = booking.BookingDate.Date;

            //Reference: Microsoft Learn.2024. Working with Data in ASP.NET Core – Entity Framework Core.
            // According to Microsoft Learn (2024), validation logic can be handled in controllers using LINQ to query the database context.
            // I used this method to prevent double bookings by checking if a booking with the same venue, event, and date already exists in the database.

            bool isDoubleBooked = await _context.Bookings
                .AnyAsync(b => b.VenueId == booking.VenueId && b.BookingDate.Date == selectedDate);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "This venue is already booked on the selected date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,EventId,VenueId")] Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

            DateTime selectedDate = booking.BookingDate.Date;

            bool isDoubleBooked = await _context.Bookings
                .AnyAsync(b =>
                    b.BookingId != booking.BookingId &&
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == selectedDate);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "This venue is already booked on the selected date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}

/*Microsoft. 2024. Working with Data in ASP.NET Core – Entity Framework Core. (Version 2.0) [Source code]. Available at: <https://learn.microsoft.com/en-us/ef/core/querying/> [Accessed 13 May 2025].
  Microsoft. 2025. Part 7, add search to an ASP.NET Core MVC app. (Version 2.0) [Source code]. Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-9.0> [Accessed 13 May 2025].
*/

/*
Reece Waving. 2025. Getting started with MVC (Version 2.0) [Source code].
Available at:
<https://www.youtube.com/watch?v=eXGo2-nZnzk&list=PL480DYS-b_kevhFsiTpPIB2RzhKPig4iK&index=4>
[Accessed 16 March 2025]. 
*/
