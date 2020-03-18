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
    public class SentEmailRecordsController : Controller
    {
        private readonly ARIDDbContext _context;

        public SentEmailRecordsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: SentEmailRecords
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.SentEmailRecords.Include(s => s.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: SentEmailRecords/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sentEmailRecords = await _context.SentEmailRecords
                .Include(s => s.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sentEmailRecords == null)
            {
                return NotFound();
            }

            return View(sentEmailRecords);
        }

        // GET: SentEmailRecords/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: SentEmailRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,EmailContentId")] SentEmailRecords sentEmailRecords)
        {
            if (ModelState.IsValid)
            {
                sentEmailRecords.Id = Guid.NewGuid();
                _context.Add(sentEmailRecords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", sentEmailRecords.ApplicationUserId);
            return View(sentEmailRecords);
        }

        // GET: SentEmailRecords/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sentEmailRecords = await _context.SentEmailRecords.SingleOrDefaultAsync(m => m.Id == id);
            if (sentEmailRecords == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", sentEmailRecords.ApplicationUserId);
            return View(sentEmailRecords);
        }

        // POST: SentEmailRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ApplicationUserId,EmailContentId")] SentEmailRecords sentEmailRecords)
        {
            if (id != sentEmailRecords.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sentEmailRecords);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SentEmailRecordsExists(sentEmailRecords.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", sentEmailRecords.ApplicationUserId);
            return View(sentEmailRecords);
        }

        // GET: SentEmailRecords/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sentEmailRecords = await _context.SentEmailRecords
                .Include(s => s.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sentEmailRecords == null)
            {
                return NotFound();
            }

            return View(sentEmailRecords);
        }

        // POST: SentEmailRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sentEmailRecords = await _context.SentEmailRecords.SingleOrDefaultAsync(m => m.Id == id);
            _context.SentEmailRecords.Remove(sentEmailRecords);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SentEmailRecordsExists(Guid id)
        {
            return _context.SentEmailRecords.Any(e => e.Id == id);
        }
    }
}
