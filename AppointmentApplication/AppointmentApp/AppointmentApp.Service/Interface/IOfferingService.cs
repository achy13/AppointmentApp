using AppointmentApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Interface
{
    public interface IOfferingService
    {
        List<Offering> GetAll();
        Offering? GetById(Guid Id);
        Offering Update(Offering offering);
        Offering DeleteById(Guid Id);
        Offering Add(Offering offering);
        
    }
}
