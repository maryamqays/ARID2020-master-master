using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;

namespace ARID.Controllers
{
    public class JournalArticleTypesController : Controller
    {
        private readonly ARIDDbContext _context;

        public JournalArticleTypesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: JournalArticleTypes
        public async Task<IActionResult> Index(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;
            var aRIDDbContext = _context.JournalArticleTypes.Where(a=>a.JournalId==id).Include(j => j.ArticleType).Include(j => j.Journal);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: JournalArticleTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalArticleType = await _context.JournalArticleTypes
                .Include(j => j.ArticleType)
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalArticleType == null)
            {
                return NotFound();
            }

            return View(journalArticleType);
        }

        // GET: JournalArticleTypes/Create
        public IActionResult Create(int id)
        {
            ViewData["jid"] = id;
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(a => a.Type != null && _context.JournalArticleTypes.Where(j => j.ArticleType.Type == a.Type && j.JournalId==id).Count() == 0), "Id", "Type");
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==id), "Id", "ArName");
            return View();
        }

        // POST: JournalArticleTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jid,[Bind("JournalId,ArticleTypeId,IsDeleted")] JournalArticleType journalArticleType)
        {
            if (journalArticleType.ArticleTypeId == 0)
            {
                return RedirectToAction("Create", new { id = jid });
            }
            if (ModelState.IsValid)
            {
                _context.Add(journalArticleType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(a=>a.Type!=null&&_context.JournalArticleTypes.Where(j=>j.ArticleType.Type==a.Type).Count()==0), "Id", "Type", journalArticleType.ArticleTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalArticleType.JournalId);
            return View(journalArticleType);
        }

        // GET: JournalArticleTypes/Edit/5
        public async Task<IActionResult> Edit(int? id,int jid)
        {
            ViewData["jid"] = jid;
            if (id == null)
            {
                return NotFound();
            }

            var journalArticleType = await _context.JournalArticleTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (journalArticleType == null)
            {
                return NotFound();
            }
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes, "Id", "Type", journalArticleType.ArticleTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", journalArticleType.JournalId);
            return View(journalArticleType);
        }

        // POST: JournalArticleTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int jid, int id, [Bind("Id,JournalId,ArticleTypeId,IsDeleted")] JournalArticleType journalArticleType)
        {
            if (id != journalArticleType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalArticleType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalArticleTypeExists(journalArticleType.Id))
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
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes, "Id", "Type", journalArticleType.ArticleTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalArticleType.JournalId);
            return View(journalArticleType);
        }

        // GET: JournalArticleTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalArticleType = await _context.JournalArticleTypes
                .Include(j => j.ArticleType)
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalArticleType == null)
            {
                return NotFound();
            }

            return View(journalArticleType);
        }

        // POST: JournalArticleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalArticleType = await _context.JournalArticleTypes.SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalArticleTypes.Remove(journalArticleType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalArticleTypeExists(int id)
        {
            return _context.JournalArticleTypes.Any(e => e.Id == id);
        }
    }
}
