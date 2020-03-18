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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using ARID.Services;
using System.Text;

namespace ARID.Controllers
{
    [Authorize]
    public class ManuscriptsController : Controller
    {

        private IHostingEnvironment _environment;
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ManuscriptsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment, IEmailSender emailSender)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> TechnicalEditorIndex(int id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);
            ViewData["jsn"] = journal.ShortName;
            var aRIDDbContext = _context.Manuscripts.Include(a => a.SubmittedforIssue).Where(a => a.JournalId == id && a.CurrentStatus == ManuscriptCurrentStatus.Published).Include(p => p.Journal).Include(p => p.CorrespondingAuthor);
            return View(await aRIDDbContext.ToListAsync());
        }


        // GET: Manuscripts
        public async Task<IActionResult> Index(string id, int j, int jid)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == jid);

            ViewData["EiC"] = journal.EiCId;
            ViewData["mcs"] = id;
            ViewData["jshortname"] = journal.ShortName;
            ViewData["journalid"] = jid;
            ViewData["j"] = j;
            ViewData["eicid"] = journal.EiCId;
            ManuscriptViewModel ManuscriptVM = new ManuscriptViewModel();
            var currentuserarticleauthoships = _context.ArticleAuthorships.Where(a => a.AuthorUserId == _userManager.GetUserId(User));

            if (j == 1 && journal.EiCId == _userManager.GetUserId(User))
            {
                ManuscriptVM = new ManuscriptViewModel()
                {
                    CoAuthorManuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId != _userManager.GetUserId(User) && currentuserarticleauthoships.Where(m => m.ManuscriptId == a.Id && m.AuthorUserId == _userManager.GetUserId(User)).Count() > 0),
                    SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission.Manuscript).Include(a => a.ReviewerUser).Where(a => a.Submission.Manuscript.JournalId == journal.Id),
                    Submissions = _context.Submissions.Include(a => a.AE.ApplicationUser).Include(a => a.Manuscript).Where(a => a.Manuscript.JournalId == journal.Id),
                    Manuscripts = _context.Manuscripts.Where(a => a.CurrentStatus.ToString() == id && a.JournalId == jid && a.IsDeleted == false).Include(m => m.CorrespondingAuthor).Include(m => m.Journal).Include(m => m.JournalArticleType).Include(m => m.JournalArticleType.ArticleType).Include(m => m.Speciality).Include(m => m.SubmittedforIssue).OrderByDescending(a => a.CreationDate),
                    ManuscriptsManuscripts = _context.Manuscripts.Where(a => a.JournalId == jid && a.IsDeleted == false).Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.CorrespondingAuthor),
                };
            }
            else if (id == "CoAuthor")
            {
                ManuscriptVM = new ManuscriptViewModel()
                {
                    CoAuthorManuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId != _userManager.GetUserId(User) && currentuserarticleauthoships.Where(m => m.ManuscriptId == a.Id && m.AuthorUserId == _userManager.GetUserId(User)).Count() > 0),
                    ManuscriptsManuscripts = _context.Manuscripts.Where(a => a.JournalId == jid && a.CorrespondingAuthorId == _userManager.GetUserId(User) && a.IsDeleted == false).Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.CorrespondingAuthor),
                    Manuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId != _userManager.GetUserId(User) && currentuserarticleauthoships.Where(m => m.ManuscriptId == a.Id && m.AuthorUserId == _userManager.GetUserId(User)).Count() > 0).Include(m => m.Journal).Include(m => m.JournalArticleType).Include(m => m.JournalArticleType.ArticleType).Include(m => m.Speciality).Include(m => m.SubmittedforIssue).OrderByDescending(a => a.CreationDate),
                };
            }
            else
            {
                ManuscriptVM = new ManuscriptViewModel()
                {
                    CoAuthorManuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId != _userManager.GetUserId(User) && currentuserarticleauthoships.Where(m => m.ManuscriptId == a.Id && m.AuthorUserId == _userManager.GetUserId(User)).Count() > 0),
                    ManuscriptsManuscripts = _context.Manuscripts.Where(a => a.JournalId == jid && a.CorrespondingAuthorId == _userManager.GetUserId(User) && a.IsDeleted == false).Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.CorrespondingAuthor),
                    Manuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId == _userManager.GetUserId(User) && a.CurrentStatus.ToString() == id && a.JournalId == jid && a.IsDeleted == false).Include(m => m.CorrespondingAuthor).Include(m => m.Journal).Include(m => m.JournalArticleType).Include(m => m.JournalArticleType.ArticleType).Include(m => m.Speciality).Include(m => m.SubmittedforIssue).OrderByDescending(a => a.CreationDate),
                };
            }
            return View(ManuscriptVM);
        }

        public async Task<IActionResult> DeleteIndex(int id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["EiC"] = journal.EiCId;
            ViewData["jid"] = journal.Id;
            if (_userManager.GetUserId(User) != journal.EiCId)
            {
                return NotFound();
            }

            ManuscriptViewModel ManuscriptVM = new ManuscriptViewModel()
            {
                SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission.Manuscript).Include(a => a.ReviewerUser).Where(a => a.Submission.Manuscript.JournalId == id),
                Submissions = _context.Submissions.Include(a => a.AE.ApplicationUser).Include(a => a.Manuscript).Where(a => a.Manuscript.JournalId == id),
                Manuscripts = _context.Manuscripts.Where(a => a.IsDeleted == true && a.JournalId == id).Include(m => m.CorrespondingAuthor).Include(m => m.Journal).Include(m => m.JournalArticleType).Include(m => m.JournalArticleType.ArticleType).Include(m => m.Speciality).Include(m => m.SubmittedforIssue).OrderByDescending(a => a.CreationDate),
            };

            return View(ManuscriptVM);
        }


        public async Task<IActionResult> AEManuscripts(string id)
        {
            ViewData["jshortname"] = id;
            IEnumerable<Submission> aRIDDbContext;
            aRIDDbContext = _context.Submissions.Where(a => a.AE.ApplicationUserId == _userManager.GetUserId(User) && a.SubmissionStatus == SubmissionStatus.Submitted).Include(m => m.Manuscript).Include(m => m.Manuscript.CorrespondingAuthor).Include(m => m.Manuscript.Journal).Include(m => m.Manuscript.JournalArticleType).Include(m => m.Manuscript.JournalArticleType.ArticleType).Include(m => m.Manuscript.Speciality).Include(m => m.Manuscript.SubmittedforIssue);
            return View(aRIDDbContext);
        }

        public async Task<IActionResult> RevManuscripts(int id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);
            ViewData["jshortname"] = journal.ShortName;
            IEnumerable<SubmissionReviewer> aRIDDbContext;
            aRIDDbContext = _context.SubmissionReviewers.Where(a => a.ReviewerUserId == _userManager.GetUserId(User) && a.Submission.Manuscript.JournalId == id).Include(a => a.Submission.Manuscript.Journal).Include(a => a.Submission.Manuscript.Journal).Include(a => a.Submission.Manuscript.JournalArticleType).Include(a => a.Submission.Manuscript.JournalArticleType.ArticleType).Include(a => a.Submission.Manuscript.Speciality).Include(a => a.Submission.Manuscript.SubmittedforIssue);
            return View(aRIDDbContext);
        }

        // GET: Manuscripts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ManuscriptViewModel manuscriptVM = new ManuscriptViewModel();

            if (_context.Submissions.Where(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted).Count() > 0)
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                var manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == id);
                manuscriptVM = new ManuscriptViewModel()
                {
                    Submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted),
                    Manuscript = await _context.Manuscripts
                   .Include(m => m.CorrespondingAuthor)
                   .Include(m => m.Journal)
                   .Include(m => m.JournalArticleType)
                   .Include(m => m.JournalArticleType.ArticleType)
                   .Include(m => m.Speciality)
                   .Include(m => m.SubmittedforIssue)
                   .SingleOrDefaultAsync(m => m.Id == id),
                    SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission).Include(a => a.Submission.Manuscript).Include(a => a.ReviewerUser).Include(a => a.CieAeUser).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.ReviewDate),
                    ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.University).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfRecord),
                    SuggestedReviewers = _context.SuggestedReviewers.Include(a => a.ReviewerUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.Id),
                    Submissions = _context.Submissions.Include(a => a.AE).Include(a => a.AE.ApplicationUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfSubmission),
                    SubmissionFiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.Id),
                    AreaEditors = _context.AreaEditors.Include(a => a.Journal).Where(a => a.JournalId == manuscript.JournalId).OrderBy(a => a.Id),
                };
                var sub = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                ViewData["sid"] = sub.Id;

            }
            else
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);

                manuscriptVM = new ManuscriptViewModel()
                {
                    Manuscript = await _context.Manuscripts
                   .Include(m => m.CorrespondingAuthor)
                   .Include(m => m.Journal)
                   .Include(m => m.JournalArticleType)
                   .Include(m => m.Speciality)
                   .Include(m => m.SubmittedforIssue)
                   .SingleOrDefaultAsync(m => m.Id == id),
                };
            }
            ViewData["mid"] = id;
            var newmanuscript = _context.Manuscripts.Include(a => a.Journal.EiC).SingleOrDefault(a => a.Id == id);
            if (newmanuscript.Journal.EiCId != _userManager.GetUserId(User) && _context.SubmissionReviewers.Where(a => a.ReviewerUserId == _userManager.GetUserId(User)).Count() == 0 && _context.Submissions.Include(a => a.AE.ApplicationUser).Where(a => a.ManuscriptId == id && a.AE.ApplicationUserId == _userManager.GetUserId(User)).Count() == 0 && newmanuscript.CorrespondingAuthorId != _userManager.GetUserId(User) && _context.ArticleAuthorships.Where(a => a.ManuscriptId == id && a.AuthorUserId == _userManager.GetUserId(User)).Count() == 0)
            {
                return NotFound();
            }
            return View(manuscriptVM);
        }


        // GET: Manuscripts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Published(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ManuscriptViewModel manuscriptVM = new ManuscriptViewModel();

            if (_context.Submissions.Where(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted).Count() > 0)
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                var manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == id);
                manuscriptVM = new ManuscriptViewModel()
                {
                    Submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted),
                    Manuscript = await _context.Manuscripts
                   .Include(m => m.CorrespondingAuthor)
                   .Include(m => m.Journal)
                   .Include(m => m.JournalArticleType)
                   .Include(m => m.JournalArticleType.ArticleType)
                   .Include(m => m.Speciality)
                   .Include(m => m.SubmittedforIssue)
                   .SingleOrDefaultAsync(m => m.Id == id),
                    SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission).Include(a => a.Submission.Manuscript).Include(a => a.ReviewerUser).Include(a => a.CieAeUser).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.ReviewDate),
                    ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.University).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfRecord),
                    SuggestedReviewers = _context.SuggestedReviewers.Include(a => a.ReviewerUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.Id),
                    Submissions = _context.Submissions.Include(a => a.AE).Include(a => a.AE.ApplicationUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfSubmission),
                    SubmissionFiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.Id),
                    AreaEditors = _context.AreaEditors.Include(a => a.Journal).Where(a => a.JournalId == manuscript.JournalId).OrderBy(a => a.Id),
                };
                var sub = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                ViewData["sid"] = sub.Id;

            }
            else
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);

                manuscriptVM = new ManuscriptViewModel()
                {
                    Manuscript = await _context.Manuscripts
                   .Include(m => m.CorrespondingAuthor)
                   .Include(m => m.Journal)
                   .Include(m => m.JournalArticleType)
                   .Include(m => m.Speciality)
                   .Include(m => m.SubmittedforIssue)
                   .SingleOrDefaultAsync(m => m.Id == id),
                };
            }
            ViewData["mid"] = id;
            var newmanuscript = _context.Manuscripts.Include(a => a.Journal.EiC).SingleOrDefault(a => a.Id == id);
           
            return View(manuscriptVM);
        }

        // GET: Manuscripts/Details/5
        [Authorize]
        public async Task<IActionResult> TechnicalEditorDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ManuscriptViewModel manuscriptVM = new ManuscriptViewModel();

            var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
            var manuscript = _context.Manuscripts.Include(a => a.Journal).SingleOrDefault(a => a.Id == id);
            manuscriptVM = new ManuscriptViewModel()
            {
                Submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted),
                Manuscript = await _context.Manuscripts
               .Include(m => m.CorrespondingAuthor)
               .Include(m => m.Journal)
               .Include(m => m.JournalArticleType)
               .Include(m => m.JournalArticleType.ArticleType)
               .Include(m => m.Speciality)
               .Include(m => m.SubmittedforIssue)
               .SingleOrDefaultAsync(m => m.Id == id),
                //SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission).Include(a => a.Submission.Manuscript).Include(a => a.ReviewerUser).Include(a => a.CieAeUser).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.ReviewDate),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.University).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfRecord),
                //SuggestedReviewers = _context.SuggestedReviewers.Include(a => a.ReviewerUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.Id),
                //Submissions = _context.Submissions.Include(a => a.AE).Include(a => a.AE.ApplicationUser).Where(a => a.ManuscriptId == id).OrderBy(a => a.DateOfSubmission),
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == submission.Id).OrderBy(a => a.Id),
                AreaEditors = _context.AreaEditors.Include(a => a.Journal).Where(a => a.JournalId == manuscript.JournalId).OrderBy(a => a.Id),
            };
            Manuscript Manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == submission.ManuscriptId);
            if (_context.SubmissionFiles.Where(a => a.SubmissionId == submission.Id && a.JournalFileType.FileTypeId == 3).Count() > 0 && _context.SubmissionFiles.Where(a => a.SubmissionId == submission.Id && a.JournalFileType.FileTypeId == 4).Count() > 0)
            {
                Manuscript.CurrentStatus = ManuscriptCurrentStatus.GalleyProof;
                _context.Update(Manuscript);
                _context.SaveChanges();
            }
            else
            {
                Manuscript.CurrentStatus = ManuscriptCurrentStatus.Published;
                _context.Update(Manuscript);
                _context.SaveChanges();
            }
            ViewData["sid"] = submission.Id;
            ViewData["mid"] = id;
            ViewData["jid"] = manuscript.JournalId;

            if (_context.AreaEditors.Where(a => a.RoleId == Role.TechnicalEditor && a.JournalId == manuscript.JournalId && a.ApplicationUserId == _userManager.GetUserId(User)).Count() == 0)
            {
                return NotFound();
            }
            return View(manuscriptVM);
        }
        // GET: Manuscripts/Create
        public IActionResult Create(int jid)
        {
            ViewData["jid"] = jid;
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName");
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == jid && a.IsDeleted == false), "Id", "ArticleType.Type");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues.Where(a => a.IsOpen == true && a.JournalId == jid), "Id", "Name");
            return View();
        }

        // POST: Manuscripts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArTitle,EnTitle,RunningTitle,EnAbstract,ArAbstract,CorrespondingAuthorId,CreationDate,DueDateforAuthorResponse,Keywords,Areas,JournalArticleTypeId,SubmittedforIssueId,SpecialityId,JournalId,CurrentStatus,IsDeleted,OpenAccess,FundingInfo,GraphicalAbstract")] Manuscript manuscript, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                manuscript.ArAbstract = manuscript.ArAbstract.Replace("\n", "<br/>");
                manuscript.EnAbstract = manuscript.EnAbstract.Replace("\n", "<br/>");
                manuscript.GraphicalAbstract = await UserFile.UploadeNewImageAsync(manuscript.GraphicalAbstract,
    myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);
                manuscript.CorrespondingAuthorId = _userManager.GetUserId(User);
                manuscript.CreationDate = DateTime.Now;
                manuscript.CurrentStatus = ManuscriptCurrentStatus.InComplete;
                _context.Add(manuscript);

                //هنا اضافة المؤلف المعني التواصل بشكل غير مباشر
                var currentuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));
                _context.ArticleAuthorships.Add(new ArticleAuthorship
                {
                    Email = currentuser.Email,
                    CountryId = currentuser.CountryId,
                    UniversityId = currentuser.UniversityId,
                    ManuscriptId = manuscript.Id,
                    AuthorUserId = _userManager.GetUserId(User),
                    DateOfRecord = DateTime.Now,
                    Indx = 1,
                    Contribution = "المؤلف المعني بالتواصل"
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "ArticleAuthorships", new {/* routeValues, for example: */ mid = manuscript.Id });
            }
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", manuscript.CorrespondingAuthorId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", manuscript.JournalId);
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes, "Id", "Id", manuscript.JournalArticleTypeId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues, "Id", "Name", manuscript.SubmittedforIssueId);
            return View(manuscript);
        }

        // GET: Manuscripts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var manuscript = await _context.Manuscripts.SingleOrDefaultAsync(m => m.Id == id);
            var jid = manuscript.JournalId;
            ViewData["jid"] = manuscript.JournalId;
            if (_context.Submissions.Where(a => a.ManuscriptId == id).Count() > 0)
            {
                var sub = await _context.Submissions.SingleOrDefaultAsync(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                ViewData["sid"] = sub.Id;
            }
            var submission = await _context.Submissions.SingleOrDefaultAsync(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);

            ViewData["mid"] = id;
            ManuscriptViewModel ManuscriptVM = new ManuscriptViewModel()
            {
                Manuscript = await _context.Manuscripts.SingleOrDefaultAsync(m => m.Id == id),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == id),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == id),
                ArticleAuthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == id),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == id),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid),

            };

            if (id == null)
            {
                return NotFound();
            }
            if (submission != null)
            {
                var requiredjournaliles = _context.JournalFileTypes.Where(a => a.JournalId == jid && a.IsRequired == true && a.IsDeleted == false);
                var requiredsubmissionfiles = _context.SubmissionFiles.Where(a => a.SubmissionId == submission.Id && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId).Count() > 0);
                if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
                {
                    ViewData["subfiles"] = "true";
                }
                else
                {
                    ViewData["subfiles"] = "false";
                }

            }
            if (manuscript == null || (manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", manuscript.CorrespondingAuthorId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", manuscript.JournalId);
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == manuscript.JournalId && a.IsDeleted == false), "Id", "ArticleType.Type", manuscript.JournalArticleTypeId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues, "Id", "Name", manuscript.SubmittedforIssueId);
            return View(ManuscriptVM);
        }

        // POST: Manuscripts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArTitle,EnTitle,RunningTitle,EnAbstract,ArAbstract,CorrespondingAuthorId,CreationDate,DueDateforAuthorResponse,Keywords,Areas,JournalArticleTypeId,SubmittedforIssueId,SpecialityId,JournalId,CurrentStatus,IsDeleted,OpenAccess,FundingInfo,GraphicalAbstract")] Manuscript manuscript, IFormFile myfile)
        {
            if (id != manuscript.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    manuscript.GraphicalAbstract = await UserFile.UploadeNewImageAsync(manuscript.GraphicalAbstract,
    myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                    _context.Update(manuscript);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManuscriptExists(manuscript.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.Id });
            }
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", manuscript.CorrespondingAuthorId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", manuscript.JournalId);
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes, "Id", "Id", manuscript.JournalArticleTypeId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues, "Id", "Name", manuscript.SubmittedforIssueId);
            return View(manuscript);
        }
        // GET: Manuscripts/Edit/5
        public async Task<IActionResult> TechnicalEditorEdit(int? id)
        {
            var manuscript = await _context.Manuscripts.SingleOrDefaultAsync(m => m.Id == id);
            var jid = manuscript.JournalId;
            ViewData["jid"] = manuscript.JournalId;
            if (_context.Submissions.Where(a => a.ManuscriptId == id).Count() > 0)
            {
                var sub = await _context.Submissions.SingleOrDefaultAsync(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);
                ViewData["sid"] = sub.Id;
            }
            var submission = await _context.Submissions.SingleOrDefaultAsync(a => a.ManuscriptId == id && a.SubmissionStatus == SubmissionStatus.Submitted);

            ViewData["mid"] = id;
            ManuscriptViewModel ManuscriptVM = new ManuscriptViewModel()
            {
                Manuscript = await _context.Manuscripts.SingleOrDefaultAsync(m => m.Id == id),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == id),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == id),
                ArticleAuthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == id),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == id),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == id),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid),

            };

            if (id == null)
            {
                return NotFound();
            }
            if (submission != null)
            {
                var requiredjournaliles = _context.JournalFileTypes.Where(a => a.JournalId == jid && a.IsRequired == true && a.IsDeleted == false);
                var requiredsubmissionfiles = _context.SubmissionFiles.Where(a => a.SubmissionId == submission.Id && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId).Count() > 0);
                if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
                {
                    ViewData["subfiles"] = "true";
                }
                else
                {
                    ViewData["subfiles"] = "false";
                }

            }
            if (manuscript == null || _context.AreaEditors.Where(a => a.JournalId == manuscript.JournalId && a.ApplicationUserId == _userManager.GetUserId(User)).Count() == 0)
            {
                return NotFound();
            }
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", manuscript.CorrespondingAuthorId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", manuscript.JournalId);
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == manuscript.JournalId && a.IsDeleted == false), "Id", "ArticleType.Type", manuscript.JournalArticleTypeId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues, "Id", "Name", manuscript.SubmittedforIssueId);
            return View(ManuscriptVM);
        }

        // POST: Manuscripts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TechnicalEditorEdit(int id, [Bind("Id,ArTitle,EnTitle,RunningTitle,EnAbstract,ArAbstract,CorrespondingAuthorId,CreationDate,DueDateforAuthorResponse,Keywords,Areas,JournalArticleTypeId,SubmittedforIssueId,SpecialityId,JournalId,CurrentStatus,IsDeleted,OpenAccess,FundingInfo,GraphicalAbstract")] Manuscript manuscript, IFormFile myfile)
        {
            if (id != manuscript.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    manuscript.GraphicalAbstract = await UserFile.UploadeNewImageAsync(manuscript.GraphicalAbstract,
    myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                    _context.Update(manuscript);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManuscriptExists(manuscript.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("TechnicalEditorDetails", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.Id });
            }
            ViewData["CorrespondingAuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", manuscript.CorrespondingAuthorId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", manuscript.JournalId);
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes, "Id", "Id", manuscript.JournalArticleTypeId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null && _context.JournalSpecialities.Include(m => m.Speciality).Where(m => m.Speciality.Name == a.Name).Count() > 0), "Id", "Name");
            ViewData["SubmittedforIssueId"] = new SelectList(_context.JournalIssues, "Id", "Name", manuscript.SubmittedforIssueId);
            return View(manuscript);
        }

        // GET: Manuscripts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manuscript = await _context.Manuscripts
                .Include(m => m.CorrespondingAuthor)
                .Include(m => m.Journal)
                .Include(m => m.JournalArticleType)
                .Include(m => m.Speciality)
                .Include(m => m.SubmittedforIssue)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }

            return View(manuscript);
        }

        // POST: Manuscripts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manuscript = await _context.Manuscripts.SingleOrDefaultAsync(m => m.Id == id);
            _context.Manuscripts.Remove(manuscript);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManuscriptExists(int id)
        {
            return _context.Manuscripts.Any(e => e.Id == id);
        }
        [Authorize]
        public IActionResult ResearchSubmission(int? id)
        {
            Manuscript manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Include(a => a.Journal).SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.Submitted;

            EmailContent CorrespondingAuthorcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "5aabbc22-bbbc-4bad-b08a-75c0fa6f3496").SingleOrDefault();
            EmailContent otherAuthorscontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "0a6caacb-e836-41c7-ad07-0e7c4e700741").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, CorrespondingAuthorcontent.ArSubject, CorrespondingAuthorcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            foreach (var item in _context.ArticleAuthorships.Where(a => a.ManuscriptId == id && a.AuthorUserId != manuscript.CorrespondingAuthorId))
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.Email, otherAuthorscontent.ArSubject, otherAuthorscontent.ArContent), TimeSpan.FromSeconds(10));
            }
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Details", "Journals", new {/* routeValues, for example: */ id = manuscript.JournalId });
            //return RedirectToAction("Details/" + id);
        }
        public IActionResult TechniclaEditorSubmission(int? id)
        {
            Manuscript manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Include(a => a.Journal).SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.GalleyProof;

            //EmailContent CorrespondingAuthorcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "5aabbc22-bbbc-4bad-b08a-75c0fa6f3496").SingleOrDefault();
            //EmailContent otherAuthorscontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "0a6caacb-e836-41c7-ad07-0e7c4e700741").SingleOrDefault();

            //StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            //MyStringBuilder.Append(manuscript.Journal.ArName);
            //MyStringBuilder.Append("</h2>");
            //MyStringBuilder.Append("<br/>");
            //MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            //MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            //MyStringBuilder.Append("</a></h3><hr/>");

            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.CorrespondingAuthor.Email, CorrespondingAuthorcontent.ArSubject, CorrespondingAuthorcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            //foreach (var item in _context.ArticleAuthorships.Where(a => a.ManuscriptId == id && a.AuthorUserId != manuscript.CorrespondingAuthorId))
            //{
            //    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.Email, otherAuthorscontent.ArSubject, otherAuthorscontent.ArContent), TimeSpan.FromSeconds(10));
            //}
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Home", "Journals", new {/* routeValues, for example: */ id = manuscript.Journal.ShortName });
            //return RedirectToAction("Details/" + id);
        }
        [Authorize]
        public IActionResult Reject(int id)
        {
            Manuscript manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Include(a => a.Journal).SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.Declined;
            _context.Update(manuscript);
            _context.SaveChanges();
            //======================================================================
            EmailContent rejectcontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "19abcff8-a942-402e-8b9b-3a1a26b45250").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            foreach (var item in _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == manuscript.Id))
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.AuthorUser.Email, rejectcontent.ArSubject, rejectcontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            }
            //======================================================================



            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.Id });
        }
        [Authorize]
        public IActionResult removedeclined(int id)
        {
            Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.Submitted;
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.Id });
        }
        public IActionResult sentbacktoauthor(int id)
        {
            Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.SubmissionsNeedingRevision;
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Details", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.Id });
        }
        public IActionResult Agreetopublish(int id)
        {
            Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.GalleyProof;
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Index", "Manuscripts", new {/* routeValues, for example: */ jid = manuscript.JournalId });
        }

        public IActionResult RejectEditing(int id)
        {
            var submission = _context.Submissions.Include(a => a.Manuscript).Include(a => a.AE.ApplicationUser).SingleOrDefault(a => a.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Include(a => a.Journal.EiC).SingleOrDefault(m => m.Id == submission.ManuscriptId);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.AuthorRejectedEditings;
            _context.Update(manuscript);
            _context.SaveChanges();
            //============================================================

            EmailContent eiccontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "a5f97cf6-11fd-48e2-ac1f-e17fac395c36").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>المؤلف المعني بالتواصل ");
            MyStringBuilder.Append(manuscript.CorrespondingAuthor.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.Journal.EiC.Email, eiccontent.ArSubject, eiccontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            if (submission.AEId != null)
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(submission.AE.ApplicationUser.Email, eiccontent.ArSubject, eiccontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            }

            //============================================================
            return RedirectToAction("Index", "Manuscripts", new {/* routeValues, for example: */ jid = manuscript.JournalId, id = manuscript.CurrentStatus.ToString() });
        }
        public IActionResult AcceptEditing(int id)
        {
            var submission = _context.Submissions.Include(a => a.Manuscript).Include(a => a.AE.ApplicationUser).SingleOrDefault(a => a.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a => a.CorrespondingAuthor).Include(a => a.Journal.EiC).SingleOrDefault(m => m.Id == submission.ManuscriptId);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CurrentStatus = ManuscriptCurrentStatus.SubmissionsBeingProcessed;
            _context.Update(manuscript);
            _context.SaveChanges();


            //============================================================

            EmailContent eiccontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "6702ae85-3840-4f03-b0b1-d259f3982d13").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<h2 align='right'>المؤلف المعني بالتواصل ");
            MyStringBuilder.Append(manuscript.CorrespondingAuthor.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Details/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(manuscript.Journal.EiC.Email, eiccontent.ArSubject, eiccontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            if (submission.AEId != null)
            {
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(submission.AE.ApplicationUser.Email, eiccontent.ArSubject, eiccontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));
            }

            //============================================================

            //          < a onclick = "javascript:return confirm('هذا الاجراء لايمكن التراجع عنه ، هل تود الاستمرار بهذه العملية؟');" asp - action = "Index" asp - controller = "Manuscripts" asp - route - id = "Inclomplete" asp - route - jid = "@Model.Manuscript.JournalId" class="btn btn-success bold col-lg-12" id="acceptediting">الموافقة على التعديلات</a><br /><br />

            return RedirectToAction("Index", "Manuscripts", new {/* routeValues, for example: */ jid = manuscript.JournalId, id = manuscript.CurrentStatus.ToString() });
        }
        public IActionResult IsDelete(int id, string cs)
        {
            Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.IsDeleted = true;
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("Index", "Manuscripts", new {/* routeValues, for example: */id = cs, jid = manuscript.JournalId, j = 1 });
        }
        public IActionResult UnDelete(int id)
        {
            Manuscript manuscript = _context.Manuscripts.SingleOrDefault(m => m.Id == id);
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.IsDeleted = false;
            _context.Update(manuscript);
            _context.SaveChanges();

            return RedirectToAction("DeleteIndex", "Manuscripts", new {/* routeValues, for example: */ id = manuscript.JournalId });
        }

        public IActionResult DeleteGallyproofFile(int id)
        {
            SubmissionFile submissionFile = _context.SubmissionFiles.Include(a => a.Submission).SingleOrDefault(m => m.Id == id);
            var sub = _context.Submissions.Include(a => a.Manuscript).SingleOrDefault(m => m.Id == submissionFile.SubmissionId);
            var manuscriptId = sub.ManuscriptId;
            if (submissionFile == null)
            {
                return NotFound();
            }
            _context.SubmissionFiles.Remove(submissionFile);
            _context.SaveChanges();

            return RedirectToAction("TechnicalEditorDetails", "Manuscripts", new {/* routeValues, for example: */ id = manuscriptId });
        }

    }
}