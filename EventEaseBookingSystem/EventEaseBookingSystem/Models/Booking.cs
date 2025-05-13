using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Event is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an event")]
        public int? EventId { get; set; }
        public Event Event { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue")]
        public int? VenueId { get; set; }
        public Venue Venue { get; set; }
    }
}
