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
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize]
    // [Authorize(Roles = RoleName.Members)]
    // [Authorize(Roles = RoleName.Admins)]
    public class PublicationsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public PublicationsController(UserManager<ApplicationUser> userManager, IHostingEnvironment environment, ARIDDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }


        //public async Task<IActionResult> UpdateDownloadsViewsCounter()
        //{

        //    foreach (var item in _context.Publications.Where(a=>a.FileLink != null))
        //    {
        //        var Userid = _context.ApplicationUsers.SingleOrDefault(a => a.Id == item.ApplicationUserId);
        //        item.DownloadHits = Convert.ToInt32(Userid.Visitors * 0.1);
        //    }

        //    _context.SaveChanges();

        //    //var Publications = _context.Publications.Select(a=>a.DownloadHits);
        //    //foreach (string item in Publications)
        //    //{


        //    //    Publications.DownloadHits = UserDetails.Visitors + 25;
        //    //}
        //    //_context.SaveChanges();
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> Download(string filename)
        //{
        //    if (filename == null) return Content("filename not present");
        //    var path = Path.Combine(_environment.WebRootPath, Properties.Resources.PublicationFiles, filename);
        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, UserFile.GetContentType(path), Path.GetFileName(path));
        //}

        // GET: Publications
        public async Task<IActionResult> Index()
        {
            var publications = _context.Publications
                .Where(e => e.ApplicationUserId == _userManager.GetUserId(User))
                .OrderBy(e => e.PublicationDate);
            //.GroupBy(e => e.PublicationType);
            return View(await publications.ToListAsync());
        }


     

        // GET: Publications/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: Publications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,PublicationType,ArTitle,EnTitle,ArAuthors,EnAuthors,ArAbstract,EnAbstract,PublicationDate,Publisher,VolumeNo,IssueNo,ISSN,DOI,Pages,FileLink,ExternalLink,DownloadHits,Keywords")]
        Publication publication, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                publication.FileLink = await UserFile.UploadeNewFileAsync(publication.FileLink,
                    myfile, _environment.WebRootPath, Properties.Resources.PublicationFiles);

                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = _userManager.GetUserId(User);
            return View(publication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (publication == null)
            {
                return NotFound();
            }
            return View(publication);
        }

        // POST: Publications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,PublicationType,ArTitle,EnTitle,ArAuthors,EnAuthors,ArAbstract,EnAbstract,PublicationDate,Publisher,VolumeNo,IssueNo,ISSN,DOI,Pages,FileLink,ExternalLink,DownloadHits,Keywords")]
        Publication publication, IFormFile myfile)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    publication.FileLink = await UserFile.UploadeNewFileAsync(
                        _context.Publications.Where(m => m.Id == id).Select(c => c.FileLink).First(),
                        myfile, _environment.WebRootPath, Properties.Resources.PublicationFiles);

                    _context.Update(publication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
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
            return View(publication);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Publications = await _context.Publications
                .Include(a => a.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (Publications == null)
            {
                return NotFound();
            }

            return View(Publications);
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publication = await _context.Publications.SingleOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == _userManager.GetUserId(User));
            _context.Publications.Remove(publication);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.PublicationFiles, publication.FileLink);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicationExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
        }
    }
}
