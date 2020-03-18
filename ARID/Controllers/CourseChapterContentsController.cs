using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;

namespace ARID.Controllers
{
    public class CourseChapterContentsController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;

        public CourseChapterContentsController(ARIDDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: CourseChapterContents
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.CourseChapterContents.Include(c => c.CourseChapter);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: CourseChapterContents/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterContent = await _context.CourseChapterContents
                .Include(c => c.CourseChapter)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterContent == null)
            {
                return NotFound();
            }

            return View(courseChapterContent);
        }

        // GET: CourseChapterContents/Create
        public IActionResult Create(Guid cid, int id)
        {
            ViewData["CourseChpterId"] = cid;
            ViewData["CourseChapterId"] = new SelectList(_context.CourseChapters.Where(a => a.Id == cid), "Id", "Name");
            return View();
        }

        // POST: CourseChapterContents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Indx,CourseChapterId,ContentType,FilePath,Duration,IsFree,IsDownloadable")] CourseChapterContent courseChapterContent, Guid cid, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                courseChapterContent.FilePath = await UserFile.UploadeNewFileAsync(courseChapterContent.FilePath,
myfile, _environment.WebRootPath, Properties.Resources.Secured);
                if (courseChapterContent.ContentType == ContentType.youtube)
                {
                    int position = courseChapterContent.FilePath.IndexOf("=");
                    courseChapterContent.FilePath = courseChapterContent.FilePath.Substring(position + 1);
                }


                courseChapterContent.Description = courseChapterContent.Description.Replace("\n", "<br/>");


                courseChapterContent.Id = Guid.NewGuid();
                _context.Add(courseChapterContent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "CourseChapters", new {/* routeValues, for example: */ id = courseChapterContent.CourseChapterId });
            }
            ViewData["CourseChapterId"] = new SelectList(_context.CourseChapters, "Id", "Name", courseChapterContent.CourseChapterId);
            return View(courseChapterContent);
        }

        // GET: CourseChapterContents/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterContent = await _context.CourseChapterContents.SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterContent == null)
            {
                return NotFound();
            }
            ViewData["CourseChapterId"] = new SelectList(_context.CourseChapters, "Id", "Name", courseChapterContent.CourseChapterId);
            return View(courseChapterContent);
        }

        // POST: CourseChapterContents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Indx,CourseChapterId,ContentType,FilePath,Duration,IsFree,IsDownloadable")] CourseChapterContent courseChapterContent, IFormFile myfile)
        {
            if (id != courseChapterContent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (courseChapterContent.ContentType == ContentType.youtube)
                    {
                        int position = courseChapterContent.FilePath.LastIndexOf("=");
                        courseChapterContent.FilePath = courseChapterContent.FilePath.Substring(position + 1);
                    }
                    if (courseChapterContent.ContentType == ContentType.vimeo)
                    {
                        int position = courseChapterContent.FilePath.LastIndexOf ("/");
                        courseChapterContent.FilePath = courseChapterContent.FilePath.Substring(position + 1);
                    }

                    courseChapterContent.FilePath = await UserFile.UploadeNewFileAsync(courseChapterContent.FilePath,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent);


                    _context.Update(courseChapterContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseChapterContentExists(courseChapterContent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var coursechapter = _context.CourseChapters.Include(c => c.Course).SingleOrDefault(c => c.Id == courseChapterContent.CourseChapterId);
                return RedirectToAction("Details", "Courses", new {/* routeValues, for example: */ id = coursechapter.CourseId });
            }
            ViewData["CourseChapterId"] = new SelectList(_context.CourseChapters, "Id", "Name", courseChapterContent.CourseChapterId);
            return View(courseChapterContent);
        }

        // GET: CourseChapterContents/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapterContent = await _context.CourseChapterContents
                .Include(c => c.CourseChapter)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapterContent == null)
            {
                return NotFound();
            }

            return View(courseChapterContent);
        }

        // POST: CourseChapterContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var courseChapterContent = await _context.CourseChapterContents.Include(c => c.CourseChapter.Course).SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseChapterContents.Remove(courseChapterContent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Courses", new {/* routeValues, for example: */ id = courseChapterContent.CourseChapter.CourseId });
        }

        private bool CourseChapterContentExists(Guid id)
        {
            return _context.CourseChapterContents.Any(e => e.Id == id);
        }
    }
}
