using EventEaseBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

// This handles the database connection and maps your models to the database tables
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // These lines represent the tables in the database
    public DbSet<Venue> Venues { get; set; }
    public DbSet<EventEaseBookingSystem.Models.Event> Events { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}
