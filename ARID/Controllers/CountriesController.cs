using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class CountriesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;

        public CountriesController(ARIDDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.ToListAsync());
        }

        public async Task<IActionResult> Statistics()
        {
            var CountryViewModel = new StatisticsViewModel
            {
                Countries= _context.Countries.ToList(),
                Cities = _context.Cities.ToList(),
                Universities = _context.Universities.ToList(),
                ApplicationUsers = _context.Users.ToList()
            };
            return View(CountryViewModel);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ArCountryName,EnCountryName,CountryCode,ShortName,Flag")] Country country,
            IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                country.Flag = await UserFile.UploadeNewImageAsync(country.Flag,
                    myfile, _environment.WebRootPath, Properties.Resources.CountryFlagFolder, 100, 100);

                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.SingleOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, [Bind("Id,ArCountryName,EnCountryName,CountryCode,ShortName,Flag")] Country country,
            IFormFile myfile)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    country.Flag = await UserFile.UploadeNewImageAsync(
                        _context.Countries.Where(m => m.Id == id).Select(c => c.Flag).First(),
                        myfile, _environment.WebRootPath, Properties.Resources.CountryFlagFolder, 100, 100);

                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
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
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .SingleOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.SingleOrDefaultAsync(m => m.Id == id);
            _context.Countries.Remove(country);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.CountryFlagFolder, country.Flag);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
