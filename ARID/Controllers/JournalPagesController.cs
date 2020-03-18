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
    public class JournalPagesController : Controller
    {
        private readonly ARIDDbContext _context;

        public JournalPagesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: JournalPages
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.JournalPages.Include(j => j.Journal);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: JournalPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalPage = await _context.JournalPages
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalPage == null)
            {
                return NotFound();
            }

            return View(journalPage);
        }

        // GET: JournalPages/Create
        public IActionResult Create()
        {
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName");
            return View();
        }

        // POST: JournalPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JournalId,Title,Body,Direction,Releasedate")] JournalPage journalPage)
        {
            if (ModelState.IsValid)
            {
                journalPage.Body=(System.Text.RegularExpressions.Regex.Replace(journalPage.Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                _context.Add(journalPage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalPage.JournalId);
            return View(journalPage);
        }

        // GET: JournalPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalPage = await _context.JournalPages.SingleOrDefaultAsync(m => m.Id == id);
            if (journalPage == null)
            {
                return NotFound();
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalPage.JournalId);
            return View(journalPage);
        }

        // POST: JournalPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JournalId,Title,Body,Direction,Releasedate")] JournalPage journalPage)
        {
            if (id != journalPage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalPage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalPageExists(journalPage.Id))
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
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalPage.JournalId);
            return View(journalPage);
        }

        // GET: JournalPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalPage = await _context.JournalPages
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalPage == null)
            {
                return NotFound();
            }

            return View(journalPage);
        }

        // POST: JournalPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalPage = await _context.JournalPages.SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalPages.Remove(journalPage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalPageExists(int id)
        {
            return _context.JournalPages.Any(e => e.Id == id);
        }
    }
}
