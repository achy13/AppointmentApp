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

namespace AppointmentApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System;
    using Microsoft.AspNetCore.Authorization;
    using global::AppointmentApp.Service.Implementation;
    using global::AppointmentApp.Domain.Identity;
    using Microsoft.AspNetCore.Identity;
    using System.Drawing;
    using global::AppointmentApp.Repository.Interface;

    namespace AppointmentApp.Web.Controllers
    {
        public class ReservationsController : Controller
        {
            private readonly IReservationService _reservationService;
            private readonly IOfferingService _offeringService;
            private readonly IRepository<ReservationOffering> _reservationOfferingRepository;
            private readonly UserManager<AppointmentAppUser> _userManager;

            public ReservationsController(IReservationService reservationService, IOfferingService offeringService, IRepository<ReservationOffering> reservationOfferingRepository, UserManager<AppointmentAppUser> userManager)
            {
                _reservationService = reservationService;
                _offeringService = offeringService;
                _reservationOfferingRepository = reservationOfferingRepository;
                _userManager = userManager;
            }



            // GET: Reservations
            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);
                string role = user?.Role?.ToString() ?? "Unknown";
                ViewBag.UserRole = role;

                return View(_reservationService.GetAll());
            }

            // GET: Reservations/Details/5
            public IActionResult Details(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var reservation = _reservationService.GetById(id.Value);
                if (reservation == null)
                {
                    return NotFound();
                }

                return View(reservation);
            }

            // GET: Reservations/Create
            public IActionResult Create(string start = null)
            {
                if (!string.IsNullOrEmpty(start))
                {
                    try
                    {
                        // Парсирај го времето без временска зона
                        DateTime parsedDate = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
                        ViewBag.StartTime = parsedDate.ToString("yyyy-MM-ddTHH:mm");
                    }
                    catch
                    {
                        ViewBag.StartTime = null;
                    }
                }
                else
                {
                    ViewBag.StartTime = null;
                }
                return View();
            }

            // POST: Reservations/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create([Bind("FullName,PhoneNumber,ReservationDate,UserId,Id")] Reservation reservation, Guid OfferingId)
            {
                if (ModelState.IsValid)
                {
                    var selectedOffering = _offeringService.GetById(OfferingId);
                    if (selectedOffering == null)
                    {
                        ModelState.AddModelError("", "Service does not exist.");
                        ViewBag.StartTime = reservation.ReservationDate?.ToString("yyyy-MM-ddTHH:mm");
                        return View(reservation);
                    }

                    var duration = TimeSpan.FromHours((double)selectedOffering.OfferingDuration);
                    var startTime = reservation.ReservationDate ?? DateTime.Now;
                    var endTime = startTime.Add(duration);

                    var existingReservations = _reservationService.GetAll()
                        .Where(r => r.ReservationDate.HasValue)
                        .Select(r => new
                        {
                            Start = r.ReservationDate.Value,
                            End = r.ReservationDate.Value.Add(TimeSpan.FromHours((double)(r.reservationOfferings?.FirstOrDefault()?.Offering?.OfferingDuration ?? 0.5m)))
                        })
                        .ToList();

                    bool isOverlapping = existingReservations.Any(r =>
                        startTime < r.End && endTime > r.Start);

                    if (isOverlapping)
                    {
                        ModelState.AddModelError("", "This slot is booked. Choose another.");
                        ViewBag.StartTime = reservation.ReservationDate?.ToString("yyyy-MM-ddTHH:mm");
                        return View(reservation);
                    }

                    reservation.Id = Guid.NewGuid();
                    var createdReservation = _reservationService.Add(reservation);

                    var reservationOffering = new ReservationOffering
                    {
                        Id = Guid.NewGuid(),
                        ReservationId = createdReservation.Id,
                        OfferingId = OfferingId
                    };
                    _reservationOfferingRepository.Insert(reservationOffering);

                    return RedirectToAction(nameof(Index));
                }
                ViewBag.StartTime = reservation.ReservationDate?.ToString("yyyy-MM-ddTHH:mm");
                return View(reservation);
            }

            

            // GET: Reservations/Edit/5
            public IActionResult Edit(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var reservation = _reservationService.GetById(id.Value);
                if (reservation == null)
                {
                    return NotFound();
                }
                return View(reservation);
            }

            // POST: Reservations/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(Guid id, [Bind("FullName,PhoneNumber,ReservationDate,UserId,Id")] Reservation reservation)
            {
                if (id != reservation.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _reservationService.Update(reservation);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ReservationExists(reservation.Id))
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
                return View(reservation);
            }

            // GET: Reservations/Delete/5
            public IActionResult Delete(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var reservation = _reservationService.GetById(id.Value);
                if (reservation == null)
                {
                    return NotFound();
                }

                return View(reservation);
            }

            // POST: Reservations/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteConfirmed(Guid id)
            {
                _reservationService.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }

            private bool ReservationExists(Guid id)
            {
                return _reservationService.GetById(id) != null;
            }

            public async Task<IActionResult> GetEvents()
            {
                var loginedUser = await _userManager.GetUserAsync(User);

                var isAdminOrProvider = loginedUser != null &&
                    (loginedUser.Role == UserRole.Admin || loginedUser.Role == UserRole.ServiceProvider);

                var reservations = _reservationService.GetAll();

                var events = reservations
                    .Where(r => r.ReservationDate.HasValue)
                    .Select(r =>
                    {
                        var offering = r.reservationOfferings?.FirstOrDefault()?.Offering;
                        var startTime = r.ReservationDate.Value;
                        var endTime = startTime.Add(TimeSpan.FromHours((double)(offering?.OfferingDuration ?? 0.5m)));

                        return new
                        {
                            id = r.Id.ToString(),
                            title = isAdminOrProvider
                                ? $"{r.FullName} - {offering?.OfferingName}"
                                : "Booked",
                            start = startTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = endTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                            allDay = false,
                            color = isAdminOrProvider ? "#a0a0a0" : "#dc3545"
                        };
                    });

                return Json(events);
            }





        }
    }
}
