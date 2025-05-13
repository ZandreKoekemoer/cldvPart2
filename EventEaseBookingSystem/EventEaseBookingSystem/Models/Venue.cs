
using Humanizer;
using System.ComponentModel.DataAnnotations;

// This keeps your model code organized under the Models folder of your project.
namespace EventEaseBookingSystem.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        public required string Name { get; set; }

        // Venue location is optional by using "?"
        public string? Location { get; set; }

        // Means it’s nullable so you can leave it blank if you don’t know the capacity
        public int? Capacity { get; set; }

        public string? ImageUrl { get; set; }

        // This is a navigation property that links the venue to the list of events happening there
        public ICollection<Event>? Events { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}


