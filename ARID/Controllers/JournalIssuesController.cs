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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;

namespace ARID.Controllers
{
    public class JournalIssuesController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public JournalIssuesController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userMrg;
            _environment = environment;
        }

        // GET: JournalIssues
        public async Task<IActionResult> Index(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;

            var aRIDDbContext = _context.JournalIssues.Where(a => a.JournalId == id).Include(j => j.Journal).Include(a => a.Volume);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: JournalIssues/Details/5
        public async Task<IActionResult> Details(int? id,int jid)
        {
            ViewData["jid"] = jid;
            if (id == null)
            {
                return NotFound();
            }

            var journalIssue = await _context.JournalIssues
                .Include(j => j.Journal)
                .Include(j => j.Volume)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalIssue == null)
            {
                return NotFound();
            }

            return View(journalIssue);
        }

        // GET: JournalIssues/Create
        public IActionResult Create(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;

            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==id), "Id", "ArName");
            ViewData["VolumeId"] = new SelectList(_context.Volumes.Where(a=>a.JournalId==id), "Id", "Name");
            return View();
        }

        // POST: JournalIssues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jid,[Bind("Name,Releasedate,Cover,Pdf,Number,JournalId,VolumeId,IsPublished,IsOpen,Visitors,PdfDownloadCounter")] JournalIssue journalIssue, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                journalIssue.Pdf = await UserFile.UploadeNewFileAsync(journalIssue.Pdf,
         myfile, _environment.WebRootPath, Properties.Resources.Secured);
                journalIssue.Cover = await UserFile.UploadeNewImageAsync(journalIssue.Cover,
               myfile1, _environment.WebRootPath, Properties.Resources.Images, 100, 150);
                _context.Add(journalIssue);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", journalIssue.JournalId);
            ViewData["VolumeId"] = new SelectList(_context.Volumes.Where(a => a.JournalId == jid), "Id", "Id", journalIssue.VolumeId);
            return View(journalIssue);
        }

        // GET: JournalIssues/Edit/5
        public async Task<IActionResult> Edit(int? id, int jid)
        {
            ViewData["jid"] = jid;

            if (id == null)
            {
                return NotFound();
            }
        
            var journalIssue = await _context.JournalIssues.SingleOrDefaultAsync(m => m.Id == id);
            if (journalIssue == null)
            {
                return NotFound();
            }
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", journalIssue.JournalId);
            ViewData["VolumeId"] = new SelectList(_context.Volumes.Where(a=>a.JournalId==jid), "Id", "Id", journalIssue.VolumeId);
            return View(journalIssue);
        }

        // POST: JournalIssues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int jid, int id, [Bind("Id,Name,Releasedate,Cover,Pdf,Number,JournalId,VolumeId,IsPublished,IsOpen,Visitors,PdfDownloadCounter")] JournalIssue journalIssue, IFormFile myfile, IFormFile myfile1)
        {
            if (id != journalIssue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    journalIssue.Pdf = await UserFile.UploadeNewFileAsync(journalIssue.Pdf,
        myfile, _environment.WebRootPath, Properties.Resources.Secured);

                    journalIssue.Cover = await UserFile.UploadeNewImageAsync(journalIssue.Cover,
                   myfile1, _environment.WebRootPath, Properties.Resources.Images, 100, 150);

                    _context.Update(journalIssue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalIssueExists(journalIssue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", journalIssue.JournalId);
            ViewData["VolumeId"] = new SelectList(_context.Volumes.Where(a=>a.JournalId==jid), "Id", "Id", journalIssue.VolumeId);
            return View(journalIssue);
        }

        // GET: JournalIssues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalIssue = await _context.JournalIssues
                .Include(j => j.Journal)
                .Include(j => j.Volume)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalIssue == null)
            {
                return NotFound();
            }

            return View(journalIssue);
        }

        // POST: JournalIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalIssue = await _context.JournalIssues.SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalIssues.Remove(journalIssue);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = journalIssue.JournalId });
        }

        private bool JournalIssueExists(int id)
        {
            return _context.JournalIssues.Any(e => e.Id == id);
        }
    }
}
