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
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Hangfire;

using Newtonsoft.Json;
namespace ARID.Controllers
{
    //[Route("Community/[action]")]
    public class CommunitiesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;

        public CommunitiesController(ARIDDbContext context, IHostingEnvironment environment, UserManager<ApplicationUser> userMrg)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        public JsonResult Skills_Read(string text)
        {
            var result = GetSkills();
            var result1 = GetExpertises();
            var result2 = GetSpeciality();

            if (!string.IsNullOrEmpty(text))
            {
                result = result.Where(p => p.Name.Contains(text)).ToList();
                result1 = result1.Where(p => p.Name.Contains(text)).ToList();
                result2 = result2.Where(p => p.Name.Contains(text) || p.EnSpecialityName.Contains(text)).ToList();
            }

            // var empIds = result.Select(x => x.Name).Concat(result1.Select(x => x.Name));

            IEnumerable<object> c = result.Cast<object>()
                          .Concat(result1.Cast<object>()).Concat(result2.Cast<object>());


            //var sb = new StringBuilder();
            //sb.Append(result);
            //sb.Append(result1);

            // return Json(sb.ToString());
            // return Json(new { result, result1 });
            return Json(c);
        }

        //public JsonResult getSkillsandExpertises(string term)
        //{
        //    List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
        //    var sa = new JsonSerializerSettings();
        //    var DbSkills = _context.Skills.Where(c => c.Name.Contains(term)).Select(a => new { label = a.Name });
        //    var DbExpertises = _context.Expertises.Where(c => c.Name.Contains(term)).Select(a => new { label = a.Name });

        //    foreach (var data in DbSkills)
        //    {
        //        CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
        //        communityAutofillModel.label = data.label;
        //        ls.Add(communityAutofillModel);
        //    }
        //    foreach (var data in DbExpertises)
        //    {
        //        CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
        //        communityAutofillModel.label = data.label;
        //        ls.Add(communityAutofillModel);
        //    }
        //    return Json(ls, sa);
        //}
        public List<CommunityAutofillModel> getSkillsandExpertises()

        {
            List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
            var sa = new JsonSerializerSettings();
            var DbSkills = _context.Skills.Select(a => new { text = a.Name, value = a.Name });
            var DbExpertises = _context.Expertises.Select(a => new { text = a.Name, value = a.Name });

            foreach (var data in DbSkills)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            foreach (var data in DbExpertises)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            return ls;
        }


        private IEnumerable<Skill> GetSkills()
        {
            var result = _context.Skills;
            return result;
        }

        private IEnumerable<Expertise> GetExpertises()
        {
            var result = _context.Expertises;
                        return result;
        }

        private IEnumerable<Speciality> GetSpeciality()
        {
            var result = _context.Specialities;
            return result;
        }


        public async Task<IActionResult> Index()
        {
            var communityViewModel = new CommunityViewModel
            {
                Communities = _context.Communities.Include(c => c.Speciality).Where(a => a.CommunityType == CommunityType.Community),
                CommunityFollower = _context.CommunityFollowers.ToList(),
                Posts = _context.Posts.ToList(),
                UserBadge = _context.UserBadges.FirstOrDefault(a => a.UserId == _userManager.GetUserId(User))
            };

            var LastScore = _context.ScoreLogs.LastOrDefault(a => a.ApplicationUserId == _userManager.GetUserId(User) & a.ScoreValueId == 1);
            string todaydate = DateTime.Now.ToString("dd/MM/yyyy");

            if (LastScore != null) { 

            DateTime dtSuppliedDate = DateTime.Parse(LastScore.Date.ToString("dd/MM/yyyy"));
            
            if (dtSuppliedDate.Subtract(DateTime.Today).Days != 0)
            {
                _context.ScoreLogs.Add(new ScoreLog
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userManager.GetUserId(User),
                    PostId = Guid.Parse("fb598acf-f2a5-40ef-a2bd-51d248843263"),
                    Date = DateTime.Now,
                    ScoreValueId = 1
                });
                _context.SaveChanges();
            }
        }

