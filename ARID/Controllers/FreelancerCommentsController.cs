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
    public class FreelancerCommentsController : Controller
    {
        private readonly ARIDDbContext _context;

        public FreelancerCommentsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: FreelancerComments
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerComments.Include(f => f.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerComments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerComment = await _context.FreelancerComments
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerComment == null)
            {
                return NotFound();
            }

            return View(freelancerComment);
        }

        // GET: FreelancerComments/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: FreelancerComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,File,FreelancerProjectId,ApplicationUserId,Commentary,DateOfRecord")] FreelancerComment freelancerComment)
        {
            if (ModelState.IsValid)
            {
                freelancerComment.Id = Guid.NewGuid();
                _context.Add(freelancerComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerComment.ApplicationUserId);
            return View(freelancerComment);
        }

        // GET: FreelancerComments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerComment = await _context.FreelancerComments.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerComment == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerComment.ApplicationUserId);
            return View(freelancerComment);
        }

        // POST: FreelancerComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,File,FreelancerProjectId,ApplicationUserId,Commentary,DateOfRecord")] FreelancerComment freelancerComment)
        {
            if (id != freelancerComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancerComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerCommentExists(freelancerComment.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerComment.ApplicationUserId);
            return View(freelancerComment);
        }

        // GET: FreelancerComments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerComment = await _context.FreelancerComments
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerComment == null)
            {
                return NotFound();
            }

            return View(freelancerComment);
        }

        // POST: FreelancerComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var freelancerComment = await _context.FreelancerComments.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerComments.Remove(freelancerComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerCommentExists(Guid id)
        {
            return _context.FreelancerComments.Any(e => e.Id == id);
        }
    }
}
