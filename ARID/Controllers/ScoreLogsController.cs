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
    public class ScoreLogsController : Controller
    {
        private readonly ARIDDbContext _context;
                private UserManager<ApplicationUser> _userManager;

        public ScoreLogsController(ARIDDbContext context,UserManager<ApplicationUser> userMrg)
        {
            _context = context;
                        _userManager = userMrg;
        }

        // GET: ScoreLogs
        public async Task<IActionResult> Index()
        {
            ViewData["CommunityScore"] = _context.ScoreLogs.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).Sum(a => a.ScoreValue.Value);
            var aRIDDbContext = _context.ScoreLogs.Include(s => s.Post).Include(s => s.ScoreValue).Where(a => a.ApplicationUserId == _userManager.GetUserId(User));
           
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: ScoreLogs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreLog = await _context.ScoreLogs
                .Include(s => s.ApplicationUser)
                .Include(s => s.Post)
                .Include(s => s.ScoreValue)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scoreLog == null)
            {
                return NotFound();
            }

            return View(scoreLog);
        }

        // GET: ScoreLogs/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Body");
            ViewData["ScoreValueId"] = new SelectList(_context.ScoreValues, "Id", "Id");
            return View();
        }

        // POST: ScoreLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ScoreValueId,PostId,Date")] ScoreLog scoreLog)
        {
            if (ModelState.IsValid)
            {
                scoreLog.Id = Guid.NewGuid();
                _context.Add(scoreLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", scoreLog.ApplicationUserId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Body", scoreLog.PostId);
            ViewData["ScoreValueId"] = new SelectList(_context.ScoreValues, "Id", "Id", scoreLog.ScoreValueId);
            return View(scoreLog);
        }

        // GET: ScoreLogs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreLog = await _context.ScoreLogs.SingleOrDefaultAsync(m => m.Id == id);
            if (scoreLog == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", scoreLog.ApplicationUserId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Body", scoreLog.PostId);
            ViewData["ScoreValueId"] = new SelectList(_context.ScoreValues, "Id", "Id", scoreLog.ScoreValueId);
            return View(scoreLog);
        }

        // POST: ScoreLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ApplicationUserId,ScoreValueId,PostId,Date")] ScoreLog scoreLog)
        {
            if (id != scoreLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scoreLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoreLogExists(scoreLog.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", scoreLog.ApplicationUserId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Body", scoreLog.PostId);
            ViewData["ScoreValueId"] = new SelectList(_context.ScoreValues, "Id", "Id", scoreLog.ScoreValueId);
            return View(scoreLog);
        }

        // GET: ScoreLogs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreLog = await _context.ScoreLogs
                .Include(s => s.ApplicationUser)
                .Include(s => s.Post)
                .Include(s => s.ScoreValue)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (scoreLog == null)
            {
                return NotFound();
            }

            return View(scoreLog);
        }

        // POST: ScoreLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var scoreLog = await _context.ScoreLogs.SingleOrDefaultAsync(m => m.Id == id);
            _context.ScoreLogs.Remove(scoreLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoreLogExists(Guid id)
        {
            return _context.ScoreLogs.Any(e => e.Id == id);
        }
    }
}
