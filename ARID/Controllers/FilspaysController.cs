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
    public class FilspaysController : Controller
    {
        private readonly ARIDDbContext _context;

        public FilspaysController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Filspays
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.Filspays.Include(f => f.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: Filspays/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filspay = await _context.Filspays
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (filspay == null)
            {
                return NotFound();
            }

            return View(filspay);
        }

        // GET: Filspays/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Filspays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserAmount,ApplicationUserId,CreationDate,Status,trn_id")] Filspay filspay)
        {
            if (ModelState.IsValid)
            {
                filspay.Id = Guid.NewGuid();
                _context.Add(filspay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", filspay.ApplicationUserId);
            return View(filspay);
        }

        // GET: Filspays/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filspay = await _context.Filspays.SingleOrDefaultAsync(m => m.Id == id);
            if (filspay == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", filspay.ApplicationUserId);
            return View(filspay);
        }

        // POST: Filspays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserAmount,ApplicationUserId,CreationDate,Status,trn_id")] Filspay filspay)
        {
            if (id != filspay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filspay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilspayExists(filspay.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", filspay.ApplicationUserId);
            return View(filspay);
        }

        // GET: Filspays/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filspay = await _context.Filspays
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (filspay == null)
            {
                return NotFound();
            }

            return View(filspay);
        }

        // POST: Filspays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var filspay = await _context.Filspays.SingleOrDefaultAsync(m => m.Id == id);
            _context.Filspays.Remove(filspay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilspayExists(Guid id)
        {
            return _context.Filspays.Any(e => e.Id == id);
        }
    }
}
