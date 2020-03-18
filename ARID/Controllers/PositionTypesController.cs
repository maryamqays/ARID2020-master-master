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
    [Authorize]
   // [Authorize(Roles = RoleName.Admins)]
    public class PositionTypesController : Controller
    {
        private readonly ARIDDbContext _context;

        public PositionTypesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: PositionTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.PositionTypes.ToListAsync());
        }

        // GET: PositionTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PositionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArPositionName,EnPositionName,IsApproved")] PositionType positionType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(positionType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(positionType);
        }

        // GET: PositionTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionType = await _context.PositionTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (positionType == null)
            {
                return NotFound();
            }
            return View(positionType);
        }

        // POST: PositionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArPositionName,EnPositionName,IsApproved")] PositionType positionType)
        {
            if (id != positionType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(positionType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionTypeExists(positionType.Id))
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
            return View(positionType);
        }

        // GET: PositionTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionType = await _context.PositionTypes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (positionType == null)
            {
                return NotFound();
            }

            return View(positionType);
        }

        // POST: PositionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var positionType = await _context.PositionTypes.SingleOrDefaultAsync(m => m.Id == id);
            _context.PositionTypes.Remove(positionType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionTypeExists(int id)
        {
            return _context.PositionTypes.Any(e => e.Id == id);
        }
    }
}
