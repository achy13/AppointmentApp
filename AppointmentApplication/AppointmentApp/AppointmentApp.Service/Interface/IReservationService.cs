using AppointmentApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Interface
{
    public interface IReservationService
    {
        List<Reservation> GetAll();
        Reservation? GetById(Guid Id);
        Reservation Update(Reservation reservation);
        Reservation DeleteById(Guid Id);
        Reservation Add(Reservation reservation);
    }
}
