using AppointmentApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Interface
{
    public interface IDataFetchService
    {
        Task<List<Offering>> FetchOfferingsFromApi();
    }
}
