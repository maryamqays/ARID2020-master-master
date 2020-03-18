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
    public class UserExpressInterestsController : Controller
    {
        private readonly ARIDDbContext _context;

        public UserExpressInterestsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: UserExpressInterests
        public async Task<IActionResult> Index(int? Id)
        {
            var aRIDDbContext = _context.UserExpressInterests.Include(u => u.ApplicationUser).Include(u => u.RegistrationForm).Include(u => u.ApplicationUser.University).Include(u => u.ApplicationUser.Country).Include(u => u.ApplicationUser.Faculty).Where(a => a.RegistrationFormId == Id).OrderByDescending(a=>a.ApplicationUser.Visitors);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: UserExpressInterests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userExpressInterest = await _context.UserExpressInterests
                .Include(u => u.ApplicationUser)
                .Include(u => u.RegistrationForm)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userExpressInterest == null)
            {
                return NotFound();
            }

            return View(userExpressInterest);
        }

        // GET: UserExpressInterests/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["RegistrationFormId"] = new SelectList(_context.RegistrationForms, "Id", "Id");
            return View();
        }

        // POST: UserExpressInterests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegistrationFormId,ApplicationUserId,CreationDate")] UserExpressInterest userExpressInterest)
        {
            if (ModelState.IsValid)
            {
                userExpressInterest.Id = Guid.NewGuid();
                _context.Add(userExpressInterest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", userExpressInterest.ApplicationUserId);
            ViewData["RegistrationFormId"] = new SelectList(_context.RegistrationForms, "Id", "Id", userExpressInterest.RegistrationFormId);
            return View(userExpressInterest);
        }

        // GET: UserExpressInterests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userExpressInterest = await _context.UserExpressInterests.SingleOrDefaultAsync(m => m.Id == id);
            if (userExpressInterest == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", userExpressInterest.ApplicationUserId);
            ViewData["RegistrationFormId"] = new SelectList(_context.RegistrationForms, "Id", "Id", userExpressInterest.RegistrationFormId);
            return View(userExpressInterest);
        }

        // POST: UserExpressInterests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RegistrationFormId,ApplicationUserId,CreationDate")] UserExpressInterest userExpressInterest)
        {
            if (id != userExpressInterest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userExpressInterest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExpressInterestExists(userExpressInterest.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", userExpressInterest.ApplicationUserId);
            ViewData["RegistrationFormId"] = new SelectList(_context.RegistrationForms, "Id", "Id", userExpressInterest.RegistrationFormId);
            return View(userExpressInterest);
        }

        // GET: UserExpressInterests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userExpressInterest = await _context.UserExpressInterests
                .Include(u => u.ApplicationUser)
                .Include(u => u.RegistrationForm)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userExpressInterest == null)
            {
                return NotFound();
            }

            return View(userExpressInterest);
        }

        // POST: UserExpressInterests/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var userExpressInterest = await _context.UserExpressInterests.SingleOrDefaultAsync(m => m.Id == Id);
            _context.UserExpressInterests.Remove(userExpressInterest);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExpressInterestExists(Guid id)
        {
            return _context.UserExpressInterests.Any(e => e.Id == id);
        }
    }
}
