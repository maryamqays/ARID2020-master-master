using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class CourseInstructorsController : Controller
    {
        private readonly ARIDDbContext _context;
        private int PagSize = 10;

        public CourseInstructorsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: CourseInstructors
        public async Task<IActionResult> Index(string SearchString,int productPage=1)
        {
            CourseInstructorViewModel CourseInstructorVM = new CourseInstructorViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CourseInstructorVM = new CourseInstructorViewModel()
                {
                    CourseInstructors = _context.CourseInstructors.Include(p => p.ApplicationUser).Include(p => p.Course),
                    courses = _context.Courses.Include(p => p.ApplicationUser)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CourseInstructorVM = new  CourseInstructorViewModel()
                {
                    CourseInstructors = _context.CourseInstructors.Where(a => a.ApplicationUser.ArName.Contains(SearchString) || a.Course.ArName.Contains(SearchString) || a.Description.Contains(SearchString)).Include(p => p.ApplicationUser),
                    courses = _context.Courses.Include(p => p.ApplicationUser)

                };
            }

            var count = CourseInstructorVM.CourseInstructors.Count();
            CourseInstructorVM.CourseInstructors = CourseInstructorVM.CourseInstructors.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CourseInstructorVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.CourseInstructors.Include(c => c.ApplicationUser).Include(c => c.Course);
            return View(CourseInstructorVM);
        }

        // GET: CourseInstructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await _context.CourseInstructors
                .Include(c => c.ApplicationUser)
                .Include(c => c.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }

            return View(courseInstructor);
        }

        // GET: CourseInstructors/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName");
            return View();
        }

        // POST: CourseInstructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Indx,CourseId,Description,ApplicationUserId")] CourseInstructor courseInstructor)
        {
            if (ModelState.IsValid)
            {
                courseInstructor.Description = courseInstructor.Description.Replace("\n", "<br/>");


                _context.Add(courseInstructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseInstructor.ApplicationUserId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseInstructor.CourseId);
            return View(courseInstructor);
        }

        // GET: CourseInstructors/Edit/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await _context.CourseInstructors.SingleOrDefaultAsync(m => m.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseInstructor.ApplicationUserId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseInstructor.CourseId);
            return View(courseInstructor);
        }

        // POST: CourseInstructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Indx,CourseId,Description,ApplicationUserId")] CourseInstructor courseInstructor)
        {
            if (id != courseInstructor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseInstructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseInstructorExists(courseInstructor.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", courseInstructor.ApplicationUserId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseInstructor.CourseId);
            return View(courseInstructor);
        }

        // GET: CourseInstructors/Delete/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await _context.CourseInstructors
                .Include(c => c.ApplicationUser)
                .Include(c => c.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }

            return View(courseInstructor);
        }

        // POST: CourseInstructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseInstructor = await _context.CourseInstructors.SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseInstructors.Remove(courseInstructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseInstructorExists(int id)
        {
            return _context.CourseInstructors.Any(e => e.Id == id);
        }
    }
}
