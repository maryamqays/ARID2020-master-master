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
    public class AreaEditorsController : Controller
    {
        private readonly ARIDDbContext _context;

        public AreaEditorsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: AreaEditors
        public async Task<IActionResult> Index(int id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);
            ViewData["jshortname"] = journal.ShortName;
            ViewData["journalname"] = journal.ArName;
            ViewData["jid"] =id;
            var aRIDDbContext = _context.AreaEditors.Where(a=>a.JournalId==id).Include(a => a.ApplicationUser).Include(a => a.Journal);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: AreaEditors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var areaEditor = await _context.AreaEditors
                .Include(a => a.ApplicationUser)
                .Include(a => a.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (areaEditor == null)
            {
                return NotFound();
            }

            return View(areaEditor);
        }

        // GET: AreaEditors/Create
        public IActionResult Create(int jid,string ss)
        {
            ViewData["jid"] = jid;
            AreaEditorViewModel AreaEditorVm = new AreaEditorViewModel()
            {
                ApplicationUsers = _context.ApplicationUsers.Include(a=>a.Country).Include(a=>a.University).Where(a => a.ArName.Contains(ss) || a.EnName.Contains(ss) || a.Email.Contains(ss) || a.Id.Contains(ss) || a.UserName.Contains(ss)),
                AreaEditors = _context.AreaEditors.Where(a=>a.JournalId== jid)
            };

            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==jid), "Id", "ArName");
            return View(AreaEditorVm);
        }

        // POST: AreaEditors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,JournalId,Description,JoinDate,IsActive,RoleId")] AreaEditor areaEditor)
        {
            if (ModelState.IsValid)
            {
                areaEditor.JoinDate = DateTime.Now;
                areaEditor.IsActive = true;

                _context.Add(areaEditor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new {id=areaEditor.JournalId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", areaEditor.ApplicationUserId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", areaEditor.JournalId);
            return View(areaEditor);
        }

        // GET: AreaEditors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var areaEditor = await _context.AreaEditors.SingleOrDefaultAsync(m => m.Id == id);
            if (areaEditor == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a=>a.Id==areaEditor.ApplicationUserId), "Id", "ArName", areaEditor.ApplicationUserId);
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a => a.Id == areaEditor.JournalId), "Id", "ArName", areaEditor.JournalId);
            return View(areaEditor);
        }

        // POST: AreaEditors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,JournalId,Description,JoinDate,IsActive,RoleId")] AreaEditor areaEditor)
        {
            if (id != areaEditor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(areaEditor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaEditorExists(areaEditor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "AreaEditors", new {/* routeValues, for example: */ id = areaEditor.JournalId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", areaEditor.ApplicationUserId);
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", areaEditor.JournalId);
            return View(areaEditor);
        }

        // GET: AreaEditors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var areaEditor = await _context.AreaEditors
                .Include(a => a.ApplicationUser)
                .Include(a => a.Journal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (areaEditor == null)
            {
                return NotFound();
            }

            return View(areaEditor);
        }

        // POST: AreaEditors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var areaEditor = await _context.AreaEditors.SingleOrDefaultAsync(m => m.Id == id);
            _context.AreaEditors.Remove(areaEditor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaEditorExists(int id)
        {
            return _context.AreaEditors.Any(e => e.Id == id);
        }
    }
}
