using AppointmentApp.Domain.Models;
using AppointmentApp.Repository;
using AppointmentApp.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _reservationsRepository;

        public ReservationService(IRepository<Reservation> reservationsRepository)
        {
            _reservationsRepository = reservationsRepository;
        }

        public Reservation Add(Reservation reservation)
        {
            reservation.Id = Guid.NewGuid();
            return _reservationsRepository.Insert(reservation);
        }

        public Reservation DeleteById(Guid Id)
        {
            var reservation = _reservationsRepository.Get(selector: x => x,
                                                    predicate: x => x.Id == Id);
            return _reservationsRepository.Delete(reservation);
        }

        public List<Reservation> GetAll()
        {
            return _reservationsRepository.GetAll(selector: x => x).ToList();
        }

        public Reservation? GetById(Guid Id)
        {
            return _reservationsRepository.Get(selector: x => x,
                                            predicate: x => x.Id == Id,
                                            include: r => r
                                                .Include(r => r.reservationOfferings!)
                                                .ThenInclude(ro => ro.Offering));
        }

        public Reservation Update(Reservation reservation)
        {
            return _reservationsRepository.Update(reservation);
        }
    }
}
