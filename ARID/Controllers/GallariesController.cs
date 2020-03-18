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
    public class GallariesController : Controller
    {
        private readonly ARIDDbContext _context;

        public GallariesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Gallaries
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.Gallaries.Include(g => g.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: Gallaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallary = await _context.Gallaries
                .Include(g => g.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gallary == null)
            {
                return NotFound();
            }

            return View(gallary);
        }

        // GET: Gallaries/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Gallaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Description,OriginalImage,ThumbImage")] Gallary gallary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gallary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", gallary.ApplicationUserId);
            return View(gallary);
        }

        // GET: Gallaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallary = await _context.Gallaries.SingleOrDefaultAsync(m => m.Id == id);
            if (gallary == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", gallary.ApplicationUserId);
            return View(gallary);
        }

        // POST: Gallaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Description,OriginalImage,ThumbImage")] Gallary gallary)
        {
            if (id != gallary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gallary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GallaryExists(gallary.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", gallary.ApplicationUserId);
            return View(gallary);
        }

        // GET: Gallaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallary = await _context.Gallaries
                .Include(g => g.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gallary == null)
            {
                return NotFound();
            }

            return View(gallary);
        }

        // POST: Gallaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallary = await _context.Gallaries.SingleOrDefaultAsync(m => m.Id == id);
            _context.Gallaries.Remove(gallary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GallaryExists(int id)
        {
            return _context.Gallaries.Any(e => e.Id == id);
        }
    }
}
