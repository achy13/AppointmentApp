using AppointmentApp.Domain.Models;
using AppointmentApp.Repository;
using AppointmentApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Implementation
{
    public class OfferingService : IOfferingService
    {
        private readonly IRepository<Offering> _offeringRepository;

        public OfferingService(IRepository<Offering> offeringRepository)
        {
            _offeringRepository = offeringRepository;
        }

        public Offering Add(Offering offering)
        {
            offering.Id = Guid.NewGuid();
            return _offeringRepository.Insert(offering);
        }

        public Offering DeleteById(Guid Id)
        {
            var offering = _offeringRepository.Get(selector: x => x,
                                                    predicate: x => x.Id == Id);
            return _offeringRepository.Delete(offering);
        }

        public List<Offering> GetAll()
        {
            return _offeringRepository.GetAll(selector: x => x).ToList();
        }

        public Offering? GetById(Guid Id)
        {
            return _offeringRepository.Get(selector: x => x,
                                            predicate: x => x.Id == Id);
        }

        public Offering Update(Offering offering)
        {
            return _offeringRepository.Update(offering);
        }
    }
}
