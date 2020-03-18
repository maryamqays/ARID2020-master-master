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
using Microsoft.AspNetCore.Hosting;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using System.Text;
using Hangfire;
using ARID.Services;

namespace ARID.Controllers
{
    public class SubmissionReviewersController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;
        private readonly IEmailSender _emailSender;


        public SubmissionReviewersController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment, IEmailSender emailSender)
        {
            _environment = environment;
            _context = context;
            _userManager = userMrg;
            _emailSender = emailSender;
        }

        // GET: SubmissionReviewers
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.SubmissionReviewers.Include(s => s.CieAeUser).Include(s => s.ReviewerUser).Include(s => s.Submission);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: SubmissionReviewers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submissionReviewer = await _context.SubmissionReviewers
                .Include(s => s.CieAeUser)
                .Include(s => s.ReviewerUser)
                .Include(s => s.Submission)
                .SingleOrDefaultAsync(m => m.Id == id);
            var submission = _context.Submissions.SingleOrDefault(a => a.Id == submissionReviewer.SubmissionId);
            ViewData["mid"] = submission.ManuscriptId;

            if (submissionReviewer == null)
            {
                return NotFound();
            }

            return View(submissionReviewer);
        }
        public async Task<IActionResult> RevReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submissionReviewer = await _context.SubmissionReviewers
                .Include(s => s.CieAeUser)
                .Include(s => s.ReviewerUser)
                .Include(s => s.Submission)
                .SingleOrDefaultAsync(m => m.Id == id);
            var submission = _context.Submissions.SingleOrDefault(a => a.Id == submissionReviewer.SubmissionId);
            ViewData["mid"] = submission.ManuscriptId;

            if (submissionReviewer == null)
            {
                return NotFound();
            }

            return View(submissionReviewer);
        }

        // GET: SubmissionReviewers/Create
        public IActionResult Create(int mid, string ss)
        {
            ViewData["mid"] = mid;
            ViewData["journalid"] = _context.Manuscripts.SingleOrDefault(a => a.Id== mid).JournalId;
            SubmissionReviewerViewModel SubmissionReviewerVM = new SubmissionReviewerViewModel()
            {
                ApplicationUsers = _context.ApplicationUsers.Include(a => a.University).Include(a => a.Country).Where(a => a.ArName.Contains(ss) || a.EnName.Contains(ss) || a.Email.Contains(ss) || a.Id.Contains(ss) || a.ARID.ToString().Contains(ss) || a.PhoneNumber.Contains(ss) || a.SecondEmail.Contains(ss)),
                SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission).Include(a => a.Submission.Manuscript).Where(a => a.Submission.ManuscriptId == mid),
                articleAuthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == mid).Include(a => a.AuthorUser),
            };
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions.Where(a => a.ManuscriptId == mid && a.SubmissionStatus == SubmissionStatus.Submitted), "Id", "CoverLetter");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");

            return View(SubmissionReviewerVM);
        }
        // POST: SubmissionReviewers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int mid, [Bind("Id,SubmissionId,ReviewerUserId,DueDate,ReviewerStatus,IsCertificateOfReviewingGranted,DateRevisionCertificae,EICInstructions,ReviewerInstructionsToEIC,ReviewerInstructionsToAuthor,DateOfRecord,ReviewDate,CieAeUserId,DecisionDate,ReviewerDecisionToReview,IsAccess,IsNewInfo,IsWithinJournalScope,AbstractCompatibility,IsFormsAccepted,IsPublishedPreviously,IsValuable,IsRepeatedInfo,ResearchLength,TheoreticalResults,Method,IsCommitted,IsMatchTitleContent,IsModernSourcesAdopted,IsDiscussionDocumentedJustified,IsInterpretedResult,Evaluation,RecommendationToPublish,NotForPublicationReason,Adjustments,PublishLocation,ReviewReportFile")] SubmissionReviewer submissionReviewer)
        {
            if (ModelState.IsValid)
            {
                submissionReviewer.DateOfRecord = DateTime.Now;
                submissionReviewer.EICInstructions = submissionReviewer.EICInstructions.Replace("\n", "<br/>");
                submissionReviewer.CieAeUserId = _userManager.GetUserId(User);
                submissionReviewer.IsAccess = true;

                var manuscriupt = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == mid);
                var ReviwerUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == submissionReviewer.ReviewerUserId);
                EmailContent Reviewercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "31c28fc6-aa5a-4a68-8cea-351d6d1c0853").SingleOrDefault();

                StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
                MyStringBuilder.Append(manuscriupt.Journal.ArName);
                MyStringBuilder.Append("</h2>");
                MyStringBuilder.Append("<h2 align='right'>تاريخ إتمام التحكيم المفترض : ");
                MyStringBuilder.Append(submissionReviewer.DueDate.Date);
                MyStringBuilder.Append("</h2>");
                MyStringBuilder.Append("<br/>");
                MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscriupt.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
                MyStringBuilder.Append("</a></h3><hr/>");

                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(ReviwerUser.Email, Reviewercontent.ArSubject, Reviewercontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));



                _context.Add(submissionReviewer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = mid });
            }
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.CieAeUserId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.ReviewerUserId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionReviewer.SubmissionId);
            return View(submissionReviewer);
        }

        // GET: SubmissionReviewers/Process/5
        public async Task<IActionResult> Process(int? id, int mid)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == mid && a.SubmissionStatus == SubmissionStatus.Submitted);

            SubmissionReviewerViewModel submissionreviewerVm = new SubmissionReviewerViewModel()
            {
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == submission.Id),
                SubmissionReviewer = await _context.SubmissionReviewers.SingleOrDefaultAsync(m => m.Id == id)
            };
            var submissionReviewer = await _context.SubmissionReviewers.SingleOrDefaultAsync(m => m.Id == id);
            if (submissionReviewer == null)
            {
                return NotFound();
            }
            ViewData["mid"] = mid;
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == submissionReviewer.CieAeUserId), "Id", "Id", submissionReviewer.CieAeUserId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == submissionReviewer.ReviewerUserId), "Id", "Id", submissionReviewer.ReviewerUserId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions.Where(a => a.ManuscriptId == mid && a.SubmissionStatus == SubmissionStatus.Submitted), "Id", "CoverLetter", submissionReviewer.SubmissionId);
            return View(submissionreviewerVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id, int maid, [Bind("Id,SubmissionId,ReviewerUserId,DueDate,ReviewerStatus,IsCertificateOfReviewingGranted,DateRevisionCertificae,EICInstructions,ReviewerInstructionsToEIC,ReviewerInstructionsToAuthor,DateOfRecord,ReviewDate,CieAeUserId,DecisionDate,ReviewerDecisionToReview,IsAccess,IsNewInfo,IsWithinJournalScope,AbstractCompatibility,IsFormsAccepted,IsPublishedPreviously,IsValuable,IsRepeatedInfo,ResearchLength,TheoreticalResults,Method,IsCommitted,IsMatchTitleContent,IsModernSourcesAdopted,IsDiscussionDocumentedJustified,IsInterpretedResult,Evaluation,RecommendationToPublish,NotForPublicationReason,Adjustments,PublishLocation,ReviewReportFile")] SubmissionReviewer submissionReviewer, IFormFile myfile)
        {
            if (id != submissionReviewer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submissionReviewer.ReviewReportFile = await UserFile.UploadeNewFileAsync(submissionReviewer.ReviewReportFile,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent);
                    submissionReviewer.ReviewDate = DateTime.Now;

                    _context.Update(submissionReviewer);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionReviewerExists(submissionReviewer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.CieAeUserId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.ReviewerUserId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionReviewer.SubmissionId);
            return RedirectToAction("Process", "SubmissionReviewers", new {/* routeValues, for example: */ id = submissionReviewer.Id, mid = maid });
        }


        // GET: SubmissionReviewers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionReviewer = await _context.SubmissionReviewers.SingleOrDefaultAsync(m => m.Id == id);
            if (submissionReviewer == null)
            {
                return NotFound();
            }
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.CieAeUserId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.ReviewerUserId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionReviewer.SubmissionId);
            return View(submissionReviewer);
        }

        // POST: SubmissionReviewers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubmissionId,ReviewerUserId,DueDate,ReviewerStatus,IsCertificateOfReviewingGranted,DateRevisionCertificae,EICInstructions,ReviewerInstructionsToEIC,ReviewerInstructionsToAuthor,DateOfRecord,ReviewDate,CieAeUserId,DecisionDate,ReviewerDecisionToReview,IsAccess,IsNewInfo,IsWithinJournalScope,AbstractCompatibility,IsFormsAccepted,IsPublishedPreviously,IsValuable,IsRepeatedInfo,ResearchLength,TheoreticalResults,Method,IsCommitted,IsMatchTitleContent,IsModernSourcesAdopted,IsDiscussionDocumentedJustified,IsInterpretedResult,Evaluation,RecommendationToPublish,NotForPublicationReason,Adjustments,PublishLocation,ReviewReportFile")] SubmissionReviewer submissionReviewer)
        {
            if (id != submissionReviewer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submissionReviewer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionReviewerExists(submissionReviewer.Id))
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
            ViewData["CieAeUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.CieAeUserId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", submissionReviewer.ReviewerUserId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionReviewer.SubmissionId);
            return View(submissionReviewer);
        }

        // GET: SubmissionReviewers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionReviewer = await _context.SubmissionReviewers
                .Include(s => s.CieAeUser)
                .Include(s => s.ReviewerUser)
                .Include(s => s.Submission)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (submissionReviewer == null)
            {
                return NotFound();
            }

            return View(submissionReviewer);
        }

        // POST: SubmissionReviewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submissionReviewer = await _context.SubmissionReviewers.SingleOrDefaultAsync(m => m.Id == id);
            _context.SubmissionReviewers.Remove(submissionReviewer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmissionReviewerExists(int id)
        {
            return _context.SubmissionReviewers.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult ReviewerAgree(int id)
        {
            SubmissionReviewer submissionreviewer = _context.SubmissionReviewers.Include(a => a.Submission.AE.ApplicationUser).Include(a => a.ReviewerUser).Include(a => a.Submission).SingleOrDefault(m => m.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == submissionreviewer.Submission.ManuscriptId);
            if (submissionreviewer == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.UnderReview;
            submissionreviewer.ReviewerDecisionToReview = ReviewerDecisionToReview.AcceptedToReview;
            submissionreviewer.DecisionDate = DateTime.Now;
            _context.Update(submissionreviewer);
            _context.Update(manuscript);
            _context.SaveChanges();


            //---------------------------------------------------------------------------
            EmailContent eicandaecontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "d74c8bda-b196-4595-bf2e-85c8b2f48fc0").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>المحكم : ");
            MyStringBuilder.Append(submissionreviewer.ReviewerUser.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>تاريخ الموافقة على التحكيم : ");
            MyStringBuilder.Append(DateTime.Now);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            var eic = _context.ApplicationUsers.SingleOrDefault(a => a.Id == manuscript.Journal.EiCId);

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(eic.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            if (submissionreviewer.Submission.AEId != null)
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(submissionreviewer.Submission.AE.ApplicationUser.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            }
            //-----------------------------------------------------------------------------------------


            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submissionreviewer.Submission.ManuscriptId });
        }
        [Authorize]
        public IActionResult ReviewerReject(int id)
        {
            SubmissionReviewer submissionreviewer = _context.SubmissionReviewers.Include(a => a.Submission.AE.ApplicationUser).Include(a => a.ReviewerUser).Include(a => a.Submission).Include(a => a.Submission.Manuscript).SingleOrDefault(m => m.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == submissionreviewer.Submission.ManuscriptId);

            if (submissionreviewer == null)
            {
                return NotFound();
            }
            submissionreviewer.ReviewerDecisionToReview = ReviewerDecisionToReview.DeclinedToReview;
            submissionreviewer.DecisionDate = DateTime.Now; ;
            _context.Update(submissionreviewer);
            _context.SaveChanges();


            //---------------------------------------------------------------------------
            EmailContent eicandaecontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "370d672f-01c5-479e-b015-e32b89b83fc1").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>المحكم : ");
            MyStringBuilder.Append(submissionreviewer.ReviewerUser.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>تاريخ رفض التحكيم : ");
            MyStringBuilder.Append(DateTime.Now);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            var eic = _context.ApplicationUsers.SingleOrDefault(a => a.Id == manuscript.Journal.EiCId);

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(eic.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            if (submissionreviewer.Submission.AE != null)
            { BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(submissionreviewer.Submission.AE.ApplicationUser.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10)); }

            //-----------------------------------------------------------------------------------------



            return RedirectToAction("Details", "Journals", new {/* routeValues, for example: */ id = submissionreviewer.Submission.Manuscript.JournalId });
        }

        [Authorize]
        public IActionResult Reviewsubmit(int id)
        {
            SubmissionReviewer submissionreviewer = _context.SubmissionReviewers.Include(a => a.Submission.AE.ApplicationUser).Include(a => a.ReviewerUser).Include(a => a.Submission).Include(a => a.Submission.Manuscript).SingleOrDefault(m => m.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == submissionreviewer.Submission.ManuscriptId);

            if (submissionreviewer == null)
            {
                return NotFound();
            }
            submissionreviewer.IsAccess = false;
            submissionreviewer.ReviewDate = DateTime.Now; ;
            _context.Update(manuscript);
            _context.Update(submissionreviewer);
            _context.SaveChanges();


            //---------------------------------------------------------------------------
            EmailContent eicandaecontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "8569cfab-f7a8-49a6-9f35-2e9148915f1f").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>المحكم : ");
            MyStringBuilder.Append(submissionreviewer.ReviewerUser.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>تاريخ تسليم التحكيم : ");
            MyStringBuilder.Append(submissionreviewer.ReviewDate);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            var eic = _context.ApplicationUsers.SingleOrDefault(a => a.Id == manuscript.Journal.EiCId);

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(eic.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            if (submissionreviewer.Submission.AE != null)
            { BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(submissionreviewer.Submission.AE.ApplicationUser.Email, eicandaecontent.ArSubject, eicandaecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10)); }

            //-----------------------------------------------------------------------------------------


            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submissionreviewer.Submission.ManuscriptId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int maid, string eicins, ReviewerStatus rs, DateTime date, [Bind("Id,UILanguage,ReferredById,ArName,EnName,Email,Password,CountryId,UniversityId")] RegisterFromOutsideViewModel registerFromOutsideViewModel)
        {
            _context.ApplicationUsers.Add(new ApplicationUser
            {
                UserName = registerFromOutsideViewModel.Email,
                Email = registerFromOutsideViewModel.Email,
                // Added by Alrshah =============================================================================================
                ArName = registerFromOutsideViewModel.ArName,
                EnName = registerFromOutsideViewModel.EnName,
                //DateofBirth = model.DateofBirth,
                //Gender = model.Gender,
                UILanguage = registerFromOutsideViewModel.UILanguage,
                CityId = 1,
                CountryId = registerFromOutsideViewModel.CountryId,
                FacultyId = 1,
                UniversityId = registerFromOutsideViewModel.UniversityId,
                Status = StatusType.New,
                RegDate = DateTime.Now,
                ARID = 0,
                ReferredById = registerFromOutsideViewModel.ReferredById,
                Visitors = 0,
                DAL = Guid.NewGuid().ToString(),
                DALEnabled = true
            });
            _context.SaveChanges();
            var userid = _context.ApplicationUsers.Where(a => a.ReferredById == registerFromOutsideViewModel.ReferredById).OrderByDescending(a => a.RegDate).First().Id;
            _context.SubmissionReviewers.Add(new SubmissionReviewer
            {
                ReviewerUserId = userid,
                SubmissionId = _context.Submissions.Where(a => a.SubmissionStatus == SubmissionStatus.Submitted).SingleOrDefault(a => a.ManuscriptId == maid).Id,
                DueDate = date,
                ReviewerStatus = rs,
                EICInstructions = eicins,
                ReviewerDecisionToReview = ReviewerDecisionToReview.WaitingInvitationAcceptance
            });
            _context.SaveChanges();

            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = maid });
        }
    }
}
