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
    [Authorize(Roles = RoleName.Admins)]
    public class SkillsController : Controller
    {
        private readonly ARIDDbContext _context;


        public SkillsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: Skills
        public async Task<IActionResult> Index(int? id)
        {

            var SkillViewModel = new SkillViewModel
            {

                Skills = _context.Skills
                .Include(s => s.ApplicationUser),
                UserSkills = _context.UserSkills

            };

            if (id != null)
            {
                SkillViewModel = new SkillViewModel
                {

                    Skills = _context.Skills
                   .Include(s => s.ApplicationUser).Where(a => a.SkillCategoryType == SkillCategoryType.DataEntry),
                    UserSkills = _context.UserSkills
                                    };
            }



            //var aRIDDbContext = _context.Skills
            //    .Include(s => s.ApplicationUser)
            //    .Include(s => s.Speciality);
            return View(SkillViewModel);
        }

        // GET: Skills/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");

            return View();
        }

        // POST: Skills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SkillCategoryType,ApplicationUserId")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", skill.ApplicationUserId);
            return View(skill);
        }

        // GET: Skills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills.SingleOrDefaultAsync(m => m.Id == id);
            if (skill == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", skill.ApplicationUserId);

            return View(skill);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SkillCategoryType,ApplicationUserId")] Skill skill)
        {
            if (id != skill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", skill.ApplicationUserId);

            return View(skill);
        }

        // GET: Skills/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var skill = await _context.Skills
        //        .Include(s => s.ApplicationUser)
        //        .Include(s => s.Speciality)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (skill == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(skill);
        //}

        // POST: Skills/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var skill = await _context.Skills.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Skills.Remove(skill);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public ActionResult Delete(IEnumerable<int> ExamPolicyIDs)
        {
            foreach (var item in ExamPolicyIDs)
            {

                var UserSkills = _context.UserSkills.Where(m => m.SkillId == item);
                _context.UserSkills.RemoveRange(UserSkills);

                var delete = _context.Skills.FirstOrDefault(s => s.Id == item);

                if (delete != null)
                {
                    _context.Skills.Remove(delete);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
