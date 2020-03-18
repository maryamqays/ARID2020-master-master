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
    public class ScoreValuesController : Controller
    {
        private readonly ARIDDbContext _context;

        public ScoreValuesController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: ScoreValues
        public async Task<IActionResult> Index()
        {
            return View(await _context.ScoreValues.ToListAsync());
        }

        // GET: ScoreValues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreValue = await _context.ScoreValues
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scoreValue == null)
            {
                return NotFound();
            }

            return View(scoreValue);
        }

        // GET: ScoreValues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScoreValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Value")] ScoreValue scoreValue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scoreValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scoreValue);
        }

        // GET: ScoreValues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreValue = await _context.ScoreValues.SingleOrDefaultAsync(m => m.Id == id);
            if (scoreValue == null)
            {
                return NotFound();
            }
            return View(scoreValue);
        }

        // POST: ScoreValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Value")] ScoreValue scoreValue)
        {
            if (id != scoreValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scoreValue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoreValueExists(scoreValue.Id))
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
            return View(scoreValue);
        }

        // GET: ScoreValues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreValue = await _context.ScoreValues
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scoreValue == null)
            {
                return NotFound();
            }

            return View(scoreValue);
        }

        // POST: ScoreValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scoreValue = await _context.ScoreValues.SingleOrDefaultAsync(m => m.Id == id);
            _context.ScoreValues.Remove(scoreValue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoreValueExists(int id)
        {
            return _context.ScoreValues.Any(e => e.Id == id);
        }
    }
}
