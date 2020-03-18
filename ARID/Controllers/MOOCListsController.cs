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
    public class MOOCListsController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;


        public MOOCListsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: MOOCLists
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            MOOCListViewModel MOOCListVM = new MOOCListViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                MOOCListVM = new MOOCListViewModel()
                {
                    MOOCLists = _context.MOOCLists.Include(p => p.ApplicationUser).Include(p => p.MOOCProvider).Include(p => p.Speciality).Include(p => p.University)
                };
            }
            else if (SearchString == "reported")
            {
                MOOCListVM = new MOOCListViewModel()
                {
                    MOOCLists = _context.MOOCLists.Where(a => a.IsReported == true).Include(p => p.ApplicationUser).Include(p => p.MOOCProvider).Include(p => p.Speciality).Include(p => p.University)
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                MOOCListVM = new MOOCListViewModel()
                {
                    MOOCLists = _context.MOOCLists.Where(a => a.Title.Contains(SearchString) || a.MOOCProvider.Name.Contains(SearchString) || a.Description.Contains(SearchString) || a.Image.Contains(SearchString) || a.Duration.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.University.ArUniversityName.Contains(SearchString) || a.Link.Contains(SearchString) || a.Tags.Contains(SearchString) || a.Instructor.Contains(SearchString) || a.YouTubeLink.Contains(SearchString)).Include(p => p.ApplicationUser).Include(p => p.MOOCProvider).Include(p => p.Speciality).Include(p => p.University)
                };
            }

            var count = MOOCListVM.MOOCLists.Count();
            MOOCListVM.MOOCLists = MOOCListVM.MOOCLists.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            MOOCListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.MOOCList.Include(m => m.ApplicationUser).Include(m => m.MOOCProvider).Include(m => m.Speciality).Include(m => m.University);
            return View(MOOCListVM);
        }

        // GET: MOOCLists/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCList = await _context.MOOCLists
                .Include(c => c.ApplicationUser)
                .Include(m => m.MOOCProvider)
                .Include(m => m.Speciality)
                .Include(m => m.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCList == null)
            {
                return NotFound();
            }

            return View(mOOCList);
        }

        // GET: MOOCLists/Create
        public async Task<IActionResult> Create(Guid? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            MOOCListViewModel MOOCListVM = new MOOCListViewModel()
            {
                // CountryId = user.CountryId
            };
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Name");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            return View(MOOCListVM);


            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            //ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProvider, "Id", "Email");
            //ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName");
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            //return View();
        }

        // POST: MOOCLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,MOOCProviderId,Description,SpecialityId,IsFree,IsReported,Language,Certificate,Duration,UniversityId,Link,YouTubeLink,Tags,Instructor,Image,StartDate,ApplicationUserId,DateOfRecord")] MOOCList mOOCList, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                mOOCList.ApplicationUserId = _userManager.GetUserId(User);
                mOOCList.DateOfRecord = DateTime.Now;
                mOOCList.Image = await UserFile.UploadeNewImageAsync(mOOCList.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                mOOCList.Description = mOOCList.Description.Replace("\n", "<br/>");



                mOOCList.Id = Guid.NewGuid();
                _context.Add(mOOCList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCList.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Name", mOOCList.MOOCProviderId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", mOOCList.SpecialityId);
            //   ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", mOOCList.UniversityId);
            return View(mOOCList);
        }

        // GET: MOOCLists/Edit/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var user = _context.MOOCLists.Include(c=>c.ApplicationUser).SingleOrDefault(a => a.Id == id).ApplicationUser;
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var mOOCList = await _context.MOOCList.SingleOrDefaultAsync(m => m.Id == id);
            //if (mOOCList == null)
            //{
            //    return NotFound();
            //}
            MOOCListViewModel MOOCListVM = new MOOCListViewModel()
            {
                MOOCList = await _context.MOOCLists.SingleOrDefaultAsync(m => m.Id == id),
                CountryId = user.CountryId,
                ApplicationUser = await _userManager.Users
              .Include(c => c.Country)
              .Include(c => c.City)
              .Include(c => c.University)
              .Include(c => c.Faculty)
              .Include(a => a.ReferredBy)
              .SingleOrDefaultAsync(u => u.Id == user.Id),


            };
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", MOOCListVM.CountryId);
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", MOOCListVM.MOOCList.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email", MOOCListVM.MOOCList.MOOCProviderId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", MOOCListVM.MOOCList.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", MOOCListVM.MOOCList.UniversityId);
            return View(MOOCListVM);
        }

        // POST: MOOCLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,MOOCProviderId,Description,SpecialityId,IsFree,IsReported,Language,Certificate,Duration,UniversityId,Link,YouTubeLink,Tags,Instructor,Image,StartDate,ApplicationUserId,DateOfRecord")] MOOCList mOOCList, IFormFile myfile)
        {
            if (id != mOOCList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mOOCList.Image = await UserFile.UploadeNewImageAsync(mOOCList.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);


                    _context.Update(mOOCList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MOOCListExists(mOOCList.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCList.ApplicationUserId);
            ViewData["MOOCProviderId"] = new SelectList(_context.MOOCProviders, "Id", "Email", mOOCList.MOOCProviderId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", mOOCList.SpecialityId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", mOOCList.UniversityId);
            return View(mOOCList);
        }

        // GET: MOOCLists/Delete/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCList = await _context.MOOCLists
                .Include(m => m.ApplicationUser)
                .Include(m => m.MOOCProvider)
                .Include(m => m.Speciality)
                .Include(m => m.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCList == null)
            {
                return NotFound();
            }

            return View(mOOCList);
        }

        // POST: MOOCLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var mOOCList = await _context.MOOCLists.SingleOrDefaultAsync(m => m.Id == id);
            _context.MOOCLists.Remove(mOOCList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MOOCListExists(Guid id)
        {
            return _context.MOOCLists.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Report(Guid id)
        {
            MOOCList MOOCList = _context.MOOCLists.Where(m => m.Id == id).SingleOrDefault();
            if (MOOCList == null)
            {
                return NotFound();
            }
            MOOCList.IsReported = true;
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

    }
}
