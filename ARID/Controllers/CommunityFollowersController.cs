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
using Microsoft.AspNetCore.Identity;

namespace ARID.Controllers
{
    public class CommunityFollowersController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;


        public CommunityFollowersController(UserManager<ApplicationUser> userMrg, ARIDDbContext context)
        {
            _context = context;
            _userManager = userMrg;

        }

        // GET: CommunityFollowers
        public async Task<IActionResult> Index(int id, string ss, string userid)
        {
            CommunityFollowersViewModel CommunityFollowersVM = new CommunityFollowersViewModel();

            if (userid == null)
            {
                CommunityFollowersVM = new CommunityFollowersViewModel
                {
                    Community = _context.Communities.SingleOrDefault(c => c.Id == id),
                    CommunityFollowers = _context.CommunityFollowers.Where(a => a.CommunityId == id).Include(c => c.ApplicationUser).Include(c => c.Community),
                    CommunityFollower = _context.CommunityFollowers.Include(c => c.ApplicationUser).Include(c => c.Community).FirstOrDefault(a => a.CommunityId == id),
                    ApplicationUsers = _context.ApplicationUsers.Where(a=>a.Id==null)
                };
            }
            else
            {
                CommunityFollowersVM = new CommunityFollowersViewModel
                {
                    Community = _context.Communities.SingleOrDefault(c => c.Id == id),
                    CommunityFollower = _context.CommunityFollowers.Include(c => c.ApplicationUser).Include(c => c.Community).FirstOrDefault(a => a.CommunityId == id),
                    CommunityFollowers = _context.CommunityFollowers.Where(a => a.CommunityId == id).Include(c => c.ApplicationUser).Include(c => c.Community),
                    ApplicationUsers = _context.ApplicationUsers
   .Include(a => a.Country)
   .Include(a => a.Faculty.Speciality)
   .Include(a => a.University)
   .Where(a => a.Faculty.Speciality.Name.Contains(ss)
   || a.Summary.Contains(ss)
   || a.ArName.Contains(ss)
   || a.EnName.Contains(ss)
   || _context.EducationalLevels
   .Where(m => m.ArCertificateName.Contains(ss) && m.ApplicationUserId == a.Id).Count() > 0
   || _context.AcademicPositions
   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
   || _context.AcademicActivities
   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss) || m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
   || _context.TeachingExperiences
   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss) || m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
   || _context.Posts
   .Where(m => m.Title.Contains(ss) || m.File.Contains(ss) || m.Body.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
   || _context.Publications
   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0),

                };

                _context.CommunityFollowers.Add(new CommunityFollower
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = userid,
                    IsAdmin = false,
                    NotifyMe = true,
                    CommunityId = id
                });
                _context.SaveChanges();
            }
            ViewData["CommunityName"] = _context.Communities.SingleOrDefault(a => a.Id == id).Name;
            ViewData["Communityid"] = id;
            return View(CommunityFollowersVM);
        }


        // GET: CommunityFollowers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityFollower = await _context.CommunityFollowers
                .Include(c => c.ApplicationUser)
                .Include(c => c.Community)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (communityFollower == null)
            {
                return NotFound();
            }

            return View(communityFollower);
        }

        // GET: CommunityFollowers/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CommunityId"] = new SelectList(_context.Communities, "Id", "BgImage");
            return View();
        }

        // POST: CommunityFollowers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CommunityId,ApplicationUserId,NotifyMe,IsAdmin,IsAccepted")] CommunityFollower communityFollower)
        {
            if (ModelState.IsValid)
            {
                communityFollower.Id = Guid.NewGuid();
                _context.Add(communityFollower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", communityFollower.ApplicationUserId);
            ViewData["CommunityId"] = new SelectList(_context.Communities, "Id", "BgImage", communityFollower.CommunityId);
            return View(communityFollower);
        }

        // GET: CommunityFollowers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityFollower = await _context.CommunityFollowers.SingleOrDefaultAsync(m => m.Id == id);
            if (communityFollower == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", communityFollower.ApplicationUserId);
            ViewData["CommunityId"] = new SelectList(_context.Communities, "Id", "Id", communityFollower.CommunityId);
            return View(communityFollower);
        }

        // POST: CommunityFollowers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CommunityId,ApplicationUserId,NotifyMe,IsAdmin,IsAccepted")] CommunityFollower communityFollower)
        {
            if (id != communityFollower.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(communityFollower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommunityFollowerExists(communityFollower.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = communityFollower.CommunityId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", communityFollower.ApplicationUserId);
            ViewData["CommunityId"] = new SelectList(_context.Communities, "Id", "BgImage", communityFollower.CommunityId);
            return View(communityFollower);
        }

        // GET: CommunityFollowers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityFollower = await _context.CommunityFollowers
                .Include(c => c.ApplicationUser)
                .Include(c => c.Community)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (communityFollower == null)
            {
                return NotFound();
            }

            return View(communityFollower);
        }

        // POST: CommunityFollowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var communityFollower = await _context.CommunityFollowers.SingleOrDefaultAsync(m => m.Id == id);
            _context.CommunityFollowers.Remove(communityFollower);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommunityFollowerExists(Guid id)
        {
            return _context.CommunityFollowers.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Join(int id,string uid)
        {
            string userid = uid;

            CommunityFollower communityFollower = _context.CommunityFollowers.SingleOrDefault(m => m.CommunityId == id && m.ApplicationUserId == userid);
            if (communityFollower == null)
            {
                return NotFound();
            }


            if (_context.CommunityFollowers.Where(f => f.CommunityId == id && f.ApplicationUserId == userid && f.IsAccepted==false).Count() > 0)
            {
                communityFollower.IsAccepted = true;
                _context.Update(communityFollower);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult UnJoin(int id,string uid)
        {
            string userid = uid;

            CommunityFollower communityFollower = _context.CommunityFollowers.SingleOrDefault(m => m.CommunityId == id && m.ApplicationUserId == userid);
            if (communityFollower == null)
            {
                return NotFound();
            }


            if (_context.CommunityFollowers.Where(f => f.CommunityId == id && f.IsAccepted==false && f.ApplicationUserId == userid).Count() > 0)
            {
                _context.CommunityFollowers.Remove(communityFollower);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

    }
}
