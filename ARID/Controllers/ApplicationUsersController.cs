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
using Microsoft.AspNetCore.Authorization;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ARID.DTOs;
using System.Linq.Dynamic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using ARID.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Encodings.Web;
using ReflectionIT.Mvc.Paging;
using cloudscribe.Pagination.Models;
using ARID.Messages.Models;


namespace ARID.Controllers
{
    public class ApplicationUsersController : Controller
    {

        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;


        public ApplicationUsersController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userMrg;
            _environment = environment;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> ControlPanel(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
            .SingleOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            //await _signInManager.SignInAsync(user, isPersistent: false);
            await _signInManager.SignInAsync(applicationUser, isPersistent: true);
            //if (result.IsCompletedSuccessfully)
            //{

            //  return RedirectToLocal(returnUrl);
            return RedirectToAction("Index", "Home");
            //}

            //   return View(applicationUser);
        }



        public async Task<IActionResult> Index(int id)
        {
            var aRIDDbContext = _context.ApplicationUsers.Where(a => a.CountryId == id).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
            return View(await aRIDDbContext.ToListAsync());
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FilterBy(int? countryId, int? cityId, int? universityId, int? facultyId, int? specialityId, string speciality)
        {

            if (countryId != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.CountryId == countryId).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = _context.Countries.SingleOrDefault(a => a.Id == countryId).ArCountryName;
                return View(await aRIDDbContext.ToListAsync());
            }

            if (cityId != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.CityId == cityId).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = _context.Cities.SingleOrDefault(a => a.Id == cityId).ArCityName;
                return View(await aRIDDbContext.ToListAsync());
            }

            if (universityId != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.UniversityId == universityId).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = _context.Universities.SingleOrDefault(a => a.Id == universityId).ArUniversityName;
                return View(await aRIDDbContext.ToListAsync());
            }

