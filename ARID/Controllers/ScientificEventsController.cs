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
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class ScientificEventsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;
        private int PagSize = 10;
        public ScientificEventsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userMrg;
            _environment = environment;
        }

        // GET: ScientificEvents
        public async Task<IActionResult> Index(string SearchString, string viewtype, int productPage = 1)
        {

            EventViewModel ScientificEventViewModel = new EventViewModel();

            if (string.IsNullOrEmpty(SearchString))
            {
                ScientificEventViewModel = new EventViewModel
                {
                    ScientificEvents = _context.ScientificEvents.Include(c => c.Speciality).Include(c => c.Country).Include(c => c.ApplicationUser),
                    UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User)),
                };
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                ScientificEventViewModel = new EventViewModel
                {
                    ScientificEvents = _context.ScientificEvents.Where(a => a.EventNameAr.Contains(SearchString) || a.EventNameEn.Contains(SearchString) || a.Image.Contains(SearchString) || a.Description.Contains(SearchString) || a.ContactDetails.Contains(SearchString) || a.Venue.Contains(SearchString) || a.Country.ArCountryName.Contains(SearchString) || a.Speciality.Name.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Organizers.Contains(SearchString) || a.Website.Contains(SearchString)).Include(c => c.Speciality).Include(c => c.Country).Include(c => c.ApplicationUser),
                    UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User)),
                };
            }
            if (SearchString == "reported")
            {
                ScientificEventViewModel = new EventViewModel
                {
                    ScientificEvents = _context.ScientificEvents.Where(a => a.ReportType.ToString() != "None").Include(c => c.Speciality).Include(c => c.Country).Include(c => c.ApplicationUser),
                    UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User)),
                };
            }
            if (SearchString == "featured")
            {
                ScientificEventViewModel = new EventViewModel
                {
                    ScientificEvents = _context.ScientificEvents.Where(a => a.IsFeatured == true).Include(c => c.Speciality).Include(c => c.Country).Include(c => c.ApplicationUser),
                    UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User)),
                };
            }
            var count = ScientificEventViewModel.ScientificEvents.Count();
            ScientificEventViewModel.ScientificEvents = ScientificEventViewModel.ScientificEvents.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();

            ScientificEventViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            //if (SearchString== "featured")
            //{
            //    ScientificEventViewModel = new EventViewModel
            //    {
            //        ScientificEvents = _context.ScientificEvent.Where(a => a.== true).Include(c => c.Speciality).Include(c => c.Country).Include(c => c.ApplicationUser),
            //        UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User)),
            //    };
            //}

            //var aRIDDbContext = _context.ScientificEvent.Include(s => s.ApplicationUser).Include(s => s.Country).Include(s => s.Speciality);
            //if (!string.IsNullOrEmpty(SearchString))
            //{
            //    aRIDDbContext = aRIDDbContext.Where(a => a.EventNameAr.Contains(SearchString) || a.EventNameEn.Contains(SearchString) || a.EventType.ToString().Contains(SearchString) || a.Image.Contains(SearchString) || a.Description.Contains(SearchString) || a.ContactDetails.Contains(SearchString) || a.Venue.Contains(SearchString) || a.Country.ArCountryName.Contains(SearchString) || a.Speciality.ArSpecialityName.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Organizers.Contains(SearchString) || a.Website.Contains(SearchString)).Include(s => s.ApplicationUser).Include(s => s.Country).Include(s => s.Speciality);
            //}
            //else if (!string.IsNullOrEmpty(Eventtype))
            //{
            //    aRIDDbContext = aRIDDbContext.Where(a => a.EventType.ToString().Contains(Eventtype)).Include(s => s.ApplicationUser).Include(s => s.Country).Include(s => s.Speciality);
            //}
            return View(ScientificEventViewModel);
        }

        // GET: ScientificEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scientificEvent = await _context.ScientificEvents
                .Include(s => s.ApplicationUser)
                .Include(s => s.Country)
                .Include(s => s.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scientificEvent == null)
            {
                return NotFound();
            }

            return View(scientificEvent);
        }

        // GET: ScientificEvents/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            return View();
        }

        // POST: ScientificEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventNameAr,EventNameEn,EventType,CountryId,Venue,Date,SpecialityId,Organizers,ApplicationUserId,ContactDetails,IsFeatured,IsVisible,IsAccepted,Website,IsDecisionMaker,Description,EventDate,AridPrivileges,Image,ReportType")] ScientificEvent scientificEvent, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                scientificEvent.ApplicationUserId = _userManager.GetUserId(User);
                scientificEvent.Date = DateTime.Now;
                scientificEvent.ReportType = ReportType.None;
                scientificEvent.IsAccepted = true;
                scientificEvent.IsVisible = true;

                scientificEvent.Image = await UserFile.UploadeNewImageAsync(scientificEvent.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);

                scientificEvent.Description = (System.Text.RegularExpressions.Regex.Replace(scientificEvent.Description, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                _context.Add(scientificEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "ArName", scientificEvent.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", scientificEvent.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", scientificEvent.SpecialityId);
            return View(scientificEvent);
        }

        // GET: ScientificEvents/Edit/5
        public async Task<IActionResult> Edit(int? id, string viewtype)
        {
            if (id == null)
            {
                return NotFound();
            }
            var scientificEvent = await _context.ScientificEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (scientificEvent == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == scientificEvent.ApplicationUserId), "Id", "ArName", scientificEvent.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", scientificEvent.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", scientificEvent.SpecialityId);

            //if (User.IsInRole("Admins") || scientificEvent.ApplicationUserId == _userManager.GetUserId(User))
            if (viewtype == "details")
            {
                return View("EventDetails", scientificEvent);

            }
            else
                return View(scientificEvent);


        }

        // POST: ScientificEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventNameAr,EventNameEn,EventType,CountryId,Venue,Date,SpecialityId,Organizers,ApplicationUserId,ContactDetails,IsFeatured,IsVisible,IsAccepted,Website,IsDecisionMaker,Description,EventDate,AridPrivileges,Image,ReportType")] ScientificEvent scientificEvent, IFormFile myfile)
        {
            if (id != scientificEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    scientificEvent.Image = await UserFile.UploadeNewImageAsync(scientificEvent.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                    scientificEvent.Description = (System.Text.RegularExpressions.Regex.Replace(scientificEvent.Description, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                    _context.Update(scientificEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScientificEventExists(scientificEvent.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "ArName", scientificEvent.ApplicationUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", scientificEvent.CountryId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", scientificEvent.SpecialityId);

            return View(scientificEvent);
        }

        // POST: ScientificEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // GET: ScientificEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scientificEvent = await _context.ScientificEvents
                .Include(s => s.ApplicationUser)
                .Include(s => s.Country)
                .Include(s => s.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scientificEvent == null)
            {
                return NotFound();
            }

            return View(scientificEvent);
        }

        // POST: ScientificEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scientificEvent = await _context.ScientificEvents.SingleOrDefaultAsync(m => m.Id == id);
            _context.ScientificEvents.Remove(scientificEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScientificEventExists(int id)
        {
            return _context.ScientificEvents.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Report(int id)
        {
            ScientificEvent ScientificEvent = _context.ScientificEvents.Where(m => m.Id == id).SingleOrDefault();
            if (ScientificEvent == null)
            {
                return NotFound();
            }
                     ScientificEvent.ReportType = ReportType.NotRelated;
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }
    }
}
