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
using System.Text;
using ARID.Services;

namespace ARID.Controllers
{
    //[Authorize(Roles = RoleName.Admins)]
    //[Authorize(Roles = RoleName.Admins)]
    [Authorize]
    public class UserBadgesController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UserBadgesController(UserManager<ApplicationUser> userManager, ARIDDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: UserBadges
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.UserBadges.Where(a => a.IsGranted == true).OrderByDescending(a => a.Id).Take(50).Include(u => u.Badge).Include(u => u.User);
            return View(await aRIDDbContext.ToListAsync());
        }

        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Unapproved()
        {
            var aRIDDbContext = _context.UserBadges.Where(a => a.IsGranted == false && a.IsRejected == false).OrderByDescending(a => a.Id).Include(u => u.Badge).Include(u => u.User);
            return View(await aRIDDbContext.ToListAsync());
        }


        public async Task<IActionResult> Personal()
        {
            var aRIDDbContext = _context.UserBadges.Where(a => a.IsGranted == false && a.UserId == _userManager.GetUserId(User)).OrderByDescending(a => a.Id).Include(u => u.Badge).Include(u => u.User);
            return View(await aRIDDbContext.ToListAsync());
        }

        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> UnGranted()
        {
            var aRIDDbContext = _context.UserBadges.Where(a => a.IsGranted == false).OrderByDescending(a => a.Id).Include(u => u.Badge).Include(u => u.User);
            return View(await aRIDDbContext.ToListAsync());
        }

        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> UsersByBadge(int id)
        {
            ViewData["BadgeName"] = _context.Badges.SingleOrDefault(a => a.Id == id).ArBadgeName;
            var aRIDDbContext = _context.UserBadges.Where(a => a.BadgeId == id).OrderByDescending(a => a.Id).Include(u => u.Badge).Include(u => u.User);
            return View(await aRIDDbContext.ToListAsync());
        }



