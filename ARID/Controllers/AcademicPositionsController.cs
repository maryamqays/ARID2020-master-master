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
    [Authorize]
    // [Authorize(Roles = RoleName.Members)]
    // [Authorize(Roles = RoleName.Admins)]
    public class AcademicPositionsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AcademicPositionsController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AcademicPositions
        public async Task<IActionResult> Index()
        {
            var academicPositions = _context.AcademicPositions.Include(a => a.ApplicationUser)
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .OrderBy(e => e.Indx)
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.Faculty)
                .Include(a => a.University)
                .Include(a => a.PositionType);
            return View(await academicPositions.ToListAsync());
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

        [AllowAnonymous]
        public JsonResult GetCitiesList(int countryid)
        {
            var cities = new SelectList(_context.Cities.Where(c => c.CountryId == countryid), "Id", "ArCityName");
            return Json(cities);
        }


        // GET: AcademicPositions/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);

            ViewData["PositionTypeId"] = new SelectList(_context.PositionTypes, "Id", "ArPositionName");

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            //  ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName");
            //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            //  ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName");
            try
            {
                ViewData["Indx"] = (_context.AcademicPositions.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).LastOrDefault().Indx) + 1;
            }
            catch (Exception)
            {

                ViewData["Indx"] = 1;
            }
                                    return View();
        }

        // POST: AcademicPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,PositionTypeId,FacultyId,UniversityId,CountryId,CityId,ArDescription,EnDescription,IsCurrent,FromYear,ToYear,Indx")] AcademicPosition academicPosition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicPosition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            ViewData["PositionTypeId"] = new SelectList(_context.PositionTypes, "Id", "ArPositionName");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", academicPosition.CityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", academicPosition.UniversityId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", academicPosition.FacultyId);

            return View(academicPosition);
        }

        // GET: AcademicPositions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicPosition = await _context.AcademicPositions.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (academicPosition == null)
            {
                return NotFound();
            }



            ViewData["PositionTypeId"] = new SelectList(_context.PositionTypes, "Id", "ArPositionName", academicPosition.PositionTypeId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", academicPosition.CountryId);
            ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == academicPosition.CountryId), "Id", "ArCityName",academicPosition.CityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u => u.CountryId == academicPosition.CountryId), "Id", "ArUniversityName", academicPosition.UniversityId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == academicPosition.UniversityId), "Id", "ArFacultyName", academicPosition.FacultyId);
            return View(academicPosition);
        }

        // POST: AcademicPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ApplicationUserId,PositionTypeId,FacultyId,UniversityId,CountryId,CityId,ArDescription,EnDescription,IsCurrent,FromYear,ToYear,Indx")]
            AcademicPosition academicPosition)
        {
            if (id != academicPosition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicPosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicPositionExists(academicPosition.Id))
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
            ViewData["PositionTypeId"] = new SelectList(_context.PositionTypes, "Id", "ArPositionName", academicPosition.PositionTypeId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", academicPosition.CountryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName",academicPosition.CityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u => u.CountryId == academicPosition.CountryId), "Id", "ArUniversityName", academicPosition.UniversityId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == academicPosition.UniversityId), "Id", "ArFacultyName", academicPosition.FacultyId);
            return View(academicPosition);
        }

        // GET: AcademicPositions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicPosition = await _context.AcademicPositions
                .Include(a => a.City)
                .Include(e => e.Country)
                .Include(e => e.Faculty)
                .Include(e => e.University)
                .Include(a => a.PositionType)
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (academicPosition == null)
            {
                return NotFound();
            }

            return View(academicPosition);
        }

        // POST: AcademicPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicPosition = await _context.AcademicPositions.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.AcademicPositions.Remove(academicPosition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicPositionExists(int id)
        {
            return _context.AcademicPositions.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
