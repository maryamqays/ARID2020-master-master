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
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace ARID.Controllers
{
    [Authorize]
    //  [Authorize(Roles = RoleName.Members)]
    // [Authorize(Roles = RoleName.Admins)]
    public class UserExpertisesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserExpertisesController(UserManager<ApplicationUser> userManager, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserExpertises
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.UserExpertises
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .Include(u => u.ApplicationUser)
                .Include(u => u.Expertise);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: UserExpertises/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            var expertiseList = _context.Expertises
                .Where(e => string.IsNullOrEmpty(e.UserId));
            ViewData["ExpertiseId"] = new SelectList(expertiseList, "Id", "Name");
            ViewBag.Skills = getSkillsandExpertises();

            return View();
        }

        public List<CommunityAutofillModel> getSkillsandExpertises()

        {
            List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
            var sa = new JsonSerializerSettings();
            //var DbSkills = _context.Skills.Select(a => new { text = a.Name, value = a.Name });
            var DbExpertises = _context.Expertises.Select(a => new { text = a.Name, value = a.Name });

            //foreach (var data in DbSkills)
            //{
            //    CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
            //    communityAutofillModel.Text = data.text;
            //    communityAutofillModel.Value = data.value;
            //    ls.Add(communityAutofillModel);
            //}
            foreach (var data in DbExpertises)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            return ls;
        }

        // POST: UserExpertises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExpertiseId,ApplicationUserId")] UserExpertise userExpertise)
        {
            if (ModelState.IsValid)
            {
               
                _context.Add(userExpertise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            var expertiseList = _context.Expertises
                .Where(e => string.IsNullOrEmpty(e.UserId));
            ViewData["ExpertiseId"] = new SelectList(expertiseList, "Id", "Name", userExpertise.ExpertiseId);
            return View(userExpertise);
        }

        // GET: UserExpertises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userExpertise = await _context.UserExpertises.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (userExpertise == null)
            {
                return NotFound();
            }
            var expertiseList = _context.Expertises
                .Where(e => string.IsNullOrEmpty(e.UserId));
            ViewData["ExpertiseId"] = new SelectList(expertiseList, "Id", "Name", userExpertise.ExpertiseId);
            return View(userExpertise);
        }

        // POST: UserExpertises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExpertiseId,ApplicationUserId")] UserExpertise userExpertise)
        {
            if (id != userExpertise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userExpertise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExpertiseExists(userExpertise.Id))
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
            var expertiseList = _context.Expertises
                .Where(e => string.IsNullOrEmpty(e.UserId));
            ViewData["ExpertiseId"] = new SelectList(expertiseList, "Id", "Name", userExpertise.ExpertiseId);
            return View(userExpertise);
        }

        // GET: UserExpertises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userExpertise = await _context.UserExpertises
                .Include(u => u.Expertise)
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (userExpertise == null)
            {
                return NotFound();
            }

            return View(userExpertise);
        }

        // POST: UserExpertises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userExpertise = await _context.UserExpertises.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.UserExpertises.Remove(userExpertise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExpertiseExists(int id)
        {
            return _context.UserExpertises.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
