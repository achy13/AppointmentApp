using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Domain.Models
{
    public class Offering : BaseEntity
    {
        [Required]
        public string? OfferingName {  get; set; }
        
        public string? OfferingDescription { get; set; }
        [Required]
        [Precision(5, 1)]
        public decimal? OfferingDuration { get; set; }
        public int? OfferingPrice { get; set; }

        public ICollection<ReservationOffering>? ReservationOfferings { get; set; }
    }
}
