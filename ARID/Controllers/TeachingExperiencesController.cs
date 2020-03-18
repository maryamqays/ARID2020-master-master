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
  //  [Authorize(Roles = RoleName.Members)]
   // [Authorize(Roles = RoleName.Admins)]
    public class TeachingExperiencesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeachingExperiencesController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TeachingExperiences
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.TeachingExperiences
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .OrderBy(e => e.Indx);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: TeachingExperiences/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            try
            {
                ViewData["Indx"] = (_context.TeachingExperiences.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).LastOrDefault().Indx) + 1;
            }
            catch (Exception)
            {

                ViewData["Indx"] = 1;
            }
            return View();
        }

        // POST: TeachingExperiences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ArDescription,EnDescription,ArTitle,EnTitle,IsCurrent,FromYear,ToYear,Indx")] TeachingExperience teachingExperience)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teachingExperience);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            return View(teachingExperience);
        }

        // GET: TeachingExperiences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachingExperience = await _context.TeachingExperiences.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (teachingExperience == null)
            {
                return NotFound();
            }            
            return View(teachingExperience);
        }

        // POST: TeachingExperiences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ArDescription,EnDescription,ArTitle,EnTitle,IsCurrent,FromYear,ToYear,Indx")] TeachingExperience teachingExperience)
        {
            if (id != teachingExperience.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teachingExperience);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeachingExperienceExists(teachingExperience.Id))
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
            return View(teachingExperience);
        }

        // GET: TeachingExperiences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachingExperience = await _context.TeachingExperiences
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (teachingExperience == null)
            {
                return NotFound();
            }

            return View(teachingExperience);
        }

        // POST: TeachingExperiences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teachingExperience = await _context.TeachingExperiences.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.TeachingExperiences.Remove(teachingExperience);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeachingExperienceExists(int id)
        {
            return _context.TeachingExperiences.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
