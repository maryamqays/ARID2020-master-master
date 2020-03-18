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
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class FreelancerPortfoliosController : Controller
    {
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private readonly ARIDDbContext _context;

        public FreelancerPortfoliosController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
            _userManager = userMrg;
        }

        // GET: FreelancerPortfolios
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerPortfolios.Include(f => f.ApplicationUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: FreelancerPortfolios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerPortfolio = await _context.FreelancerPortfolios
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerPortfolio == null)
            {
                return NotFound();
            }

            return View(freelancerPortfolio);
        }

        // GET: FreelancerPortfolios/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == _userManager.GetUserId(User)), "Id", "Id");
            ViewBag.Skills = getSkillsandExpertises();
            return View();
        }
        public List<CommunityAutofillModel> getSkillsandExpertises()
        {
            List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
            var sa = new JsonSerializerSettings();
            var DbSkills = _context.Skills.Select(a => new { text = a.Name, value = a.Name });

            foreach (var data in DbSkills)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            return ls;
        }




        // POST: FreelancerPortfolios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Title,Skills,Description,ExternalLink,Image,File,Likes,Views,DateOfRecord,DateOfAchievement")] FreelancerPortfolio freelancerPortfolio, IFormFile myfile, IFormFile myfile2)
        {
            if (ModelState.IsValid)
            {
                freelancerPortfolio.Image = await UserFile.UploadeNewImageAsync(freelancerPortfolio.Image,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                freelancerPortfolio.File = await UserFile.UploadeNewFileAsync(freelancerPortfolio.File,
          myfile2, _environment.WebRootPath, Properties.Resources.Files);


                freelancerPortfolio.DateOfRecord = DateTime.Now;

                _context.Add(freelancerPortfolio);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("FreelancerDetails", "ApplicationUsers", new {/* routeValues, for example: */ id = freelancerPortfolio.ApplicationUserId });

            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerPortfolio.ApplicationUserId);
            return View(freelancerPortfolio);
        }

        // GET: FreelancerPortfolios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerPortfolio = await _context.FreelancerPortfolios.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerPortfolio == null)
            {
                return NotFound();
            }
            ViewBag.Skills = getSkillsandExpertises();
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerPortfolio.ApplicationUserId);
            return View(freelancerPortfolio);
        }

        // POST: FreelancerPortfolios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Title,Skills,Description,ExternalLink,Image,File,Likes,Views,DateOfRecord,DateOfAchievement")] FreelancerPortfolio freelancerPortfolio,IFormFile myfile,IFormFile myfile2)
        {
            if (id != freelancerPortfolio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    freelancerPortfolio.Image = await UserFile.UploadeNewImageAsync(freelancerPortfolio.Image,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                    freelancerPortfolio.File = await UserFile.UploadeNewFileAsync(freelancerPortfolio.File,
              myfile2, _environment.WebRootPath, Properties.Resources.Files);



                    _context.Update(freelancerPortfolio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerPortfolioExists(freelancerPortfolio.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerPortfolio.ApplicationUserId);
            return View(freelancerPortfolio);
        }

        // GET: FreelancerPortfolios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerPortfolio = await _context.FreelancerPortfolios
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerPortfolio == null)
            {
                return NotFound();
            }

            return View(freelancerPortfolio);
        }

        // POST: FreelancerPortfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerPortfolio = await _context.FreelancerPortfolios.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerPortfolios.Remove(freelancerPortfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult LikesUp(int id)
        {

            FreelancerPortfolio freelancerPortfolio = _context.FreelancerPortfolios.Where(m => m.Id == id).SingleOrDefault();

            if (freelancerPortfolio == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid != freelancerPortfolio.ApplicationUserId)
            {
                freelancerPortfolio.Likes += 1;
                _context.Update(freelancerPortfolio);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult LikesDown(int id)
        {

            FreelancerPortfolio freelancerPortfolio = _context.FreelancerPortfolios.Where(m => m.Id == id).SingleOrDefault();

            if (freelancerPortfolio == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid != freelancerPortfolio.ApplicationUserId)
            {
                freelancerPortfolio.Likes -= 1;
                _context.Update(freelancerPortfolio);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        private bool FreelancerPortfolioExists(int id)
        {
            return _context.FreelancerPortfolios.Any(e => e.Id == id);
        }
    }
}
