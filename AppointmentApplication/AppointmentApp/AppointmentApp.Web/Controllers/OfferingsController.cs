using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointmentApp.Domain.Models;
using AppointmentApp.Repository;
using AppointmentApp.Service.Interface;
using AppointmentApp.Service.Implementation;

namespace AppointmentApp.Web.Controllers
{
    public class OfferingsController : Controller
    {
        private readonly IOfferingService _offeringService;
        private readonly IDataFetchService _fetchService;

        public OfferingsController(IOfferingService offeringService, IDataFetchService fetchService)
        {
            _offeringService = offeringService;
            _fetchService = fetchService;
        }

        // GET: Offerings
        public IActionResult Index()
        {
            return View(_offeringService.GetAll());
        }

        // GET: Offerings/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = _offeringService.GetById(id.Value);
            if (offering == null)
            {
                return NotFound();
            }

            return View(offering);
        }

        // GET: Offerings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Offerings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("OfferingName,OfferingDescription,OfferingDuration,OfferingPrice,Id")] Offering offering)
        {
            if (ModelState.IsValid)
            {
                _offeringService.Add(offering);
                return RedirectToAction(nameof(Index));
            }
            return View(offering);
        }

        // GET: Offerings/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = _offeringService.GetById(id.Value);
            if (offering == null)
            {
                return NotFound();
            }
            return View(offering);
        }

        // POST: Offerings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("OfferingName,OfferingDescription,OfferingDuration,OfferingPrice,Id")] Offering offering)
        {
            if (id != offering.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _offeringService.Update(offering);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferingExists(offering.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(offering);
        }

        // GET: Offerings/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = _offeringService.GetById(id.Value);
            if (offering == null)
            {
                return NotFound();
            }

            return View(offering);
        }

        // POST: Offerings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _offeringService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        private bool OfferingExists(Guid id)
        {
            return _offeringService.GetById(id) == null ? false : true;
        }

        public async Task<IActionResult> FetchOfferings()
        {
            await _fetchService.FetchOfferingsFromApi();
            return RedirectToAction(nameof(Index));
        }
    }
}
