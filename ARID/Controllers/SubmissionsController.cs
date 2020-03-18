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
using System.Text;
using Hangfire;
using ARID.Services;

namespace ARID.Controllers
{
    public class SubmissionsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;



        public SubmissionsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userMrg;
            _emailSender = emailSender;
        }
        // GET: Submissions
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.Submissions.Include(s => s.AE).Include(s => s.Manuscript);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: Submissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions
    .Include(s => s.AE)
    .Include(s => s.Manuscript)
    .SingleOrDefaultAsync(m => m.Id == id);


            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.Include(s => s.AE).Include(s => s.Manuscript).SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Include(a => a.AE).Include(a => a.AE.ApplicationUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.ReviewerUser).Include(a => a.CieAeUser).Where(a => a.SubmissionId == submission.Id),
            };


            if (submission == null)
            {
                return NotFound();
            }

            return View(SubmissionVM);
        }

        // GET: Submissions/Create
        public IActionResult Create(int mid)
        {
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            var jid = manuscript.JournalId;
            if (_context.Submissions.Where(a => a.ManuscriptId == mid).Count() > 0)
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == mid);
                ViewData["sid"] = submission.Id;
            }
            if (manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete)
            {
                return NotFound();
            }


            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == mid),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == mid),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid),

            };
            ViewData["mid"] = mid;
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description");
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == mid), "Id", "ArTitle");
            return View(SubmissionVM);
        }

        // GET: Submissions/Create
        public IActionResult Resubmission(int mid)
        {
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            var jid = manuscript.JournalId;
            ViewData["jid"] = jid;
            if (manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionsBeingProcessed || manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == mid),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == mid),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid),

            };
            ViewData["mid"] = mid;
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description");
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == mid), "Id", "ArTitle");
            return View(SubmissionVM);
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resubmission([Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            var AllPreviousSubmissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId);
            foreach (var item in AllPreviousSubmissions)
            {
                item.SubmissionStatus = SubmissionStatus.NeglectedSubmission;
                _context.Update(item);
            }


            if (ModelState.IsValid)
            {
                submission.DateOfSubmission = DateTime.Now;
                submission.SubmissionStatus = SubmissionStatus.NotSubmitted;
                submission.CoverLetter = submission.CoverLetter.Replace("\n", "<br/>");
                submission.SubmissionStatus = SubmissionStatus.Submitted;

                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction("ResubmissionFiles", "SubmissionFiles", new {/* routeValues, for example: */ sid = submission.Id });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }
        // POST: Submissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                submission.DateOfSubmission = DateTime.Now;
                submission.SubmissionStatus = SubmissionStatus.NotSubmitted;
                if(submission.CoverLetter != null) { 
                submission.CoverLetter = submission.CoverLetter.Replace("\n", "<br/>");
                }
                submission.SubmissionStatus = SubmissionStatus.Submitted;


                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "SubmissionFiles", new {/* routeValues, for example: */ sid = submission.Id });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }

        // GET: Submissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            if ((manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            if (submission != null)
            {
                var sub = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == manuscript.Id && a.SubmissionStatus == SubmissionStatus.Submitted);
                var requiredjournaliles = _context.JournalFileTypes.Include(a => a.FileType).Where(a => a.JournalId == manuscript.JournalId && a.IsRequired == true && a.IsDeleted == false);
                var requiredsubmissionfiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Where(a => a.SubmissionId == sub.Id && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId && a.JournalFileType.FileType != null).Count() > 0);
                if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
                {
                    ViewData["subfiles"] = "true";
                }
                else
                {
                    ViewData["subfiles"] = "false";
                }

            }

            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == submission.ManuscriptId),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == submission.ManuscriptId),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),
                Manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == manuscript.Id)

            };

            if (submission == null)
            {
                return NotFound();
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        public async Task<IActionResult> FinalShow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            if ((manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            if (submission != null)
            {
                var sub = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == manuscript.Id && a.SubmissionStatus == SubmissionStatus.Submitted);
                var requiredjournaliles = _context.JournalFileTypes.Include(a => a.FileType).Where(a => a.JournalId == manuscript.JournalId && a.IsRequired == true && a.IsDeleted == false);
                var requiredsubmissionfiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Where(a => a.SubmissionId == sub.Id && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId && a.JournalFileType.FileType != null).Count() > 0);
                if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
                {
                    ViewData["subfiles"] = "true";
                }
                else
                {
                    ViewData["subfiles"] = "false";
                }

            }

            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.University).Where(a => a.ManuscriptId == submission.ManuscriptId),
                Manuscripts = _context.Manuscripts.Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.Speciality).Include(a => a.SubmittedforIssue).Where(a => a.Id == submission.ManuscriptId),
                SuggestedReviewers = _context.SuggestedReviewers.Include(a => a.ReviewerUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.JournalFileType.FileType).Where(a => a.Submission.ManuscriptId == submission.ManuscriptId),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),
                Manuscript = _context.Manuscripts.Include(a => a.Journal).Include(a => a.JournalArticleType.ArticleType).Include(a => a.Speciality).Include(a => a.SubmittedforIssue).SingleOrDefault(a => a.Id == manuscript.Id)

            };

            if (submission == null)
            {
                return NotFound();
            }
            var requiredjournalfiles = _context.JournalFileTypes.Include(a => a.FileType).Where(a => a.JournalId == manuscript.JournalId && a.IsRequired == true && a.IsDeleted == false);
            var requiredsubmissionfile = _context.SubmissionFiles.Include(a => a.JournalFileType).Where(a => a.SubmissionId == submission.Id && requiredjournalfiles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId && a.JournalFileType.FileType != null).Count() > 0);
            if (_context.Manuscripts.Where(a => a.Id == manuscript.Id).Count() == 0 || _context.ArticleAuthorships.Where(a => a.ManuscriptId == manuscript.Id).Count() == 0 || requiredjournalfiles.Count() < requiredsubmissionfile.Count())
            {
                return NotFound();
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Submissions", new {/* routeValues, for example: */ mid = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }


        // GET: Submissions/Edit/5
        public async Task<IActionResult> EditResubmission(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            if (manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionsBeingProcessed)
            {
                return NotFound();
            }
            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.SubmissionId == submission.Id),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),

            };

            if (submission == null)
            {
                return NotFound();
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResubmission(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditResubmission", "Submissions", new {/* routeValues, for example: */ mid = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }

        // GET: Submissions/EICDecision/5
        public async Task<IActionResult> EICDecision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.Include(a => a.Manuscript.Journal).SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Include(a => a.Manuscript.Journal).Where(a => a.ManuscriptId == submission.ManuscriptId),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == submission.ManuscriptId),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == submission.ManuscriptId),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),
            };

            if (submission == null)
            {
                return NotFound();
            }

            //========================================================================
            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>السلام عليكم.... تحية طيبة </h2>");
            MyStringBuilder.Append("<h2>تقرير مساعد رئيس التحرير</h2><br/>");
            MyStringBuilder.Append("<p>");
            MyStringBuilder.Append(submission.AEDecisionText);
            MyStringBuilder.Append("</p>");

            foreach (var item in _context.SubmissionReviewers.Where(a => a.SubmissionId == submission.Id && a.IsAccess == false))
            {
                MyStringBuilder.Append("<h2>تقرير المحكم</h2><br/>");
                MyStringBuilder.Append("<p>");
                MyStringBuilder.Append(item.ReviewerInstructionsToAuthor);
                MyStringBuilder.Append("</p>");
                if (item.ReviewReportFile != null)
                {
                    MyStringBuilder.Append("<h2>ملف ملاحظات المحكم </h2><br/>");
                    MyStringBuilder.Append("<p>");


                    MyStringBuilder.AppendFormat(string.Format("<a href='https://portal.arid.my/Secured/{0}' >", item.ReviewReportFile));
                    MyStringBuilder.AppendFormat(string.Format("https://portal.arid.my/Secured/{0}", item.ReviewReportFile));
                    MyStringBuilder.Append("</a><hr/>");
                }

                //MyStringBuilder.Append(item.ReviewReportFile);
                //MyStringBuilder.Append("</a>");
                //MyStringBuilder.Append("<hr/>");
            }
            ViewData["eicdecision"] = MyStringBuilder.ToString();
            //========================================================================






            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EICDecision(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submission.DecisionDate = DateTime.Now;
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //===============================================================
                Manuscript manuscript = _context.Manuscripts.Include(a => a.Journal).Include(a => a.CorrespondingAuthor).SingleOrDefault(a => a.Id == submission.ManuscriptId);
                var ae = _context.AreaEditors.Include(a => a.ApplicationUser).SingleOrDefault(a => a.Id == submission.AEId);
                EmailContent acceptcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "8050076f-5efb-4b4d-b6ca-6f0b1abe6df6").SingleOrDefault();
                EmailContent needrevisiontcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "3e88b5a0-5c7d-4ca6-9f89-bfb321279ac7").SingleOrDefault();
                EmailContent RejectWithResubmitcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "036d72f3-ee23-49bd-b7eb-bccc6f8abdf8").SingleOrDefault();
                EmailContent Rejectcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "aea536a2-80a8-4fc2-b69c-e63ec93c5ffc").SingleOrDefault();
                EmailContent RejectWithAppealcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "27ca16e0-30cf-42dc-a4a0-7094860bc20f").SingleOrDefault();
                EmailContent RejectTransfercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "15536cf5-7a99-4e8a-b839-822a8d30d0ce").SingleOrDefault();

                StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
                MyStringBuilder.Append(manuscript.Journal.ArName);
                MyStringBuilder.Append("</h2>");
                MyStringBuilder.Append("<br/>");
                MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", submission.ManuscriptId));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
                MyStringBuilder.Append("</a></h3><hr/>");
                //Accept
                if (submission.EICDecision.ToString() == "Accept")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.Published;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, acceptcontent.ArSubject, acceptcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //Minor-major-LanguageEditing
                if (submission.EICDecision.ToString() == "Minor" || submission.EICDecision.ToString() == "Major" || submission.EICDecision.ToString() == "ReviseForLanguageEditing")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.SubmissionsNeedingRevision;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, needrevisiontcontent.ArSubject, needrevisiontcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //RejectWithResubmit
                if (submission.EICDecision.ToString() == "RejectWithResubmit")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.SubmissionSentBackToAuthor;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, RejectWithResubmitcontent.ArSubject, RejectWithResubmitcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //Reject
                if (submission.EICDecision.ToString() == "Reject")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.Declined;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, Rejectcontent.ArSubject, Rejectcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //RejectWithAppealcontent
                if (submission.EICDecision.ToString() == "RejectWithAppeal")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.Declined;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, RejectWithAppealcontent.ArSubject, RejectWithAppealcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //RejectTransfer
                if (submission.EICDecision.ToString() == "RejectTransfer")
                {
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.Declined;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, RejectTransfercontent.ArSubject, RejectTransfercontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                }
                //===============================================================
                return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }

        // GET: Submissions/EICDecision/5
        public async Task<IActionResult> AEDecision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.Include(a => a.Manuscript.Journal).SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Include(a => a.Manuscript.Journal).Where(a => a.ManuscriptId == submission.ManuscriptId),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == submission.ManuscriptId),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == submission.ManuscriptId),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),
            };

            if (submission == null)
            {
                return NotFound();
            }

            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AEDecision(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submission.AEDecisionDate = DateTime.Now;
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }


        // GET: Submissions/SentBackToAuthor/5
        public async Task<IActionResult> SentBackToAuthor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Single(a => a.Id == submission.ManuscriptId);
            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == submission.ManuscriptId),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == submission.ManuscriptId),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == submission.ManuscriptId),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == submission.ManuscriptId),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == submission.Manuscript.JournalId),
            };


            if (submission == null)
            {
                return NotFound();
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/SentBackToAuthor/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SentBackToAuthor(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();

                    Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == submission.ManuscriptId);
                    if (manuscript == null)
                    {
                        return NotFound();
                    }
                    manuscript.CurrentStatus = ManuscriptCurrentStatus.SubmissionSentBackToAuthor;
                    _context.Update(manuscript);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors, "Id", "Description", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }



        // GET: Submissions/AddAreaEditor/5
        public async Task<IActionResult> AddAreaEditor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var submission = await _context.Submissions.Include(a => a.Manuscript.Journal).SingleOrDefaultAsync(m => m.Id == id);
            ViewData["mid"] = submission.ManuscriptId;
            ViewData["jid"] = submission.Manuscript.JournalId;
            ViewData["sid"] = submission.Id;
            var manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Single(a => a.Id == submission.ManuscriptId);
            var articleauthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == submission.ManuscriptId);
            SubmissionViewModel SubmissionVM = new SubmissionViewModel()
            {
                Submission = await _context.Submissions.Include(a => a.AE.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == submission.ManuscriptId),
            };

            if (submission == null)
            {
                return NotFound();
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors.Include(a => a.ApplicationUser).Where(a => a.JournalId == submission.Manuscript.JournalId && articleauthorships.Where(m => m.AuthorUserId == a.ApplicationUserId).Count() == 0 && a.IsActive == true && a.ApplicationUserId != manuscript.CorrespondingAuthorId), "Id", "ApplicationUser.ArName", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == submission.ManuscriptId), "Id", "ArTitle", submission.ManuscriptId);
            return View(SubmissionVM);
        }

        // POST: Submissions/AddAreaEditor/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAreaEditor(int id, [Bind("Id,ManuscriptId,AEId,DateOfSubmission,ResponsetoDecision,EICDecision,SubmissionStatus,DecisionDate,EicDecisionText,DecisionLetter,PlagiarismConfirmation,DuplicateSubmissionConfirmation,CoverLetter,AEDecision,AEDecisionDate,AEDecisionText")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //===============================================================
                var manuscript = _context.Manuscripts.Include(a => a.Journal).Include(a => a.CorrespondingAuthor).SingleOrDefault(a => a.Id == submission.ManuscriptId);
                var ae = _context.AreaEditors.Include(a => a.ApplicationUser).SingleOrDefault(a => a.Id == submission.AEId);
                EmailContent aecontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "39c244a0-21f3-4b1e-b113-a9cc5b1bce4f").SingleOrDefault();

                StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
                MyStringBuilder.Append(manuscript.Journal.ArName);
                MyStringBuilder.Append("</h2>");
                MyStringBuilder.Append("<br/>");
                MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", submission.ManuscriptId));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
                MyStringBuilder.Append("</a></h3><hr/>");

                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(ae.ApplicationUser.Email, aecontent.ArSubject, aecontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
                //===============================================================




                return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = submission.ManuscriptId });
            }
            ViewData["AEId"] = new SelectList(_context.AreaEditors.Include(a => a.ApplicationUser).Where(a => a.JournalId == submission.Manuscript.JournalId), "Id", "ApplicationUser.ArName", submission.AEId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", submission.ManuscriptId);
            return View(submission);
        }

        // GET: Submissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .Include(s => s.AE)
                .Include(s => s.Manuscript)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.Submissions.SingleOrDefaultAsync(m => m.Id == id);
            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction("Create", "Submissions", new {/* routeValues, for example: */ mid = submission.ManuscriptId });
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.Id == id);
        }

        public IActionResult AEDelete(int id)
        {
            Submission submission = _context.Submissions.SingleOrDefault(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }
            submission.AEId = null;
            submission.AEDecision = 0;
            submission.AEDecisionDate = null;
            submission.AEDecisionText = null;
            _context.Update(submission);
            _context.SaveChanges();

            return RedirectToAction("AddAreaEditor", "Submissions", new {/* routeValues, for example: */ id = submission.Id });
        }

        public IActionResult ReviewerDelete(int id)
        {
            SubmissionReviewer submissionReviewer = _context.SubmissionReviewers.Include(a => a.Submission).SingleOrDefault(m => m.Id == id);
            if (submissionReviewer == null)
            {
                return NotFound();
            }
            _context.SubmissionReviewers.Remove(submissionReviewer);
            _context.SaveChanges();

            return RedirectToAction("Details", "Submissions", new {/* routeValues, for example: */ id = submissionReviewer.SubmissionId });
        }

    }
}