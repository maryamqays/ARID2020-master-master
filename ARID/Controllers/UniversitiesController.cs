using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic;
using ARID.DTOs;

namespace ARID.Controllers
{
    //[Authorize(Roles = RoleName.Admins)]
    public class UniversitiesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;

        public UniversitiesController(ARIDDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int id)
        {
            var StatisticsViewModel = new StatisticsViewModel
            {
                Universities = _context.Universities.Where(a => a.CountryId == id).ToList(),
                ApplicationUsers = _context.Users.Where(a => a.CountryId == id).ToList()
                            };
            return View(StatisticsViewModel);
        }

        public async Task<IActionResult> Unindexed()
        {
            return View(await _context.Universities.Where(a => a.IsIndexed == false).ToListAsync());
        }



        public async Task<IActionResult> Search(string keyword)
        {
            var ARIDDbContext = _context.Universities.Where(a => a.Country.ArCountryName.Contains(keyword) || a.Country.EnCountryName.Contains(keyword) || a.EnUniversityName.Contains(keyword) || a.ArUniversityName.Contains(keyword) || a.Website.Contains(keyword)).Include(a => a.Country);
            return View(await ARIDDbContext.ToListAsync());

        }


        [AllowAnonymous]
        public async Task<IActionResult> SearchByUser(string keyword)
        {
            var ARIDDbContext = _context.Universities.Where(a => a.Country.ArCountryName.Contains(keyword) || a.Country.EnCountryName.Contains(keyword) || a.EnUniversityName.Contains(keyword) || a.ArUniversityName.Contains(keyword) || a.Website.Contains(keyword)).Include(a => a.Country);
            return View(await ARIDDbContext.ToListAsync());

        }


        [Route("api/GetUniversities")]
        //[Authorize(Roles = RoleName.Admins)]
        public IActionResult GetUniversities()
        {
            DataTableProperties DTP = DataTableProperties.GetDataTableProperties(Request);

            // Getting all universities list  
            List<University> universities = _context.Universities
                                .Include(a => a.Country)
                                .ToList();

            // Sorting
            // To use OrderBy by passing strings you must install wilx.System.Linq.Dynamic.Core
            // and must use using System.Linq.Dynamic;
            if (!string.IsNullOrEmpty(DTP.SortColumn) && !string.IsNullOrEmpty(DTP.SortColumnDirection))
            {
                universities = universities.OrderBy(DTP.SortColumn + " " + DTP.SortColumnDirection).ToList();
            }

            // Searching 
            if (!string.IsNullOrEmpty(DTP.SearchValue))
            {
                universities = universities.Where(a =>
                                    a.Id.ToString().ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    (a.ArUniversityName != null ? a.ArUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.EnUniversityName != null ? a.EnUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.Website != null ? a.Website.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.Country != null ? a.Country.ArCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    (a.Country != null ? a.Country.EnCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) : false) ||
                                    a.YearofEstablishment.ToString().ToLower().Contains(DTP.SearchValue.ToLower())
                                  ).ToList();
            }

            // Total number of rows count   
            DTP.RecordsTotal = universities.Count();

            // Paging   
            universities = universities.Skip(DTP.Skip).Take(DTP.PageSize).ToList();

            List<UniversityDTO> uniDTO = new List<UniversityDTO>();
            foreach (var university in universities)
            {
                uniDTO.Add(new UniversityDTO
                {
                    Id = university.Id,
                    ArUniversityName = university.ArUniversityName,
                    EnUniversityName = university.EnUniversityName,
                    Website = university.Website,
                    LogoHD = university.LogoHD,
                    YearofEstablishment = university.YearofEstablishment,

                    Country = university.Country.ArCountryName
                });
            }

            // Returning Json Data  
            return Json(new { draw = DTP.Draw, recordsFiltered = DTP.RecordsTotal, recordsTotal = DTP.RecordsTotal, data = uniDTO });
        }

        // GET: Universities/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View();
        }

        // POST: Universities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ArUniversityName,EnUniversityName,Website,StaffNo,StudentNo,LogoHD,LogoThumb,YearofEstablishment,Governmental,SemiPrivate,Private,CountryId,IsIndexed")] University university,
            IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                university.LogoHD = await UserFile.UploadeNewImageAsync(university.LogoHD,
                    myfile, _environment.WebRootPath, Properties.Resources.UniversityLogoFolder, 400, 400);

                _context.Add(university);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", university.CountryId);
            return View(university);
        }

        [AllowAnonymous]
        public IActionResult CreateUnIndexed()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View();
        }


        // POST: Universities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUnIndexed(
            [Bind("Id,ArUniversityName,EnUniversityName,Website,StaffNo,StudentNo,LogoHD,LogoThumb,YearofEstablishment,Governmental,SemiPrivate,Private,CountryId,IsIndexed")] University university,
            IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                //university.LogoHD = await UserFile.UploadeNewImageAsync(university.LogoHD,
                //    myfile, _environment.WebRootPath, Properties.Resources.UniversityLogoFolder, 400, 400);
                university.IsIndexed = false;

                _context.Add(university);
                await _context.SaveChangesAsync();
                return RedirectToAction("ThankYou");
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", university.CountryId);
            return View(university);
        }

        [AllowAnonymous]
        public IActionResult ThankYou()
        {
            return View();
        }

        // GET: Universities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities.SingleOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", university.CountryId);
            return View(university);
        }

        // POST: Universities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ArUniversityName,EnUniversityName,Website,StaffNo,StudentNo,LogoHD,LogoThumb,YearofEstablishment,Governmental,SemiPrivate,Private,CountryId,IsIndexed")] University university,
            IFormFile myfile)
        {
            if (id != university.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    university.LogoHD = await UserFile.UploadeNewImageAsync(
                        _context.Universities.Where(m => m.Id == id).Select(c => c.LogoHD).First(),
                        myfile, _environment.WebRootPath, Properties.Resources.UniversityLogoFolder, 400, 400);

                    _context.Update(university);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityExists(university.Id))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", university.CountryId);
            return View(university);
        }

        // GET: Universities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities
                .Include(u => u.Country)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // POST: Universities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var university = await _context.Universities.SingleOrDefaultAsync(m => m.Id == id);
            _context.Universities.Remove(university);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.UniversityLogoFolder, university.LogoHD);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UniversityExists(int id)
        {
            return _context.Universities.Any(e => e.Id == id);
        }
    }
}
