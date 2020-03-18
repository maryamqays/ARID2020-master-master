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
    public class ChapterStudyStatusController : Controller
    {
        private readonly ARIDDbContext _context;

        public ChapterStudyStatusController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: ChapterStudyStatus
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.ChapterStudyStatuses.Include(c => c.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: ChapterStudyStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterStudyStatus = await _context.ChapterStudyStatuses
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chapterStudyStatus == null)
            {
                return NotFound();
            }

            return View(chapterStudyStatus);
        }

        // GET: ChapterStudyStatus/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: ChapterStudyStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseChapterContentId,ApplicationUserId,DateOfRecord,Status")] ChapterStudyStatus chapterStudyStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chapterStudyStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", chapterStudyStatus.ApplicationUserId);
            return View(chapterStudyStatus);
        }

        // GET: ChapterStudyStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterStudyStatus = await _context.ChapterStudyStatuses.SingleOrDefaultAsync(m => m.Id == id);
            if (chapterStudyStatus == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", chapterStudyStatus.ApplicationUserId);
            return View(chapterStudyStatus);
        }

        // POST: ChapterStudyStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseChapterContentId,ApplicationUserId,DateOfRecord,Status")] ChapterStudyStatus chapterStudyStatus)
        {
            if (id != chapterStudyStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chapterStudyStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterStudyStatusExists(chapterStudyStatus.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", chapterStudyStatus.ApplicationUserId);
            return View(chapterStudyStatus);
        }

        // GET: ChapterStudyStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterStudyStatus = await _context.ChapterStudyStatuses
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chapterStudyStatus == null)
            {
                return NotFound();
            }

            return View(chapterStudyStatus);
        }

        // POST: ChapterStudyStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chapterStudyStatus = await _context.ChapterStudyStatuses.SingleOrDefaultAsync(m => m.Id == id);
            _context.ChapterStudyStatuses.Remove(chapterStudyStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChapterStudyStatusExists(int id)
        {
            return _context.ChapterStudyStatuses.Any(e => e.Id == id);
        }
    }
}
