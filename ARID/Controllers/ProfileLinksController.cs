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
    // [Authorize(Roles = RoleName.Admins)]
    public class ProfileLinksController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileLinksController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProfileLinks
        public async Task<IActionResult> Index()
        {
            var profileLinks = _context.ProfileLinks
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User));           
            return View(await profileLinks.ToListAsync());
        }

        // GET: ProfileLinks/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: ProfileLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ProfileType,ProfileUrl,AccessType")] ProfileLink profileLink)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profileLink);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            return View(profileLink);
        }

        // GET: ProfileLinks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profileLink = await _context.ProfileLinks.SingleOrDefaultAsync(m => m.Id == id);
            if (profileLink == null)
            {
                return NotFound();
            }

            return View(profileLink);
        }

        // POST: ProfileLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,ApplicationUserId,ProfileType,ProfileUrl,AccessType")]
            ProfileLink profileLink)
        {
            if (id != profileLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profileLink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileLinkExists(profileLink.Id))
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

            return View(profileLink);
        }

        // GET: ProfileLinks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profileLink = await _context.ProfileLinks.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (profileLink == null)
            {
                return NotFound();
            }

            return View(profileLink);
        }

        // POST: ProfileLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profileLink = await _context.ProfileLinks.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.ProfileLinks.Remove(profileLink);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileLinkExists(int id)
        {
            return _context.ProfileLinks.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
