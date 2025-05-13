using System;
using System.ComponentModel.DataAnnotations;
//This keeps your model code organized under the Models folder of your project.
namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        
        [Key]
        public int EventId { get; set; }
        //Reference Data Annotations in ASP.NET Core

        //According to Microsoft(2024) A value is considered present only if input is entered for it. Therefore, client-side validation handles non-nullable types the same as nullable types.
        //I used this reference because the project displayed that it needs  this annotation to work properly and so i installed it
        public required string EventName { get; set; }
        //Required makes sure the user must fill this in when creating or editing an event
        public required DateTime EventDate { get; set; }

        //means it’s nullable  so you can leave it blank if you don’t know the capacity
        public string? Description { get; set; }
        // This is a foreign key that is used to link the event to a specific venue
        public int VenueId { get; set; }

        //Reference What are the Navigation Properties in Entity Framework

        //According to StackOverFlow(2024) Navigation properties represents related entities to the principal entity. Foreign Keys are usually represented by navigation properties.
        //I used this reference because i was unsure how to link the modules within a class
        // This is a navigation property, which lets you access all the venue details directly from the event.
        public Venue? Venue { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
/* Referencees
* 
* Microsoft.2024.Data Annotations in ASP.NET Core.(Version 2.0) [Source code].Available at:<https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation[Accessed>[Accessed 7 April 2025].
* 
*  StackOverFlow.2024.What are the Navigation Properties in Entity Framework.(Version 2.0) [Source code].Available at:<https://stackoverflow.com/questions/11508103/what-are-the-navigation-properties-in-entity-framework[Accessed>[Accessed 6 April 2025]
*  
*  Microsoft.2024. Migrations - EF Core.(Version 2.0) [Source code].Available at:<https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/ >[Accessed 7 April 2025].
* 
* 
*  */
