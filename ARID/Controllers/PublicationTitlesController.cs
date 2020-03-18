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
    public class PublicationTitlesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;   
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;


        public PublicationTitlesController(ARIDDbContext context, UserManager<ApplicationUser> userMrg,IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: PublicationTitles
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            PublicationTitleViewModel publicationTitleVM = new PublicationTitleViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                 publicationTitleVM = new PublicationTitleViewModel()
                {
                    PublicationTitles = _context.PublicationTitles.Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                 publicationTitleVM = new PublicationTitleViewModel()
                {
                    PublicationTitles = _context.PublicationTitles.Where(a => a.ArName.Contains(SearchString)||a.EnName.Contains(SearchString)||a.ArDescription.Contains(SearchString) || a.Logo.Contains(SearchString)|| a.PISSN.Contains(SearchString)|| a.EISSN.Contains(SearchString)|| a.Email.Contains(SearchString)|| a.ApplicationUser.ArName.Contains(SearchString) || a.Country.ArCountryName.Contains(SearchString) || a.Website.Contains(SearchString) || a.Fb.Contains(SearchString) || a.ContactUs.Contains(SearchString) || a.EnName.Contains(SearchString) || a.Languages.Contains(SearchString) || a.Tags.Contains(SearchString)).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality)
                };
            }
            if (SearchString == "reported")
            {
                publicationTitleVM = new PublicationTitleViewModel
                {
                    PublicationTitles = _context.PublicationTitles.Where(a => a.ReportType.ToString() != "None").Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality)
                };
            }
            if (SearchString == "featured")
            {
                publicationTitleVM = new PublicationTitleViewModel
                {
                    PublicationTitles = _context.PublicationTitles.Where(a => a.IsFeatured == true).Include(p => p.ApplicationUser).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality)
                };
            }
            if (SearchString == "acceptance")
            {
                publicationTitleVM = new PublicationTitleViewModel
                {
                    PublicationTitles = _context.PublicationTitles.Where(a => a.IsAdminAccepted == false).Include(p => p.ApplicationUser).Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality)
                };
            }

            var count = publicationTitleVM.PublicationTitles.Count();
            publicationTitleVM.PublicationTitles = publicationTitleVM.PublicationTitles.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            publicationTitleVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            //var aRIDDbContext = _context.PublicationTitle.Include(p => p.ApplicationUser).Include(p => p.Country).Include(p => p.Publisher).Include(p => p.Speciality);
            return View(publicationTitleVM);

        }

        // GET: PublicationTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicationTitle = await _context.PublicationTitles
                .Include(p => p.ApplicationUser)
                .Include(p => p.Country)
                .Include(p => p.Publisher)
                .Include(p => p.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (publicationTitle == null)
            {
                return NotFound();
            }

            return View(publicationTitle);
        }

        // GET: PublicationTitles/Create
        public IActionResult Create()
        {
            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            return View();
        }

        // POST: PublicationTitles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArName,EnName,ArDescription,Logo,PISSN,EISSN,Email,EstablishmentDate,DateOfRecord,ApplicationUserId,SpecialityId,CountryId,EditorinChief,PublisherId,IsAdminAccepted,IsVisible,IsFeatured,IsIIndexed,IsScopusIndexed,IsImpactFactor,Website,Fb,Twitter,ContactUs,Languages,Tags,Status,ReportType,PublicationTypes")] PublicationTitle publicationTitle,IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                publicationTitle.DateOfRecord = DateTime.Now;
                publicationTitle.ApplicationUserId =_userManager.GetUserId(User);
                publicationTitle.Logo = await UserFile.UploadeNewImageAsync(publicationTitle.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                publicationTitle.ArDescription = publicationTitle.ArDescription.Replace("\n", "<br/>");


                _context.Add(publicationTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", publicationTitle.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publicationTitle.CountryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", publicationTitle.PublisherId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publicationTitle.SpecialityId);
            return View(publicationTitle);
        }

        // GET: PublicationTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicationTitle = await _context.PublicationTitles.SingleOrDefaultAsync(m => m.Id == id);
            if (publicationTitle == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a=>a.Id==publicationTitle.ApplicationUserId), "Id", "Id", publicationTitle.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publicationTitle.CountryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", publicationTitle.PublisherId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publicationTitle.SpecialityId);
            return View(publicationTitle);
        }

        // POST: PublicationTitles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArName,EnName,ArDescription,Logo,PISSN,EISSN,Email,EstablishmentDate,DateOfRecord,ApplicationUserId,SpecialityId,CountryId,EditorinChief,PublisherId,IsAdminAccepted,IsVisible,IsFeatured,IsIIndexed,IsScopusIndexed,IsImpactFactor,Website,Fb,Twitter,ContactUs,Languages,Tags,Status,ReportType,PublicationTypes")] PublicationTitle publicationTitle,IFormFile myfile)
        {
            if (id != publicationTitle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                publicationTitle.Logo = await UserFile.UploadeNewImageAsync(publicationTitle.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 50, 50);

                try
                {
                    _context.Update(publicationTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationTitleExists(publicationTitle.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", publicationTitle.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", publicationTitle.CountryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", publicationTitle.PublisherId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", publicationTitle.SpecialityId);
            return View(publicationTitle);
        }

        // GET: PublicationTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicationTitle = await _context.PublicationTitles
                .Include(p => p.ApplicationUser)
                .Include(p => p.Country)
                .Include(p => p.Publisher)
                .Include(p => p.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (publicationTitle == null)
            {
                return NotFound();
            }

            return View(publicationTitle);
        }

        // POST: PublicationTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publicationTitle = await _context.PublicationTitles.SingleOrDefaultAsync(m => m.Id == id);
            _context.PublicationTitles.Remove(publicationTitle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicationTitleExists(int id)
        {
            return _context.PublicationTitles.Any(e => e.Id == id);
        }
        [Authorize]
        public IActionResult Report(int id)
        {
            PublicationTitle publicationTitle = _context.PublicationTitles.Where(m => m.Id == id).SingleOrDefault();
            if (publicationTitle == null)
            {
                return NotFound();
            }
            publicationTitle.ReportType = ReportType.NotRelated;
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

    }
}
