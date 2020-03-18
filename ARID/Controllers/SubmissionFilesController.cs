using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ARID.Controllers
{
    public class SubmissionFilesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;

        public SubmissionFilesController(ARIDDbContext context, IHostingEnvironment environment, UserManager<ApplicationUser> userMrg)
        {
            _userManager = userMrg;
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: SubmissionFiles
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.SubmissionFiles.Include(s => s.JournalFileType).Include(s => s.Submission);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: SubmissionFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionFile = await _context.SubmissionFiles
                .Include(s => s.JournalFileType)
                .Include(s => s.JournalFileType.FileType)
                .Include(s => s.Submission)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (submissionFile == null)
            {
                return NotFound();
            }

            return View(submissionFile);
        }

        // GET: SubmissionFiles/Create
        public IActionResult Create(int sid)
        {
            var submission = _context.Submissions.SingleOrDefault(a => a.Id == sid);
            var mid = submission.ManuscriptId;
            ViewData["sid"] = sid;
            ViewData["mid"] = mid;
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            var jid = manuscript.JournalId;
            ViewData["jid"] = jid;
            if ((manuscript.CurrentStatus != ManuscriptCurrentStatus.InComplete && manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionSentBackToAuthor) || manuscript.CorrespondingAuthorId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var requiredjournaliles = _context.JournalFileTypes.Where(a => a.JournalId == jid && a.IsRequired == true && a.IsDeleted == false);
            var requiredsubmissionfiles = _context.SubmissionFiles.Where(a => a.SubmissionId == sid && requiredjournaliles.Where(m => m.FileTypeId == a.JournalFileType.FileTypeId).Count() > 0);
            if (requiredjournaliles.Count() == requiredsubmissionfiles.Count())
            {
                ViewData["subfiles"] = "true";
            }
            else
            {
                ViewData["subfiles"] = "false";
            }

            SubmissionFileViewModel SubmissionFileVM = new SubmissionFileViewModel()
            {
                manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid),
                Submission = _context.Submissions.SingleOrDefault(a => a.Id == submission.Id),
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.Submission).Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == sid),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == mid),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                ArticleAuthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == mid),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid)
            };
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == jid), "Id", "ArticleType.Type");

            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes.Include(a => a.FileType).Where(a => a.FileTypeId != 3 && a.FileTypeId != 4 && a.IsDeleted == false && a.JournalId == jid && _context.SubmissionFiles.Where(m => m.SubmissionId == sid && m.JournalFileType.FileType.FileName == a.FileType.FileName).Count() == 0), "Id", "FileType.FileName");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter");
            return View(SubmissionFileVM);
        }

        // POST: SubmissionFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubmissionId,JournalFileTypeId,FileName,Description")] SubmissionFile submissionFile, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                if (submissionFile.Description != null)
                {
                    submissionFile.Description = submissionFile.Description.Replace("\n", "<br/>");
                }
                submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                _context.Add(submissionFile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes, "Id", "Id", submissionFile.JournalFileTypeId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }


        // GET: SubmissionFiles/Create
        public IActionResult TechnicalCreate(int sid)
        {
            var submission = _context.Submissions.SingleOrDefault(a => a.Id == sid);
            var mid = submission.ManuscriptId;
            ViewData["sid"] = sid;
            ViewData["mid"] = mid;
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            var jid = manuscript.JournalId;
            ViewData["jid"] = jid;

            SubmissionFileViewModel SubmissionFileVM = new SubmissionFileViewModel()
            {
                manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid),
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.Submission).Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == sid),
                SuggestedReviewers = _context.SuggestedReviewers.Where(a => a.ManuscriptId == mid),
                Manuscripts = _context.Manuscripts.Where(a => a.Id == mid),
                ArticleAuthorships = _context.ArticleAuthorships.Where(a => a.ManuscriptId == mid),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid)
            };
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == jid), "Id", "ArticleType.Type");
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes.Include(a => a.FileType).Where(a => a.FileTypeId == 3 || a.FileTypeId == 4 ).Where(a=>a.IsDeleted == false && a.JournalId == jid && _context.SubmissionFiles.Where(m => m.SubmissionId == sid && m.JournalFileType.FileType.FileName == a.FileType.FileName).Count() == 0), "Id", "FileType.FileName");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter");
            return View(SubmissionFileVM);
        }

        // POST: SubmissionFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TechnicalCreate(int sid, [Bind("Id,SubmissionId,JournalFileTypeId,FileName,Description")] SubmissionFile submissionFile, IFormFile myfile)
        {
            var sub = _context.Submissions.SingleOrDefault(a => a.Id == submissionFile.SubmissionId);
            var ManuscriptId = sub.ManuscriptId;
            var journalfile = _context.JournalFileTypes.SingleOrDefault(a => a.Id == submissionFile.JournalFileTypeId);
            if (ModelState.IsValid)
            {
                if (submissionFile.Description != null)
                {
                    submissionFile.Description = submissionFile.Description.Replace("\n", "<br/>");
                }
                if (journalfile.FileTypeId == 4) { 
                submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.PublicationFiles);
                }
                else
                {
                    submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.Secured);
                }

                _context.Add(submissionFile);
                await _context.SaveChangesAsync();
                return RedirectToAction("TechnicalEditorDetails", "Manuscripts", new {/* routeValues, for example: */ id = ManuscriptId });
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes, "Id", "Id", submissionFile.JournalFileTypeId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }





        // GET: SubmissionFiles/ResubmissionFiles
        public IActionResult ResubmissionFiles(int sid)
        {
            var submission = _context.Submissions.SingleOrDefault(a => a.Id == sid);
            var mid = submission.ManuscriptId;
            ViewData["sid"] = sid;
            ViewData["mid"] = mid;
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == mid);
            var jid = manuscript.JournalId;
            ViewData["jid"] = jid;
            if (manuscript.CurrentStatus != ManuscriptCurrentStatus.SubmissionsBeingProcessed)
            {
                return NotFound();
            }


            SubmissionFileViewModel SubmissionFileVM = new SubmissionFileViewModel()
            {
                SubmissionFiles = _context.SubmissionFiles.Include(a => a.Submission).Include(a => a.JournalFileType).Include(a => a.JournalFileType.FileType).Where(a => a.SubmissionId == sid),
                Submissions = _context.Submissions.Where(a => a.ManuscriptId == mid),
                JournalFileTypes = _context.JournalFileTypes.Where(a => a.JournalId == jid)
            };
            ViewData["JournalArticleTypeId"] = new SelectList(_context.JournalArticleTypes.Include(a => a.ArticleType).Where(a => a.JournalId == jid), "Id", "ArticleType.Type");

            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes.Include(a => a.FileType).Where(a => a.JournalId == jid), "Id", "FileType.FileName");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter");
            return View(SubmissionFileVM);
        }

        // POST: SubmissionFiles/ResubmissionFiles
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResubmissionFiles([Bind("Id,SubmissionId,JournalFileTypeId,FileName,Description")] SubmissionFile submissionFile, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                submissionFile.Description = submissionFile.Description.Replace("\n", "<br/>");
                submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                _context.Add(submissionFile);
                await _context.SaveChangesAsync();
                return RedirectToAction("ResubmissionFiles", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes, "Id", "Id", submissionFile.JournalFileTypeId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }


        // GET: SubmissionFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionFile = await _context.SubmissionFiles.Include(a => a.Submission.Manuscript).SingleOrDefaultAsync(m => m.Id == id);
            if (submissionFile == null)
            {
                return NotFound();
            }
            var sid = submissionFile.SubmissionId;
            var manuscript = _context.Manuscripts.SingleOrDefault(a => a.Id == submissionFile.Submission.ManuscriptId);
            var jid = manuscript.JournalId;
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes.Include(a => a.FileType).Where(a => a.Id != 3 && a.IsDeleted == false && a.JournalId == jid), "Id", "FileType.FileName");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }

        // POST: SubmissionFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubmissionId,JournalFileTypeId,FileName,Description")] SubmissionFile submissionFile, IFormFile myfile)
        {
            if (id != submissionFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                    _context.Update(submissionFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionFileExists(submissionFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Create", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes, "Id", "Id", submissionFile.JournalFileTypeId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }
        // GET: SubmissionFiles/Edit/5
        public async Task<IActionResult> EditResubFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionFile = await _context.SubmissionFiles.Include(a => a.Submission.Manuscript.Journal).SingleOrDefaultAsync(m => m.Id == id);
            if (submissionFile == null)
            {
                return NotFound();
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes.Include(a => a.FileType).Where(a => a.JournalId == submissionFile.Submission.Manuscript.JournalId), "Id", "FileType.FileName");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }

        // POST: SubmissionFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResubFile(int id, [Bind("Id,SubmissionId,JournalFileTypeId,FileName,Description")] SubmissionFile submissionFile, IFormFile myfile)
        {
            if (id != submissionFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submissionFile.FileName = await UserFile.UploadeNewFileAsync(submissionFile.FileName,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                    _context.Update(submissionFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionFileExists(submissionFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ResubmissionFiles", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
            }
            ViewData["JournalFileTypeId"] = new SelectList(_context.JournalFileTypes, "Id", "Id", submissionFile.JournalFileTypeId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "CoverLetter", submissionFile.SubmissionId);
            return View(submissionFile);
        }

        // GET: SubmissionFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionFile = await _context.SubmissionFiles
                .Include(s => s.JournalFileType)
                .Include(s => s.Submission)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (submissionFile == null)
            {
                return NotFound();
            }

            return View(submissionFile);
        }

        // POST: SubmissionFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submissionFile = await _context.SubmissionFiles.SingleOrDefaultAsync(m => m.Id == id);
            _context.SubmissionFiles.Remove(submissionFile);
            await _context.SaveChangesAsync();
            return RedirectToAction("Create", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
        }

        // POST: SubmissionFiles/Delete/5
        [HttpPost, ActionName("Deleteresubfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deleteresubfile(int id)
        {
            var submissionFile = await _context.SubmissionFiles.SingleOrDefaultAsync(m => m.Id == id);
            _context.SubmissionFiles.Remove(submissionFile);
            await _context.SaveChangesAsync();
            return RedirectToAction("ResubmissionFiles", "SubmissionFiles", new {/* routeValues, for example: */ sid = submissionFile.SubmissionId });
        }

        private bool SubmissionFileExists(int id)
        {
            return _context.SubmissionFiles.Any(e => e.Id == id);
        }
    }
}