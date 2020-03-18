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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using System.IO;
using System.Drawing;
using System.Net;
using ARID.Extensions;
using System.Text.RegularExpressions;
//using DinkToPdf;
//using DinkToPdf.Contracts;
//using DinkToPdf.Contracts;
namespace ARID.Controllers
{
    public class JournalsController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;
        //private IConverter _converter;
        public JournalsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
            //_converter = converter;
        }

        // GET: Journals
        //[AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.Journals.Include(j => j.CreatedByUser).Include(j => j.EiC).Include(j => j.Publisher);
            return View(await aRIDDbContext.ToListAsync());
        }


        // GET: Journals/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {

            ViewData["journalid"] = id;

            _context.Journals.SingleOrDefault(a => a.Id == id).Visitors++;

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var journal = await _context.Journals
            //    .Include(j => j.ApplicantUser)
            //    .Include(j => j.EditorinChief)
            //    .Include(j => j.Publisher)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (journal == null)
            //{
            //    return NotFound();
            //}
            var currentuserarticleauthoships = _context.ArticleAuthorships.Where(a => a.AuthorUserId == _userManager.GetUserId(User));

            JournalViewModel JournalVM = new JournalViewModel()
            {
                CoAuthorManuscripts = _context.Manuscripts.Where(a => a.CorrespondingAuthorId != _userManager.GetUserId(User) && currentuserarticleauthoships.Where(m => m.ManuscriptId == a.Id && m.AuthorUserId == _userManager.GetUserId(User)).Count() > 0),
                Manuscripts = _context.Manuscripts.Where(a => a.JournalId == id && a.CorrespondingAuthorId == _userManager.GetUserId(User) && a.IsDeleted == false).Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.CorrespondingAuthor),
                AreaEditors = _context.AreaEditors.Where(a => a.JournalId == id).Include(a => a.Journal),

                Journal = await _context.Journals
                .Include(j => j.CreatedByUser)
                .Include(j => j.EiC)
                .Include(j => j.Publisher)
                .SingleOrDefaultAsync(m => m.Id == id),
            };



            _context.SaveChanges();
            return View(JournalVM);
        }

        // GET: Journals/ReviewersSearch/5
        [Authorize]
        public async Task<IActionResult> ReviewersSearch(int? id, string ss)
        {
            var journal = _context.Journals.SingleOrDefault(a => a.Id == id);
            ViewData["jid"] = id;
            ViewData["jan"] = journal.ArName;
            ViewData["jsn"] = journal.ShortName;

            JournalViewModel JournalVM = new JournalViewModel();
            if (journal.EiCId != _userManager.GetUserId(User) && _context.AreaEditors.Where(a => a.JournalId == journal.Id && a.ApplicationUserId == _userManager.GetUserId(User)).Count() == 0)
            {
                return NotFound();
            }
            if (ss != null)
            {
                JournalVM = new JournalViewModel()
                {
                    JournalReviewers = _context.ApplicationUsers
                   .Include(a => a.Country)
                   .Include(a => a.University)
                   .Include(a => a.Faculty.Speciality)
                   .Where(a => a.ArName != null && _context.Manuscripts
                   .Where(m => m.JournalId == id && _context.SuggestedReviewers.Where(s => s.ManuscriptId == m.Id && s.ReviewerUserId == a.Id).Count() > 0).Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss) || m.ArAbstract.Contains(ss) || m.EnAbstract.Contains(ss)).Count() > 0),

                    OtherJournalsReviewers = _context.ApplicationUsers
                   .Include(a => a.Country)
                   .Include(a => a.University)
                   .Include(a => a.Faculty.Speciality)
                   .Where(a => a.ArName != null && _context.Manuscripts
                   .Where(m => m.JournalId != id && m.ArTitle != null && _context.SuggestedReviewers.Where(s => s.ManuscriptId == m.Id && s.ReviewerUserId == a.Id).Count() > 0).Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss) || m.ArAbstract.Contains(ss) || m.EnAbstract.Contains(ss)).Count() > 0),

                    JournalAuthors = _context.ApplicationUsers
                   .Include(a => a.Country)
                   .Include(a => a.University)
                   .Include(a => a.Faculty.Speciality)
                   .Where(a => a.ArName != null && _context.ArticleAuthorships
                   .Include(t => t.Manuscript)
                   .Where(t => t.Manuscript.JournalId == id && t.AuthorUserId == a.Id && _context.Manuscripts.Where(m => m.Id == t.ManuscriptId).Where(m => m.ArAbstract.Contains(ss) || m.EnAbstract.Contains(ss) || m.EnTitle.Contains(ss) || m.ArTitle.Contains(ss)).Count() > 0).Count() > 0),

                    OtherJournalsAuthors = _context.ApplicationUsers
                   .Include(a => a.Country)
                   .Include(a => a.University)
                   .Include(a => a.Faculty.Speciality)
                   .Where(a => a.ArName != null && _context.ArticleAuthorships
                   .Include(t => t.Manuscript)
                   .Where(t => t.Manuscript.JournalId != id && t.AuthorUserId == a.Id && _context.Manuscripts.Where(m => m.Id == t.ManuscriptId).Where(m => m.ArAbstract.Contains(ss) || m.EnAbstract.Contains(ss) || m.EnTitle.Contains(ss) || m.ArTitle.Contains(ss)).Count() > 0).Count() > 0),

                    Users = _context.ApplicationUsers
                   .Include(a => a.Country)
                   .Include(a => a.Faculty.Speciality)
                   .Include(a => a.University)
                   .Where(a => a.Faculty.Speciality.Name.Contains(ss)
                   || a.Summary.Contains(ss)
                   || _context.EducationalLevels
                   .Where(m => m.ArCertificateName.Contains(ss) && m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicPositions
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicActivities
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss) || m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.TeachingExperiences
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss) || m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Posts
                   .Where(m => m.Title.Contains(ss) || m.File.Contains(ss) || m.Body.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Publications
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0),

                    EducationalLevels = _context.EducationalLevels.Include(e => e.AcademicDegree).Include(e => e.ApplicationUser).Include(e => e.Speciality),

                    Journal = await _context.Journals
                   .Include(j => j.CreatedByUser)
                   .Include(j => j.EiC)
                   .Include(j => j.Publisher)
                   .SingleOrDefaultAsync(m => m.Id == id),
                };
            }
            else
            {
                JournalVM = new JournalViewModel()
                {

                    Journal = await _context.Journals
.Include(j => j.CreatedByUser)
.Include(j => j.EiC)
.Include(j => j.Publisher)
.SingleOrDefaultAsync(m => m.Id == id),
                };

            }


            _context.SaveChanges();
            return View(JournalVM);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Home(string id)
        {
            var j = await _context.Journals.Include(a => a.CreatedByUser)
                      .SingleOrDefaultAsync(m => m.ShortName == id);

            if (HttpContext.Session.GetString("JId") != id)
            {
                j.Visitors++;
                HttpContext.Session.SetString("JId", id);
            }

            _context.SaveChanges();

            _context.SaveChanges();
            ViewData["journalid"] = j.Id;
            ViewData["jshortname"] = id;
            var journalViewModel = new JournalViewModel
            {
                AreaEditors = _context.AreaEditors.Include(a => a.Journal).Where(a => a.JournalId == j.Id).OrderBy(a => a.Id),
                ManuscriptsManuscripts = _context.Manuscripts.Where(a => a.JournalId == j.Id).Include(a => a.Journal).Include(a => a.JournalArticleType).Include(a => a.CorrespondingAuthor),
                SubmissionReviewers = _context.SubmissionReviewers.Include(a => a.Submission.Manuscript.Journal).Where(a => a.Submission.Manuscript.JournalId == j.Id),
                Journal = await _context.Journals.Include(a => a.Publisher)
                     .SingleOrDefaultAsync(m => m.ShortName == id),
                JournalIssues = _context.JournalIssues
                     .Where(m => m.JournalId == j.Id && m.IsPublished == true).ToList(),
                JournalPages = _context.JournalPages
                     .Where(m => m.JournalId == j.Id).ToList(),
            };
            return View(journalViewModel);
        }

        // GET: Journals/Create
        [Authorize(Roles = RoleName.Admins)]
        public IActionResult Create()
        {
            ViewData["ApplicantUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["EditorinChiefId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName");
            return View();
        }

        // POST: Journals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArName,EnName,ArDescription,EnDescription,ShortName,Logo,PISSN,EISSN,Email,ActivationDate,CreatedByUserId,EiCId,JournalStatus,Visitors,ReviewerCertificateBackgroundFile,JournalHeader,AuthorCertificateBackgroundFile,PublisherId")] Journal journal, IFormFile myfile, IFormFile myfile1, IFormFile myfile2, IFormFile myfile3)
        {
            if (ModelState.IsValid)
            {
                journal.Logo = await UserFile.UploadeNewImageAsync(journal.Logo,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                journal.JournalHeader = await UserFile.UploadeNewImageAsync(journal.JournalHeader,
myfile1, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                journal.ReviewerCertificateBackgroundFile = await UserFile.UploadeNewFileAsync(journal.ReviewerCertificateBackgroundFile,
myfile2, _environment.WebRootPath, Properties.Resources.Images);

                journal.AuthorCertificateBackgroundFile = await UserFile.UploadeNewFileAsync(journal.AuthorCertificateBackgroundFile,
myfile3, _environment.WebRootPath, Properties.Resources.Images);

                journal.CreatedByUserId = _userManager.GetUserId(User);
                journal.ArDescription = journal.ArDescription.Replace("\n", "<br/>");
                journal.EnDescription = journal.EnDescription.Replace("\n", "<br/>");
                journal.ActivationDate = DateTime.Now;
                journal.JournalStatus = JournalStatus.UnderReview;


                _context.Add(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicantUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.CreatedByUserId);
            ViewData["EditorinChiefId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.EiCId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", journal.PublisherId);
            return View(journal);
        }

        // GET: Journals/Edit/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals.SingleOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }
            ViewData["ApplicantUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.CreatedByUserId);
            ViewData["EditorinChiefId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.EiC);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", journal.PublisherId);
            return View(journal);
        }

        // POST: Journals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArName,EnName,ArDescription,EnDescription,ShortName,Logo,PISSN,EISSN,Email,ActivationDate,CreatedByUserId,EiCId,JournalStatus,Visitors,ReviewerCertificateBackgroundFile,JournalHeader,AuthorCertificateBackgroundFile,PublisherId")] Journal journal, IFormFile myfile, IFormFile myfile1, IFormFile myfile2, IFormFile myfile3)
        {
            if (id != journal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                journal.Logo = await UserFile.UploadeNewImageAsync(journal.Logo,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                journal.JournalHeader = await UserFile.UploadeNewImageAsync(journal.JournalHeader,
myfile1, _environment.WebRootPath, Properties.Resources.Images, 500, 500);

                journal.ReviewerCertificateBackgroundFile = await UserFile.UploadeNewFileAsync(journal.ReviewerCertificateBackgroundFile,
myfile2, _environment.WebRootPath, Properties.Resources.Images);

                journal.AuthorCertificateBackgroundFile = await UserFile.UploadeNewFileAsync(journal.AuthorCertificateBackgroundFile,
myfile3, _environment.WebRootPath, Properties.Resources.Images);



                try
                {
                    _context.Update(journal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalExists(journal.Id))
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
            ViewData["ApplicantUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.CreatedByUserId);
            ViewData["EditorinChiefId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", journal.EiC);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "ArName", journal.PublisherId);
            return View(journal);
        }

        // GET: Journals/Delete/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .Include(j => j.CreatedByUser)
                .Include(j => j.EiC)
                .Include(j => j.Publisher)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // POST: Journals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journal = await _context.Journals.SingleOrDefaultAsync(m => m.Id == id);
            //  var journalarticletype =  _context.JournalArticleTypes.Where(m => m.JournalId == id);

            //   _context.Journals.RemoveRange(journalarticletype);
            _context.Journals.Remove(journal);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalExists(int id)
        {
            return _context.Journals.Any(e => e.Id == id);
        }

        //public FileResult ImageExport(int? id)
        //{
        //    var journals = _context.Journals
        //        .Include(a=>a.EiC)
        //      .SingleOrDefault(m => m.Id == id);


        //    var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        //    var imageUrl = baseUrl + "/ProfileImages/" + journals.EiC.ProfileImage;
        //    var imageFilePath = Path.Combine(_environment.WebRootPath, "ExportImage", "background.jpg");
        //    Image profileImage;
        //    using (WebClient client = new WebClient())
        //    {
        //        Stream stream = client.OpenRead(imageUrl);
        //        profileImage = Image.FromStream(stream);
        //    }

        //    Bitmap newBitmap;
        //    using (var bitmap = (Bitmap)Image.FromFile(imageFilePath))//load the image file
        //    {
        //        using (Graphics graphics = Graphics.FromImage(bitmap))
        //        {
        //            graphics.DrawRectangle(new Pen(Color.FromArgb(242, 228, 125), 3), 1033, 148, 103, 103); // Saif:  whats for ?
        //            graphics.DrawImage(profileImage, 1035, 150, 100, 100);
        //            using (var sf = new StringFormat()
        //            {
        //                Alignment = StringAlignment.Center,
        //                LineAlignment = StringAlignment.Center
        //            })
        //            {
        //                SolidBrush textBrush = new SolidBrush(Color.FromArgb(27, 62, 142));
        //                graphics.DrawString(journals.ShortName, new Font("Verdana", 12, FontStyle.Bold), textBrush, 1000, 255);
        //                using (Font arialFont = new Font("Traditional Arabic", 16, FontStyle.Bold))
        //                {
        //                    StringFormat arabic = (StringFormat)sf.Clone();
        //                    arabic.FormatFlags = StringFormatFlags.DirectionRightToLeft;
        //                    graphics.DrawString(journals.EnName, arialFont, Brushes.Red, 800, 525, arabic);
        //                    //graphics.DrawString(export.LectureName, arialFont, textBrush, 1000, 575, sf);
        //                    //graphics.DrawString(export.date.ToShortDateString(), arialFont, textBrush, 1000, 650, sf);
        //                    //graphics.DrawString(" الساعة " + export.date.ToShortTimeString() + " مساء ", arialFont, textBrush, 1000, 725, arabic);
        //                   graphics.DrawString(journals.EiC.ArName, arialFont, textBrush, 1000, 803, arabic);

        //                }
        //            }

        //        }
        //        newBitmap = new Bitmap(bitmap);
        //    }

        //    var imageArray = newBitmap.ToByteArray();
        //    newBitmap.Dispose();
        //    return File(imageArray, "image/jpeg", DateTime.Now.Ticks + ".jpeg");
        //}

        // https://code-maze.com/create-pdf-dotnetcore/
        //public async Task<FileResult> PDFExport(int? id)
        //{
        //    var journals = _context.Journals
        //          .Include(a => a.EiC)
        //        .SingleOrDefault(m => m.Id == id);


        //    Export export = new Export();
        //    export.LectureName = journals.ArName;
        //    //export.date = lecture.Date;
        //    export.Profile = journals.EiC.ProfileImage;
        //    //export.Name = lecture.Instructor.ArName;
        //    //export.Venue = lecture.Branch.Address;
        //    export.Venue = "test Venue";
        //    var pdfHtml = HtmlHelper.ToHtml(this, "PDFExport", export);
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        //    var htmlForWkhtml = Regex.Replace(pdfHtml.ToString(), "<head>", string.Format("<head><base href=\"{0}\" />", baseUrl), RegexOptions.IgnoreCase);

        //    var htmlToPdf = new HtmlToPdfDocument
        //    {
        //        GlobalSettings =
        //        {
        //            ColorMode=ColorMode.Color,
        //            ImageQuality=100,
        //            Margins=new MarginSettings(0,0,0,0),
        //            Orientation=Orientation.Landscape,
        //            PaperSize=PaperKind.A5
        //        },
        //        Objects =
        //        {
        //            new ObjectSettings
        //            {
        //                PagesCount = true,
        //                HtmlContent=htmlForWkhtml
        //            }
        //        }
        //    };
        //    var pdfArray = _converter.Convert(htmlToPdf);
        //    return File(pdfArray, "application/pdf", DateTime.Now.Ticks + ".pdf");
        //}
    }
}
