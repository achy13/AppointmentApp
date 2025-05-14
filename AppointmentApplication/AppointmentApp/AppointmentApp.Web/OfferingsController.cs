using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointmentApp.Domain.Models;
using AppointmentApp.Repository;

namespace AppointmentApp.Web
{
    public class OfferingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OfferingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Offerings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Offerings.ToListAsync());
        }

        // GET: Offerings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = await _context.Offerings
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("OfferingName,OfferingDescription,OfferingDuration,OfferingPrice,Id")] Offering offering)
        {
            if (ModelState.IsValid)
            {
                offering.Id = Guid.NewGuid();
                _context.Add(offering);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(offering);
        }

        // GET: Offerings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = await _context.Offerings.FindAsync(id);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("OfferingName,OfferingDescription,OfferingDuration,OfferingPrice,Id")] Offering offering)
        {
            if (id != offering.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offering);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offering = await _context.Offerings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offering == null)
            {
                return NotFound();
            }

            return View(offering);
        }

        // POST: Offerings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var offering = await _context.Offerings.FindAsync(id);
            if (offering != null)
            {
                _context.Offerings.Remove(offering);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferingExists(Guid id)
        {
            return _context.Offerings.Any(e => e.Id == id);
        }
    }
}
