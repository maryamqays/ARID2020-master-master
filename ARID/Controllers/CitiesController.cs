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
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class CitiesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitiesController(UserManager<ApplicationUser> userManager, ARIDDbContext context)//====================
        {
            _userManager = userManager;
            _context = context;    
        }

        // GET: Cities
        public async Task<IActionResult> Index(int id)
        {       
            var currentuser = await _userManager.GetUserAsync(User);//currentuser.City.CountryId
            var aRIDDbContext = _context.Cities.Where(a=>a.CountryId == id).Include(u => u.Country);
            return View(await aRIDDbContext.ToListAsync());
        }

        public async Task<IActionResult> Search(string keyword)
        {
                       var ARIDDbContext = _context.Cities.Where(a => a.Country.ArCountryName.Contains(keyword) || a.Country.EnCountryName.Contains(keyword) || a.EnCityName.Contains(keyword) || a.ArCityName.Contains(keyword)).Include(a => a.Country);
                        return View(await ARIDDbContext.ToListAsync());
          
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArCityName,EnCityName,CountryId")] City city)
        {
            if (ModelState.IsValid)
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", city.CountryId);
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.SingleOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArCityName,EnCityName,CountryId")] City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", city.CountryId);
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.SingleOrDefaultAsync(m => m.Id == id);
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
