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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;

namespace ARID.Controllers
{
    public class SideAdsController : Controller
    {
        private readonly ARIDDbContext _context;
                private UserManager<ApplicationUser> _userManager;
                private IHostingEnvironment _environment;
        public SideAdsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg,IHostingEnvironment environment)
        {
            _context = context;
                        _userManager = userMrg;
                        _environment = environment;
        }

        // GET: SideAds
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.SideAds.Include(s => s.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: SideAds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sideAds = await _context.SideAds
                .Include(s => s.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sideAds == null)
            {
                return NotFound();
            }

            return View(sideAds);
        }

        // GET: SideAds/Create
        public IActionResult Create()
        {
            try
            {
                ViewData["Indx"] = (_context.SideAds.LastOrDefault().Indx) + 1;
            }
            catch (Exception)
            {

                ViewData["Indx"] = 1;
            }
            return View();
        }

        // POST: SideAds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image,ExternalLink,Controller,Action,Indx,Hits,Date,IsVisible,ApplicationUserId,AdsPositionType,TargetType")] SideAd sideAds,IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                sideAds.Image = await UserFile.UploadeNewImageAsync(sideAds.Image,
                 myfile, _environment.WebRootPath, Properties.Resources.Images, 200, 200);
                sideAds.ApplicationUserId = _userManager.GetUserId(User);
                sideAds.Hits = 0;
                sideAds.Date = DateTime.Now;

                _context.Add(sideAds);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
          
            return View(sideAds);
        }

        // GET: SideAds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sideAds = await _context.SideAds.SingleOrDefaultAsync(m => m.Id == id);
            if (sideAds == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", sideAds.ApplicationUserId);
            return View(sideAds);
        }

        // POST: SideAds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image,ExternalLink,Controller,Action,Indx,Hits,Date,IsVisible,ApplicationUserId,AdsPositionType,TargetType")] SideAd sideAds)
        {
            if (id != sideAds.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sideAds);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SideAdsExists(sideAds.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", sideAds.ApplicationUserId);
            return View(sideAds);
        }

        // GET: SideAds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sideAds = await _context.SideAds
                .Include(s => s.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sideAds == null)
            {
                return NotFound();
            }

            return View(sideAds);
        }

        // POST: SideAds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sideAds = await _context.SideAds.SingleOrDefaultAsync(m => m.Id == id);
            _context.SideAds.Remove(sideAds);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SideAdsExists(int id)
        {
            return _context.SideAds.Any(e => e.Id == id);
        }
    }
}
