using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class CommitteesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;

        public CommitteesController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: Committees
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            CommitteeViewModel CommitteeVM = new CommitteeViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CommitteeVM = new CommitteeViewModel()
                {
                    Committees = _context.Committees.Include(p => p.ApplicationUser),
                    CommitteeProfiles = _context.CommitteeProfiles.Include(p => p.ApplicationUser).Include(p => p.Committee)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CommitteeVM = new CommitteeViewModel()
                {
                    Committees = _context.Committees.Where(a => a.Name.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString)).Include(p => p.ApplicationUser),
                    CommitteeProfiles = _context.CommitteeProfiles.Include(p => p.ApplicationUser).Include(p => p.Committee)

                };
            }

            var count = CommitteeVM.Committees.Count();
            CommitteeVM.Committees = CommitteeVM.Committees.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CommitteeVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.Committees.Include(c => c.ApplicationUser);
            return View(CommitteeVM);
        }

        // GET: Committees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["CommitteeId"] = id;

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var committee = await _context.Committees
            //    .Include(c => c.ApplicationUser)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (committee == null)
            //{
            //    return NotFound();
            //}

            //return View(committee);
            CommitteeViewModel CommitteeVM = new CommitteeViewModel()
            {
                CommitteeAchievements=_context.CommitteeAchievements.Include(a=>a.CommitteeProfile).Include(a=>a.ApplicationUser),
                CommitteeProfiles = _context.CommitteeProfiles.Where(a => a.CommitteeId == id).Include(c => c.ApplicationUser).Include(c => c.Committee),
                Committee = await _context.Committees
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

        // GET: Committees/Create
        [Authorize(Roles = "Admins")]
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Committees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DateOfRecord,StartDate,EndDate,Image,ApplicationUserId,IsActive")] Committee committee, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                committee.ApplicationUserId = _userManager.GetUserId(User);
                committee.DateOfRecord = DateTime.Now;
                committee.Image = await UserFile.UploadeNewImageAsync(committee.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);


                _context.Add(committee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committee.ApplicationUserId);
            return View(committee);
        }

        // GET: Committees/Edit/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees.Include(a => a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (committee == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committee.ApplicationUserId);
            return View(committee);
        }

        // POST: Committees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateOfRecord,StartDate,EndDate,Image,ApplicationUserId,IsActive")] Committee committee, IFormFile myfile)
        {
            if (id != committee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    committee.Image = await UserFile.UploadeNewImageAsync(committee.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);


                    _context.Update(committee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommitteeExists(committee.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", committee.ApplicationUserId);
            return View(committee);
        }

        // GET: Committees/Delete/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        // POST: Committees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.Id == id);
            _context.Committees.Remove(committee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommitteeExists(int id)
        {
            return _context.Committees.Any(e => e.Id == id);
        }
    }
}
