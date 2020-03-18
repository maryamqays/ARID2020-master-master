using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ARID.Controllers
{
    public class CourseChapterExamsController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;


        public CourseChapterExamsController(ARIDDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        // GET: CourseChapterExams
        public async Task<IActionResult> Index()
        {
            return View(await _context.CourseChapterExams.ToListAsync());
        }

        // GET: CourseChapterExams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExam = await _context.CourseChapterExams
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExam == null)
            {
                return NotFound();
            }

            return View(courseChapterExam);
        }

        // GET: CourseChapterExams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseChapterExams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,CourseChapterId,Indx,IsHidden,Description,Imgurl")] CourseChapterExam courseChapterExam,IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                courseChapterExam.Imgurl = await UserFile.UploadeNewImageAsync(courseChapterExam.Imgurl,
myfile, _environment.WebRootPath, Properties.Resources.Secured, 500, 500);

                _context.Add(courseChapterExam);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit","CourseChapters",new { id=courseChapterExam.CourseChapterId});
            }
            return View(courseChapterExam);
        }

        // GET: CourseChapterExams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExam = await _context.CourseChapterExams.SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExam == null)
            {
                return NotFound();
            }
            return View(courseChapterExam);
        }

        // POST: CourseChapterExams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,CourseChapterId,Indx,IsHidden,Description,Imgurl")] CourseChapterExam courseChapterExam,IFormFile myfile)
        {
            if (id != courseChapterExam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseChapterExam.Imgurl = await UserFile.UploadeNewImageAsync(courseChapterExam.Imgurl,
myfile, _environment.WebRootPath, Properties.Resources.Secured, 500, 500);

                    _context.Update(courseChapterExam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseChapterExamExists(courseChapterExam.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit","CourseChapters",new { id=courseChapterExam.CourseChapterId});
            }
            return View(courseChapterExam);
        }

        // GET: CourseChapterExams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterExam = await _context.CourseChapterExams
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterExam == null)
            {
                return NotFound();
            }

            return View(courseChapterExam);
        }

        // POST: CourseChapterExams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseChapterExam = await _context.CourseChapterExams.SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseChapterExams.Remove(courseChapterExam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseChapterExamExists(int id)
        {
            return _context.CourseChapterExams.Any(e => e.Id == id);
        }
    }
}
