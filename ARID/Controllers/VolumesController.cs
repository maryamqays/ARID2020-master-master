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
    public class VolumesController : Controller
    {
        private readonly ARIDDbContext _context;

        public VolumesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Volumes
        public async Task<IActionResult> Index(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;

            var aRIDDbContext = _context.Volumes.Where(v=>v.JournalId==id).Include(v => v.Journal);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: Volumes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volume = await _context.Volumes
                .Include(v => v.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (volume == null)
            {
                return NotFound();
            }

            return View(volume);
        }

        // GET: Volumes/Create
        public IActionResult Create(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;

            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==id), "Id", "ArName");
            return View();
        }

        // POST: Volumes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jid,[Bind("Year,Description,JournalId,IsPublished,Name")] Volume volume)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volume);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", volume.JournalId);
            return View(volume);
        }

        // GET: Volumes/Edit/5
        public async Task<IActionResult> Edit(int? id, int jid)
        {
            ViewData["jid"] = jid;
            if (id == null)
            {
                return NotFound();
            }

            var volume = await _context.Volumes.SingleOrDefaultAsync(m => m.Id == id);
            if (volume == null)
            {
                return NotFound();
            }
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", volume.JournalId);
            return View(volume);
        }

        // POST: Volumes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int jid, int id, [Bind("Id,Year,Description,JournalId,IsPublished,Name")] Volume volume)
        {
            if (id != volume.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volume);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolumeExists(volume.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", volume.JournalId);
            return View(volume);
        }

        // GET: Volumes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volume = await _context.Volumes
                .Include(v => v.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (volume == null)
            {
                return NotFound();
            }

            return View(volume);
        }

        // POST: Volumes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volume = await _context.Volumes.SingleOrDefaultAsync(m => m.Id == id);
            _context.Volumes.Remove(volume);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolumeExists(int id)
        {
            return _context.Volumes.Any(e => e.Id == id);
        }
    }
}