            if (facultyId != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.FacultyId == facultyId).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = _context.Faculties.SingleOrDefault(a => a.Id == facultyId).ArFacultyName;
                return View(await aRIDDbContext.ToListAsync());
            }

            if (specialityId != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.Faculty.SpecialityId == specialityId).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = "تخصص " + _context.Specialities.SingleOrDefault(a => a.Id == specialityId).Name;
                return View(await aRIDDbContext.ToListAsync());
            }


            if (speciality != null)
            {
                var aRIDDbContext = _context.ApplicationUsers.Where(a => a.Faculty.ArFacultyName.Contains(speciality)).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
                ViewData["EntityName"] = "تخصص " + speciality;
                return View(await aRIDDbContext.ToListAsync());
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FilterBySpeciality(int specialityId, string specialityAr, string specialityEn)
        //string speciality
        {
            //var aRIDDbContext = _context.ApplicationUsers.Where(a => a.Faculty.SpecialityId == specialityId || a.Faculty.ArFacultyName.Contains(specialityAr) || a.Faculty.EnFacultyName.Contains(specialityEn)).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);

            //if (aRIDDbContext != null)
            //{
            var aRIDDbContext2 = _context.ApplicationUsers.Where(a => a.Faculty.SpecialityId == specialityId || a.Faculty.ArFacultyName.Contains(specialityAr) || a.Faculty.EnFacultyName.Contains(specialityEn)).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
            return View(await aRIDDbContext2.ToListAsync());
            //}
            //|| a.Faculty.ArFacultyName.Contains(speciality)
            //ViewData["EntityName"] = "تخصص " + speciality;
            //return View(await aRIDDbContext.ToListAsync());
        }



        public async Task<IActionResult> Last100()
        {

            var aRIDDbContext = _context.ApplicationUsers.OrderByDescending(a => a.RegDate).Take(100).Include(u => u.Country).Include(u => u.City).Include(u => u.University).Include(u => u.Faculty);
            return View(await aRIDDbContext.ToListAsync());
        }


        //public async Task<IActionResult> UpdateVisitors()
        //{
        //    var FollowedUsers = _context.Follows.Select(a => a.FollowedUserId);
        //    foreach (string UserId in FollowedUsers)
        //    {
        //        var UserDetails = _context.Users
        //                .SingleOrDefault(m => m.Id == UserId);
        //        UserDetails.Visitors = UserDetails.Visitors + 25;
        //    }
        //    _context.SaveChanges();
        //    return View();
        //}





        public async Task<IActionResult> GenerateARID()
        {
            var Users = _context.Users.Where(a => a.ARID == 0 & a.ArName != null).Select(a => a.Id);
            int maxARID = _context.ApplicationUsers.Max(a => a.ARID);
            foreach (string UserId in Users)
            {
                var UserDetails = _context.Users
                        .SingleOrDefault(m => m.Id == UserId);

                maxARID = maxARID + 1;
                UserDetails.ARID = maxARID;
                UserDetails.ARIDDate = DateTime.Now;
                UserDetails.Status = StatusType.Active;

            }
            _context.SaveChanges();
            return View();
        }




        public async Task<IActionResult> FastSearch(string keyword)
        {
            //var ARIDDbContext = _context.ApplicationUsers.Where(a =>
            //                        a.ARID.ToString().ToLower().Contains(keyword.ToLower()) ||
            //                        a.ArName.ToLower().Contains(keyword.ToLower()) ||
            //                              a.Email.ToLower().Contains(keyword.ToLower()) ||
            //                        //  a.EnName.ToLower().Contains(keyword.ToLower()) ||
            //                        a.University.ArUniversityName.ToLower().Contains(keyword.ToLower()) ||
            //                        //  a.University.EnUniversityName.ToLower().Contains(keyword.ToLower()) ||
            //                        a.Faculty.ArFacultyName.ToLower().Contains(keyword.ToLower()) ||
            //                        //  a.Faculty.EnFacultyName.ToLower().Contains(keyword.ToLower()) ||
            //                        a.Country.ArCountryName.ToLower().Contains(keyword.ToLower()) ||
            //                        //  a.Country.EnCountryName.ToLower().Contains(keyword.ToLower()) ||
            //                        a.City.ArCityName.ToLower().Contains(keyword.ToLower()) //||
            //                                                                                //  a.City.EnCityName.ToLower().Contains(keyword.ToLower())
            //                      );

            var ARIDDbContext = _context.ApplicationUsers.Where(a => a.Email.Contains(keyword) || a.Id == keyword || a.EnName.Contains(keyword) || a.ArName.Contains(keyword) || a.PhoneNumber.Contains(keyword) || a.SecondEmail.Contains(keyword) || a.ARID.ToString().Contains(keyword)).Include(a => a.Country).Include(a => a.City).Include(a => a.University).Include(a => a.Faculty);

            return View(await ARIDDbContext.ToListAsync());
            // return View(await ARIDDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult InviteFriend()

        {
            return View();
        }

        [HttpPost]
        public IActionResult InviteFriend(string email, string name = "")
        {
            //-----------------------------------email content-----------------------------------------
            var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
            Welcome.AppendFormat(string.Format(name));
            Welcome.Append("</h3>");
            Welcome.Append("</br>");

            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
            Footer.Append("<h3 align ='right'>");
            Footer.AppendFormat(string.Format(applicationuser.ArName));
            Footer.Append("</h3>");

            EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "96fde3bb-4c9f-4c2d-babe-564f366eb1ec").SingleOrDefault();
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, content2.ArSubject, content2.ArContent), TimeSpan.FromSeconds(1));


            //         StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Account/Register/' >"));
            //MyStringBuilder.Append("اضغط هنا للتسجيل في منصة اريد ");
            //MyStringBuilder.Append("</a></h2>");

            string link = "https://portal.arid.my/ar-LY/Account/Register/" + applicationuser.Id;

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, applicationuser.ArName + " يدعوك للتسجيل في منصة أُريد للعلماء والخبراء والباحثين", Welcome.ToString() + content2.ArContent +
                   $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط التسجيل في منصة أُريد </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));

            //-----------------------------------email content-----------------------------------------
            ViewData["true"] = "تم إرسال الدعوة بنجاح";

            return View();
        }



        // GET: RegisteredByMe
        public async Task<IActionResult> RegisteredByMe()
        {
            var currentuser = await _userManager.GetUserAsync(User);//currentuser.City.CountryId
            var aRIDDbContext = _context.ApplicationUsers
                .Include(u => u.Faculty)
                .Include(u => u.University)
                .Include(u => u.City)
                .Include(u => u.Country)
                .Where(u => u.ReferredById == currentuser.Id);
            return View(await aRIDDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Badges(int id, int pageNumber = 1, int pageSize = 10)
        {
            var ExcludedRecordes = (pageSize * pageNumber) - pageSize;
            ViewData["BadgeName"] = _context.Badges.SingleOrDefault(a => a.Id == id).ArBadgeName;
            var GetAllBadges = _context.UserBadges.Where(a => a.BadgeId == id && a.IsGranted == true).Select(a => a.UserId);
            var aRIDDbContext = _context.Users.Where(r => GetAllBadges.Contains(r.Id))
                .Include(u => u.Faculty)
                .Include(u => u.University)
                .Include(u => u.City)
                .Include(u => u.Country)
                .Skip(ExcludedRecordes)
                .Take(pageSize);
            var result = new PagedResult<ApplicationUser>
            {
                Data = aRIDDbContext.AsNoTracking().ToList(),
                TotalItems = _context.Users.Where(r => GetAllBadges.Contains(r.Id)).Count(),
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
            return View(result);
        }

        // GET: ApplicationUsers
        //[Authorize(Roles = RoleName.Admins)]
        //public async Task<IActionResult> Index()
        //{
        //    var applicationUsers = _userManager.Users
        //        .Include(c => c.Country)
        //        .Include(c => c.City)
        //        .Include(c => c.University)
        //        .Include(c => c.Faculty)
        //        .Include(a => a.ReferredBy);
        //    return View(await applicationUsers.ToListAsync());
        //}

        [Authorize(Roles = RoleName.Admins)]
        public IActionResult AssignARID(string id)
        {
            ApplicationUser applicationUser = _context.ApplicationUsers.Where(m => m.Id == id).SingleOrDefault();
            if ((applicationUser != null) && (id != null))
            {
                if (applicationUser.ARID == 0)
                {
                    int maxARID = _context.ApplicationUsers.Max(a => a.ARID);
                    applicationUser.ARID = maxARID + 1;
                    applicationUser.ARIDDate = DateTime.Now;
                    applicationUser.Status = StatusType.Active;

                    _context.SaveChanges();
                }
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult Follow(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString() == "d3499ff5-0bd8-4990-a27c-3d020e230c1f").SingleOrDefault();
            ApplicationUser applicationUser = _context.ApplicationUsers.Where(m => m.Id == id).SingleOrDefault();
            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationUser.Email, content.ArSubject, content.ArContent), TimeSpan.FromSeconds(10));
            //_emailSender.SendEmailAsync("info@filspay.com", content.ArSubject, content.ArContent);
            //_emailSender.SendEmailAsync(applicationUser.Email, content.ArSubject, content.ArContent);

            if (applicationUser == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);

            if (_context.Follows.Where(f => f.FollowedUserId == id && f.UserId == currentuserid).Count() == 0)
            {
                _context.Follows.Add(new Follow
                {
                    UserId = currentuserid,
                    FollowedUserId = id
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult Unfollow(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser applicationUser = _context.ApplicationUsers.Where(m => m.Id == id).SingleOrDefault();
            if (applicationUser == null)
            {
                return NotFound();
            }

            if (_context.Follows.Where(f => f.FollowedUserId == id).Count() > 0)
            {
                var followeds = _context.Follows.Where(f => f.FollowedUserId == id && f.UserId == _userManager.GetUserId(User)).ToList();

                foreach (var item in followeds)
                {
                    _context.Follows.Remove(item);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Details/" + id);
        }

        // GET: ApplicationUsers
        [AllowAnonymous]
        public ViewResult Search()
        {
            return View();
        }

        [Route("api/ApplicationUsers")]
        [AllowAnonymous]
        public IActionResult GetApplicationUsers()
        {
            DataTableProperties DTP = DataTableProperties.GetDataTableProperties(Request);

            // Getting all ApplicationUsers data  
            List<ApplicationUser> users = _context.ApplicationUsers
                                .Include(a => a.City)
                                .Include(a => a.Country)
                                .Include(a => a.Faculty)
                                .Include(a => a.University)
                                .Include(a => a.Faculty.Speciality)
                                .OrderByDescending(a => a.RegDate)
                                .ToList();

            // Sorting
            // To use OrderBy by passing strings you must install wilx.System.Linq.Dynamic.Core
            // and must use using System.Linq.Dynamic;
            if (!string.IsNullOrEmpty(DTP.SortColumn) && !string.IsNullOrEmpty(DTP.SortColumnDirection))
            {
                users = users.OrderBy(DTP.SortColumn + " " + DTP.SortColumnDirection).ToList();
            }

            // Searching 
            if (!string.IsNullOrEmpty(DTP.SearchValue))
            {
                users = users.Where(a =>
                                    a.ToString().ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     a.ArName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     //  a.EnName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     a.University.ArUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    // a.University.EnUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.Faculty.ArFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     // a.Faculty.EnFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     a.Country.ArCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     //  a.Country.EnCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     a.City.ArCityName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                     //  a.City.EnCityName.ToLower().Contains(DTP.SearchValue.ToLower())
                                     a.Email.ToLower().Contains(DTP.SearchValue.ToLower())
                                  ).OrderByDescending(a => a.RegDate)
                                  .ToList();
            }

            // Total number of rows count   
            DTP.RecordsTotal = users.Count();

            // Paging   
            users = users.Skip(DTP.Skip).Take(DTP.PageSize).ToList();

            List<ApplicationUserDTO> usersDTO = new List<ApplicationUserDTO>();
            foreach (var user in users)
            {
                usersDTO.Add(new ApplicationUserDTO
                {
                    Id = user.Id,
                    ARID = Common.ARIDFormat(user.ARID),
                    ArName = user.ArName,
                    EnName = user.EnName,
                    Faculty = user.Faculty.ArFacultyName,
                    University = user.University.ArUniversityName,
                    Country = user.Country.ArCountryName,
                    City = user.City.ArCityName
                });
            }

            // Returning Json Data  
            return Json(new { draw = DTP.Draw, recordsFiltered = DTP.RecordsTotal, recordsTotal = DTP.RecordsTotal, data = usersDTO });
        }



        [Authorize(Roles = RoleName.Admins)]
        public ViewResult AdminSearch()
        {
            return View();
        }

        [Route("api/FullApplicationUsers")]
        [Authorize(Roles = RoleName.Admins)]
        public IActionResult GetFullApplicationUsers()
        {
            DataTableProperties DTP = DataTableProperties.GetDataTableProperties(Request);

            // Getting all ApplicationUsers data  
            List<ApplicationUser> users = _context.ApplicationUsers
                                .Include(a => a.City)
                                .Include(a => a.Country)
                                .Include(a => a.Faculty)
                                .Include(a => a.University)
                                .OrderByDescending(a => a.RegDate)
                                .ToList();

            // Sorting
            // To use OrderBy by passing strings you must install wilx.System.Linq.Dynamic.Core
            // and must use using System.Linq.Dynamic;
            if (!string.IsNullOrEmpty(DTP.SortColumn) && !string.IsNullOrEmpty(DTP.SortColumnDirection))
            {
                users = users.OrderBy(DTP.SortColumn + " " + DTP.SortColumnDirection).ToList();
            }

            // Searching 
            if (!string.IsNullOrEmpty(DTP.SearchValue))
            {
                users = users.Where(a =>
                                    a.ARID.ToString().ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.ArName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                          a.Email.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    //  a.EnName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.University.ArUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    //  a.University.EnUniversityName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.Faculty.ArFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    //  a.Faculty.EnFacultyName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.Country.ArCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    //  a.Country.EnCountryName.ToLower().Contains(DTP.SearchValue.ToLower()) ||
                                    a.City.ArCityName.ToLower().Contains(DTP.SearchValue.ToLower()) //||
                                                                                                    //  a.City.EnCityName.ToLower().Contains(DTP.SearchValue.ToLower())
                                  ).ToList();
            }

            // Total number of rows count   
            DTP.RecordsTotal = users.Count();

            // Paging   
            users = users.Skip(DTP.Skip).Take(DTP.PageSize).ToList();

            List<ApplicationUserFullDTO> usersDTO = new List<ApplicationUserFullDTO>();
            foreach (var user in users)
            {
                usersDTO.Add(new ApplicationUserFullDTO
                {
                    Id = user.Id,
                    RegDate = user.RegDate.Date,
                    ARID = Common.ARIDFormat(user.ARID),
                    ArName = user.ArName,
                    EnName = user.EnName,
                    Faculty = user.Faculty.ArFacultyName,
                    University = user.University.ArUniversityName,
                    Country = user.Country.ArCountryName,
                    City = user.City.ArCityName,
                    Status = Enum.GetName(typeof(StatusType), user.Status),
                    EmailConfirmed = user.EmailConfirmed.ToString(),
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed.ToString(),
                    TwoFactorEnabled = user.TwoFactorEnabled.ToString()
                });
            }

            // Returning Json Data  
            return Json(new { draw = DTP.Draw, recordsFiltered = DTP.RecordsTotal, recordsTotal = DTP.RecordsTotal, data = usersDTO });
        }

        public async Task<ProfileViewModel> CreateProfile(string id)
        {
            var applicationUser = _context.Users.SingleOrDefault(u => u.Id == id);
            var universityid = applicationUser.UniversityId;

            var profileViewModel = new ProfileViewModel
            {
                ApplicationUser = await _userManager.Users
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.University)
                .Include(c => c.Faculty)
                .Include(a => a.ReferredBy)
                .SingleOrDefaultAsync(u => u.Id == id),

                Likes = _context.PostMetrics.Where(p => p.ApplicationUserId == id).Count(),

                UserSkills = _context.UserSkills
                .Include(a => a.Skill)
                .Where(a => a.ApplicationUserId == id)
                .ToList(),

                ApplicationUsers = _context.Users.Include(a => a.Faculty)
                .Where(a => a.UniversityId == universityid)
                .Take(3)
                .ToList(),

                FreelancerPortfolios = _context.FreelancerPortfolios
                .Include(f => f.ApplicationUser)
                .Where(f => f.ApplicationUserId == id),

                FreelancerRatings = _context.FreelancerRatings
                .Include(f => f.ApplicationUser)
                .Include(f => f.FreelancerProject)
                .Where(f => f.ApplicationUserId == id),


                UserExpertises = _context.UserExpertises
                .Include(a => a.Expertise)
                .Where(a => a.ApplicationUserId == id)
                .ToList(),

                AcademicActivities = _context.AcademicActivities
                .Include(a => a.Country)
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.ActivityDate)
                .ToList(),

                AcademicPositions = _context.AcademicPositions
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.Faculty)
                .Include(a => a.University)
                .Include(a => a.PositionType)
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.FromYear)
                .ToList(),

                EducationalLevels = _context.EducationalLevels
                .Include(a => a.AcademicDegree)
                .Include(a => a.Faculty)
                .Include(a => a.University)
                .Include(a => a.Speciality)
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.ToYear)
                .ToList(),

                ProfileLinks = _context.ProfileLinks
                .Where(a => a.ApplicationUserId == id && a.AccessType == AccessType.ForPublic)
                .ToList(),

                Projects = _context.Projects
                .Include(a => a.Country)
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.FromYear)
                .ToList(),

                Publications = _context.Publications
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.PublicationDate)
                .ToList(),

                TeachingExperiences = _context.TeachingExperiences
                .Where(a => a.ApplicationUserId == id)
                .OrderByDescending(a => a.FromYear)
                .ToList(),

                UserBadges = _context.UserBadges
                .Include(a => a.Badge)
                .Where(a => a.UserId == id)
                .OrderByDescending(a => a.DateofGrant)
                .ToList(),

                postsCount = _context.Posts.Where(a => a.ApplicationUserId == id && a.Community.CommunityType == CommunityType.Personal)
                .Count(),

                Followers = _context.Follows
                .Include(f => f.User.Faculty)
                .Where(f => f.FollowedUserId == id)
                .Take(3)
                .ToList(),

                UserFollowers = _context.Follows
                .Include(f => f.User.Faculty)
                .Where(f => f.FollowedUserId == id)
                .ToList(),

                Followings = _context.Follows
                .Include(f => f.FollowedUser.Faculty)
                .Where(f => f.UserId == id)
                .Take(3)
                .ToList(),

                RegisteredByYou = _context.ApplicationUsers
                .Where(u => u.ReferredById == id)
                .Count(),

                SameUniversityCount = _context.ApplicationUsers
                .Where(u => u.UniversityId == applicationUser.UniversityId)
                .Count(),

                FollowersCount = _context.Follows
                .Where(f => f.FollowedUserId == id)
                .Count(),

                FollowingsCount = _context.Follows
                .Where(f => f.UserId == id)
                .Count(),

            };

            return profileViewModel;
        }



        [AllowAnonymous]
        //[Route("{id?}")]
        //[Route("{culture}/Profile/{id?}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int arid = Common.ARIDUnformat(id);

            if (arid <= 0)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.ARID == arid).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }


            if (HttpContext.Session.GetString("Id") == null)
            {
                user.Visitors++;
                HttpContext.Session.SetString("Id", user.Id);
            }

            _context.SaveChanges();
            //BackgroundJob.Schedule(() => _context.SaveChanges(), TimeSpan.FromSeconds(10));
            //return RedirectToAction("Details", "ApplicationUsers", new { user.Id});
            ViewData["ResearcherName"] = user.ArName + " | " + user.EnName;
            return View("Details", await CreateProfile(user.Id));
        }

        public async Task<IActionResult> PrintProfile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int arid = Common.ARIDUnformat(id);

            if (arid <= 0)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.ARID == arid).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }


            //if (HttpContext.Session.GetString("Id") == null)
            //{
            //    user.Visitors++;
            //    HttpContext.Session.SetString("Id", user.Id);
            //}

            _context.SaveChanges();
            //BackgroundJob.Schedule(() => _context.SaveChanges(), TimeSpan.FromSeconds(10));
            //return RedirectToAction("Details", "ApplicationUsers", new { user.Id});
            ViewData["ResearcherName"] = user.ArName + " | " + user.EnName;
            return View("Print", await CreateProfile(user.Id));
        }

        public async Task<IActionResult> MobileProfile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int arid = Common.ARIDUnformat(id);

            if (arid <= 0)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.ARID == arid).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }


            //if (HttpContext.Session.GetString("Id") == null)
            //{
            //    user.Visitors++;
            //    HttpContext.Session.SetString("Id", user.Id);
            //}

            _context.SaveChanges();
            //BackgroundJob.Schedule(() => _context.SaveChanges(), TimeSpan.FromSeconds(10));
            //return RedirectToAction("Details", "ApplicationUsers", new { user.Id});
            ViewData["ResearcherName"] = user.ArName + " | " + user.EnName;
            return View("Mobile", await CreateProfile(user.Id));
        }


        [Authorize]
        public async Task<IActionResult> ResearchersSearch(string ss)
        {


            JournalViewModel JournalVM = new JournalViewModel();

            if (ss != null)
            {
                JournalVM = new JournalViewModel()
                {
                    Users = _context.ApplicationUsers
                   .Where(a => a.Faculty.Speciality.Name.Contains(ss)
                   || a.Summary.Contains(ss)
                   || a.ArName.Contains(ss)
                   || a.EnName.Contains(ss)
                   || _context.EducationalLevels
                   .Where(m => m.ArCertificateName.Contains(ss) && m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicPositions
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicActivities
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.TeachingExperiences
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss) || m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Posts
                   .Where(m => m.Title.Contains(ss) || m.Body.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Publications
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0),

                };
            }

            _context.SaveChanges();
            return View(JournalVM);
        }

        [Authorize]
        public async Task<IActionResult> ResearchersSendEmails(string ss, string message)
        {
            JournalViewModel JournalVM = new JournalViewModel();
            if (ss != null)
            {
                JournalVM = new JournalViewModel()
                {
                    Users = _context.ApplicationUsers
                   .Where(a => a.Faculty.Speciality.Name.Contains(ss)
                   || a.Summary.Contains(ss)
                   || a.ArName.Contains(ss)
                   || a.EnName.Contains(ss)
                   || _context.EducationalLevels
                   .Where(m => m.ArCertificateName.Contains(ss) && m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicPositions
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.AcademicActivities
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.TeachingExperiences
                   .Where(m => m.ArDescription.Contains(ss) || m.EnDescription.Contains(ss) || m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Posts
                   .Where(m => m.Title.Contains(ss) || m.Body.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0
                   || _context.Publications
                   .Where(m => m.ArTitle.Contains(ss) || m.EnTitle.Contains(ss)).Where(m => m.ApplicationUserId == a.Id).Count() > 0),

                };
            }

            if (message != null)
            {

            }

            return View(JournalVM);
        }

        // GET: ApplicationUsers/Details/5
        [AllowAnonymous]
        //[Route("{culture}/ApplicationUsers/Details/{id?}")]
        public async Task<IActionResult> Mobile(string id, int pid)

        {
            if (id == null)
            {
                return NotFound();
            }

            ProfileViewModel profileViewModel = await CreateProfile(id);

            if (!string.IsNullOrEmpty(profileViewModel.ApplicationUser.CVURL)
                && !string.IsNullOrWhiteSpace(profileViewModel.ApplicationUser.CVURL))
                ViewData["CVURL"] = Path.Combine(_environment.WebRootPath, Properties.Resources.CVFileFolder, profileViewModel.ApplicationUser.CVURL);
            else
                ViewData["CVURL"] = "لايوجد ملف سيرة ذاتية";

            if (profileViewModel.ApplicationUser == null)
            {
                return NotFound();
            }

            if (profileViewModel == null)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.Id == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Id") == null)
            {
                user.Visitors++;
                HttpContext.Session.SetString("Id", user.Id);
            }
            _context.SaveChanges();


            //الجزء الخاص بطلب البحث
            string currentuserid = _userManager.GetUserId(User);
            ApplicationUser applicationUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == currentuserid);
            if (pid != 0)
            {
                Publication publication = _context.Publications.SingleOrDefault(p => p.Id == pid);
                //-----------------------------------email content-----------------------------------------
                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
                Welcome.AppendFormat(string.Format(user.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
                Footer.Append("<h3 align ='right'>");
                Footer.AppendFormat(string.Format(applicationUser.ArName));
                Footer.Append("</h3>");

                EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "f3c08827-afc2-4289-a517-f990911e21e6").SingleOrDefault();
                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, content2.ArSubject, content2.ArContent), TimeSpan.FromSeconds(1));
                //MyStringBuilder.Append("اضغط هنا للتسجيل في منصة اريد ");
                //MyStringBuilder.Append("</a></h2>");

                string link = "https://portal.arid.my/ar-LY/Publications/Edit/" + pid;

                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(user.Email, "طلب البحث الخاص بكم في منصة أريد", Welcome.ToString() + content2.ArContent +
                    applicationUser.ArName + " يدعوكم لإرسال البحث الخاص بك في منصة اريد  " + "و المعنون ب : " + publication.ArTitle +
                       $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط رفع البحث </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));

                //-----------------------------------email content-----------------------------------------
                ViewData["true"] = "تم إرسال الطلب بنجاح";

            }
            return View(profileViewModel);
        }

        // GET: ApplicationUsers/Details/5
        [AllowAnonymous]
        //[Route("{culture}/ApplicationUsers/Details/{id?}")]
        public async Task<IActionResult> FreelancerDetails(string id)

        {

            var p = 0;
            var c = 0;
            var q = 0;
            var e = 0;
            var d = 0;
            var r = 0;

            var freelancerratings = _context.FreelancerRatings.Where(f => f.ApplicationUserId == id);
            foreach (var item in freelancerratings)
            {
                p += item.Professionalism;
                c += item.Communication;
                q += item.Quality;
                e += item.Experience;
                d += item.Delivery;
                r += item.RehirePossibility;
            }

            if (freelancerratings.Count() > 0)
            {
                ViewData["Professionalism"] = p / freelancerratings.Count();
                ViewData["Communication"] = c / freelancerratings.Count();
                ViewData["Quality"] = q / freelancerratings.Count();
                ViewData["Experience"] = e / freelancerratings.Count();
                ViewData["Delivery"] = d / freelancerratings.Count();
                ViewData["RehirePossibility"] = r / freelancerratings.Count();
            }
            if (id == null)
            {
                return NotFound();
            }

            ProfileViewModel profileViewModel = await CreateProfile(id);

            if (!string.IsNullOrEmpty(profileViewModel.ApplicationUser.CVURL)
                && !string.IsNullOrWhiteSpace(profileViewModel.ApplicationUser.CVURL))
                ViewData["CVURL"] = Path.Combine(_environment.WebRootPath, Properties.Resources.CVFileFolder, profileViewModel.ApplicationUser.CVURL);
            else
                ViewData["CVURL"] = "لايوجد ملف سيرة ذاتية";

            if (profileViewModel.ApplicationUser == null)
            {
                return NotFound();
            }

            if (profileViewModel == null)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.Id == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Id") == null)
            {
                user.Visitors++;
                HttpContext.Session.SetString("Id", user.Id);
            }
            _context.SaveChanges();


            return View(profileViewModel);
        }

        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> CheckBadgeRequirements()

        {
            var applicationUser = _context.ApplicationUsers.Where(a => a.Id == _userManager.GetUserId(User));
            var Badges = _context.UserBadges.Select(a => a.UserId);
            //   var applicationUser = _context.ApplicationUsers.Where(a => a.Id != null && _context.UserBadges.Where(b => b.UserId ==a.Id).Count()==0);


            //  foreach (var item in applicationUser.Where(a=>a.Id==_userManager.GetUserId(User)))
            foreach (var item in applicationUser)
            {
                if (item.ProfileImage == null || item.CVURL == null || item.Summary == null || item.UniversityId == 0 || item.CountryId == 0 || item.FacultyId == 0 || item.ContactMeDetail == null || _context.EducationalLevels.Where(e => e.ApplicationUserId == item.Id).Count() == 0 || _context.AcademicPositions.Where(e => e.ApplicationUserId == item.Id).Count() == 0 || _context.TeachingExperiences.Where(e => e.ApplicationUserId == item.Id).Count() == 0 || _context.Publications.Where(e => e.ApplicationUserId == item.Id).Count() == 0 || _context.AcademicActivities.Where(e => e.ApplicationUserId == item.Id).Count() == 0)
                {
                    int count = 0;
                    //-----------------------------------email content-----------------------------------------
                    StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
                    Welcome.AppendFormat(string.Format(item.ArName));
                    Welcome.Append("</h3>");
                    Welcome.Append("</br>");

                    StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
                    Footer.Append("<h3 align ='right'>");
                    Footer.AppendFormat("فريق منصة أريد");
                    Footer.Append("</h3>");

                    StringBuilder Requirements = new StringBuilder("<h3 align ='right'>النقوصات المطلوبة");
                    Requirements.Append("</h3>");

                    EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "f3c08827-afc2-4289-a517-f990911e21e6").SingleOrDefault();

                    string link = "https://portal.arid.my/ar-LY/ApplicationUsers/Details/" + item.Id;

                    //-----------------------------------email content-----------------------------------------

                    if (item.ProfileImage == null)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- الصورة الشخصية");
                        Requirements.Append("</h5>");

                    }
                    if (item.CVURL == null)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- رابط ملف السيرة الذاتية");
                        Requirements.Append("</h5>");
                    }

                    if (item.Summary == null)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- ملخص عن مسيرتك العلمية");
                        Requirements.Append("</h5>");
                    }
                    if (item.UniversityId == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة جامعتك الحالية");
                        Requirements.Append("</h5>");
                    }
                    if (item.CountryId == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة بلدك الحالي");
                        Requirements.Append("</h5>");
                    }
                    if (item.FacultyId == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة قسمك");
                        Requirements.Append("</h5>");
                    }
                    if (item.ContactMeDetail == null)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة معلومات التواصل معك");
                        Requirements.Append("</h5>");
                    }
                    if (_context.EducationalLevels.Where(e => e.ApplicationUserId == item.Id).Count() == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة مؤهلاتك الأكاديمية");
                        Requirements.Append("</h5>");
                    }
                    if (_context.AcademicPositions.Where(e => e.ApplicationUserId == item.Id).Count() == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة المناصب الأكاديمية و الإدارية");
                        Requirements.Append("</h5>");
                    }
                    if (_context.TeachingExperiences.Where(e => e.ApplicationUserId == item.Id).Count() == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة الخبرات التدريسية");
                        Requirements.Append("</h5>");
                    }
                    if (_context.Publications.Where(e => e.ApplicationUserId == item.Id).Count() == 0)
                    {
                        count += 1;
                        Requirements.Append("<h5 align ='right'>" + count + "- إضافة منشوراتك العلمية");
                        Requirements.Append("</h5>");
                    }

                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.Email, "النقوصات الخاصة بوسام باحث مبادر", Welcome.ToString() + content2.ArContent + Requirements.ToString() +
                           $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط صفحتك الشخصية </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));


                }
            }
            return View();
        }

        [AllowAnonymous]
        //[Route("{culture}/ApplicationUsers/Details/{id?}")]
        public async Task<IActionResult> Print(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProfileViewModel profileViewModel = await CreateProfile(id);

            if (!string.IsNullOrEmpty(profileViewModel.ApplicationUser.CVURL)
                && !string.IsNullOrWhiteSpace(profileViewModel.ApplicationUser.CVURL))
                ViewData["CVURL"] = Path.Combine(_environment.WebRootPath, Properties.Resources.CVFileFolder, profileViewModel.ApplicationUser.CVURL);
            else
                ViewData["CVURL"] = "لايوجد ملف سيرة ذاتية";

            if (profileViewModel.ApplicationUser == null)
            {
                return NotFound();
            }

            if (profileViewModel == null)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.Id == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Id") == null)
            {
                user.Visitors++;
                HttpContext.Session.SetString("Id", user.Id);
            }
            _context.SaveChanges();

            return View(profileViewModel);
        }

        [AllowAnonymous]
        //[Route("{culture}/ApplicationUsers/Details/{id?}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProfileViewModel profileViewModel = await CreateProfile(id);

            if (!string.IsNullOrEmpty(profileViewModel.ApplicationUser.CVURL)
                && !string.IsNullOrWhiteSpace(profileViewModel.ApplicationUser.CVURL))
                ViewData["CVURL"] = Path.Combine(_environment.WebRootPath, Properties.Resources.CVFileFolder, profileViewModel.ApplicationUser.CVURL);
            else
                ViewData["CVURL"] = "لايوجد ملف سيرة ذاتية";

            if (profileViewModel.ApplicationUser == null)
            {
                return NotFound();
            }

            if (profileViewModel == null)
            {
                return NotFound();
            }

            ApplicationUser user = _context.ApplicationUsers.Where(a => a.Id == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Id") == null)
            {
                user.Visitors++;
                HttpContext.Session.SetString("Id", user.Id);
            }
            _context.SaveChanges();

            return View(profileViewModel);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> DownloadCV(string filename)
        //{
        //    if (filename == null)
        //        return Content("filename not present");

        //    var path = Path.Combine(_environment.WebRootPath,
        //        Properties.Resources.CVFileFolder, filename);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }

        //    memory.Position = 0;
        //    return File(memory, UserFile.GetContentType(path), Path.GetFileName(path));
        //}

        // GET: ApplicationUsers/Edit/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.Users
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.University)
                .Include(c => c.Faculty)
                .Include(a => a.ReferredBy)
                .SingleOrDefaultAsync(u => u.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == applicationUser.CountryId), "Id", "ArCityName", applicationUser.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", applicationUser.CountryId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == applicationUser.UniversityId), "Id", "ArFacultyName", applicationUser.FacultyId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", applicationUser.UniversityId);
            ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", applicationUser.ReferredById);
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
            ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName");

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Edit(string id, [Bind("ReferredById,ArName,EnName,OtherNames,DateofBirth,Gender,SecondEmail,RegDate,ARID,ARIDDate,Status,UILanguage,ProfileImage,FeaturedImage,CVURL,Summary,ContactMeDetail,CountryId,CityId,UniversityId,FacultyId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,Visitors")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser thisUser = _userManager.FindByIdAsync(applicationUser.Id).Result;
                    thisUser.UserName = applicationUser.UserName;
                    thisUser.Email = applicationUser.Email;
                    //thisUser.RegDate = applicationUser.RegDate; // because it is desabled in the view
                    //thisUser.ARID = applicationUser.ARID; // because it is desabled in the view
                    //thisUser.ARIDDate = applicationUser.ARIDDate; // because it is desabled in the view
                    //thisUser.ReferredById = applicationUser.ReferredById; // because it is desabled in the view
                    thisUser.Status = applicationUser.Status;
                    thisUser.UniversityId = applicationUser.UniversityId;
                    thisUser.FacultyId = applicationUser.FacultyId;
                    thisUser.SecondEmail = applicationUser.SecondEmail;
                    thisUser.PhoneNumber = applicationUser.PhoneNumber;
                    thisUser.ArName = applicationUser.ArName;
                    thisUser.EnName = applicationUser.EnName;
                    thisUser.OtherNames = applicationUser.OtherNames;
                    thisUser.DateofBirth = applicationUser.DateofBirth;
                    thisUser.Gender = applicationUser.Gender;
                    thisUser.UILanguage = applicationUser.UILanguage;
                    thisUser.CountryId = applicationUser.CountryId;
                    thisUser.CityId = applicationUser.CityId;
                    thisUser.ProfileImage = applicationUser.ProfileImage;
                    thisUser.FeaturedImage = applicationUser.FeaturedImage;
                    thisUser.CVURL = applicationUser.CVURL;
                    thisUser.Summary = applicationUser.Summary;
                    thisUser.ContactMeDetail = applicationUser.ContactMeDetail;

                    await _userManager.UpdateAsync(thisUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "ApplicationUsers", new { id });
            }
            ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == applicationUser.CountryId), "Id", "ArCityName", applicationUser.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", applicationUser.CountryId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == applicationUser.UniversityId), "Id", "ArFacultyName", applicationUser.FacultyId);
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName", applicationUser.UniversityId);
            ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", applicationUser.ReferredById);
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
            ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName");

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.University)
                .Include(c => c.Faculty)
                .Include(a => a.ReferredBy)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admins)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            var Follow = _context.Follows.Where(m => m.UserId == applicationUser.Id || m.FollowedUserId == id);
            var AllAcademicActivities = _context.AcademicActivities.Where(m => m.ApplicationUserId == id);
            var AllPublications = _context.Publications.Where(m => m.ApplicationUserId == id);
            var AllUserSkills = _context.UserSkills.Where(m => m.ApplicationUserId == id);
            var AllUserExpertises = _context.UserExpertises.Where(m => m.ApplicationUserId == id);
            var AllUserBadges = _context.UserBadges.Where(m => m.UserId == id);
            var AllTeachingExperiences = _context.TeachingExperiences.Where(m => m.ApplicationUserId == id);
            var AllProjects = _context.Projects.Where(m => m.ApplicationUserId == id);
            var AllProfileLinks = _context.ProfileLinks.Where(m => m.ApplicationUserId == id);
            //var AllNotificationlogs = _context.Notificationlogs.Where(m => m.ApplicationUserId == id);
            var AllGallaries = _context.Gallaries.Where(m => m.ApplicationUserId == id);
            var AllEducationalLevels = _context.EducationalLevels.Where(m => m.ApplicationUserId == id);
            var AllAcademicPositions = _context.AcademicPositions.Where(m => m.ApplicationUserId == id);
            var skills = _context.Skills.Where(m => m.ApplicationUserId == id);
            var RegisteredBy = _context.ApplicationUsers.Where(m => m.ReferredById == id);

            var AllTickets = _context.Tickets.Where(m => m.ApplicationUserId == id);

            foreach (var item in RegisteredBy)
            {
                item.ReferredById = null;
            }

            foreach (var item in AllTickets)
            {
                var AllTicketReplies = _context.TicketReplies.Where(m => m.TicketId == item.Id);
                _context.TicketReplies.RemoveRange(AllTicketReplies);
            }
            //_context.Update(ApplicationUsers);


            _context.Follows.RemoveRange(Follow);
            _context.ApplicationUsers.Remove(applicationUser);
            _context.AcademicActivities.RemoveRange(AllAcademicActivities);
            _context.Publications.RemoveRange(AllPublications);
            _context.UserSkills.RemoveRange(AllUserSkills);
            _context.UserExpertises.RemoveRange(AllUserExpertises);
            _context.UserBadges.RemoveRange(AllUserBadges);
            _context.TeachingExperiences.RemoveRange(AllTeachingExperiences);
            _context.Projects.RemoveRange(AllProjects);
            _context.ProfileLinks.RemoveRange(AllProfileLinks);
            //_context.Notificationlogs.RemoveRange(AllNotificationlogs);
            _context.Gallaries.RemoveRange(AllGallaries);
            _context.Tickets.RemoveRange(AllTickets);
            _context.EducationalLevels.RemoveRange(AllEducationalLevels);
            _context.AcademicPositions.RemoveRange(AllAcademicPositions);
            _context.Skills.RemoveRange(skills);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.ProfileImageFolder, applicationUser.ProfileImage);
            UserFile.DeleteOldFile(_environment.WebRootPath, Properties.Resources.CVFileFolder, applicationUser.CVURL);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = RoleName.Admins)]
        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
        [Authorize]


        public IActionResult PublicationDownloads(int id)
        {
            Publication publication = _context.Publications.SingleOrDefault(p => p.Id == id);
            var user = _context.Users.SingleOrDefault(a => a.Id == publication.ApplicationUserId);
            if (publication == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid != publication.ApplicationUserId)
            {
                publication.DownloadHits = publication.DownloadHits + 1;
                _context.Update(publication);
                _context.SaveChanges();
                return RedirectToAction("Details", "ApplicationUsers", user.Id);
            }
            return RedirectToAction("Details", "ApplicationUsers", user.Id);

            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
        }
        public IActionResult DeletePortfolio(int id)
        {
            FreelancerPortfolio freelancerPortfolio = _context.FreelancerPortfolios.SingleOrDefault(p => p.Id == id);
            if (freelancerPortfolio == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid == freelancerPortfolio.ApplicationUserId)
            {
                _context.FreelancerPortfolios.Remove(freelancerPortfolio);
                _context.SaveChanges();
            }
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return RedirectToAction("Details/" + id);
        }

        public JsonResult SameUniversityUsers(string id)
        {
            var applicationUser = _context.Users.SingleOrDefault(a => a.Id == id);
            var ApplicationUsers = _context.Users.Include(a => a.Faculty).Where(a => a.UniversityId == applicationUser.UniversityId).ToList();
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(new { list = ApplicationUsers });
        }

        public JsonResult FollowersUsers(string id)
        {
            var Followers = _context.Follows
                .Include(f => f.User.Faculty)
                .Include(f => f.User.University)
                .Where(f => f.FollowedUserId == id)
                .ToList();
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(new { list = Followers });
        }

        public JsonResult FollowingsUsers(string uid)
        {
            var Followers = _context.Follows
                .Include(f => f.FollowedUser.Faculty)
                .Include(f => f.FollowedUser.University)
                .Where(f => f.UserId == uid)
                .ToList();
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(new { list = Followers });
        }

        public JsonResult Blogs(string id)
        {
            var Posts = _context.Posts.Where(a => a.ApplicationUserId == id && a.Community.CommunityType == CommunityType.Personal && a.IsHidden == false)
            .OrderByDescending(a => a.DateTime)
            .ToList();
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(new { list = Posts });
        }

        public JsonResult EducationalLevelsDetails(int id)
        {
            var educationallevel = _context.EducationalLevels.Include(e => e.Speciality).Include(e => e.University).Include(e => e.ApplicationUser).Include(e => e.Country).Include(e => e.Faculty).SingleOrDefault(e => e.Id == id);
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(educationallevel);
        }

        public JsonResult AcademicPositionsDetails(int id)
        {
            var AcademicPosition = _context.AcademicPositions.Include(e => e.University).Include(e => e.ApplicationUser).Include(e => e.Country).Include(e => e.Faculty).Include(e => e.PositionType).Include(e => e.City).SingleOrDefault(e => e.Id == id);
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(AcademicPosition);
        }

        public JsonResult TeachingExperiencesDetails(int id)
        {
            var TeachingExperience = _context.TeachingExperiences.Include(e => e.ApplicationUser).SingleOrDefault(e => e.Id == id);
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(TeachingExperience);
        }

        public JsonResult AcademicActivitiesDetails(int id)
        {
            var AcademicActivity = _context.AcademicActivities.Include(e => e.ApplicationUser).Include(e => e.Country).SingleOrDefault(e => e.Id == id);
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(AcademicActivity);
        }


        public JsonResult ProjectsDetails(int id)
        {
            var Project = _context.Projects.Include(e => e.ApplicationUser).Include(e => e.Country).SingleOrDefault(e => e.Id == id);
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return Json(Project);
        }

        public IActionResult ResearchRequest(int pid, string comment)
        {
            string currentuserid = _userManager.GetUserId(User);
            ApplicationUser applicationUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == currentuserid);
            Publication publication = _context.Publications.SingleOrDefault(p => p.Id == pid);
            var user = _context.Users.SingleOrDefault(a => a.Id == publication.ApplicationUserId);
            //-----------------------------------email content-----------------------------------------
            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
            Welcome.AppendFormat(string.Format(user.ArName));
            Welcome.Append("</h3>");
            Welcome.Append("</br>");

            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
            Footer.Append("<h3 align ='right'>");
            Footer.AppendFormat(string.Format(applicationUser.ArName));
            Footer.Append("</h3>");

            string content2 = comment;
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, content2.ArSubject, content2.ArContent), TimeSpan.FromSeconds(1));
            //MyStringBuilder.Append("اضغط هنا للتسجيل في منصة اريد ");
            //MyStringBuilder.Append("</a></h2>");

            string link = "https://portal.arid.my/ar-LY/Publications/Edit/" + pid;

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(user.Email, "طلب البحث الخاص بكم في منصة أريد", Welcome.ToString() + content2 +
                applicationUser.ArName + " يدعوكم لإرسال البحث الخاص بك في منصة اريد  " + "و المعنون ب : " + publication.ArTitle +
                   $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط رفع البحث </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));

            //-----------------------------------email content-----------------------------------------
            return RedirectToAction("", "", new { id = pid });
        }

        public async Task<IActionResult> CreateMessage(IFormFile file, string message, string title, string appuser)
        {
            ApplicationUser currentUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == appuser);
            if (file != null)
                {
                _context.Messages.Add(new Message
                {
                    Content = message,
                    Subject = title,
                    Attachment = await UserFile.UploadeNewFileAsync("", file, _environment.WebRootPath, Properties.Resources.Secured),
                    FromApplicationUserId = _userManager.GetUserId(User),
                    DateOfRecord = DateTime.Now,
                    ToApplicationUserId = appuser,
                    IsDeleted = false,
                    IsRead = false,
                    IsReported = false,
                    LastActivitydate = DateTime.Now,
                    });
                }
                else
                {
                    _context.Messages.Add(new Message
                    {
                        Content = message,
                        Subject = title,
                        FromApplicationUserId = _userManager.GetUserId(User),
                        DateOfRecord = DateTime.Now,
                        ToApplicationUserId = appuser,
                        IsDeleted = false,
                        IsRead = false,
                        IsReported = false,
                        LastActivitydate = DateTime.Now,
            });
                }
                _context.SaveChanges();

            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
            Welcome.AppendFormat(string.Format(currentUser.ArName));
            Welcome.Append("</h3>");
            Welcome.Append("</br>");

            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
            Footer.Append("<h3 align ='right'>فريق منصة اريد");
         
            Footer.Append("</h3>");

            EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "a5a6a6a4-c7f5-46f6-aea7-c27e5ea82d4c").SingleOrDefault();
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, content2.ArSubject, content2.ArContent), TimeSpan.FromSeconds(1));


            //         StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Account/Register/' >"));
            //MyStringBuilder.Append("اضغط هنا للتسجيل في منصة اريد ");
            //MyStringBuilder.Append("</a></h2>");

            string link = "https://portal.arid.my/ar-LY/Account/DAL/" + currentUser.DAL;

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(currentUser.Email, currentUser.ArName + " تلقيت رسالة جديدة عبر منصة اريد بعنوان :" + title, Welcome.ToString() + content2.ArContent +
                   $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط الدخول المباشر الى حسابكم في منصة أُريد </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));

            return RedirectToAction("Details", "FreelancerProjects", new { id = appuser });
        }
    }
}
