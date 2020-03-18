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
    [Authorize(Roles = RoleName.Admins)]
    public class ExpertisesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpertisesController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expertises
        public async Task<IActionResult> Index()
        {
            var expertiseViewModel = new ExpertiseViewModel
            {
                Expertises = _context.Expertises
                               .Include(e => e.Speciality)
                .Include(e => e.User),
                UserExpertise = _context.UserExpertises

            };


            return View(expertiseViewModel);
        }

        // GET: Expertises/Create
        public IActionResult Create()
        {
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: Expertises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SpecialityId,UserId")] Expertise expertise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expertise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", expertise.SpecialityId);
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View(expertise);
        }

        // GET: Expertises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expertise = await _context.Expertises.SingleOrDefaultAsync(m => m.Id == id);
            if (expertise == null)
            {
                return NotFound();
            }
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", expertise.SpecialityId);
            return View(expertise);
        }

        // POST: Expertises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SpecialityId,UserId")] Expertise expertise)
        {
            if (id != expertise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expertise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpertiseExists(expertise.Id))
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
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", expertise.SpecialityId);
            return View(expertise);
        }

        //// GET: Expertises/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var expertise = await _context.Expertises
        //        .Include(e => e.Speciality)
        //        .Include(e => e.User)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (expertise == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(expertise);
        //}

        // POST: Expertises/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var expertise = await _context.Expertises.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Expertises.Remove(expertise);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public ActionResult Delete(IEnumerable<int> ExamPolicyIDs)
        {
            foreach (var item in ExamPolicyIDs)
            {
              
                var UserExpertises = _context.UserExpertises.Where(m => m.ExpertiseId == item);
                _context.UserExpertises.RemoveRange(UserExpertises);

                var delete = _context.Expertises.FirstOrDefault(s => s.Id == item);

                if (delete != null)
                {
                    _context.Expertises.Remove(delete);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool ExpertiseExists(int id)
        {
            return _context.Expertises.Any(e => e.Id == id);
        }
    }
}
