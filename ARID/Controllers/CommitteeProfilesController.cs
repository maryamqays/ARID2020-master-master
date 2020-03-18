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
    public class CommitteeProfilesController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;

        public CommitteeProfilesController(ARIDDbContext context, UserManager<ApplicationUser> userMrg)
        {
            _context = context;
            _userManager = userMrg;
        }

        // GET: CommitteeProfiles
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            CommitteeProfileViewModel CommitteeProfileVM = new CommitteeProfileViewModel();

            if (string.IsNullOrEmpty(SearchString))
            {
                CommitteeProfileVM = new CommitteeProfileViewModel()
                {
                    CommitteeProfiles = _context.CommitteeProfiles.Include(c => c.ApplicationUser).Include(c => c.Committee)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CommitteeProfileVM = new CommitteeProfileViewModel()
                {
                    CommitteeProfiles = _context.CommitteeProfiles.Include(c => c.ApplicationUser).Include(c => c.Committee)
                };
            }

            var count = CommitteeProfileVM.CommitteeProfiles.Count();
            CommitteeProfileVM.CommitteeProfiles = CommitteeProfileVM.CommitteeProfiles.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CommitteeProfileVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.committeeProfiles.Include(c => c.ApplicationUser).Include(c => c.Committee);
            return View(CommitteeProfileVM);
        }

        // GET: CommitteeProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["CommitteeProfileId"] = id;

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var committeeProfile = await _context.committeeProfiles
            //    .Include(c => c.ApplicationUser)
            //    .Include(c => c.Committee)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (committeeProfile == null)
            //{
            //    return NotFound();
            //}

            //return View(committeeProfile);

            CommitteeViewModel CommitteeVM = new CommitteeViewModel()
            {
                CommitteeAchievements = _context.CommitteeAchievements.Where(a => a.CommitteeProfileId == id).Include(c => c.ApplicationUser).Include(c => c.CommitteeProfile),
                CommitteeProfile = await _context.CommitteeProfiles
        .Include(c => c.ApplicationUser)
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


        // GET: CommitteeProfiles/Create
        public IActionResult Create(int cid)
        {
           // ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CommitteeId"] = new SelectList(_context.Committees.Where(a=>a.Id==cid), "Id", "Name");
            return View();
        }

        // POST: CommitteeProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? cid, [Bind("Id,ApplicationUserId,IsActive,IsCommitteeAdmin,JoinDate,CommitteeId,Task")] CommitteeProfile committeeProfile)
        {
            if (ModelState.IsValid)
            {
                committeeProfile.JoinDate = DateTime.Now;

                _context.Add(committeeProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = committeeProfile.Id });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeProfile.ApplicationUserId);
            ViewData["CommitteeId"] = new SelectList(_context.Committees.Where(a => a.Id == committeeProfile.CommitteeId), "Id", "Name", cid);
            return View(committeeProfile);
        }

        // GET: CommitteeProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committeeProfile = await _context.CommitteeProfiles.Include(a => a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (committeeProfile == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeProfile.ApplicationUserId);
            ViewData["CommitteeId"] = new SelectList(_context.Committees, "Id", "Name", committeeProfile.CommitteeId);
            return View(committeeProfile);
        }

        // POST: CommitteeProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,IsActive,IsCommitteeAdmin,JoinDate,CommitteeId,Task")] CommitteeProfile committeeProfile)
        {
            if (id != committeeProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(committeeProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommitteeProfileExists(committeeProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Committees", new {/* routeValues, for example: */ id = committeeProfile.CommitteeId });

            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committeeProfile.ApplicationUserId);
            ViewData["CommitteeId"] = new SelectList(_context.Committees, "Id", "Name", committeeProfile.CommitteeId);
            return View(committeeProfile);
        }

        // GET: CommitteeProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committeeProfile = await _context.CommitteeProfiles
                .Include(c => c.ApplicationUser)
                .Include(c => c.Committee)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (committeeProfile == null)
            {
                return NotFound();
            }

            return View(committeeProfile);
        }

        // POST: CommitteeProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var committeeProfile = await _context.CommitteeProfiles.SingleOrDefaultAsync(m => m.Id == id);
            var CommitteeId = committeeProfile.CommitteeId;
            _context.CommitteeProfiles.Remove(committeeProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Committees", new {/* routeValues, for example: */ id = CommitteeId });
        }

        private bool CommitteeProfileExists(int id)
        {
            return _context.CommitteeProfiles.Any(e => e.Id == id);
        }
    }
}
