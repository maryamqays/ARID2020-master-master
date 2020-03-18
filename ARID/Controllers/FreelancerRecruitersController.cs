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
    public class FreelancerRecruitersController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerRecruitersController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerRecruiters
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerRecruiters.Include(f => f.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerRecruiters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRecruiter = await _context.FreelancerRecruiters
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRecruiter == null)
            {
                return NotFound();
            }

            return View(freelancerRecruiter);
        }

        // GET: FreelancerRecruiters/Create
        public IActionResult Create(int pid,string SearchString)
        {
            FreelancerRecruiterViewModel FreelancerRecruiterVM = new FreelancerRecruiterViewModel()
            {
                ApplicationUsers = _context.ApplicationUsers.Where(a => a.ArName.Contains(SearchString) || a.EnName.Contains(SearchString) || a.Email.Contains(SearchString) || a.Id.Contains(SearchString) || a.UserName.Contains(SearchString)),
                FreelancerRecruiters = _context.FreelancerRecruiters.Where(a => a.FreelancerProjectId==pid),
            };
            ViewData["pid"]= pid;
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["FreelancerProjectId"] = new SelectList(_context.FreelancerProjects.Where(a=>a.Id==pid), "Id", "Name");
            return View(FreelancerRecruiterVM);
        }

        // POST: FreelancerRecruiters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,FreelancerProjectId,DateOfRecord,RecruiterStatus,ReportToAdmin")] FreelancerRecruiter freelancerRecruiter)
        {
            if (ModelState.IsValid)
            {
                freelancerRecruiter.RecruiterStatus = RecruiterStatus.Free;
                freelancerRecruiter.DateOfRecord = DateTime.Now;

                _context.Add(freelancerRecruiter);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "FreeLancerProjects", new {/* routeValues, for example: */ id = freelancerRecruiter.FreelancerProjectId });

                //return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRecruiter.ApplicationUserId);
            return View(freelancerRecruiter);
        }

        // GET: FreelancerRecruiters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRecruiter = await _context.FreelancerRecruiters.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRecruiter == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRecruiter.ApplicationUserId);
            return View(freelancerRecruiter);
        }

        // POST: FreelancerRecruiters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,FreelancerProjectId,DateOfRecord,RecruiterStatus,ReportToAdmin")] FreelancerRecruiter freelancerRecruiter)
        {
            if (id != freelancerRecruiter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerRecruiter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerRecruiterExists(freelancerRecruiter.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRecruiter.ApplicationUserId);
            return View(freelancerRecruiter);
        }

        // GET: FreelancerRecruiters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerRecruiter = await _context.FreelancerRecruiters
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerRecruiter == null)
            {
                return NotFound();
            }

            return View(freelancerRecruiter);
        }

        // POST: FreelancerRecruiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerRecruiter = await _context.FreelancerRecruiters.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerRecruiters.Remove(freelancerRecruiter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerRecruiterExists(int id)
        {
            return _context.FreelancerRecruiters.Any(e => e.Id == id);
        }
    }
}
