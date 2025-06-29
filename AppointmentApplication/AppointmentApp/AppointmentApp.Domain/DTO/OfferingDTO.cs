using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Domain.DTO
{
    public class OfferingDTO
    {
        public string id {  get; set; }
        public string service { get; set; }
        public string description { get; set; }
        public decimal duration { get; set; }
        public double price {  get; set; }
    }
}
