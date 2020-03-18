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
    public class FreelancerProposalsController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerProposalsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerProposals
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerProposals.Include(f => f.ApplicationUser).Include(f => f.ReportedByUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerProposals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerProposal = await _context.FreelancerProposals
                .Include(f => f.ApplicationUser)
                .Include(f => f.ReportedByUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerProposal == null)
            {
                return NotFound();
            }

            return View(freelancerProposal);
        }

        // GET: FreelancerProposals/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ReportedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: FreelancerProposals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,IsNameVisible,IsPrivate,IsAssigned,IsReported,Isvisible,ReportedByUserId,Description,DaysRequired,FreelancerProjectId,DateofRecord,DateOfAssignment,DeliveredDate,BidAmount,ProposalStatus")] FreelancerProposal freelancerProposal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freelancerProposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ApplicationUserId);
            ViewData["ReportedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ReportedByUserId);
            return View(freelancerProposal);
        }

        // GET: FreelancerProposals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var freelancerProposal = await _context.FreelancerProposals.Include(f => f.FreelancerProject).SingleOrDefaultAsync(m => m.Id == id);
            FreelancerProject freelancerProject = _context.FreelancerProjects.SingleOrDefault(f => f.Id == freelancerProposal.FreelancerProjectId);

            if (id == null || freelancerProject.FreelancerProjectStatus != FreelancerProjectStatus.Acceptingoffers || FreelancerProject.GetHours(freelancerProposal.DateofRecord) > 24)
            {
                return NotFound();
            }

            if (freelancerProposal == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == freelancerProposal.ApplicationUserId), "Id", "Id", freelancerProposal.ApplicationUserId);
            ViewData["ReportedByUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == freelancerProposal.ReportedByUserId), "Id", "Id", freelancerProposal.ReportedByUserId);
            return View(freelancerProposal);
        }

        // POST: FreelancerProposals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,IsNameVisible,IsPrivate,IsAssigned,IsReported,Isvisible,ReportedByUserId,Description,DaysRequired,FreelancerProjectId,DateofRecord,DateOfAssignment,DeliveredDate,BidAmount,ProposalStatus")] FreelancerProposal freelancerProposal)
        {
            if (id != freelancerProposal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerProposal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerProposalExists(freelancerProposal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "FreelancerProjects", new { id = freelancerProposal.FreelancerProjectId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ApplicationUserId);
            ViewData["ReportedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ReportedByUserId);
            return View(freelancerProposal);
        }

        // GET: FreelancerProposals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerProposal = await _context.FreelancerProposals
                .Include(f => f.ApplicationUser)
                .Include(f => f.ReportedByUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerProposal == null)
            {
                return NotFound();
            }

            return View(freelancerProposal);
        }

        // POST: FreelancerProposals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerProposal = await _context.FreelancerProposals.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerProposals.Remove(freelancerProposal);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "FreelancerProjects", new { id = freelancerProposal.FreelancerProjectId });
        }

        private bool FreelancerProposalExists(int id)
        {
            return _context.FreelancerProposals.Any(e => e.Id == id);
        }
    }
}
