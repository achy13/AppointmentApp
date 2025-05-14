using AppointmentApp.Domain.Identity;
using AppointmentApp.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.Repository
{
    public class ApplicationDbContext : IdentityDbContext<AppointmentAppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Offering> Offerings { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ReservationOffering> ReservationOfferings { get; set; }
    }
}
