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

namespace ARID.Controllers
{
    public class AddressesController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;


        public AddressesController(UserManager<ApplicationUser> userMrg, ARIDDbContext context)
        {
            _context = context;
            _userManager = userMrg;
        }

        // GET: Addresses
        public async Task<IActionResult> Index(string ss,int productPage = 1)
        {
            AddressViewModel addressVM = new AddressViewModel();


            if (ss == null) { 
            addressVM=new AddressViewModel()
            {
                Addresses = _context.Addresses.Include(a => a.ApplicationUser).Include(a => a.City).Include(a => a.Country).Include(a => a.Faculty).Include(a => a.University),
                UserAddresses = _context.Addresses.Where(a=>a.ApplicationUserId==_userManager.GetUserId(User) && a.IsDeleted==false).Include(a => a.ApplicationUser).Include(a => a.City).Include(a => a.Country).Include(a => a.Faculty).Include(a => a.University),
            };
            }
            else
            {
                ViewData["search"] = ss;

                addressVM = new AddressViewModel()
                {
                    Addresses = _context.Addresses.Include(a => a.ApplicationUser).Include(a => a.City).Include(a => a.Country).Include(a => a.Faculty).Include(a => a.University).Where(a=>a.AddressSaveName.Contains(ss) ||a.FullAddress.Contains(ss)||a.PhoneNumber.Contains(ss) || a.ApplicationUser.ArName.Contains(ss)|| a.EnName.Contains(ss)|| a.ArName.Contains(ss)|| a.City.ArCityName.Contains(ss)|| a.City.EnCityName.Contains(ss)|| a.Country.ArCountryName.Contains(ss)|| a.Country.EnCountryName.Contains(ss)|| a.University.ArUniversityName.Contains(ss)|| a.University.EnUniversityName.Contains(ss)|| a.Faculty.ArFacultyName.Contains(ss)|| a.Faculty.EnFacultyName.Contains(ss)),
                    UserAddresses = _context.Addresses.Where(a => a.ApplicationUserId == _userManager.GetUserId(User) && a.IsDeleted == false).Include(a => a.ApplicationUser).Include(a => a.City).Include(a => a.Country).Include(a => a.Faculty).Include(a => a.University),
                };

            }

            var count = addressVM.Addresses.Count();
            addressVM.Addresses = addressVM.Addresses.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            addressVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };


            return View(addressVM);
        }

        // GET: Addresses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.ApplicationUser)
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.Faculty)
                .Include(a => a.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Addresses/Create
        public IActionResult Create()
        {
            var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));
            ViewData["tel"] = applicationuser.PhoneNumber;
            ViewData["arname"] = applicationuser.ArName;
            ViewData["enname"] = applicationuser.EnName;
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", applicationuser.CountryId);
            ViewData["CityId"] = new SelectList(_context.Cities.Where(c=>c.CountryId== applicationuser.CountryId), "Id", "ArCityName" , applicationuser.CityId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(f=>f.UniversityId==applicationuser.UniversityId), "Id", "ArFacultyName",applicationuser.FacultyId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u=>u.CountryId==applicationuser.CountryId), "Id", "ArUniversityName",applicationuser.University);
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullAddress,PhoneNumber,AddressSaveName,ArName,EnName,ApplicationUserId,IsDeleted,CountryId,CityId,UniversityId,FacultyId")] Address address)
        {
            if (ModelState.IsValid)
            {
                address.Id = Guid.NewGuid();
                address.ApplicationUserId = _userManager.GetUserId(User);
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", address.ApplicationUserId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", address.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", address.CountryId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", address.FacultyId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", address.UniversityId);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.Include(a=>a.City).Include(a=>a.Country).Include(a=>a.Faculty).Include(a=>a.University).Include(a=>a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", address.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", address.CountryId);
            ViewData["CityId"] = new SelectList(_context.Cities.Where(a=>a.CountryId==address.CountryId), "Id", "ArCityName", address.CityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities.Where(a => a.CountryId == address.CountryId), "Id", "ArUniversityName", address.UniversityId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(a => a.UniversityId == address.UniversityId), "Id", "ArFacultyName", address.FacultyId);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullAddress,PhoneNumber,AddressSaveName,ArName,EnName,ApplicationUserId,IsDeleted,CountryId,CityId,UniversityId,FacultyId")] Address address)
        {
            if (id != address.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", address.ApplicationUserId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "ArCityName", address.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", address.CountryId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "ArFacultyName", address.FacultyId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", address.UniversityId);
            return View(address);
        }

        // GET: Addresses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.ApplicationUser)
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.Faculty)
                .Include(a => a.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deleting(Guid id)
        {
            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.Id == id);
            address.IsDeleted = true;
            _context.Update(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(Guid id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }
    }
}
