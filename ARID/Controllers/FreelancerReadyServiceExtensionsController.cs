using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;

namespace ARID.Controllers
{
    public class FreelancerReadyServiceExtensionsController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerReadyServiceExtensionsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerReadyServiceExtensions
        public async Task<IActionResult> Index()
        {
            return View(await _context.FreelancerReadyServiceExtensions.ToListAsync());
        }

        // GET: FreelancerReadyServiceExtensions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerReadyServiceExtension = await _context.FreelancerReadyServiceExtensions
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerReadyServiceExtension == null)
            {
                return NotFound();
            }

            return View(freelancerReadyServiceExtension);
        }

        // GET: FreelancerReadyServiceExtensions/Create
        public IActionResult Create(Guid id)
        {
            ViewData["id"] = id;
            ViewData["FreelancerReadyServiceId"] = new SelectList(_context.FreelancerReadyServices.Where(f => f.Id == id), "Id", "Title");
            return View();
        }

        // POST: FreelancerReadyServiceExtensions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,PricingType,RequiredDays,FreelancerReadyServiceId")] FreelancerReadyServiceExtension freelancerReadyServiceExtension)
        {
            if (ModelState.IsValid)
            {
                freelancerReadyServiceExtension.Id = Guid.NewGuid();
                _context.Add(freelancerReadyServiceExtension);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "FreelancerReadyServices", new {/* routeValues, for example: */ id = freelancerReadyServiceExtension.FreelancerReadyServiceId });

                //return RedirectToAction(nameof(Index));
            }
            return View(freelancerReadyServiceExtension);
        }

        // GET: FreelancerReadyServiceExtensions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerReadyServiceExtension = await _context.FreelancerReadyServiceExtensions.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerReadyServiceExtension == null)
            {
                return NotFound();
            }
            return View(freelancerReadyServiceExtension);
        }

        // POST: FreelancerReadyServiceExtensions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,PricingType,RequiredDays,FreelancerReadyServiceId")] FreelancerReadyServiceExtension freelancerReadyServiceExtension)
        {
            if (id != freelancerReadyServiceExtension.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerReadyServiceExtension);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerReadyServiceExtensionExists(freelancerReadyServiceExtension.Id))
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
            return View(freelancerReadyServiceExtension);
        }

        // GET: FreelancerReadyServiceExtensions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerReadyServiceExtension = await _context.FreelancerReadyServiceExtensions
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerReadyServiceExtension == null)
            {
                return NotFound();
            }

            return View(freelancerReadyServiceExtension);
        }

        // POST: FreelancerReadyServiceExtensions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var freelancerReadyServiceExtension = await _context.FreelancerReadyServiceExtensions.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerReadyServiceExtensions.Remove(freelancerReadyServiceExtension);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "FreelancerReadyServices",new { id= freelancerReadyServiceExtension.FreelancerReadyServiceId});
        }

        private bool FreelancerReadyServiceExtensionExists(Guid id)
        {
            return _context.FreelancerReadyServiceExtensions.Any(e => e.Id == id);
        }
    }
}
