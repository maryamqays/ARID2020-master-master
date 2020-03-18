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
    //[Authorize(Roles = RoleName.Members)]
    //  [Authorize(Roles = RoleName.Admins)]
    public class UserSkillsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSkillsController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserSkills
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.UserSkills
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .Include(u => u.ApplicationUser).Include(u => u.Skill);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: UserSkills/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            var skillsList = _context.Skills
                .Where(e => string.IsNullOrEmpty(e.ApplicationUserId));
            ViewData["SkillId"] = new SelectList(skillsList, "Id", "Name");
            return View();
        }

        // POST: UserSkills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SkillId,ApplicationUserId")] UserSkill userSkill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userSkill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            var skillsList = _context.Skills
                .Where(e => string.IsNullOrEmpty(e.ApplicationUserId));
            ViewData["SkillId"] = new SelectList(skillsList, "Id", "Name", userSkill.SkillId);
            return View(userSkill);
        }

        // GET: UserSkills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSkill = await _context.UserSkills.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (userSkill == null)
            {
                return NotFound();
            }
            var skillsList = _context.Skills
                .Where(e => string.IsNullOrEmpty(e.ApplicationUserId));
            ViewData["SkillId"] = new SelectList(skillsList, "Id", "Name", userSkill.SkillId);
            return View(userSkill);
        }

        // POST: UserSkills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SkillId,ApplicationUserId")] UserSkill userSkill)
        {
            if (id != userSkill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userSkill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSkillExists(userSkill.Id))
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
            var skillsList = _context.Skills
                .Where(e => string.IsNullOrEmpty(e.ApplicationUserId));
            ViewData["SkillId"] = new SelectList(skillsList, "Id", "Name", userSkill.SkillId);
            return View(userSkill);
        }

        // GET: UserSkills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSkill = await _context.UserSkills
                .Include(u => u.ApplicationUser)
                .Include(u => u.Skill)
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (userSkill == null)
            {
                return NotFound();
            }

            return View(userSkill);
        }

        // POST: UserSkills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userSkill = await _context.UserSkills.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.UserSkills.Remove(userSkill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserSkillExists(int id)
        {
            return _context.UserSkills.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
