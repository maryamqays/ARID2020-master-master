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
    public class CourseChaptersController : Controller
    {
        private readonly ARIDDbContext _context;
        private int PagSize = 10;

        public CourseChaptersController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: CourseChapters
        public async Task<IActionResult> Index(string SearchString, int coid, int productPage = 1)
        {
            CourseChapterViewModel CourseChapterVM = new CourseChapterViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CourseChapterVM = new CourseChapterViewModel()
                {
                    CourseChapters = _context.CourseChapters.Where(a => a.CourseId == coid).OrderBy(a => a.Indx).Include(p => p.Course),
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CourseChapterVM = new CourseChapterViewModel()
                {
                    CourseChapters = _context.CourseChapters.Where(a => a.CourseId == coid).Where(a => a.Name.Contains(SearchString) || a.Course.ArName.Contains(SearchString)).OrderBy(a => a.Indx).Include(p => p.Course),

                };
            }

            var count = CourseChapterVM.CourseChapters.Count();
            CourseChapterVM.CourseChapters = CourseChapterVM.CourseChapters.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CourseChapterVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            return View(CourseChapterVM);

            //var aRIDDbContext = _context.CourseChapters.Include(c => c.Course);
            //return View(await aRIDDbContext.ToListAsync());
        }

        // GET: CourseChapters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            ViewData["CourseChapterId"] = id;


            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var courseChapter = await _context.CourseChapters
            //    .Include(c => c.Course)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (courseChapter == null)
            //{
            //    return NotFound();
            //}

            //return View(courseChapter);

            CourseChapterViewModel CourseChapreVM = new CourseChapterViewModel()
            {
                CourseChapter = await _context.CourseChapters
                   .Include(c => c.Course)
                   .SingleOrDefaultAsync(m => m.Id == id),
                CourseChapterContents = _context.CourseChapterContents.Where(a => a.CourseChapterId == id).OrderBy(a => a.Indx).Include(a => a.CourseChapter)
            };
            return View(CourseChapreVM);


        }

        // GET: CourseChapters/Create
        public IActionResult Create(int coid)
        {
            ViewData["coid"] = coid;
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(a => a.Id == coid), "Id", "ArName");
            return View();
        }

        // POST: CourseChapters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Indx,CourseId,IsDeleted,Ishidden")] CourseChapter courseChapter)
        {
            if (ModelState.IsValid)
            {
                courseChapter.Id = Guid.NewGuid();
                _context.Add(courseChapter);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Courses", new {/* routeValues, for example: */ id = courseChapter.CourseId });
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseChapter.CourseId);
            return View(courseChapter);
        }

        // GET: CourseChapters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CourseViewModel CourseVM = new CourseViewModel()
            {
                CourseChapter = await _context.CourseChapters.SingleOrDefaultAsync(m => m.Id == id),

                CourseChapterExams = _context.CourseChapterExams
                .Include(f => f.CourseChapter)
                .Where(f => f.CourseChapterId == id)
                .ToList(),

                CourseChapterChoices = _context.CourseChapterChoices
                .Include(f => f.CourseChapterExam)
                .Where(f => f.CourseChapterExam.CourseChapterId == id)
                .ToList(),

                //              CourseSponsors = _context.CourseSponsors
                // .Include(f => f.Course)
                //.Where(f => f.CourseId == id)
                //.ToList(),

                //              ApplicationUser = await _userManager.Users
                //.Include(c => c.Country)
                //.Include(c => c.City)
                //.Include(c => c.University)
                //.Include(c => c.Faculty)
                //.Include(a => a.ReferredBy)
                //.SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),

                //         Course = await _context.Courses.Include(a => a.ApplicationUser).Include(a => a.Speciality)
                //.SingleOrDefaultAsync(m => m.Id == id),

            };

            var courseChapter = await _context.CourseChapters.SingleOrDefaultAsync(m => m.Id == id);

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseChapter.CourseId);

            return View(CourseVM);

        }

        // POST: CourseChapters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Indx,CourseId")] CourseChapter courseChapter)
        {
            if (id != courseChapter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseChapter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseChapterExists(courseChapter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Courses", new { id = courseChapter.CourseId });
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseChapter.CourseId);
            return View(courseChapter);
        }

        // GET: CourseChapters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseChapter = await _context.CourseChapters
                .Include(c => c.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseChapter == null)
            {
                return NotFound();
            }

            return View(courseChapter);
        }

        // POST: CourseChapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var courseChapter = await _context.CourseChapters.SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseChapters.Remove(courseChapter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseChapterExists(Guid id)
        {
            return _context.CourseChapters.Any(e => e.Id == id);
        }
    }
}
