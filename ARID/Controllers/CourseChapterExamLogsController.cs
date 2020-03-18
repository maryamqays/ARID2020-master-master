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
    public class CourseChapterExamLogsController : Controller
    {
        private readonly ARIDDbContext _context;

        public CourseChapterExamLogsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: CourseChapterExamLogs
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.ChapterExamLogs.Include(c => c.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: CourseChapterExamLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExamLog = await _context.ChapterExamLogs
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExamLog == null)
            {
                return NotFound();
            }

            return View(courseChapterExamLog);
        }

        // GET: CourseChapterExamLogs/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: CourseChapterExamLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,AnswerDate,CourseChapterChoiceId")] CourseChapterExamLog courseChapterExamLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseChapterExamLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseChapterExamLog.ApplicationUserId);
            return View(courseChapterExamLog);
        }

        // GET: CourseChapterExamLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExamLog = await _context.ChapterExamLogs.SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExamLog == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseChapterExamLog.ApplicationUserId);
            return View(courseChapterExamLog);
        }

        // POST: CourseChapterExamLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,AnswerDate,CourseChapterChoiceId")] CourseChapterExamLog courseChapterExamLog)
        {
            if (id != courseChapterExamLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseChapterExamLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseChapterExamLogExists(courseChapterExamLog.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseChapterExamLog.ApplicationUserId);
            return View(courseChapterExamLog);
        }

        // GET: CourseChapterExamLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExamLog = await _context.ChapterExamLogs
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExamLog == null)
            {
                return NotFound();
            }

            return View(courseChapterExamLog);
        }

        // POST: CourseChapterExamLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseChapterExamLog = await _context.ChapterExamLogs.SingleOrDefaultAsync(m => m.Id == id);
            _context.ChapterExamLogs.Remove(courseChapterExamLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseChapterExamLogExists(int id)
        {
            return _context.ChapterExamLogs.Any(e => e.Id == id);
        }
    }
}
