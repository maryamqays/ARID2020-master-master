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
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
   // [Authorize(Roles = RoleName.Admins)]
    public class MOOCProvidersController : Controller
    {           
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;

        public MOOCProvidersController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;

        }

        // GET: MOOCProviders
        public async Task<IActionResult> Index(string SearchString, int productPage = 1)
        {
            MOOCProviderViewModel MOOCProviderVM = new MOOCProviderViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                MOOCProviderVM = new MOOCProviderViewModel()
                {
                    MOOCProviders = _context.MOOCProviders.Include(p => p.ApplicationUser),
                    MOOCLists = _context.MOOCLists
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                MOOCProviderVM = new MOOCProviderViewModel()
                {
                    MOOCProviders = _context.MOOCProviders.Where(a => a.Name.Contains(SearchString) || a.Description.Contains(SearchString) || a.Email.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Website.Contains(SearchString) || a.Fb.Contains(SearchString) || a.Twitter.Contains(SearchString)).Include(p => p.ApplicationUser),
                    MOOCLists = _context.MOOCLists
                };
            }

            var count = MOOCProviderVM.MOOCProviders.Count();
            MOOCProviderVM.MOOCProviders = MOOCProviderVM.MOOCProviders.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            MOOCProviderVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            //var aRIDDbContext = _context.MOOCProvider.Include(m => m.ApplicationUser);
            return View(MOOCProviderVM);
        }

        // GET: MOOCProviders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var mOOCProvider = await _context.MOOCProvider
            //    .Include(m => m.ApplicationUser)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (mOOCProvider == null)
            //{
            //    return NotFound();
            //}
            //return View(mOOCProvider);

            MOOCProviderViewModel MOOCProviderVM = new MOOCProviderViewModel()
            {
                MOOCProviderFollowers = _context.MOOCProviderFollowers
              .Include(f => f.ApplicationUser)
               .Include(f => f.MOOCProvider)
              .Where(f => f.ApplicationUserId == _userManager.GetUserId(User))
              .ToList(),

                ApplicationUser = await _userManager.Users
              .Include(c => c.Country)
              .Include(c => c.City)
              .Include(c => c.University)
              .Include(c => c.Faculty)
              .Include(a => a.ReferredBy)
              .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),

                MOOCProvider = await _context.MOOCProviders
                               .SingleOrDefaultAsync(m => m.Id == id),

            };
            return View(MOOCProviderVM);
        }

        // GET: MOOCProviders/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: MOOCProviders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Description,Website,Fb,Twitter,ApplicationUserId,DateOfRecord,Logo")] MOOCProvider mOOCProvider, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                mOOCProvider.ApplicationUserId = _userManager.GetUserId(User);
                mOOCProvider.DateOfRecord = DateTime.Now;
                mOOCProvider.Logo = await UserFile.UploadeNewImageAsync(mOOCProvider.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                mOOCProvider.Description = mOOCProvider.Description.Replace("\n", "<br/>");


                _context.Add(mOOCProvider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProvider.ApplicationUserId);
            return View(mOOCProvider);
        }

        // GET: MOOCProviders/Edit/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCProvider = await _context.MOOCProviders.Include(a=>a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCProvider == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProvider.ApplicationUserId);
            return View(mOOCProvider);
        }

        // POST: MOOCProviders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Description,Website,Fb,Twitter,ApplicationUserId,DateOfRecord,Logo")] MOOCProvider mOOCProvider, IFormFile myfile)
        {
            if (id != mOOCProvider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mOOCProvider.Logo = await UserFile.UploadeNewImageAsync(mOOCProvider.Logo,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);


                    _context.Update(mOOCProvider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MOOCProviderExists(mOOCProvider.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", mOOCProvider.ApplicationUserId);
            return View(mOOCProvider);
        }

        // GET: MOOCProviders/Delete/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mOOCProvider = await _context.MOOCProviders
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mOOCProvider == null)
            {
                return NotFound();
            }

            return View(mOOCProvider);
        }

        // POST: MOOCProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mOOCProvider = await _context.MOOCProviders.SingleOrDefaultAsync(m => m.Id == id);
            _context.MOOCProviders.Remove(mOOCProvider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MOOCProviderExists(int id)
        {
            return _context.MOOCProviders.Any(e => e.Id == id);
        }
        [Authorize]
        public IActionResult Follow(int id)
        {
            MOOCProvider MOOCProvider = _context.MOOCProviders.Where(m => m.Id == id).SingleOrDefault();
            if (MOOCProvider == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);

            if (_context.MOOCProviderFollowers.Where(f => f.MOOCProviderId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                _context.MOOCProviderFollowers.Add(new MOOCProviderFollower
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = currentuserid,
                    MOOCProviderId = id,
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult Unfollow(int id)
        {
            MOOCProvider MOOCProvider = _context.MOOCProviders.Where(m => m.Id == id).SingleOrDefault();
            if (MOOCProvider == null)
            {
                return NotFound();
            }

            if (_context.MOOCProviderFollowers.Where(f => f.MOOCProviderId == id && f.ApplicationUserId == _userManager.GetUserId(User)).Count() > 0)
            {
                var Followed = _context.MOOCProviderFollowers.SingleOrDefault(f => f.MOOCProviderId == id && f.ApplicationUserId == _userManager.GetUserId(User));
                _context.MOOCProviderFollowers.Remove(Followed);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

    }
}
