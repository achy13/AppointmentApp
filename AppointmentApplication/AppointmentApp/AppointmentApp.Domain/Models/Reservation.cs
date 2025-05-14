using AppointmentApp.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Domain.Models
{
    public class Reservation : BaseEntity
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string? UserId { get; set; }
        public AppointmentAppUser? User { get; set; }
        
        public ICollection<ReservationOffering>? reservationOfferings { get; set; } 
    }
}
