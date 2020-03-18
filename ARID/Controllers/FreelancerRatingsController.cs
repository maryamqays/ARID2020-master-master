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
    public class FreelancerRatingsController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerRatingsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerRatings
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerRatings.Include(f => f.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRating = await _context.FreelancerRatings
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRating == null)
            {
                return NotFound();
            }

            return View(freelancerRating);
        }

        // GET: FreelancerRatings/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: FreelancerRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,FreelancerProjectId,Isvisible,DateofRecord,Comment,Professionalism,Communication,Quality,Experience,Delivery,RehirePossibility")] FreelancerRating freelancerRating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freelancerRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRating.ApplicationUserId);
            return View(freelancerRating);
        }

        // GET: FreelancerRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRating = await _context.FreelancerRatings.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRating == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRating.ApplicationUserId);
            return View(freelancerRating);
        }

        // POST: FreelancerRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,FreelancerProjectId,Isvisible,DateofRecord,Comment,Professionalism,Communication,Quality,Experience,Delivery,RehirePossibility")] FreelancerRating freelancerRating)
        {
            if (id != freelancerRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerRatingExists(freelancerRating.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRating.ApplicationUserId);
            return View(freelancerRating);
        }

        // GET: FreelancerRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRating = await _context.FreelancerRatings
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRating == null)
            {
                return NotFound();
            }

            return View(freelancerRating);
        }

        // POST: FreelancerRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerRating = await _context.FreelancerRatings.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerRatings.Remove(freelancerRating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerRatingExists(int id)
        {
            return _context.FreelancerRatings.Any(e => e.Id == id);
        }
    }
}