        ViewData["CommunityScore"] = _context.ScoreLogs.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).Sum(a => a.ScoreValue.Value);
            //var aRIDDbContext = _context.Community.Include(c => c.ApplicationUser).Include(c => c.Speciality);
            //return View(await aRIDDbContext.ToListAsync());
            return View(communityViewModel);
        }

       


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Count"] = _context.CommunityFollowers.Count(a => a.CommunityId == id);
            ViewData["CommunityId"] = id;
            //var commentMetric = await _context.Community
            //                   .SingleOrDefaultAsync(m => m.Id == id);
            //if (commentMetric == null)
            //{
            //    return NotFound();
            //}

            var communityViewModel = new CommunityViewModel
            {
                ApplicationUser = await _userManager.Users
              .Include(c => c.Country)
              .Include(c => c.City)
              .Include(c => c.University)
              .Include(c => c.Faculty)
              .Include(a => a.ReferredBy)
              .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),

                CommunityFollower = _context.CommunityFollowers
              .Include(f => f.ApplicationUser)
               .Include(f => f.Community)
              .Where(f => f.ApplicationUserId == _userManager.GetUserId(User))
              .ToList(),

                Community = await _context.Communities.Include(c=>c.ApplicationUser)
                               .SingleOrDefaultAsync(m => m.Id == id),

                Posts = _context.Posts.Where(a => a.CommunityId == id).Include(p => p.ApplicationUser).Include(p => p.Community),
                PostComments = _context.PostComments.Where(a => a.Post.CommunityId == id),
                PostMetrics = _context.PostMetrics.Where(a => a.Post.CommunityId == id)
            };

            return View(communityViewModel);
        }

    
        // GET: Communities/Create
        public IActionResult Create()
        {

            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            ViewBag.Skills = getSkillsandExpertises();
            return View();
        }

        // POST: Communities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Name,ShortName,BgImage,Logo,Description,CreationDate,CommunityType,SpecialityId,SecurityLevel,IsCommentsAllowed,IsFeatured,IsApproved,IsSuspended,Tags")] Community community, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                community.ApplicationUserId = _userManager.GetUserId(User);
                community.BgImage = await UserFile.UploadeNewFileAsync(community.BgImage,
myfile, _environment.WebRootPath, Properties.Resources.Community);

                community.Logo = await UserFile.UploadeNewImageAsync(community.Logo,
myfile1, _environment.WebRootPath, Properties.Resources.Community, 50, 50);

                community.CreationDate = DateTime.Now;
                community.IsFeatured = false;
                community.IsSuspended = false;
                community.IsApproved = false;
                community.IsCommentsAllowed = true;
                community.SecurityLevel = SecurityLevel.Open;
                community.CommunityType = CommunityType.Community;
                _context.Add(community);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", community.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", community.SpecialityId);
            return View(community);
        }

        // GET: Communities/Edit/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities.SingleOrDefaultAsync(m => m.Id == id);
            if (community == null)
            {
                return NotFound();
            }

            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", community.SpecialityId);
            return View(community);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Name,ShortName,BgImage,Logo,Description,CreationDate,CommunityType,SpecialityId,SecurityLevel,IsCommentsAllowed,IsFeatured,IsApproved,IsSuspended,Tags")] Community community, IFormFile myfile, IFormFile myfile1)
        {
            if (id != community.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    community.BgImage = await UserFile.UploadeNewFileAsync(community.BgImage,
myfile, _environment.WebRootPath, Properties.Resources.Community);

                    community.Logo = await UserFile.UploadeNewImageAsync(community.Logo,
    myfile1, _environment.WebRootPath, Properties.Resources.Community, 50, 50);

                    _context.Update(community);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommunityExists(community.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", community.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", community.SpecialityId);
            return View(community);
        }

        // GET: Communities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities
                .Include(c => c.ApplicationUser)
                .Include(c => c.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (community == null)
            {
                return NotFound();
            }

            return View(community);
        }

        // POST: Communities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var community = await _context.Communities.SingleOrDefaultAsync(m => m.Id == id);
            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommunityExists(int id)
        {
            return _context.Communities.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Follow(int id)
        {
            Community community = _context.Communities.Where(m => m.Id == id).SingleOrDefault();
            if (community == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);

            if (_context.CommunityFollowers.Where(f => f.CommunityId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                _context.CommunityFollowers.Add(new CommunityFollower
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = currentuserid,
                    CommunityId = id,
                    NotifyMe = true,
                    IsAccepted = true,
                    IsAdmin = false
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult Unfollow(int id)
        {
            Community community = _context.Communities.Where(m => m.Id == id).SingleOrDefault();
            if (community == null)
            {
                return NotFound();
            }

            if (_context.CommunityFollowers.Where(f => f.CommunityId == id && f.ApplicationUserId == _userManager.GetUserId(User)).Count() > 0)
            {
                var Followed = _context.CommunityFollowers.SingleOrDefault(f => f.CommunityId == id && f.ApplicationUserId == _userManager.GetUserId(User));
                _context.CommunityFollowers.Remove(Followed);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }
    }
}
