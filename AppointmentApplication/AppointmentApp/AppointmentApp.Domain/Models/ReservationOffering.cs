using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Domain.Models
{
    public class ReservationOffering : BaseEntity
    {
        public Guid ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public Guid OfferingId { get; set; }
        public Offering? Offering { get; set; } 
    }
}
