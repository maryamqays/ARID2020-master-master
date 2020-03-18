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
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class CourseSponsorsController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private int PagSize = 10;

        public CourseSponsorsController(ARIDDbContext context,IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: CourseSponsors
        public async Task<IActionResult> Index(string SearchString,int productPage=1)
        {
            CourseSponsorViewModel CourseSponsorVM = new CourseSponsorViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CourseSponsorVM = new CourseSponsorViewModel()
                {
                    CourseSponsors = _context.CourseSponsors.Include(p => p.Course),
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CourseSponsorVM = new CourseSponsorViewModel()
                {
                    CourseSponsors = _context.CourseSponsors.Where(a => a.Link.Contains(SearchString) || a.Course.ArName.Contains(SearchString) || a.Name.Contains(SearchString)).Include(p => p.Course),
                };
            }

            var count = CourseSponsorVM.CourseSponsors.Count();
            CourseSponsorVM.CourseSponsors = CourseSponsorVM.CourseSponsors.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CourseSponsorVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.CourseSponsors.Include(c => c.Course);
            return View(CourseSponsorVM);
        }

        // GET: CourseSponsors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSponsor = await _context.CourseSponsors
                .Include(c => c.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseSponsor == null)
            {
                return NotFound();
            }

            return View(courseSponsor);
        }

        // GET: CourseSponsors/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName");
            return View();
        }

        // POST: CourseSponsors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Indx,CourseId,Logo,Link")] CourseSponsor courseSponsor,IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                courseSponsor.Logo = await UserFile.UploadeNewImageAsync(courseSponsor.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);


                _context.Add(courseSponsor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseSponsor.CourseId);
            return View(courseSponsor);
        }

        // GET: CourseSponsors/Edit/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSponsor = await _context.CourseSponsors.SingleOrDefaultAsync(m => m.Id == id);
            if (courseSponsor == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseSponsor.CourseId);
            return View(courseSponsor);
        }

        // POST: CourseSponsors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Indx,CourseId,Logo,Link")] CourseSponsor courseSponsor,IFormFile myfile)
        {
            if (id != courseSponsor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                courseSponsor.Logo = await UserFile.UploadeNewImageAsync(courseSponsor.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);

                try
                {
                    _context.Update(courseSponsor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseSponsorExists(courseSponsor.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ArName", courseSponsor.CourseId);
            return View(courseSponsor);
        }

        // GET: CourseSponsors/Delete/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSponsor = await _context.CourseSponsors
                .Include(c => c.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (courseSponsor == null)
            {
                return NotFound();
            }

            return View(courseSponsor);
        }

        // POST: CourseSponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseSponsor = await _context.CourseSponsors.SingleOrDefaultAsync(m => m.Id == id);
            _context.CourseSponsors.Remove(courseSponsor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseSponsorExists(int id)
        {
            return _context.CourseSponsors.Any(e => e.Id == id);
        }
    }
}
