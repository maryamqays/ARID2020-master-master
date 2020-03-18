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
    public class VipDeclarationsController : Controller
    {
        private readonly ARIDDbContext _context;
        private int PagSize = 10;

        public VipDeclarationsController(ARIDDbContext context)
        {
            _context = context;
        }

        // GET: VipDeclarations
        public async Task<IActionResult> Index(int productPage=1)
        {
            VipDeclarationViewModel vipDeclarationVM = new VipDeclarationViewModel()
            {
                vipDeclarations= _context.VipDeclarations.Include(v => v.ApplicationUser)
            };
            //var aRIDDbContext = _context.VipDeclaration.Include(v => v.ApplicationUser);
            //return View(await aRIDDbContext.ToListAsync
            var count = vipDeclarationVM.vipDeclarations.Count();
            vipDeclarationVM.vipDeclarations = vipDeclarationVM.vipDeclarations.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            vipDeclarationVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            return View(vipDeclarationVM);
        }

        // GET: VipDeclarations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vipDeclaration = await _context.VipDeclarations
                .Include(v => v.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vipDeclaration == null)
            {
                return NotFound();
            }

            return View(vipDeclaration);
        }

        // GET: VipDeclarations/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: VipDeclarations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,DeclarationDate,CreationDate,DeclarationBody,DeclarationOccasion")] VipDeclaration vipDeclaration)
        {
            if (ModelState.IsValid)
            {
                vipDeclaration.CreationDate=DateTime.Now;

                _context.Add(vipDeclaration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", vipDeclaration.ApplicationUserId);
            return View(vipDeclaration);
        }

        // GET: VipDeclarations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vipDeclaration = await _context.VipDeclarations.SingleOrDefaultAsync(m => m.Id == id);
            if (vipDeclaration == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", vipDeclaration.ApplicationUserId);
            return View(vipDeclaration);
        }

        // POST: VipDeclarations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,DeclarationDate,CreationDate,DeclarationBody,DeclarationOccasion")] VipDeclaration vipDeclaration)
        {
            if (id != vipDeclaration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vipDeclaration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VipDeclarationExists(vipDeclaration.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", vipDeclaration.ApplicationUserId);
            return View(vipDeclaration);
        }

        // GET: VipDeclarations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vipDeclaration = await _context.VipDeclarations
                .Include(v => v.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vipDeclaration == null)
            {
                return NotFound();
            }

            return View(vipDeclaration);
        }

        // POST: VipDeclarations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vipDeclaration = await _context.VipDeclarations.SingleOrDefaultAsync(m => m.Id == id);
            _context.VipDeclarations.Remove(vipDeclaration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VipDeclarationExists(int id)
        {
            return _context.VipDeclarations.Any(e => e.Id == id);
        }
    }
}
