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
    public class PublishersController : Controller
    {
        private readonly ARIDDbContext _context;
        private int PagSize = 10;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;


        public PublishersController(ARIDDbContext context, UserManager<ApplicationUser> userMrg,IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: Publishers
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            PublisherViewModel publisherVM = new PublisherViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                publisherVM = new PublisherViewModel()
                {
                    publishers = _context.Publishers.Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                publisherVM = new PublisherViewModel()
                {
                    publishers = _context.Publishers.Where(a => a.ArName.Contains(SearchString) || a.EnName.Contains(SearchString) || a.Description.Contains(SearchString) || a.Logo.Contains(SearchString) || a.Email.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Country.ArCountryName.Contains(SearchString) || a.Website.Contains(SearchString) || a.Fb.Contains(SearchString) || a.ContactUs.Contains(SearchString) || a.EnName.Contains(SearchString) || a.Languages.Contains(SearchString) || a.Tags.Contains(SearchString)|| a.Address.Contains(SearchString)|| a.RegistrationNumber.Contains(SearchString)).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality)
                };
            }
            if (SearchString == "reported")
            {
                publisherVM = new PublisherViewModel()
                {
                    publishers = _context.Publishers.Where(a => a.ReportType.ToString() != "None").Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality)
                };
            }
            if (SearchString == "featured")
            {
                publisherVM = new PublisherViewModel()
                {
                    publishers = _context.Publishers.Where(a => a.IsFeatured == true).Include(p => p.ApplicationUser).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality)
                };
            }
            if (SearchString == "acceptance")
            {
                publisherVM = new PublisherViewModel()
                {
                    publishers = _context.Publishers.Where(a => a.IsAdminAccepted == false).Include(p => p.ApplicationUser).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality)
                };
            }

            var count = publisherVM.publishers.Count();
            publisherVM.publishers = publisherVM.publishers.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            publisherVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.Publisher.Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Speciality);
            return View(publisherVM);
        }

        // GET: Publishers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .Include(p => p.ApplicationUser)
                .Include(p => p.Country)
                .Include(p => p.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // GET: Publishers/Create
        public IActionResult Create()
        {
         
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            return View();
        }

        // POST: Publishers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArName,EnName,Description,Email,ApplicationUserId,Status,EstablishmentDate,DateOfRecord,SpecialityId,CountryId,Address,Website,Fb,Twitter,ContactUs,IsFeatured,ReportType,Logo,IsVisible,RegistrationNumber,Languages,Tags,IsAdminAccepted")] Publisher publisher,IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                publisher.ApplicationUserId = _userManager.GetUserId(User);
                publisher.DateOfRecord = DateTime.Now;
                          _context.Add(publisher);
                publisher.Logo = await UserFile.UploadeNewImageAsync(publisher.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                //publisher.Description = publisher.Description.Replace("\n", "<br/>");



                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", publisher.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publisher.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publisher.SpecialityId);
            return View(publisher);
        }

        // GET: Publishers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.SingleOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", publisher.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publisher.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publisher.SpecialityId);
            return View(publisher);
        }

        // POST: Publishers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArName,EnName,Description,Email,ApplicationUserId,Status,EstablishmentDate,DateOfRecord,SpecialityId,CountryId,Address,Website,Fb,Twitter,ContactUs,IsFeatured,ReportType,Logo,IsVisible,RegistrationNumber,Languages,Tags,IsAdminAccepted")] Publisher publisher)
        {
            if (id != publisher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    publisher.ApplicationUserId = _userManager.GetUserId(User);
                    publisher.DateOfRecord = DateTime.Now;
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.Id))
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
            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", publisher.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publisher.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publisher.SpecialityId);
            return View(publisher);
        }

        // GET: Publishers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .Include(p => p.ApplicationUser)
                .Include(p => p.Country)
                .Include(p => p.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.SingleOrDefaultAsync(m => m.Id == id);
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Report(int id)
        {
            Publisher publisher = _context.Publishers.Where(m => m.Id == id).SingleOrDefault();
            if (publisher == null)
            {
                return NotFound();
            }
            publisher.ReportType = ReportType.NotRelated;
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

    }
}
