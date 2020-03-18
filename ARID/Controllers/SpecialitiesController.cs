using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class SpecialitiesController : Controller
    {
        private readonly ARIDDbContext _context;

        public SpecialitiesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Specialities
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialities.Where(a => a.Id != 1).ToListAsync());
        }

        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.Specialities.ToListAsync());
        }

        // GET: Specialities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,EnSpecialityName")] Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                _context.Add(speciality);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(speciality);
        }

        // GET: Specialities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _context.Specialities.SingleOrDefaultAsync(m => m.Id == id);
            if (speciality == null)
            {
                return NotFound();
            }
            return View(speciality);
        }

        // POST: Specialities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnSpecialityName")] Speciality speciality)
        {
            if (id != speciality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(speciality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialityExists(speciality.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(speciality);
        }

        // GET: Specialities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _context.Specialities
                .SingleOrDefaultAsync(m => m.Id == id);
            if (speciality == null)
            {
                return NotFound();
            }

            return View(speciality);
        }

        // POST: Specialities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var speciality = await _context.Specialities.SingleOrDefaultAsync(m => m.Id == id);
            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SpecialityExists(int id)
        {
            return _context.Specialities.Any(e => e.Id == id);
        }
    }
}
