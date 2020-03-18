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
    public class MOOCProviderFollowersController : Controller
    {
        private readonly ARIDDbContext _context;

        public MOOCProviderFollowersController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: MOOCProviderFollowers
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.MOOCProviderFollowers.Include(m => m.ApplicationUser).Include(m => m.MOOCProvider);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: MOOCProviderFollowers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCProviderFollower = await _context.MOOCProviderFollowers
                .Include(m => m.ApplicationUser)
                .Include(m => m.MOOCProvider)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCProviderFollower == null)
            {
                return NotFound();
            }

            return View(mOOCProviderFollower);
        }

        // GET: MOOCProviderFollowers/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email");
            return View();
        }

        // POST: MOOCProviderFollowers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MOOCProviderId,ApplicationUserId")] MOOCProviderFollower mOOCProviderFollower)
        {
            if (ModelState.IsValid)
            {
                mOOCProviderFollower.Id = Guid.NewGuid();
                _context.Add(mOOCProviderFollower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProviderFollower.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email", mOOCProviderFollower.MOOCProviderId);
            return View(mOOCProviderFollower);
        }

        // GET: MOOCProviderFollowers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCProviderFollower = await _context.MOOCProviderFollowers.SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCProviderFollower == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProviderFollower.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email", mOOCProviderFollower.MOOCProviderId);
            return View(mOOCProviderFollower);
        }

        // POST: MOOCProviderFollowers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MOOCProviderId,ApplicationUserId")] MOOCProviderFollower mOOCProviderFollower)
        {
            if (id != mOOCProviderFollower.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mOOCProviderFollower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MOOCProviderFollowerExists(mOOCProviderFollower.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProviderFollower.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email", mOOCProviderFollower.MOOCProviderId);
            return View(mOOCProviderFollower);
        }

        // GET: MOOCProviderFollowers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCProviderFollower = await _context.MOOCProviderFollowers
                .Include(m => m.ApplicationUser)
                .Include(m => m.MOOCProvider)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCProviderFollower == null)
            {
                return NotFound();
            }

            return View(mOOCProviderFollower);
        }

        // POST: MOOCProviderFollowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var mOOCProviderFollower = await _context.MOOCProviderFollowers.SingleOrDefaultAsync(m => m.Id == id);
            _context.MOOCProviderFollowers.Remove(mOOCProviderFollower);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MOOCProviderFollowerExists(Guid id)
        {
            return _context.MOOCProviderFollowers.Any(e => e.Id == id);
        }
    }
}
