using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;

namespace ARID.Controllers
{
    public class FreelancerSkillVerificationsController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerSkillVerificationsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerSkillVerifications
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerSkillVerifications.Include(f => f.ApplicationUser).Include(f => f.Skill).Include(f => f.VerifiedByUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerSkillVerifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerSkillVerification = await _context.FreelancerSkillVerifications
                .Include(f => f.ApplicationUser)
                .Include(f => f.Skill)
                .Include(f => f.VerifiedByUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerSkillVerification == null)
            {
                return NotFound();
            }

            return View(freelancerSkillVerification);
        }

        // GET: FreelancerSkillVerifications/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name");
            ViewData["VerifiedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: FreelancerSkillVerifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,VerifiedByUserId,SkillId,Details,IsVerified,IsArabic,IsEnglish,SinceDate,Certificates,ExternalLink")] FreelancerSkillVerification freelancerSkillVerification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freelancerSkillVerification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.ApplicationUserId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", freelancerSkillVerification.SkillId);
            ViewData["VerifiedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.VerifiedByUserId);
            return View(freelancerSkillVerification);
        }

        // GET: FreelancerSkillVerifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerSkillVerification = await _context.FreelancerSkillVerifications.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerSkillVerification == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.ApplicationUserId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", freelancerSkillVerification.SkillId);
            ViewData["VerifiedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.VerifiedByUserId);
            return View(freelancerSkillVerification);
        }

        // POST: FreelancerSkillVerifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,VerifiedByUserId,SkillId,Details,IsVerified,IsArabic,IsEnglish,SinceDate,Certificates,ExternalLink")] FreelancerSkillVerification freelancerSkillVerification)
        {
            if (id != freelancerSkillVerification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerSkillVerification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerSkillVerificationExists(freelancerSkillVerification.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.ApplicationUserId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", freelancerSkillVerification.SkillId);
            ViewData["VerifiedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerSkillVerification.VerifiedByUserId);
            return View(freelancerSkillVerification);
        }

        // GET: FreelancerSkillVerifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerSkillVerification = await _context.FreelancerSkillVerifications
                .Include(f => f.ApplicationUser)
                .Include(f => f.Skill)
                .Include(f => f.VerifiedByUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerSkillVerification == null)
            {
                return NotFound();
            }

            return View(freelancerSkillVerification);
        }

        // POST: FreelancerSkillVerifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerSkillVerification = await _context.FreelancerSkillVerifications.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerSkillVerifications.Remove(freelancerSkillVerification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerSkillVerificationExists(int id)
        {
            return _context.FreelancerSkillVerifications.Any(e => e.Id == id);
        }
    }
}
