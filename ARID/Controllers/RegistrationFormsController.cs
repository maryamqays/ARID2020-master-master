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
using Hangfire;
using ARID.Services;
using Microsoft.AspNetCore.Hosting;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ARID.Controllers
{
    [Authorize]
    public class RegistrationFormsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _environment;

        public RegistrationFormsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IEmailSender emailSender, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userMrg;
            _emailSender = emailSender;
            _environment = environment;
        }

        // GET: RegistrationForms
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.RegistrationForms.Include(r => r.ApplicationUser).Include(r => r.EmailContent).Include(r => r.EmailContentReminder);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: RegistrationForms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.UserBadges.Where(a => a.UserId == _userManager.GetUserId(User)).Count() == 0)
            {
                ViewData["BadgeStatus"] = "false";
            };
            var XViewModel = new RegistrationFormViewModel
            {
                RegistrationForm = await _context.RegistrationForms
                .Include(r => r.ApplicationUser)
                .Include(r => r.EmailContent)
                .Include(r => r.EmailContentReminder)
                .SingleOrDefaultAsync(m => m.Id == id),
                UserExpressInterests = _context.UserExpressInterests.Where(a => a.RegistrationFormId == id).Include(a => a.ApplicationUser)

            };

            //var registrationForm = await _context.RegistrationForm
            //    .Include(r => r.ApplicationUser)
            //    .Include(r => r.EmailContent)
            //    .Include(r => r.EmailContentReminder)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (registrationForm == null)
            //{
            //    return NotFound();
            //}

            return View(XViewModel);
        }

        // GET: RegistrationForms/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject");
            ViewData["EmailContentReminderId"] = new SelectList(_context.EmailContents, "Id", "ArSubject");
            return View();
        }

        // POST: RegistrationForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsActive,ApplicationUserId,CreationDate,EmailContentId,ExpiryDate,EmailContentReminderId,IsEntBadgeRequired,ReminderDays,IsPdfInvitation,InvitationHeader,InvitationFooter,InvitationContext,EventDate")] RegistrationForm registrationForm, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                registrationForm.InvitationHeader = await UserFile.UploadeNewFileAsync(registrationForm.InvitationHeader,
  myfile, _environment.WebRootPath, Properties.Resources.Secured);

                registrationForm.InvitationFooter = await UserFile.UploadeNewFileAsync(registrationForm.InvitationFooter,
          myfile1, _environment.WebRootPath, Properties.Resources.Secured);

                if (registrationForm.InvitationContext != null)
                {
                    registrationForm.InvitationContext = (System.Text.RegularExpressions.Regex.Replace(registrationForm.InvitationContext, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                }
                registrationForm.ApplicationUserId = _userManager.GetUserId(User);
                registrationForm.CreationDate = DateTime.Now;
                _context.Add(registrationForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", registrationForm.ApplicationUserId);
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentId);
            ViewData["EmailContentReminderId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentReminderId);
            return View(registrationForm);
        }

        // GET: RegistrationForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrationForm = await _context.RegistrationForms.SingleOrDefaultAsync(m => m.Id == id);
            if (registrationForm == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", registrationForm.ApplicationUserId);
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentId);
            ViewData["EmailContentReminderId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentReminderId);
            return View(registrationForm);
        }

        // POST: RegistrationForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsActive,ApplicationUserId,CreationDate,EmailContentId,ExpiryDate,EmailContentReminderId,IsEntBadgeRequired")] RegistrationForm registrationForm)
        {
            if (id != registrationForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    registrationForm.ApplicationUserId = _userManager.GetUserId(User);
                    _context.Update(registrationForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationFormExists(registrationForm.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", registrationForm.ApplicationUserId);
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentId);
            ViewData["EmailContentReminderId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", registrationForm.EmailContentReminderId);
            return View(registrationForm);
        }

        private bool RegistrationFormExists(int id)
        {
            throw new NotImplementedException();
        }

        // GET: RegistrationForms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrationForm = await _context.RegistrationForms
                .Include(r => r.ApplicationUser)
                .Include(r => r.EmailContent)
                .Include(r => r.EmailContentReminder)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (registrationForm == null)
            {
                return NotFound();
            }

            return View(registrationForm);
        }

        // POST: RegistrationForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var registrationForm = await _context.RegistrationForms.SingleOrDefaultAsync(m => m.Id == id);
            _context.RegistrationForms.Remove(registrationForm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationFormExists(int? id)
        {
            return _context.RegistrationForms.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Register(int id)
        {
            Guid ExpressInterestGuid = Guid.NewGuid();
            RegistrationForm registrationform = _context.RegistrationForms.Where(m => m.Id == id).SingleOrDefault();
            if (registrationform == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (_context.UserExpressInterests.Where(f => f.RegistrationFormId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {

                _context.UserExpressInterests.Add(new UserExpressInterest
                {
                    Id = ExpressInterestGuid,
                    ApplicationUserId = _userManager.GetUserId(User),
                    RegistrationFormId = id,
                    CreationDate = DateTime.Now
                });

            }

            _context.SaveChanges();
            ApplicationUser applicationUser = _context.ApplicationUsers.Where(m => m.Id == _userManager.GetUserId(User)).SingleOrDefault();

            EmailContent WelcomeEmail = _context.EmailContents.Where(m => m.Id == registrationform.EmailContentId).SingleOrDefault();
            EmailContent ReminderEmail = _context.EmailContents.Where(m => m.Id == registrationform.EmailContentReminderId).SingleOrDefault();
            //   DateTime ExpiryDate = registrationform.ExpiryDate.AddDays(-1);
            if (WelcomeEmail.Id != 1027)
            {
                if (_context.SentEmailRecords.Where(f => f.EmailContentId == WelcomeEmail.Id && f.ApplicationUserId == currentuserid).Count() == 0)
                {
                    var UserDetails = _context.ApplicationUsers
                            .SingleOrDefaultAsync(m => m.Id == _userManager.GetUserId(User)).Result;

                    StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
                    Welcome.AppendFormat(string.Format(UserDetails.ArName));
                    Welcome.Append("</h3>");
                    Welcome.Append("</br>");

                    _context.SentEmailRecords.Add(new SentEmailRecords
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        EmailContentId = WelcomeEmail.Id
                    });

                    _context.SaveChanges();

                    if (registrationform.IsPdfInvitation == true)
                    {
                        StringBuilder unsubscribe = new StringBuilder("<hr/><h2 align ='center'>لتحميل خطاب الدعوة الرسمي اضغط هنا </h2>");


                        unsubscribe.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/UserExpressInterests/Pdf?id={0}' >", ExpressInterestGuid));
                        unsubscribe.Append("Invitation Letter | خطاب الدعوة");
                        unsubscribe.Append("</a></h3><hr/>");


                        BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationUser.Email, WelcomeEmail.ArSubject, Welcome.ToString() + WelcomeEmail.ArContent + unsubscribe.ToString()), TimeSpan.FromMinutes(1));
                    }
                    else
                    { BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationUser.Email, WelcomeEmail.ArSubject, Welcome.ToString() + WelcomeEmail.ArContent), TimeSpan.FromMinutes(1)); }

                }


            }

            if (ReminderEmail.Id != 1027)
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationUser.Email, ReminderEmail.ArSubject, ReminderEmail.ArContent), TimeSpan.FromDays(registrationform.ReminderDays));
            }

            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult UnRegister(int id)
        {
            RegistrationForm registrationform = _context.RegistrationForms.Where(m => m.Id == id).SingleOrDefault();
            if (registrationform == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (_context.UserExpressInterests.Where(f => f.RegistrationFormId == id && f.ApplicationUserId == currentuserid).Count() > 0)
            {
                var expressinterest = _context.UserExpressInterests.SingleOrDefault(a => a.RegistrationFormId == id && a.ApplicationUserId == currentuserid);
                _context.UserExpressInterests.Remove(expressinterest);
            }
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

    }
}
