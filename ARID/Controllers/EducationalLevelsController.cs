using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    //[Authorize(Roles = RoleName.Members)]
    // [Authorize(Roles = RoleName.Admins)]
    [Authorize]
    public class EducationalLevelsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EducationalLevelsController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EducationalLevels
        public async Task<IActionResult> Index()
        {
            var educationalLevels = _context.EducationalLevels
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                  //.OrderBy(e => e.ToYear)
                  .OrderByDescending(e => e.ToYear)
                .Include(e => e.AcademicDegree)
                .Include(e => e.Speciality);

            return View(await educationalLevels.ToListAsync());
        }

        [AllowAnonymous]
        public JsonResult GetUniversitiesList(int countryid)
        {
            var universities = new SelectList(_context.Universities.Where(u => u.CountryId == countryid), "Id", "ArUniversityName");
            return Json(universities);
        }

        [AllowAnonymous]
        public JsonResult GetFacultiesList(int universityid)
        {
            var faculties = new SelectList(_context.Faculties.Where(c => c.UniversityId == universityid), "Id", "ArFacultyName");
            return Json(faculties);
        }

        // GET: EducationalLevels/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            ViewData["AcademicDegreeId"] = new SelectList(_context.AcademicDegrees, "Id", "ArDegreeName");
            //ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            try
            {
                ViewData["Indx"] = (_context.EducationalLevels.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).LastOrDefault().Indx) + 1;
            }
            catch (Exception)
            {

                ViewData["Indx"] = 1;
            }
            return View();
        }

        // POST: EducationalLevels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,AcademicDegreeId,SpecialityId,CountryId,ArCertificateName,EnCertificateName,FacultyId,UniversityId,ArDescription,EnDescription,IsCurrent,FromYear,ToYear,Indx")] EducationalLevel educationalLevels)
        {
            if (ModelState.IsValid)
            {
                //educationalLevels.Indx = 0;
                _context.Add(educationalLevels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            ViewData["AcademicDegreeId"] = new SelectList(_context.AcademicDegrees, "Id", "ArDegreeName", educationalLevels.AcademicDegreeId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", educationalLevels.FacultyId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", educationalLevels.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", educationalLevels.UniversityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View(educationalLevels);
        }

        // GET: EducationalLevels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var educationalLevels = await _context.EducationalLevels.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (educationalLevels == null)
            {
                return NotFound();
            }
            ViewData["AcademicDegreeId"] = new SelectList(_context.AcademicDegrees, "Id", "ArDegreeName", educationalLevels.AcademicDegreeId);
            //  ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", educationalLevels.FacultyId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", educationalLevels.SpecialityId);
            //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", educationalLevels.UniversityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", educationalLevels.CountryId);

            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u => u.CountryId == educationalLevels.CountryId), "Id", "ArUniversityName", educationalLevels.UniversityId);
            //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", educationalLevels.UniversityId);

            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == educationalLevels.UniversityId), "Id", "ArFacultyName", educationalLevels.FacultyId);

            return View(educationalLevels);
        }

        // POST: EducationalLevels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ApplicationUserId,AcademicDegreeId,SpecialityId,CountryId,ArCertificateName,EnCertificateName,FacultyId,UniversityId,ArDescription,EnDescription,IsCurrent,FromYear,ToYear,Indx")]
            EducationalLevel educationalLevels)
        {
            if (id != educationalLevels.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //educationalLevels.Indx = 0;
                    _context.Update(educationalLevels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducationalLevelsExists(educationalLevels.Id))
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
            ViewData["AcademicDegreeId"] = new SelectList(_context.AcademicDegrees, "Id", "ArDegreeName", educationalLevels.AcademicDegreeId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", educationalLevels.FacultyId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", educationalLevels.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", educationalLevels.UniversityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", educationalLevels.CountryId);
            return View(educationalLevels);
        }

        // GET: EducationalLevels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var educationalLevels = await _context.EducationalLevels
                .Include(e => e.AcademicDegree)
                .Include(e => e.Faculty)
                .Include(e => e.Speciality)
                .Include(e => e.University)
                .Include(e => e.Country)
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (educationalLevels == null)
            {
                return NotFound();
            }

            return View(educationalLevels);
        }

        // POST: EducationalLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var educationalLevels = await _context.EducationalLevels.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.EducationalLevels.Remove(educationalLevels);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EducationalLevelsExists(int id)
        {
            return _context.EducationalLevels.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }





    }
}
