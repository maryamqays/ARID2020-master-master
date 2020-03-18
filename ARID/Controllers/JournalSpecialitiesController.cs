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
    public class JournalSpecialitiesController : Controller
    {
        private readonly ARIDDbContext _context;

        public JournalSpecialitiesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: JournalSpecialities
        public async Task<IActionResult> Index(int? id)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;

            var aRIDDbContext = _context.JournalSpecialities.Where(j=>j.JournalId==id).Include(j => j.Journal).Include(j => j.Speciality);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: JournalSpecialities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalSpeciality = await _context.JournalSpecialities
                .Include(j => j.Journal)
                .Include(j => j.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalSpeciality == null)
            {
                return NotFound();
            }

            return View(journalSpeciality);
        }

        // GET: JournalSpecialities/Create
        public IActionResult Create(int? id)
        {

            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);

            ViewData["jid"] = id;
            ViewData["jsn"] = journal.ShortName;
            ViewData["jan"] = journal.ArName;
            ViewData["JournalId"] = new SelectList(_context.Journals.Where(a=>a.Id==id), "Id", "ArName");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a=>a.Name!=null && _context.JournalSpecialities.Where(j => j.Speciality.Name == a.Name).Count() == 0), "Id", "Name");
          //  ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a => a.Name != null), "Id", "Name");

            return View();
        }

        // POST: JournalSpecialities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jid,[Bind("JournalId,SpecialityId")] JournalSpeciality journalSpeciality)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journalSpeciality);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = jid });
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalSpeciality.JournalId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities.Where(a=>a.Name!=null &&_context.JournalSpecialities.Where(j=>j.Speciality.Name==a.Name).Count()==0), "Id", "Name", journalSpeciality.SpecialityId);
            return View(journalSpeciality);
        }

        // GET: JournalSpecialities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalSpeciality = await _context.JournalSpecialities.SingleOrDefaultAsync(m => m.Id == id);
            if (journalSpeciality == null)
            {
                return NotFound();
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalSpeciality.JournalId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", journalSpeciality.SpecialityId);
            return View(journalSpeciality);
        }

        // POST: JournalSpecialities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JournalId,SpecialityId")] JournalSpeciality journalSpeciality)
        {
            if (id != journalSpeciality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalSpeciality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalSpecialityExists(journalSpeciality.Id))
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
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "ArName", journalSpeciality.JournalId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", journalSpeciality.SpecialityId);
            return View(journalSpeciality);
        }

        // GET: JournalSpecialities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalSpeciality = await _context.JournalSpecialities
                .Include(j => j.Journal)
                .Include(j => j.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journalSpeciality == null)
            {
                return NotFound();
            }

            return View(journalSpeciality);
        }

        // POST: JournalSpecialities/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalSpeciality = await _context.JournalSpecialities.Include(a=>a.Journal).SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalSpecialities.Remove(journalSpeciality);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = journalSpeciality.JournalId });
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            var journalSpeciality = await _context.JournalSpecialities.Include(a=>a.Journal).SingleOrDefaultAsync(m => m.Id == id);
            _context.JournalSpecialities.Remove(journalSpeciality);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = journalSpeciality.JournalId });
        }

        private bool JournalSpecialityExists(int id)
        {
            return _context.JournalSpecialities.Any(e => e.Id == id);
        }
    }
}
