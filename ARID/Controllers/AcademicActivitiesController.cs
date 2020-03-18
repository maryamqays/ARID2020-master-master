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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;

namespace ARID.Controllers
{
    [Authorize]
    // [Authorize(Roles = RoleName.Members)]
    // [Authorize(Roles = RoleName.Admins)]
    public class AcademicActivitiesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public AcademicActivitiesController(UserManager<ApplicationUser> userManager, IHostingEnvironment environment, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: AcademicActivities
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.AcademicActivities
                .Include(a => a.Country)
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .OrderBy(e => e.ActivityDate);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: AcademicActivities/Create
        public IActionResult Create()
        {
          ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View();
        }

        // POST: AcademicActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ActivityType,ArTitle,EnTitle,ArDescription,EnDescription,ActivityDate,ActivityURL,RelationType,Language,CountryId,Photo")]
        AcademicActivity academicActivity, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                academicActivity.Photo = await UserFile.UploadeNewImageAsync(academicActivity.Photo,
                    myfile, _environment.WebRootPath, Properties.Resources.ActivityPhotos, 300, 300);

                _context.Add(academicActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", academicActivity.CountryId);
            return View(academicActivity);
        }

        // GET: AcademicActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicActivity = await _context.AcademicActivities.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (academicActivity == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", academicActivity.CountryId);
            return View(academicActivity);
        }

        // POST: AcademicActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ActivityType,ArTitle,EnTitle,ArDescription,EnDescription,ActivityDate,ActivityURL,RelationType,Language,CountryId,Photo")]
        AcademicActivity academicActivity, IFormFile myfile)
        {
            if (id != academicActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    academicActivity.Photo = await UserFile.UploadeNewImageAsync(
                        _context.AcademicActivities.Where(m => m.Id == id).Select(c => c.Photo).First(),
                        myfile, _environment.WebRootPath, Properties.Resources.ActivityPhotos, 100, 100);

                    _context.Update(academicActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicActivityExists(academicActivity.Id))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", academicActivity.CountryId);
            return View(academicActivity);
        }

        // GET: AcademicActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicActivity = await _context.AcademicActivities
                .Include(a => a.Country)
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (academicActivity == null)
            {
                return NotFound();
            }

            return View(academicActivity);
        }

        // POST: AcademicActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicActivity = await _context.AcademicActivities.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.AcademicActivities.Remove(academicActivity);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.ActivityPhotos, academicActivity.Photo);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicActivityExists(int id)
        {
            return _context.AcademicActivities.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