        // GET: UserBadges/Create
        [Authorize(Roles = RoleName.Admins)]
        public IActionResult Create(string id)
        {
            ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName");
            ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == id), "Id", "ArName");
            return View();
        }

        // POST: UserBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.Admins)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BadgeId,UserId,DateofRequest,DateofGrant,IsRejected,RejectCount,RejectReason,IsGranted")] UserBadge userBadge)
        {
            //if (ModelState.IsValid)
            //{
            var Badge = _context.Badges
.SingleOrDefault(m => m.Id == userBadge.BadgeId);

            var UserDetails = _context.Users
                    .SingleOrDefault(m => m.Id == userBadge.UserId);

            userBadge.DateofGrant = DateTime.Now;
            userBadge.DateofGrant = DateTime.Now;
            userBadge.DateofRequest = DateTime.Now;
            userBadge.IsGranted = true;
            userBadge.RejectCount = 0;
            userBadge.IsRejected = false;
            //userBadge.UserId = id;
            _context.Add(userBadge);

            EmailContent content = _context.EmailContents.Where(m => m.Id == Badge.EmailContentId).SingleOrDefault();
            await _emailSender.SendEmailAsync(UserDetails.Email, content.ArSubject, content.ArContent);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName", userBadge.BadgeId);
            ////ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == userBadge.UserId), "Id", "ArName");
            //return View(userBadge);
        }
        // GET: UserBadges/Create

        public IActionResult InnovativeBadge(int id)
        {
            ViewData["BadgeId"] = new SelectList(_context.Badges.Where(a => a.Id == id), "Id", "ArBadgeName");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName");
            ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            return View();
        }

        // POST: UserBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InnovativeBadge([Bind("RejectReason")] UserBadge userBadge)
        //  public async Task<IActionResult> InnovativeBadge(int id, [Bind("Id,BadgeId,UserId,DateofRequest,DateofGrant,IsRejected,RejectCount,RejectReason,IsGranted")] UserBadge userBadge)
        {
            //if (ModelState.IsValid)
            //{
            //            var Badge = _context.Badges
            //.SingleOrDefault(m => m.Id == userBadge.BadgeId);

            //var UserDetails = _context.Users
            //        .SingleOrDefault(m => m.Id == userBadge.UserId);
            userBadge.UserId = _userManager.GetUserId(User);
            userBadge.BadgeId = 3;
            userBadge.DateofGrant = DateTime.Now;
            userBadge.DateofRequest = DateTime.Now;
            userBadge.IsGranted = false;
            userBadge.RejectCount = 0;
            userBadge.IsRejected = false;
            //userBadge.UserId = id;
            _context.Add(userBadge);

            //  EmailContent content = _context.EmailContents.Where(m => m.Id == Badge.EmailContentId).SingleOrDefault();
            //  await _emailSender.SendEmailAsync(UserDetails.Email, content.ArSubject, content.ArContent);

            await _context.SaveChangesAsync();
            return RedirectToAction("Personal");

            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName", userBadge.BadgeId);
            ////ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == userBadge.UserId), "Id", "ArName");
            //return View(userBadge);
        }


        public IActionResult BackerBadge(int id)
        {
            ViewData["BadgeId"] = new SelectList(_context.Badges.Where(a => a.Id == id), "Id", "ArBadgeName");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName");
            ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            return View();
        }

        // POST: UserBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BackerBadge([Bind("RejectReason")] UserBadge userBadge)
        //  public async Task<IActionResult> InnovativeBadge(int id, [Bind("Id,BadgeId,UserId,DateofRequest,DateofGrant,IsRejected,RejectCount,RejectReason,IsGranted")] UserBadge userBadge)
        {
            //if (ModelState.IsValid)
            //{
            //            var Badge = _context.Badges
            //.SingleOrDefault(m => m.Id == userBadge.BadgeId);

            //var UserDetails = _context.Users
            //        .SingleOrDefault(m => m.Id == userBadge.UserId);
            userBadge.UserId = _userManager.GetUserId(User);
            userBadge.BadgeId = 8;
            userBadge.DateofGrant = DateTime.Now;
            userBadge.DateofRequest = DateTime.Now;
            userBadge.IsGranted = false;
            userBadge.RejectCount = 0;
            userBadge.IsRejected = false;
            //userBadge.UserId = id;
            _context.Add(userBadge);

            //  EmailContent content = _context.EmailContents.Where(m => m.Id == Badge.EmailContentId).SingleOrDefault();
            //  await _emailSender.SendEmailAsync(UserDetails.Email, content.ArSubject, content.ArContent);

            await _context.SaveChangesAsync();
            return RedirectToAction("Personal");

            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName", userBadge.BadgeId);
            ////ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == userBadge.UserId), "Id", "ArName");
            //return View(userBadge);
        }


        // GET: UserBadges/Create

        public IActionResult Apply(int id)
        {
            var UserDetails = _context.Users
                .Include(a => a.University)
  .SingleOrDefault(m => m.Id == _userManager.GetUserId(User));
            var UserBadge = _context.UserBadges.SingleOrDefault(a => a.UserId == _userManager.GetUserId(User) && a.BadgeId == id);


            ViewData["BadgeName"] = _context.Badges.SingleOrDefault(a => a.Id == id).ArBadgeName;
            ViewData["Id"] = id;


            if (UserBadge == null)
            {
                StringBuilder stb = new StringBuilder();

                switch (id)
                {
                    case 1: //Ê”«„ »«ÕÀ „»«œ—

                        ViewData["Conditions"] = "··«”› ·«Ì„ﬂ‰  ﬁœÌ„ «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „»«œ— «·« »⁄œ «ﬂ„«· «·‘—Êÿ √œ‰«Â";
                        if (UserDetails.ArName == null | UserDetails.EnName == null)
                        {
                            stb.Append("«”„ «·»«ÕÀ »«··€ Ì‰ €Ì— „ﬂ „·. «·—Ã«¡  ÕœÌÀ «·»Ì«‰«  «·‘Œ’Ì…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« · ÕœÌÀ «·«”„</a><br/>");
                        }

                        if (UserDetails.Summary == null || UserDetails.Summary.Length < 20)
                        {
                            stb.Append("«·‰»–… «·„Œ ’—… ⁄‰ „Ã«· «·«Â „«„ «·»ÕÀÌ €Ì— „ﬂ „·… " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« · ÕœÌÀ «·‰»–… «·„Œ ’—…</a><br/>");
                        }

                        if (UserDetails.ProfileImage == null)
                        {
                            stb.Append("«·’Ê—… «·‘Œ’Ì… €Ì— „ÊÃÊœ…. «·—Ã«¡  ÕœÌÀ «·»Ì«‰«  «·‘Œ’Ì…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« ·—›⁄ «·’Ê—… «·‘Œ’Ì…</a><br/>");
                        }

                        if (UserDetails.CVURL == null)
                        {
                            stb.Append("«·”Ì—… «·–« Ì… €Ì— „ÊÃÊœ…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« ·—›⁄ «·”Ì—… «·–« Ì…</a><br/>");
                        }

                        if (_context.AcademicActivities.Count(a => a.ApplicationUserId == UserDetails.Id) < 5)
                        {
                            stb.Append("«·«‰‘ÿ… «·«ﬂ«œÌ„Ì… ÌÃ» √‰  ﬂÊ‰ ⁄·Ï «·√ﬁ· Œ„”… √‰‘ÿ…. " + "<a target='_blank' href='/ar-LY/AcademicActivities/'>«÷€ÿ Â‰« · ÕœÌÀ «·√‰‘ÿ… «·«ﬂ«œÌ„Ì…</a><br/>");
                        }

                        if (_context.EducationalLevels.Count(a => a.ApplicationUserId == UserDetails.Id) < 2)
                        {
                            stb.Append("«·„ƒÂ·«  «·«ﬂ«œÌ„Ì… ÌÃ» √‰  ﬂÊ‰ ⁄·Ï «·√ﬁ· «À‰Ì‰. " + "<a target='_blank' href='/ar-LY/EducationalLevels/'>«÷€ÿ Â‰« · ÕœÌÀ «·„ƒÂ·«  «·«ﬂ«œÌ„Ì…</a><br/>");
                        }

                        if (_context.UserSkills.Count(a => a.ApplicationUserId == UserDetails.Id) < 1)
                        {
                            stb.Append("ÌÃ»  ÕœÌœ «·„Â«—«  «·Œ«’… »ﬂ„ . " + "<a target='_blank' href='/ar-LY/UserSkills/'>«÷€ÿ Â‰« · ÕœÌÀ «·„Â«—« </a><br/>");
                        }

                        ViewData["Message"] = stb.ToString();

                        break;

                    case 13: //Ê”«„ »«ÕÀ „» œÌ¡

                        var EntUserBadge = _context.UserBadges.SingleOrDefault(a => a.UserId == _userManager.GetUserId(User) && a.BadgeId == 1);
                        if (EntUserBadge == null)
                        {
                            ViewData["Conditions"] = "··«”› ·«Ì„ﬂ‰  ﬁœÌ„ «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „» œÌ¡ «·« »⁄œ «ﬂ„«· «·‘—Êÿ √œ‰«Â";
                            if (UserDetails.ArName == null | UserDetails.EnName == null)
                            {
                                stb.Append("«”„ «·»«ÕÀ »«··€ Ì‰ €Ì— „ﬂ „·. «·—Ã«¡  ÕœÌÀ «·»Ì«‰«  «·‘Œ’Ì…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« · ÕœÌÀ «·«”„</a><br/>");
                            }

                            if (UserDetails.Summary == null || UserDetails.Summary.Length < 20)
                            {
                                stb.Append("«·‰»–… «·„Œ ’—… ⁄‰ „Ã«· «·«Â „«„ «·»ÕÀÌ €Ì— „ﬂ „·… " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« · ÕœÌÀ «·‰»–… «·„Œ ’—…</a><br/>");
                            }

                            if (UserDetails.ProfileImage == null)
                            {
                                stb.Append("«·’Ê—… «·‘Œ’Ì… €Ì— „ÊÃÊœ…. «·—Ã«¡  ÕœÌÀ «·»Ì«‰«  «·‘Œ’Ì…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« ·—›⁄ «·’Ê—… «·‘Œ’Ì…</a><br/>");
                            }

                            if (UserDetails.CVURL == null)
                            {
                                stb.Append("«·”Ì—… «·–« Ì… €Ì— „ÊÃÊœ…. " + "<a target='_blank' href='/ar-LY/Manage/'>«÷€ÿ Â‰« ·—›⁄ «·”Ì—… «·–« Ì…</a><br/>");
                            }


                            if (_context.EducationalLevels.Count(a => a.ApplicationUserId == UserDetails.Id) < 1)
                            {
                                stb.Append("ÌÃ»  ÊÀÌﬁ „—Õ·… «·œ—«”… «·Õ«·Ì…. " + "<a target='_blank' href='/ar-LY/EducationalLevels/'>«÷€ÿ Â‰« · ÕœÌÀ «·„ƒÂ·«  «·«ﬂ«œÌ„Ì…</a><br/>");
                            }

                            if (_context.UserSkills.Count(a => a.ApplicationUserId == UserDetails.Id) < 1)
                            {
                                stb.Append("ÌÃ»  ÕœÌœ «·„Â«—«  «·Œ«’… »ﬂ„ . " + "<a target='_blank' href='/ar-LY/UserSkills/'>«÷€ÿ Â‰« · ÕœÌÀ «·„Â«—« </a><br/>");
                            }
                            ViewData["Message"] = stb.ToString();
                        }
                        else
                        { ViewData["Conditions"] = "·«ÌÕﬁ ·ﬂ„ «· ﬁœÌ„ ⁄·Ï Ê”«„ »«ÕÀ „» œ∆ »⁄œ Õ’Ê·ﬂ„ ⁄·Ï Ê”«„ »«ÕÀ „»«œ—. ›Ê”«„ »«ÕÀ „» œ∆ Ìı„‰Õ ··»«ÕÀˆ «·„» œ∆ «·–Ì ÌﬂÊ‰ ›Ì «·„—Õ·… «·Ã«„⁄Ì…° Ê·œÌÂ ‰‘«ÿÒ Ê«ÕœÒ ⁄·Ï «·√ﬁ·"; }
                        break;

                    case 2:
                        var applicationusersReferredById = _context.ApplicationUsers.Where(a => a.ReferredById == UserDetails.Id && a.UniversityId == UserDetails.UniversityId);
                        var applicationuserssameuniandbadge = _context.UserBadges.Where(a => a.BadgeId == 1 && applicationusersReferredById.Where(m => m.Id == a.UserId).Count() > 0);
                        //   ViewData["UsersWithEntBadgefromSameOrganization"] = applicationuserssameuniandbadge.Count();


                        //    var UsersWithEntBadgefromSameOrganization = _context.UserBadges.Where(a => a.BadgeId == 1 && a.User.UniversityId == UserDetails.UniversityId).Select(a => a.UserId);
                        //   var UsersFromSameOranization = _context.Users.Where(r => UsersWithEntBadgefromSameOrganization.Contains(r.Id)).Select(a => a.ReferredById == _userManager.GetUserId(User));
                        //a => a.ReferredById == _userManager.GetUserId(User) &&




                        var EntBadge = _context.UserBadges.Where(a => a.UserId == _userManager.GetUserId(User));
                        if (EntBadge.Count() < 1)
                        {
                            ViewData["Conditions"] = "ÌÃ» «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „»«œ— Ê„—«Ã⁄… „Õ ÊÏ ’›Õ ﬂ„ »‘ﬂ· œﬁÌﬁ ﬁ»· «· ﬁœÌ„. ÕÌÀ «‰ ⁄œœ «·„—«  «·„”„ÊÕ »Â« ·· ﬁœÌ„ ÂÌ À·«À „—«  ›ﬁÿ.";
                            break;
                        }

                        if (applicationuserssameuniandbadge.Count() < 15)
                        {
                            ViewData["Conditions"] = "··«”› ·«Ì„ﬂ‰ «· ﬁœÌ„ ⁄·Ï Â–« «·Ê”«„ «·« »⁄œ  ”ÃÌ· 15 »«ÕÀ« Õ«’·« ⁄·Ï Ê”«„ »«ÕÀ „»«œ— „‰ „ƒ”” ﬂ„. ÕÌÀ «‰ ⁄œœ «·»«ÕÀÌ‰ «·„”Ã·Ì‰ Ê«·Õ«’·Ì‰ ⁄·Ï Ê”«„ »«ÕÀ „»«œ— ⁄‰ ÿ—Ìﬁﬂ„ „‰ " + UserDetails.University.ArUniversityName + " : " + applicationuserssameuniandbadge.Count();
                        }
                        else
                        {
                            ViewData["Message"] = "";
                            ViewData["Conditions"] = "Õ”«»ﬂ„ „” Ê›Ì ‘—Êÿ «· ﬁœÌ„ ⁄·Ï Ê”«„ »«ÕÀ ‰«‘ÿ Ê›ﬁ  ﬁÊÌ„ „‰’… «—Ìœ";
                        }



                        break;

                    case 3:
                        break;

                    case 4:
                        var applicationusersReferredById1 = _context.ApplicationUsers.Where(a => a.ReferredById == UserDetails.Id && a.UniversityId != UserDetails.UniversityId);
                        var applicationuserssameuniandbadge1 = _context.UserBadges.Where(a => a.BadgeId == 1 && applicationusersReferredById1.Where(m => m.Id == a.UserId).Count() > 0);

                        var EntBadge1 = _context.UserBadges.Where(a => a.UserId == _userManager.GetUserId(User));
                        if (EntBadge1.Count() < 1)
                        {
                            ViewData["Conditions"] = "ÌÃ» «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „»«œ— Ê„—«Ã⁄… „Õ ÊÏ ’›Õ ﬂ„ »‘ﬂ· œﬁÌﬁ ﬁ»· «· ﬁœÌ„. ÕÌÀ «‰ ⁄œœ «·„—«  «·„”„ÊÕ »Â« ·· ﬁœÌ„ ÂÌ À·«À „—«  ›ﬁÿ.";
                            break;
                        }

                        if (applicationuserssameuniandbadge1.Count() < 40)
                        {
                            ViewData["Conditions"] = "  ··«”› ·«Ì„ﬂ‰ «· ﬁœÌ„ ⁄·Ï Â–« «·Ê”«„ «·« »⁄œ  ”ÃÌ· 40 ⁄÷Ê« Õ«’·« ⁄·Ï Ê”«„ »«ÕÀ „»«œ— „‰ Œ«—Ã „ƒ”” ﬂ «·Õ«·Ì…. ÕÌÀ «‰ ⁄œœ «·«⁄÷«¡ «·„”Ã·Ì‰ Ê«·Õ«’·Ì‰ ⁄·Ï Ê”«„ »«ÕÀ „»«œ— ⁄‰ ÿ—Ìﬁﬂ„ :   " + applicationuserssameuniandbadge1.Count();

                        }
                        else
                        {
                            ViewData["Message"] = "";
                            ViewData["Conditions"] = "Õ”«»ﬂ„ „” Ê›Ì ‘—Êÿ «· ﬁœÌ„ ⁄·Ï Ê”«„ ‰«‘ˆÿ œÊ·Ì¯ Ê›ﬁ  ﬁÊÌ„ „‰’… «—Ìœ";
                        }


                        break;

                    case 9:
                        var Publications = _context.Publications.Where(a => a.ApplicationUserId == _userManager.GetUserId(User));



                        if (Publications.Count() < 25)
                        {
                            ViewData["Conditions"] = "··«”› ·« Ì„ﬂ‰ «· ﬁœÌ„ ⁄·Ï Â–« «·Ê”«„. ÕÌÀ ÌÃ» «‰   Õ ÊÌ ’›Õ ﬂ„ ⁄·Ï „Õ ÊÏ ﬂ«„· ·Œ„”… Ê⁄‘—Ì‰ »ÕÀ« „‰‘Ê—«° √Ê ﬂ «»« „‰‘Ê—« ›√ﬂÀ—. ⁄œœ «·»ÕÊÀ «·Õ«·Ì ›Ì Õ”«»ﬂ„ " + Publications.Count();

                        }
                        else
                        {
                            ViewData["Message"] = "";
                            ViewData["Conditions"] = "Õ”«»ﬂ„ „” Ê›Ì ‘—Êÿ «· ﬁœÌ„ ⁄·Ï Ê”«„ ‰Û«‘ˆ— „ı „Ì¯ˆ“ Ê›ﬁ  ﬁÊÌ„ „‰’… «—Ìœ";
                        }
                        break;

                    case 10:
                        var EntBadge2 = _context.UserBadges.Where(a => a.UserId == _userManager.GetUserId(User));
                        if (EntBadge2.Count() < 1)
                        {
                            ViewData["Conditions"] = "ÌÃ» «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „»«œ— Ê„—«Ã⁄… „Õ ÊÏ ’›Õ ﬂ„ »‘ﬂ· œﬁÌﬁ ﬁ»· «· ﬁœÌ„. ÕÌÀ «‰ ⁄œœ «·„—«  «·„”„ÊÕ »Â« ·· ﬁœÌ„ ÂÌ À·«À „—«  ›ﬁÿ.";
                            break;
                        }
                        else
                        {
                            if (UserDetails.Visitors > 1000)
                            {
                                ViewData["Message"] = "";
                                ViewData["Conditions"] = "Ê”«„ «·’›Õ… «·„ „Ì“…";
                            }
                            else
                            { ViewData["Conditions"] = "’›Õ ﬂ„ «·Õ«·Ì… €Ì— „ÿ«»ﬁ… ·„⁄«ÌÌ— «·’›Õ«  «·„ÌÌ“…. «·—Ã«¡ «·⁄„· ⁄·Ï «À—«¡ «·’›Õ… »«‰ «Ãﬂ„ «·⁄·„Ì  Ê ﬁœÌ„ «·ÿ·» ·«Õﬁ«"; }
                        }
                        break;


                    case 11:
                        var EntResBadge = _context.UserBadges.Where(a => a.UserId == _userManager.GetUserId(User));



                        if (EntResBadge.Count() < 1)
                        {
                            ViewData["Conditions"] = "ÌÃ» «·Õ’Ê· ⁄·Ï Ê”«„ »«ÕÀ „»«œ— Ê„—«Ã⁄… „Õ ÊÏ ’›Õ ﬂ„ »‘ﬂ· œﬁÌﬁ ﬁ»· «· ﬁœÌ„. ÕÌÀ «‰ ⁄œœ «·„—«  «·„”„ÊÕ »Â« ·· ﬁœÌ„ ÂÌ À·«À „—«  ›ﬁÿ. «Õ—’ ⁄·Ï  Ê«›— ‘—Êÿ «·Ê”«„ ·œÌﬂ„ Ê”Ì „ „—«Ã⁄… Õ”«»ﬂ„ »œﬁ… „‰ ﬁ»· ·Ã‰… „‰Õ «·«Ê”„….";

                        }
                        else
                        {
                            ViewData["Message"] = "";
                            //ViewData["Conditions"] = "Ê”«„ «·’›Õ… «·„ „Ì“…";
                        }
                        break;
                }
            }



            else if (UserBadge.IsGranted == true)
            {
                ViewData["Message"] = " „ „‰Õﬂ„ «·Ê”«„";
            }

            else if (UserBadge.Disabled == true)
            {
                ViewData["Message"] = "·«Ì„ﬂ‰ﬂ„ «· ﬁœÌ„ ⁄·Ï Â–« «·Ê”«„";
            }

            else if (UserBadge.IsRejected == true)
            {
                ViewData["Message"] = " „ —›÷ ÿ·» «·Õ’Ê· ⁄·Ï «·Ê”«„. «·—Ã«¡ „⁄«·Ã… «·”»» : " + UserBadge.RejectReason;
                ViewData["ReApply"] = "ReApply";
            }

            else
            {
                ViewData["Message"] = " „ «” ·«„ «·ÿ·» ÊÃ«—Ì „—«Ã⁄ Â „‰ ﬁ»· «··Ã‰… «·„Œ ’…. ”Ì „ «⁄·«„ﬂ„ ⁄»— «·»—Ìœ «·«·ﬂ —Ê‰Ì »Õ«·… «·ÿ·»";
            }


            return View();
        }

        // POST: UserBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        public async Task<IActionResult> BadgeApplication(int id)
        {
            _context.UserBadges.Add(
new UserBadge
{
    BadgeId = id,
    DateofRequest = DateTime.Now,
    Disabled = false,
    IsGranted = false,
    IsRejected = false,
    RejectCount = 0,
    UserId = _userManager.GetUserId(User)
});


            _context.SaveChanges();

            return RedirectToAction("Apply", new { id = id });

            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "Id", userBadge.BadgeId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            //return View();
        }


        public async Task<IActionResult> ReApply(int id)
        {

            var UserBadge = _context.UserBadges.SingleOrDefault(a => a.UserId == _userManager.GetUserId(User) && a.BadgeId == id);

            UserBadge.IsRejected = false;
            UserBadge.RejectCount++;
            UserBadge.DateofRequest = DateTime.Now;

            _context.Update(UserBadge);
            _context.SaveChanges();

            return RedirectToAction("Apply", new { id = id });

            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "Id", userBadge.BadgeId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            //return View();
        }

        // GET: UserBadges/Edit/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var userBadge = await _context.UserBadges.SingleOrDefaultAsync(m => m.Id == id);

            if (userBadge == null)
            {
                return NotFound();
            }
            ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName", userBadge.BadgeId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(a => a.Id == userBadge.UserId), "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            return View(userBadge);
        }

        // POST: UserBadges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.Admins)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BadgeId,UserId,DateofRequest,DateofGrant,IsRejected,RejectCount,RejectReason,IsGranted")] UserBadge userBadge)
        {
            if (id != userBadge.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var Badge = _context.Badges
.SingleOrDefault(m => m.Id == userBadge.BadgeId);
                    var UserDetails = _context.Users
                           .SingleOrDefault(m => m.Id == userBadge.UserId);
                    if (userBadge.IsGranted == true)
                    {
                        userBadge.DateofGrant = DateTime.Now;
                      //  EmailContent content = _context.EmailContents.Where(m => m.Id == 1).SingleOrDefault();
                        EmailContent content = _context.EmailContents.Where(m => m.Id == Badge.EmailContentId).SingleOrDefault();
                        await _emailSender.SendEmailAsync(UserDetails.Email, content.ArSubject, content.ArContent);
                    }


                    if (userBadge.IsRejected == true)
                    {
                        userBadge.RejectCount++;
                        if (userBadge.BadgeId == 1)
                        {
                            EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString()== "979f0462-f293-4223-909e-6b5c51bb91f5").SingleOrDefault();
                            await _emailSender.SendEmailAsync(UserDetails.Email, content.ArSubject, content.ArContent);
                        }
                        else
                        {
                            EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString() == "21b9d8a1-1833-49a7-9d7e-c734c8d59529").SingleOrDefault();
                            //var Badge = _context.Badges.SingleOrDefault(a => a.Id == userBadge.BadgeId);
                            string BadgeName = "·„   „ «·„Ê«›ﬁ… ⁄·Ï „‰Õﬂ„ " + Badge.ArBadgeName;
                            await _emailSender.SendEmailAsync(UserDetails.Email, BadgeName, content.ArContent + userBadge.RejectReason);
                        }



                    }
                    _context.Update(userBadge);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBadgeExists(userBadge.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Unapproved");
            }
            //ViewData["BadgeId"] = new SelectList(_context.Badges, "Id", "ArBadgeName", userBadge.BadgeId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "ArName", userBadge.UserId);
            //ViewData["UserId"] = new SelectList(_userManager.Users.Where(u => u.Id == _userManager.GetUserId(User)), "Id", "ArName");
            return View(userBadge);
        }

        // GET: UserBadges/Delete/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBadge = await _context.UserBadges
                .Include(u => u.Badge)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userBadge == null)
            {
                return NotFound();
            }

            return View(userBadge);
        }

        // POST: UserBadges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userBadge = await _context.UserBadges.SingleOrDefaultAsync(m => m.Id == id);
            _context.UserBadges.Remove(userBadge);
            await _context.SaveChangesAsync();
            return RedirectToAction("Unapproved");
        }

        private bool UserBadgeExists(int id)
        {
            return _context.UserBadges.Any(e => e.Id == id);
        }
    }
}
