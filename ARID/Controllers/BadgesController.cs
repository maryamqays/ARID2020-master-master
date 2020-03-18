using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class BadgesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;

        public BadgesController(ARIDDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Badges
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.Badges;
            return View(await aRIDDbContext.ToListAsync());
        }



        public async Task<IActionResult> IndexAdmin()
        {
            var aRIDDbContext = _context.Badges.Include(b => b.EmailContent);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: EmailContents/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailContent = await _context.Badges
                               .SingleOrDefaultAsync(m => m.Id == id);
            if (emailContent == null)
            {
                return NotFound();
            }

            return View(emailContent);
        }

        // GET: Badges/Create
        public IActionResult Create()
        {
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject");
            return View();
        }

        // POST: Badges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ArBadgeName,EnBadgeName,ArBadgeDesc,EnBadgeDesc,BadgeLogo,EmailContentId")] Badge badge,
            IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                badge.BadgeLogo = await UserFile.UploadeNewImageAsync(badge.BadgeLogo,
                    myfile, _environment.WebRootPath, Properties.Resources.BadgeLogoFolder, 100, 100);

                _context.Add(badge);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", badge.EmailContentId);
            return View(badge);
        }

        // GET: Badges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badges.SingleOrDefaultAsync(m => m.Id == id);
            if (badge == null)
            {
                return NotFound();
            }
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", badge.EmailContentId);
            return View(badge);
        }

        // POST: Badges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ArBadgeName,EnBadgeName,ArBadgeDesc,EnBadgeDesc,BadgeLogo,EmailContentId")] Badge badge,
            IFormFile myfile)
        {
            if (id != badge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    badge.BadgeLogo = await UserFile.UploadeNewImageAsync(
                        _context.Badges.Where(m => m.Id == id).Select(c => c.BadgeLogo).First(),
                        myfile, _environment.WebRootPath, Properties.Resources.BadgeLogoFolder, 100, 100);

                    _context.Update(badge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BadgeExists(badge.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["EmailContentId"] = new SelectList(_context.EmailContents, "Id", "ArSubject", badge.EmailContentId);
            return View(badge);
        }

        // GET: Badges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badges
                .Include(b => b.EmailContent)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // POST: Badges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var badge = await _context.Badges.SingleOrDefaultAsync(m => m.Id == id);
            _context.Badges.Remove(badge);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.BadgeLogoFolder, badge.BadgeLogo);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BadgeExists(int id)
        {
            return _context.Badges.Any(e => e.Id == id);
        }
    }
}
