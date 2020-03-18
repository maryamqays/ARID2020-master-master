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
    public class CourseChapterChoicesController : Controller
    {
        private readonly ARIDDbContext _context;

        public CourseChapterChoicesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: CourseChapterChoices
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.CourseChapterChoices.Include(c => c.CourseChapterExam);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: CourseChapterChoices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterChoice = await _context.CourseChapterChoices
                .Include(c => c.CourseChapterExam)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterChoice == null)
            {
                return NotFound();
            }

            return View(courseChapterChoice);
        }

        // GET: CourseChapterChoices/Create
        public IActionResult Create()
        {
            ViewData["CourseChapterExamId"] = new SelectList(_context.CourseChapterExams, "Id", "Question");
            return View();
        }

        // POST: CourseChapterChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Option,CourseChapterExamId,IsCorrectAnswer,IsDeleted,Mark")] CourseChapterChoice courseChapterChoice)
        {
            if (ModelState.IsValid)
            {
                courseChapterChoice.Id = Guid.NewGuid();
                _context.Add(courseChapterChoice);
                await _context.SaveChangesAsync();
                courseChapterChoice.IsDeleted = false;
                var courseexam = _context.CourseChapterExams.Include(c => c.CourseChapter).SingleOrDefault(c => c.Id == courseChapterChoice.CourseChapterExamId);
                return RedirectToAction("Edit","CourseChapters",new {id= courseexam.CourseChapterId });
            }
            ViewData["CourseChapterExamId"] = new SelectList(_context.CourseChapterExams, "Id", "Question", courseChapterChoice.CourseChapterExamId);
            return View(courseChapterChoice);
        }

        // GET: CourseChapterChoices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterChoice = await _context.CourseChapterChoices.Include(c=>c.CourseChapterExam).SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterChoice == null)
            {
                return NotFound();
            }
            ViewData["CourseChapterExamId"] = new SelectList(_context.CourseChapterExams.Where(c=>c.Id==courseChapterChoice.CourseChapterExamId), "Id", "Question", courseChapterChoice.CourseChapterExamId);
            return View(courseChapterChoice);
        }

        // POST: CourseChapterChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Option,CourseChapterExamId,IsCorrectAnswer,IsDeleted,Mark")] CourseChapterChoice courseChapterChoice)
        {
            if (id != courseChapterChoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseChapterChoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseChapterChoiceExists(courseChapterChoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var coursechapterexam = _context.CourseChapterExams.SingleOrDefault(c => c.Id == courseChapterChoice.CourseChapterExamId);
                return RedirectToAction("Edit","CourseChapters",new { id= coursechapterexam.CourseChapterId});
            }
            ViewData["CourseChapterExamId"] = new SelectList(_context.CourseChapterExams, "Id", "Question", courseChapterChoice.CourseChapterExamId);
            return View(courseChapterChoice);
        }

        // GET: CourseChapterChoices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterChoice = await _context.CourseChapterChoices
                .Include(c => c.CourseChapterExam)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterChoice == null)
            {
                return NotFound();
            }

            return View(courseChapterChoice);
        }

        // POST: CourseChapterChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var courseChapterChoice = await _context.CourseChapterChoices.Include(c=>c.CourseChapterExam).SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseChapterChoices.Remove(courseChapterChoice);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","CourseChapters",new { id=courseChapterChoice.CourseChapterExam.CourseChapterId});
        }

        private bool CourseChapterChoiceExists(Guid id)
        {
            return _context.CourseChapterChoices.Any(e => e.Id == id);
        }
    }
}
