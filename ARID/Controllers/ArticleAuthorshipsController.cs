using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models.AccountViewModels;
using ARID.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Hangfire;
using ARID.Services;

namespace ARID.Controllers
{
    public class ArticleAuthorshipsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;


        public ArticleAuthorshipsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userMrg;
            _emailSender = emailSender;

        }

        // GET: ArticleAuthorships
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.Manuscript).Include(a => a.University);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: ArticleAuthorships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleAuthorship = await _context.ArticleAuthorships
                .Include(a => a.AuthorUser)
                .Include(a => a.Country)
                .Include(a => a.Manuscript)
                .Include(a => a.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (articleAuthorship == null)
            {
                return NotFound();
            }

            return View(articleAuthorship);
        }

        // GET: ArticleAuthorships/Create
        public IActionResult Create(int mid, string ss)
        {
            ViewData["mid"] = mid;
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            if (_context.Submissions.Where(a => a.ManuscriptId == mid).Count() > 0)
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == mid && a.SubmissionStatus==SubmissionStatus.Submitted);
                ViewData["sid"] = submission.Id;
            }
            if ((manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || manuscript.CorrespondingAuthorId !=_userManager.GetUserId(User))
            {
                return NotFound();
            }
            var sub = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == manuscript.Id && a.SubmissionStatus == SubmissionStatus.Submitted);

            if (sub != null)
            {
                var requiredjournaliles = _context.JournalFileTypes.Where(a => a.JournalId == manuscript.JournalId && a.IsRequired == true);
                var requiredsubmissionfiles = _context.SubmissionFiles.Where(a => a.SubmissionId == sub.Id && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId).Count() > 0);
                if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
                {
                    ViewData["subfiles"] = "true";
                }
                else
                {
                    ViewData["subfiles"] = "false";
                }
            }

            ArticleAuthorshipViewModel ArticleAuthorshipVM = new ArticleAuthorshipViewModel()
            {
                //الاشخاص المجلوبين عند البحث
                ApplicationUsers = _context.ApplicationUsers.Include(a => a.Country).Include(a => a.University).Where(a => a.ArName.Contains(ss) || a.EnName.Contains(ss) || a.Email.Contains(ss) || a.Id.Contains(ss) || a.UserName.Contains(ss)),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Include(a => a.Country).Include(a => a.University).Where(a => a.ManuscriptId == mid).OrderBy(a => a.Indx),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == mid),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == mid),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == manuscript.JournalId),
                Manuscript=_context.Manuscripts.Include(a=>a.CorrespondingAuthor).SingleOrDefault(a=>a.Id==mid)
                
            };
            ViewData["AuthorUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == mid), "Id", "ArTitle");
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");

            return View(ArticleAuthorshipVM);
        }

        // POST: ArticleAuthorships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ManuscriptId,CountryId,UniversityId,AuthorUserId,DateOfRecord,Indx,Email,Contribution")] ArticleAuthorship articleAuthorship)
        {
            if (ModelState.IsValid)
            {
                articleAuthorship.Contribution = articleAuthorship.Contribution.Replace("\n", "<br/>");
                articleAuthorship.DateOfRecord = DateTime.Now;

                _context.Add(articleAuthorship);



                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "ArticleAuthorships", new {/* routeValues, for example: */ mid = articleAuthorship.ManuscriptId });
            }
            ViewData["AuthorUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", articleAuthorship.AuthorUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", articleAuthorship.CountryId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", articleAuthorship.ManuscriptId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", articleAuthorship.UniversityId);
            return View(articleAuthorship);
        }

        // GET: ArticleAuthorships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleAuthorship = await _context.ArticleAuthorships.Include(a => a.Manuscript).Include(a=>a.Manuscript.CorrespondingAuthor).SingleOrDefaultAsync(m => m.Id == id);
            if (articleAuthorship == null)
            {
                return NotFound();
            }
            if ((articleAuthorship.Manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && articleAuthorship.Manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || articleAuthorship.Manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            ViewData["AuthorUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == articleAuthorship.AuthorUserId), "Id", "ArName", articleAuthorship.AuthorUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", articleAuthorship.CountryId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a => a.Id == articleAuthorship.ManuscriptId), "Id", "ArTitle", articleAuthorship.ManuscriptId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", articleAuthorship.UniversityId);
            return View(articleAuthorship);
        }

        // POST: ArticleAuthorships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ManuscriptId,CountryId,UniversityId,AuthorUserId,DateOfRecord,Indx,Email,Contribution")] ArticleAuthorship articleAuthorship)
        {
            if (id != articleAuthorship.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articleAuthorship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleAuthorshipExists(articleAuthorship.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Create", "ArticleAuthorships", new {/* routeValues, for example: */ mid = articleAuthorship.ManuscriptId });
            }
            ViewData["AuthorUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", articleAuthorship.AuthorUserId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", articleAuthorship.CountryId);
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", articleAuthorship.ManuscriptId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", articleAuthorship.UniversityId);
            return View(articleAuthorship);
        }

        // GET: ArticleAuthorships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleAuthorship = await _context.ArticleAuthorships
                .Include(a => a.AuthorUser)
                .Include(a => a.Country)
                .Include(a => a.Manuscript)
                .Include(a => a.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (articleAuthorship == null)
            {
                return NotFound();
            }

            return View(articleAuthorship);
        }

        // POST: ArticleAuthorships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articleAuthorship = await _context.ArticleAuthorships.SingleOrDefaultAsync(m => m.Id == id);
            _context.ArticleAuthorships.Remove(articleAuthorship);
            await _context.SaveChangesAsync();
            return RedirectToAction("Create", "ArticleAuthorships", new {/* routeValues, for example: */ mid = articleAuthorship.ManuscriptId });
        }

        private bool ArticleAuthorshipExists(int id)
        {
            return _context.ArticleAuthorships.Any(e => e.Id == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int indx, string contr, int maid, [Bind("Id,UILanguage,ReferredById,ArName,EnName,Email,Password,CountryId,UniversityId")] RegisterFromOutsideViewModel registerFromOutsideViewModel)
        {
            _context.ApplicationUsers.Add(new ApplicationUser
            {
                PasswordHash = null,
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
                DALEnabled = true,

            });
            _context.SaveChanges();
            var userid = _context.ApplicationUsers.Where(a => a.ReferredById == registerFromOutsideViewModel.ReferredById).OrderByDescending(a => a.RegDate).First().Id;
            _context.ArticleAuthorships.Add(new ArticleAuthorship
            {
                AuthorUserId = userid,
                ManuscriptId = maid,
                CountryId = registerFromOutsideViewModel.CountryId,
                UniversityId = registerFromOutsideViewModel.UniversityId,
                Email = registerFromOutsideViewModel.Email,
                Indx = indx,
                Contribution = contr,

            });
            _context.SaveChanges();
            return RedirectToAction("Create", "ArticleAuthorships", new {/* routeValues, for example: */ mid = maid });
        }
        public IActionResult changecorrepondingauthor(int id)
        {
            var ArticleAuthorship = _context.ArticleAuthorships.SingleOrDefault(a => a.Id == id);
            Manuscript manuscript = _context.Manuscripts.Include(a=>a.Journal).SingleOrDefault(m => m.Id == ArticleAuthorship.ManuscriptId );
            if (manuscript == null)
            {
                return NotFound();
            }
            manuscript.CorrespondingAuthorId = ArticleAuthorship.AuthorUserId;
            _context.Update(manuscript);
            _context.SaveChanges();

            //==========================================================================

            EmailContent Reviewercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "6a761361-e63c-47b6-a409-a0b7f4cb0971").SingleOrDefault();

            StringBuilder MyStringBuilder = new StringBuilder("<h2 align='right'>اسم المجلة : ");
            MyStringBuilder.Append(manuscript.Journal.ArName);
            MyStringBuilder.Append("</h2>");
            MyStringBuilder.Append("<br/>");
            MyStringBuilder.AppendFormat(string.Format("<h3 align ='center'><a href='https://portal.arid.my/ar-LY/Manuscripts/Edit/{0}' >", manuscript.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل البحث");
            MyStringBuilder.Append("</a></h3><hr/>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(ArticleAuthorship.Email, Reviewercontent.ArSubject, Reviewercontent.ArContent + MyStringBuilder.ToString()), TimeSpan.FromSeconds(10));

            //==========================================================================

            return RedirectToAction("Details", "Journals", new {/* routeValues, for example: */ id = manuscript.JournalId });
        }

    }
}
