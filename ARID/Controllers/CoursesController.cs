using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 10;

        public CoursesController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
        }

        // GET: Courses
        public IActionResult Index(string SearchString, int productPage = 1)
        {
            CourseViewModel CourseVM = new CourseViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                    CourseRegisterations = _context.CourseRegisterations.Include(c => c.ApplicationUser).Include(c => c.Course).Where(c => c.ApplicationUserId == _userManager.GetUserId(User)),
                };
            }
            else if (SearchString == "featured")
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.IsFeatured == true).Include(c => c.ApplicationUser).Include(c => c.Speciality),
                    CourseRegisterations = _context.CourseRegisterations.Include(c => c.ApplicationUser).Include(c => c.Course).Where(c => c.ApplicationUserId == _userManager.GetUserId(User)),
                };
            }
            else if (SearchString == "accepted")
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.IsAdminApproved == false).Include(c => c.ApplicationUser).Include(c => c.Speciality),
                    CourseRegisterations = _context.CourseRegisterations.Include(c => c.ApplicationUser).Include(c => c.Course).Where(c => c.ApplicationUserId == _userManager.GetUserId(User)),
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.ArName.Contains(SearchString) || a.EnName.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Effort.Contains(SearchString) || a.Overview.Contains(SearchString) || a.Introduction.Contains(SearchString) || a.Tags.Contains(SearchString) || a.LearningOutcomes.Contains(SearchString) || a.Requirements.Contains(SearchString) || a.TargetStudents.Contains(SearchString) || a.Speciality.Name.Contains(SearchString)).Include(p => p.ApplicationUser).Include(p => p.Speciality).Include(p => p.ApplicationUser),
                    CourseRegisterations = _context.CourseRegisterations.Include(c => c.ApplicationUser).Include(c => c.Course).Where(c => c.ApplicationUserId == _userManager.GetUserId(User)),
                };
            }

            var count = CourseVM.Courses.Count();
            CourseVM.Courses = CourseVM.Courses.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CourseVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };



            //var aRIDDbContext = _context.Courses.Include(c => c.ApplicationUser).Include(c => c.Speciality);
            return View(CourseVM);
        }

        // GET: Courses
        public IActionResult MyCourses(string SearchString, int productPage = 1)
        {
            CourseViewModel CourseVM = new CourseViewModel();
            if (string.IsNullOrEmpty(SearchString))
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(c => c.Id != 0 && _context.CourseRegisterations.Where(r => r.ApplicationUserId == _userManager.GetUserId(User) && r.CourseId == c.Id).Any()),
                    
                };
            }
            else if (SearchString == "featured")
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.IsFeatured == true).Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(c => c.Id != 0 && _context.CourseRegisterations.Where(r => r.ApplicationUserId == _userManager.GetUserId(User) && r.CourseId == c.Id).Any()),
                };
            }
            else if (SearchString == "accepted")
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.IsAdminApproved == false).Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(c => c.Id != 0 && _context.CourseRegisterations.Where(r => r.ApplicationUserId == _userManager.GetUserId(User) && r.CourseId == c.Id).Any()),
                };
            }
            else if (!string.IsNullOrEmpty(SearchString))
            {
                CourseVM = new CourseViewModel()
                {
                    Courses = _context.Courses.Where(a => a.ArName.Contains(SearchString) || a.EnName.Contains(SearchString) || a.ApplicationUser.ArName.Contains(SearchString) || a.Effort.Contains(SearchString) || a.Overview.Contains(SearchString) || a.Introduction.Contains(SearchString) || a.Tags.Contains(SearchString) || a.LearningOutcomes.Contains(SearchString) || a.Requirements.Contains(SearchString) || a.TargetStudents.Contains(SearchString) || a.Speciality.Name.Contains(SearchString)).Include(p => p.ApplicationUser).Include(p => p.Speciality).Include(p => p.ApplicationUser).Where(c => c.Id != 0 && _context.CourseRegisterations.Where(r => r.ApplicationUserId == _userManager.GetUserId(User) && r.CourseId == c.Id).Any()),
                };
            }

            var count = CourseVM.Courses.Count();
            CourseVM.Courses = CourseVM.Courses.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            CourseVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };



            //var aRIDDbContext = _context.Courses.Include(c => c.ApplicationUser).Include(c => c.Speciality);
            return View(CourseVM);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["CoursId"] = id;

            CourseViewModel CourseVM = new CourseViewModel()
            {
                CourseRegisterations = _context.CourseRegisterations
                .Include(f => f.ApplicationUser)
                 .Include(f => f.Course)
                .Where(f => f.ApplicationUserId == _userManager.GetUserId(User))
                .ToList(),

                //              CourseSponsors = _context.CourseSponsors
                // .Include(f => f.Course)
                //.Where(f => f.CourseId == id)
                //.ToList(),

                CourseChapters = _context.CourseChapters
              .Include(f => f.Course)
              .Where(f => f.CourseId == id)
              .ToList().OrderBy(c => c.Indx),

                CourseChapterContents = _context.CourseChapterContents
              .Include(f => f.CourseChapter.Course)
              .Where(f => f.CourseChapter.CourseId == id)
              .ToList().OrderBy(c => c.Indx),

                //              ApplicationUser = await _userManager.Users
                //.Include(c => c.Country)
                //.Include(c => c.City)
                //.Include(c => c.University)
                //.Include(c => c.Faculty)
                //.Include(a => a.ReferredBy)
                //.SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),

                Course = await _context.Courses.Include(a => a.ApplicationUser).Include(a => a.Speciality)
                   .SingleOrDefaultAsync(m => m.Id == id),

            };
            return View(CourseVM);
        }
        // GET: Courses/Details/5
        public async Task<IActionResult> Learn(int? id)
        {
            ViewData["CoursId"] = id;

            CourseViewModel CourseVM = new CourseViewModel()
            {
                CourseRegisterations = _context.CourseRegisterations
                .Include(f => f.ApplicationUser)
                 .Include(f => f.Course)
                .Where(f => f.ApplicationUserId == _userManager.GetUserId(User))
                .ToList(),

                //              CourseSponsors = _context.CourseSponsors
                // .Include(f => f.Course)
                //.Where(f => f.CourseId == id)
                //.ToList(),

                CourseChapters = _context.CourseChapters
              .Include(f => f.Course)
              .Where(f => f.CourseId == id)
              .ToList().OrderBy(c => c.Indx),

                CourseChapterContents = _context.CourseChapterContents
              .Include(f => f.CourseChapter.Course)
              .Where(f => f.CourseChapter.CourseId == id)
              .ToList().OrderBy(c => c.Indx),

                CourseChapterExams = _context.CourseChapterExams
              .Include(f => f.CourseChapter.Course)
              .Where(f => f.CourseChapter.CourseId == id)
              .ToList().OrderBy(c => c.Indx),

                CourseChapterChoices = _context.CourseChapterChoices
              .Include(f => f.CourseChapterExam.CourseChapter)
              .Where(f => f.CourseChapterExam.CourseChapter.CourseId == id)
              .ToList(),

                //              ApplicationUser = await _userManager.Users
                //.Include(c => c.Country)
                //.Include(c => c.City)
                //.Include(c => c.University)
                //.Include(c => c.Faculty)
                //.Include(a => a.ReferredBy)
                //.SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User)),

                Course = await _context.Courses.Include(a => a.ApplicationUser).Include(a => a.Speciality)
                   .SingleOrDefaultAsync(m => m.Id == id),

            };
            return View(CourseVM);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArName,EnName,ApplicationUserId,StartingDate,EndingDate,DateOfRecord,LastUpdate,Image,IntroductoryVideo,Flyer,FilePdf,Effort,IsPaid,CourseFees,Certificate,CertificateCost,IsAdminApproved,IsActive,IsFeatured,Overview,Introduction,LearningOutcomes,Requirements,Tags,SpecialityId,TargetStudents,ImportantDates,Language,IsPrerecorded,IsDeleted,Ishidden,IsReported,IsPrivate,AccessPassword,PassingMark")] Course course, IFormFile myfile, IFormFile myfile1, IFormFile myfile2, IFormFile myfile3)
        {
            if (ModelState.IsValid)
            {
                course.ApplicationUserId = _userManager.GetUserId(User);
                course.DateOfRecord = DateTime.Now;
                course.Image = await UserFile.UploadeNewImageAsync(course.Image,
myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                course.Flyer = await UserFile.UploadeNewImageAsync(course.Flyer,
myfile1, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                course.FilePdf = await UserFile.UploadeNewFileAsync(course.FilePdf,
          myfile2, _environment.WebRootPath, Properties.Resources.ScientificEvent);
                course.IntroductoryVideo = await UserFile.UploadeNewFileAsync(course.IntroductoryVideo,
          myfile3, _environment.WebRootPath, Properties.Resources.ScientificEvent);
                course.Effort = course.Effort.Replace("\n", "<br/>");
                course.Overview = course.Overview.Replace("\n", "<br/>");
                course.Introduction = course.Introduction.Replace("\n", "<br/>");
                course.LearningOutcomes = course.LearningOutcomes.Replace("\n", "<br/>");
                course.Requirements = course.Requirements.Replace("\n", "<br/>");
                course.ImportantDates = course.ImportantDates.Replace("\n", "<br/>");
                course.IsActive = true;



                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", course.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", course.SpecialityId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.Include(a => a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", course.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", course.SpecialityId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArName,EnName,ApplicationUserId,StartingDate,EndingDate,DateOfRecord,LastUpdate,Image,IntroductoryVideo,Flyer,FilePdf,Effort,IsPaid,CourseFees,Certificate,CertificateCost,IsAdminApproved,IsActive,IsFeatured,Overview,Introduction,LearningOutcomes,Requirements,Tags,SpecialityId,TargetStudents,ImportantDates,Language")] Course course, IFormFile myfile, IFormFile myfile1, IFormFile myfile2)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    course.LastUpdate = DateTime.Now;
                    course.Image = await UserFile.UploadeNewImageAsync(course.Image,
    myfile, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                    course.Flyer = await UserFile.UploadeNewImageAsync(course.Flyer,
    myfile1, _environment.WebRootPath, Properties.Resources.ScientificEvent, 500, 500);
                    course.FilePdf = await UserFile.UploadeNewFileAsync(course.FilePdf,
              myfile2, _environment.WebRootPath, Properties.Resources.ScientificEvent);


                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", course.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", course.SpecialityId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.ApplicationUser)
                .Include(c => c.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        [Authorize]
        public IActionResult Reg(int id)
        {
            Course Course = _context.Courses.Where(m => m.Id == id).SingleOrDefault();
            if (Course == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);

            if (_context.CourseRegisterations.Where(f => f.CourseId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                _context.CourseRegisterations.Add(new CourseRegisteration
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = currentuserid,
                    CourseId = id,
                    JoinDate = DateTime.Now
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult CancelReg(int id)
        {
            Course Course = _context.Courses.Where(m => m.Id == id).SingleOrDefault();
            if (Course == null)
            {
                return NotFound();
            }

            if (_context.CourseRegisterations.Where(f => f.CourseId == id && f.ApplicationUserId == _userManager.GetUserId(User)).Count() > 0)
            {
                var Registered = _context.CourseRegisterations.SingleOrDefault(f => f.CourseId == id && f.ApplicationUserId == _userManager.GetUserId(User));
                _context.CourseRegisterations.Remove(Registered);
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        public async Task<IActionResult> AddChapter(string name, int index, int coid)
        {
            if (name != null && index != 0 && coid != 0)
            {
                _context.CourseChapters.Add(new CourseChapter
                {
                    Name = name,
                    Indx = index,
                    CourseId = coid,
                    IsDeleted = false,
                    Ishidden = false,
                });
            }
            _context.SaveChanges();

            return RedirectToAction("Details", "Courses", new { id = coid });
        }
        public async Task<IActionResult> AddChapterContent(IFormFile file, string title, string desc, string duration, string vimeo, string youtube, string link, bool isfree, bool isdownload, int indx, ContentType contenttype, Guid coursechid)
        {
            if (contenttype == ContentType.file)
            {

                _context.CourseChapterContents.Add(new CourseChapterContent
                {
                    Title = title,
                    FilePath = await UserFile.UploadeNewFileAsync("", file, _environment.WebRootPath, Properties.Resources.Secured),
                    Description = desc,
                    Duration = duration,
                    Indx = indx,
                    ContentType = contenttype,
                    CourseChapterId = coursechid,
                    IsFree = isfree,
                    IsDownloadable = isdownload,
                    IsDeleted = false,
                    Ishidden = false,
                    IsReported = false,
                });
            }
            else
            {
                if (youtube != null)
                {
                    int position = youtube.LastIndexOf("=");
                    _context.CourseChapterContents.Add(new CourseChapterContent
                    {
                        FilePath = youtube.Substring(position + 1),
                        Title = title,
                        Description = desc,
                        Duration = duration,
                        Indx = indx,
                        ContentType = contenttype,
                        CourseChapterId = coursechid,
                        IsFree = isfree,
                        IsDownloadable = isdownload,
                        IsDeleted = false,
                        Ishidden = false,
                        IsReported = false,
                    });

                }
                else if(vimeo !=null)
                {
                    int position = vimeo.LastIndexOf("/");
                    _context.CourseChapterContents.Add(new CourseChapterContent
                    {
                        FilePath=vimeo.Substring(position + 1),
                        Title = title,
                        Description = desc,
                        Duration = duration,
                        Indx = indx,
                        ContentType = contenttype,
                        CourseChapterId = coursechid,
                        IsFree = isfree,
                        IsDownloadable = isdownload,
                        IsDeleted = false,
                        Ishidden = false,
                        IsReported = false,
                    });
                }
                else
                {
                    _context.CourseChapterContents.Add(new CourseChapterContent
                    {
                        FilePath = link,
                        Title = title,
                        Description = desc,
                        Duration = duration,
                        Indx = indx,
                        ContentType = contenttype,
                        CourseChapterId = coursechid,
                        IsFree = isfree,
                        IsDownloadable = isdownload,
                        IsDeleted = false,
                        Ishidden = false,
                        IsReported = false,
                    });

                }
            }
            _context.SaveChanges();

            return RedirectToAction("Details", "FreelancerProjects", new { id = coursechid });
        }



    }
}
