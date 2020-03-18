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

namespace ARID.Controllers
{
    public class CommitteeAchievementsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public CommitteeAchievementsController(ARIDDbContext context,UserManager<ApplicationUser> userMrg)
        {
            _context = context;
            _userManager = userMrg;
        }

        // GET: CommitteeAchievements
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.CommitteeAchievements.Include(c => c.ApplicationUser).Include(c => c.CommitteeProfile);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: CommitteeAchievements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var committeeAchievement = await _context.committeeAchievements
            //    .Include(c => c.ApplicationUser)
            //    .Include(c => c.CommitteeProfile)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (committeeAchievement == null)
            //{
            //    return NotFound();
            //}

            //return View(committeeAchievement);
            CommitteeViewModel CommitteeVM = new CommitteeViewModel()
            {
                CommitteeAchievement = await _context.CommitteeAchievements
.Include(c => c.ApplicationUser)
.Include(c => c.CommitteeProfile)
.SingleOrDefaultAsync(m => m.Id == id),

                ApplicationUser = await _userManager.Users
.Include(c => c.Country)
.Include(c => c.City)
.Include(c => c.University)
.Include(c => c.Faculty)
.Include(a => a.ReferredBy)
.SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),



            };
            return View(CommitteeVM);

        }

        // GET: CommitteeAchievements/Create
        public IActionResult Create(int cid)
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CommitteeProfileId"] = new SelectList(_context.CommitteeProfiles.Where(a=>a.Id==cid), "Id", "Id");
            return View();
        }

        // POST: CommitteeAchievements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CommitteeProfileId,ApplicationUserId,DateOfRecord,AchievementType,Description")] CommitteeAchievement committeeAchievement,int cid)
        {
            if (ModelState.IsValid)
            {
                committeeAchievement.ApplicationUserId = _userManager.GetUserId(User);
                committeeAchievement.Description = committeeAchievement.Description.Replace("\n", "<br/>");


                _context.Add(committeeAchievement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = committeeAchievement.Id });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeAchievement.ApplicationUserId);
            ViewData["CommitteeProfileId"] = new SelectList(_context.CommitteeProfiles, "Id", "Id", committeeAchievement.CommitteeProfileId);
            return View(committeeAchievement);
        }

        // GET: CommitteeAchievements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committeeAchievement = await _context.CommitteeAchievements.SingleOrDefaultAsync(m => m.Id == id);
            if (committeeAchievement == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeAchievement.ApplicationUserId);
            ViewData["CommitteeProfileId"] = new SelectList(_context.CommitteeProfiles, "Id", "Id", committeeAchievement.CommitteeProfileId);
            return View(committeeAchievement);
        }

        // POST: CommitteeAchievements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommitteeProfileId,ApplicationUserId,DateOfRecord,AchievementType,Description")] CommitteeAchievement committeeAchievement)
        {
            if (id != committeeAchievement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(committeeAchievement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommitteeAchievementExists(committeeAchievement.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeAchievement.ApplicationUserId);
            ViewData["CommitteeProfileId"] = new SelectList(_context.CommitteeProfiles, "Id", "Id", committeeAchievement.CommitteeProfileId);
            return View(committeeAchievement);
        }

        // GET: CommitteeAchievements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committeeAchievement = await _context.CommitteeAchievements
                .Include(c => c.ApplicationUser)
                .Include(c => c.CommitteeProfile)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (committeeAchievement == null)
            {
                return NotFound();
            }

            return View(committeeAchievement);
        }

        // POST: CommitteeAchievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var committeeAchievement = await _context.CommitteeAchievements.SingleOrDefaultAsync(m => m.Id == id);
            _context.CommitteeAchievements.Remove(committeeAchievement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommitteeAchievementExists(int id)
        {
            return _context.CommitteeAchievements.Any(e => e.Id == id);
        }
    }
}
