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
using System.Linq.Dynamic;
using ARID.DTOs;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class FacultiesController : Controller
    {
        private readonly ARIDDbContext _context;

        public FacultiesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Faculties
        public ViewResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Unindexed()
        {
            return View(await _context.Faculties.Where(a => a.IsIndexed == false).ToListAsync());
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var ARIDDbContext = _context.Faculties.Where(a => a.ArFacultyName.Contains(keyword) || a.EnFacultyName.Contains(keyword) || a.Speciality.Name.Contains(keyword) || a.Speciality.EnSpecialityName.Contains(keyword) || a.University.ArUniversityName.Contains(keyword) || a.University.EnUniversityName.Contains(keyword)).Include(a => a.University).Include(a => a.Speciality);
            return View(await ARIDDbContext.ToListAsync());

        }



        [Route("api/GetFaculties")]
        [Authorize(Roles = RoleName.Admins)]
        public IActionResult GetFaculties()
        {
            DataTableProperties DTP = DataTableProperties.GetDataTableProperties(Request);

            // Getting all faculties list  
            List<Faculty> faculties = _context.Faculties
                                .Include(a => a.City)
                                .Include(a => a.City.Country)
                                .Include(a => a.Speciality)
                                .Include(a => a.University)
                                .ToList();

            // Sorting
            // To use OrderBy by passing strings you must install wilx.System.Linq.Dynamic.Core
            // and must use using System.Linq.Dynamic;
            if (!string.IsNullOrEmpty(DTP.SortColumn) && !string.IsNullOrEmpty(DTP.SortColumnDirection))
            {
                faculties = faculties.OrderBy(DTP.SortColumn + " " + DTP.SortColumnDirection).ToList();
            }

            // Searching 
            if (!string.IsNullOrEmpty(DTP.SearchValue))
            {
                faculties = faculties.Where(a =>
                                    a.Id.ToString().ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    (a.ArFacultyName != null ? a.ArFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.EnFacultyName != null ? a.EnFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.Speciality != null ? a.Speciality.Name.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.Speciality != null ? a.Speciality.EnSpecialityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.City != null ? a.City.ArCityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.City != null ? a.City.EnCityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.City != null ? a.City.Country.ArCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.City != null ? a.City.Country.ArCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.University != null ? a.University.ArUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.University != null ? a.University.EnUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false)
                                  ).ToList();
            }

            // Total number of rows count   
            DTP.RecordsTotal = faculties.Count();

            // Paging   
            faculties = faculties.Skip(DTP.Skip).Take(DTP.PageSize).ToList();

            List<FacultyDTO> uniDTO = new List<FacultyDTO>();
            foreach (var faculty in faculties)
            {
                uniDTO.Add(new FacultyDTO
                {
                    Id = faculty.Id,
                    ArFacultyName = faculty.ArFacultyName,
                    EnFacultyName = faculty.EnFacultyName,
                    HasPostGraduation = faculty.HasPostGraduation,
                    City = faculty.City.ArCityName,
                    Speciality = faculty.Speciality.Name,
                    University = faculty.University.ArUniversityName
                });
            }

            // Returning Json Data  
            return Json(new { draw = DTP.Draw, recordsFiltered = DTP.RecordsTotal, recordsTotal = DTP.RecordsTotal, data = uniDTO });
        }

        [AllowAnonymous]
        public JsonResult GetCitiesList(int universityid)
        {
            int countryid = _context.Universities.Where(u => u.Id == universityid).Select(u => u.CountryId).FirstOrDefault();
            var cities = new SelectList(_context.Cities.Where(c => c.CountryId == countryid), "Id", "ArCityName");
            return Json(cities);
        }

        // GET: Faculties/Create
        [AllowAnonymous]
        public IActionResult Create(int universityid)
        {
            // ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(a => a.Id == universityid), "Id", "ArUniversityName");
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int universityid, [Bind("Id,ArFacultyName,EnFacultyName,HasPostGraduation,UniversityId,SpecialityId,CityId,IsIndexed")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                faculty.IsIndexed = false;
                faculty.CityId = 1;
                //faculty.UniversityId = universityid;
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction("ThankYou", "Universities");
                //return RedirectToAction("Index");
            }
            // ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", faculty.CityId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", faculty.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(a => a.Id == universityid), "Id", "ArUniversityName");
            return View(faculty);
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.SingleOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", faculty.CityId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", faculty.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(a => a.Id == faculty.UniversityId), "Id", "ArUniversityName");
            return View(faculty);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArFacultyName,EnFacultyName,HasPostGraduation,UniversityId,SpecialityId,CityId,IsIndexed")] Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    faculty.CityId = 1;
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            //ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", faculty.CityId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", faculty.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(a => a.Id == faculty.UniversityId), "Id", "ArUniversityName");
            return View(faculty);
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.City)
                .Include(f => f.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties.SingleOrDefaultAsync(m => m.Id == id);
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}