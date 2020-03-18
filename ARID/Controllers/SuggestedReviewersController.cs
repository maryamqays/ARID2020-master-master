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

namespace ARID.Controllers
{
    public class SuggestedReviewersController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public SuggestedReviewersController(ARIDDbContext context,UserManager<ApplicationUser> userMrg)
        {
            _context = context;
            _userManager = userMrg;
        }

        // GET: SuggestedReviewers
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.SuggestedReviewers.Include(s => s.Manuscript).Include(s => s.ReviewerUser);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: SuggestedReviewers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestedReviewer = await _context.SuggestedReviewers
                .Include(s => s.Manuscript)
                .Include(s => s.ReviewerUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (suggestedReviewer == null)
            {
                return NotFound();
            }

            return View(suggestedReviewer);
        }

        // GET: SuggestedReviewers/Create
        public IActionResult Create(int mid,string ss)
        {
            ViewData["mid"] = mid;
            if (_context.Submissions.Where(a => a.ManuscriptId == mid).Count() > 0)
            {
                var submission = _context.Submissions.SingleOrDefault(a => a.ManuscriptId == mid && a.SubmissionStatus == SubmissionStatus.Submitted);
                ViewData["sid"] = submission.Id;
            }
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
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



            var jid = _context.Manuscripts.SingleOrDefault(a => a.Id == mid).JournalId;
            SuggestedReviewerViewModel SuggestedReviewerVM = new SuggestedReviewerViewModel()
            {
                Manuscript=_context.Manuscripts.SingleOrDefault(a=>a.Id==mid),
                SuggestedReviewers = _context.SuggestedReviewers.Include(a=>a.ReviewerUser).Where(a => a.ManuscriptId == mid),
                ApplicationUsers = _context.ApplicationUsers.Where(a => a.ArName.Contains(ss) || a.EnName.Contains(ss) || a.Email.Contains(ss) || a.Id.Contains(ss) || a.UserName.Contains(ss)),
                ArticleAuthorships = _context.ArticleAuthorships.Include(a => a.AuthorUser).Where(a => a.ManuscriptId == mid),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                SubmissionFiles = _context.SubmissionFiles.Where(a => a.Submission.ManuscriptId == mid),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId ==mid ),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId ==jid),
            };
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a=>a.Id==mid), "Id", "ArTitle");
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName");
            return View(SuggestedReviewerVM);
        }

        // POST: SuggestedReviewers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CanReviewEnglish,CanReviewArabic,ReasonForSuggesion,ManuscriptId,ReviewerUserId,SuggestionStatus")] SuggestedReviewer suggestedReviewer)
        {
            if (ModelState.IsValid)
            {
                

                _context.Add(suggestedReviewer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "SuggestedReviewers", new {/* routeValues, for example: */ mid = suggestedReviewer.ManuscriptId });
            }
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArAbstract", suggestedReviewer.ManuscriptId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", suggestedReviewer.ReviewerUserId);
            return View(suggestedReviewer);
        }

        // GET: SuggestedReviewers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestedReviewer = await _context.SuggestedReviewers.SingleOrDefaultAsync(m => m.Id == id);
            if (suggestedReviewer == null)
            {
                return NotFound();
            }
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts.Where(a=>a.Id==suggestedReviewer.ManuscriptId), "Id", "ArTitle", suggestedReviewer.ManuscriptId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers.Where(a=>a.Id==suggestedReviewer.ReviewerUserId), "Id", "ArName", suggestedReviewer.ReviewerUserId);
            return View(suggestedReviewer);
        }

        // POST: SuggestedReviewers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CanReviewEnglish,CanReviewArabic,ReasonForSuggesion,ManuscriptId,ReviewerUserId,SuggestionStatus")] SuggestedReviewer suggestedReviewer)
        {
            if (id != suggestedReviewer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suggestedReviewer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuggestedReviewerExists(suggestedReviewer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Create", "SuggestedReviewers", new {/* routeValues, for example: */ mid = suggestedReviewer.ManuscriptId });
            }
            ViewData["ManuscriptId"] = new SelectList(_context.Manuscripts, "Id", "ArTitle", suggestedReviewer.ManuscriptId);
            ViewData["ReviewerUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", suggestedReviewer.ReviewerUserId);
            return View(suggestedReviewer);
        }

        // GET: SuggestedReviewers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestedReviewer = await _context.SuggestedReviewers
                .Include(s => s.Manuscript)
                .Include(s => s.ReviewerUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (suggestedReviewer == null)
            {
                return NotFound();
            }

            return View(suggestedReviewer);
        }

        // POST: SuggestedReviewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suggestedReviewer = await _context.SuggestedReviewers.SingleOrDefaultAsync(m => m.Id == id);
            _context.SuggestedReviewers.Remove(suggestedReviewer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Create", "SuggestedReviewers", new {/* routeValues, for example: */ mid = suggestedReviewer.ManuscriptId });
        }

        private bool SuggestedReviewerExists(int id)
        {
            return _context.SuggestedReviewers.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(bool cra,bool cre,SuggestionStatus sstatus,string ros,int maid, [Bind("Id,UILanguage,ReferredById,ArName,EnName,Email,Password,CountryId,UniversityId")] RegisterFromOutsideViewModel registerFromOutsideViewModel)
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
            _context.SuggestedReviewers.Add(new SuggestedReviewer
            {
                ReviewerUserId = userid,
                ManuscriptId = maid,
                SuggestionStatus =sstatus,
                CanReviewArabic =cra,
                CanReviewEnglish = cre,
                ReasonForSuggesion = ros,

            });
            _context.SaveChanges();
            return RedirectToAction("Create", "SuggestedReviewers", new {/* routeValues, for example: */ mid = maid });
        }

    }
}
