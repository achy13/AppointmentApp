using AppointmentApp.Domain.DTO;
using AppointmentApp.Domain.Models;
using AppointmentApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentApp.Service.Implementation
{
    public class DataFetchService : IDataFetchService
    {
        private readonly HttpClient _httpClient;
        private readonly IOfferingService _offeringService;

        public DataFetchService(HttpClient httpClient, IOfferingService offeringService)
        {
            _httpClient = httpClient;
            _offeringService = offeringService;
        }

        public async Task<List<Offering>> FetchOfferingsFromApi()
        {
            var offeringDto = await _httpClient.GetFromJsonAsync<List<OfferingDTO>>("https://686182418e74864084463a4c.mockapi.io/api/salon/services/services");
            var offerings = offeringDto.Select(x => new Offering()
            {
                Id = Guid.NewGuid(),
                OfferingName = x.service,
                OfferingDescription = x.description,
                OfferingDuration = x.duration,
                OfferingPrice = (int?)(x.price * 54.5)

            }).ToList();
            _offeringService.InsertMany(offerings);
            return offerings;
        }
    }
}
