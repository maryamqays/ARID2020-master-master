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
    public class JournalFileTypesController : Controller
    {
        private readonly ARIDDbContext _context;

        public JournalFileTypesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: JournalFileTypes
        public async Task<IActionResult> Index(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);
            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;
              var  aRIDDbContext = _context.JournalFileTypes.Where(a=>a.JournalId==id).Include(j => j.FileType).Include(j => j.Journal);

            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: JournalFileTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalFileType = await _context.JournalFileTypes
                .Include(j => j.FileType)
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalFileType == null)
            {
                return NotFound();
            }

            return View(journalFileType);
        }

        // GET: JournalFileTypes/Create
        public IActionResult Create(int id)
        {
            ViewData["jid"] =id;
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes.Where(a=>a.Id!=0 && _context.JournalFileTypes.Include(j=>j.FileType).Where(j=>j.FileType.FileName==a.FileName && j.JournalId==id).Count()==0), "Id", "FileName");
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==id), "Id", "ArName");
            return View();
        }

        // POST: JournalFileTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jid,[Bind("JournalId,FileTypeId,IsRequired,IsDeleted")] JournalFileType journalFileType)
        {
            if(journalFileType.FileTypeId==3 || journalFileType.FileTypeId == 4)
            {
                journalFileType.IsRequired = false;
            }
            if (journalFileType.FileTypeId == 0)
            {
                return RedirectToAction("Create", new { id = jid });
            }

            if (ModelState.IsValid)
            {
                _context.Add(journalFileType);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "Id", "FileName", journalFileType.FileTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalFileType.JournalId);
            return View(journalFileType);
        }

        // GET: JournalFileTypes/Edit/5
        public async Task<IActionResult> Edit(int? id,int jid)
        {
            ViewData["jid"]=jid;
            if (id == null)
            {
                return NotFound();
            }

            var journalFileType = await _context.JournalFileTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (journalFileType == null)
            {
                return NotFound();
            }
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "Id", "FileName", journalFileType.FileTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName", journalFileType.JournalId);
            return View(journalFileType);
        }

        // POST: JournalFileTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int jid,int id, [Bind("Id,JournalId,FileTypeId,IsRequired,IsDeleted")] JournalFileType journalFileType)
        {
            if (id != journalFileType.Id)
            {
                return NotFound();
            }
            if(journalFileType.FileTypeId==3 || journalFileType.FileTypeId == 4)
            {
                journalFileType.IsRequired = false;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalFileType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalFileTypeExists(journalFileType.Id))
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
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "Id", "FileName", journalFileType.FileTypeId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalFileType.JournalId);
            return View(journalFileType);
        }

        // GET: JournalFileTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalFileType = await _context.JournalFileTypes
                .Include(j => j.FileType)
                .Include(j => j.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalFileType == null)
            {
                return NotFound();
            }

            return View(journalFileType);
        }

        // POST: JournalFileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalFileType = await _context.JournalFileTypes.SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalFileTypes.Remove(journalFileType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalFileTypeExists(int id)
        {
            return _context.JournalFileTypes.Any(e => e.Id == id);
        }
    }
}
