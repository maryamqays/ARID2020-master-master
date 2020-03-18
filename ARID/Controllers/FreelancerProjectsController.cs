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
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text;
using Hangfire;
using ARID.Services;
//testing
namespace ARID.Controllers
{
    [Authorize]
    public class FreelancerProjectsController : Controller
    {
        private readonly ARIDDbContext _context;
        private int PagSize = 10;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;
        private readonly IEmailSender _emailSender;

        public FreelancerProjectsController(IHostingEnvironment environment, ARIDDbContext context, UserManager<ApplicationUser> userMrg, IEmailSender emailSender)
        {
            _userManager = userMrg;
            _context = context;
            _environment = environment;
            _emailSender = emailSender;
        }

        public List<CommunityAutofillModel> getSkillsandExpertises()

        {
            List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
            var sa = new JsonSerializerSettings();
            var DbSkills = _context.Skills.Select(a => new { text = a.Name, value = a.Name });
            var DbExpertises = _context.Expertises.Select(a => new { text = a.Name, value = a.Name });

            foreach (var data in DbSkills)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            foreach (var data in DbExpertises)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            return ls;
        }


        // GET: FreelancerProjects
        public IActionResult MyProjects(int productPage = 1)
        {
            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            FreelancerProjectVm = new FreelancerProjectViewModel()
            {
                FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(a => a.ApplicationUserId == _userManager.GetUserId(User)),
                AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.SpecialityId == p.Id).Any()),
            };

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
        }

        // GET: FreelancerProjects
        public IActionResult RejectedProjects(int productPage = 1)
        {
            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            FreelancerProjectVm = new FreelancerProjectViewModel()
            {
                FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(a => a.FreelancerProjectStatus == FreelancerProjectStatus.Rejected),
                AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.SpecialityId == p.Id).Any()),
            };

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
        }

        // GET: FreelancerProjects
        public IActionResult AllOffersProjects(int productPage = 1)
        {
            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            FreelancerProjectVm = new FreelancerProjectViewModel()
            {
                FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f=>f.Id!=0 && _context.FreelancerProposals.Where(p=>p.FreelancerProjectId==f.Id && p.ApplicationUserId==_userManager.GetUserId(User)).Any()),
                AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.SpecialityId == p.Id).Any()),
            };

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
        }

        // GET: FreelancerProjects
        public IActionResult RecentlyProjects(int productPage = 1)
        {
            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            FreelancerProjectVm = new FreelancerProjectViewModel()
            {
                FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f=>f.FreelancerProjectStatus== FreelancerProjectStatus.Execution),
                AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.SpecialityId == p.Id).Any()),
            };

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
        }

        // GET: FreelancerProjects
        public IActionResult IndependentProjects(int productPage = 1)
        {
            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            FreelancerProjectVm = new FreelancerProjectViewModel()
            {
                FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.Id != 0 && _context.FreelancerProposals.Where(a => a.ApplicationUserId == _userManager.GetUserId(User) && a.FreelancerProjectId == f.Id && a.IsAssigned==true).Any()),
                AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
            };

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
        }

        // GET: FreelancerProjects
        public async Task<IActionResult> Index(string ss, string s, string b, int productPage = 1)
        {
            ViewData["search"] = ss;
            ViewData["speciality"] = s;
            ViewData["bugettype"] = b;

            FreelancerProjectViewModel FreelancerProjectVm = new FreelancerProjectViewModel();
            if (string.IsNullOrEmpty(ss) && string.IsNullOrEmpty(s) && string.IsNullOrEmpty(b))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    FreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };
            }
            else if (!string.IsNullOrEmpty(ss) && string.IsNullOrEmpty(s) && string.IsNullOrEmpty(b))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Where(a => a.Name.Contains(ss) || a.Description.Contains(ss) || a.ApplicationUser.ArName.Contains(ss) || a.Speciality.Name.Contains(ss) || a.Skills.Contains(ss)).Include(p => p.ApplicationUser).Include(p => p.Speciality),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };
            }
            else if (!string.IsNullOrEmpty(s) && string.IsNullOrEmpty(b) && string.IsNullOrEmpty(ss))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.Speciality.Name.Contains(s) || a.Speciality.EnSpecialityName.Contains(s)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };
            }
            else if (!string.IsNullOrEmpty(b) && string.IsNullOrEmpty(s) && string.IsNullOrEmpty(ss))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.BugetType.ToString().Contains(b)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };

            }
            else if (!string.IsNullOrEmpty(b) && !string.IsNullOrEmpty(s) && string.IsNullOrEmpty(ss))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.BugetType.ToString().Contains(b) && a.Speciality.Name.Contains(s)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };

            }

            else if (!string.IsNullOrEmpty(b) && !string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(ss))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.BugetType.ToString().Contains(b) && a.Speciality.Name.Contains(s) && a.Name.Contains(ss)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };

            }
            else if (!string.IsNullOrEmpty(b) && !string.IsNullOrEmpty(ss) && string.IsNullOrEmpty(s))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.BugetType.ToString().Contains(b) && a.Name.Contains(ss)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };

            }

            else if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(ss) && string.IsNullOrEmpty(b))
            {
                FreelancerProjectVm = new FreelancerProjectViewModel()
                {
                    AllFreelancerProjects = _context.FreelancerProjects.Include(c => c.ApplicationUser).Include(c => c.Speciality).Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers),
                    FreelancerProjects = _context.FreelancerProjects.Include(f => f.Speciality).Where(a => a.Speciality.Name.Contains(s) && a.Name.Contains(ss)),
                    FreelancerProposals = _context.FreelancerProposals.Include(c => c.ApplicationUser).Include(c => c.FreelancerProject),
                    Specialities = _context.Specialities.Where(p => p.Name != null && _context.FreelancerProjects.Where(f => f.FreelancerProjectStatus == FreelancerProjectStatus.Acceptingoffers && f.SpecialityId == p.Id).Any()),
                };

            }

            var count = FreelancerProjectVm.FreelancerProjects.Count();
            FreelancerProjectVm.FreelancerProjects = FreelancerProjectVm.FreelancerProjects.OrderByDescending(a => a.DateOfRecord)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            FreelancerProjectVm.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };
            return View(FreelancerProjectVm);
            //var aRIDDbContext = _context.FreelancerProjects.Include(f => f.ApplicationUser).Include(f => f.Speciality);
            //return View(await aRIDDbContext.ToListAsync());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("Id,File,FreelancerProjectId,ApplicationUserId,Commentary,DateOfRecord")] FreelancerComment freelancerComment, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                freelancerComment.ApplicationUserId = _userManager.GetUserId(User);
                freelancerComment.DateOfRecord = DateTime.Now;
                freelancerComment.File = await UserFile.UploadeNewFileAsync(freelancerComment.File,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                freelancerComment.Id = Guid.NewGuid();
                _context.Add(freelancerComment);
                await _context.SaveChangesAsync();

                var freelancerproject = _context.FreelancerProjects.Include(a => a.ApplicationUser).SingleOrDefault(f => f.Id == freelancerComment.FreelancerProjectId);
                var freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Include(a => a.FreelancerProject).SingleOrDefault(a => a.FreelancerProjectId == freelancerComment.FreelancerProjectId && a.IsAssigned == true);


                //-----------------------------------email content-----------------------------------------
                var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
                Welcome.AppendFormat(string.Format(applicationuser.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                Welcome.Append("</h3>");
                Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                Welcome.Append("</h3>");


                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "5e478593-035e-4662-9eab-459e76c13d54").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", freelancerComment.FreelancerProjectId));
                MyStringBuilder.Append("اضغط هنا للاطلاع على التفاصيل ");
                MyStringBuilder.Append("</a></h2>");

                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerproject.ApplicationUser.Email, usercontent.ArSubject + freelancerproject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + freelancerComment.Commentary + Footer.ToString()), TimeSpan.FromSeconds(10));
                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerProposal.ApplicationUser.Email, usercontent.ArSubject + freelancerproject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + freelancerComment.Commentary + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------

                return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = freelancerComment.FreelancerProjectId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerComment.ApplicationUserId);
            return View(freelancerComment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal([Bind("Id,ApplicationUserId,IsNameVisible,IsPrivate,IsAssigned,IsReported,Isvisible,ReportedByUserId,Description,DaysRequired,FreelancerProjectId,DateofRecord,DateOfAssignment,DeliveredDate,BidAmount,ProposalStatus")] FreelancerProposal freelancerProposal)
        {
            if (ModelState.IsValid)
            {
                freelancerProposal.ProposalStatus = ProposalStatus.Pending;
                freelancerProposal.ApplicationUserId = _userManager.GetUserId(User);
                freelancerProposal.Description = freelancerProposal.Description.Replace("\n", "<br/>");
                freelancerProposal.DateofRecord = DateTime.Now;
                freelancerProposal.ProposalStatus = ProposalStatus.Pending;
                freelancerProposal.Isvisible = true;
                freelancerProposal.IsNameVisible = true;

                _context.Add(freelancerProposal);

                //-----------------------------------email content-----------------------------------------
                var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
                Welcome.AppendFormat(string.Format(applicationuser.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                Welcome.Append("</h3>");
                Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                Welcome.Append("</h3>");

                FreelancerProject freelancerproject = _context.FreelancerProjects.Include(a => a.ApplicationUser).SingleOrDefault(a => a.Id == freelancerProposal.FreelancerProjectId);
                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "412be09e-94a7-4853-a2fd-4ea6e81234d9").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", freelancerproject.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل العرض");
                MyStringBuilder.Append("</a></h2>");

                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerproject.ApplicationUser.Email, usercontent.ArSubject + freelancerproject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------


                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = freelancerProposal.FreelancerProjectId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ApplicationUserId);
            ViewData["ReportedByUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProposal.ReportedByUserId);
            return View(freelancerProposal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRating([Bind("Id,ApplicationUserId,FreelancerProjectId,Isvisible,DateofRecord,Comment,Professionalism,Communication,Quality,Experience,Delivery,RehirePossibility,FreelancerReadyServiceId")] FreelancerRating freelancerRating)
        {
            if (ModelState.IsValid)
            {
                freelancerRating.DateofRecord = DateTime.Now;
                freelancerRating.Isvisible = true;

                _context.Add(freelancerRating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = freelancerRating.FreelancerProjectId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerRating.ApplicationUserId);
            return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = freelancerRating.FreelancerProjectId });
        }


        // GET: FreelancerProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            FreelancerProposal freelancerProposal;
            var freelancerproject = _context.FreelancerProjects.SingleOrDefault(f => f.Id == id);
            if(freelancerproject.FreelancerProjectStatus!= FreelancerProjectStatus.FreelancerReadyServices &&freelancerproject.FreelancerProjectStatus!= FreelancerProjectStatus.HireMe ) { 
             freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Include(a => a.FreelancerProject).SingleOrDefault(a => a.FreelancerProjectId == id&&a.IsAssigned==true);
            }
            else
            {
                freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Include(a => a.FreelancerProject).SingleOrDefault(a => a.FreelancerProjectId == id);
            }

            if (freelancerproject.FreelancerProjectStatus != FreelancerProjectStatus.Acceptingoffers  && _userManager.GetUserId(User) != freelancerproject.ApplicationUserId && _userManager.GetUserId(User) != freelancerProposal.ApplicationUserId && _context.FreelancerRecruiters.Where(f => f.FreelancerProjectId == id && f.ApplicationUserId == _userManager.GetUserId(User)).Count() == 0 && !User.IsInRole("Admins"))
            {
                return NotFound();
            }
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var freelancerProject = await _context.FreelancerProjects
            //    .Include(f => f.ApplicationUser)
            //    .Include(f => f.Speciality)
            //    .SingleOrDefaultAsync(m => m.Id == id);
            //if (freelancerProject == null)
            //{
            //    return NotFound();
            //}

            //return View(freelancerProject);
            var FreelancerProject = await _context.FreelancerProjects
   .SingleOrDefaultAsync(m => m.Id == id);
            if (_context.FreelancerReadyServices.Where(f => f.Title == FreelancerProject.Name).Any())
            {
                ViewData["srvices"] = "true";
            }
            else
            {
                ViewData["srvices"] = "false";
            }

            if (_context.FreelancerProposals.Where(f => f.IsAssigned == true && f.FreelancerProjectId == id).Any())
            {
                ViewData["assigned"] = "true";
            }
            else
            {
                ViewData["assigned"] = "false";
            }

            ViewData["fpid"] = id;
            FreelancerProjectViewModel FreelancerProjectVM = new FreelancerProjectViewModel()

            {
                FreelancerProject = await _context.FreelancerProjects
                   .Include(f => f.ApplicationUser)
                    .Include(f => f.Speciality)
                   .SingleOrDefaultAsync(m => m.Id == id),
                FreelancerProposals = _context.FreelancerProposals.Where(a => a.FreelancerProjectId == id).Include(a => a.ApplicationUser).Include(a => a.FreelancerProject).Include(a => a.ReportedByUser),
                FreelancerComments = _context.FreelancerComments.Where(a => a.FreelancerProjectId == id).OrderBy(a => a.DateOfRecord).Include(a => a.ApplicationUser).Include(a => a.FreelancerProject),
                FreelancerRatings = _context.FreelancerRatings.Where(a => a.FreelancerProjectId == id).Include(a => a.ApplicationUser).Include(a => a.FreelancerProject),
                FreelancerRecruiters = _context.FreelancerRecruiters.Where(a => a.FreelancerProjectId == id).Include(a => a.ApplicationUser).Include(a => a.FreelancerProject),
                FreelancerReadyServices = _context.FreelancerReadyServices.Include(a => a.ApplicationUser),
                FreelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Include(a => a.FreelancerProject).SingleOrDefault(a => a.FreelancerProjectId == id && a.IsAssigned == true),
            };
            return View(FreelancerProjectVM);

        }

        // GET: FreelancerProjects/Create
        public IActionResult CreateSimilar(int id)
        {
            var freelancerproject = _context.FreelancerProjects.Include(f => f.Speciality).SingleOrDefault(f => f.Id == id);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name", freelancerproject.SpecialityId);

            return View(freelancerproject);
        }

        public IActionResult Create()
        {
            ViewBag.Skills = getSkillsandExpertises();
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");
            return View();
        }

        public IActionResult HireMe(string id)
        {

            ViewBag.Skills = getSkillsandExpertises();
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "Name");

            FreelancerProjectViewModel freelancerProjectVM = new FreelancerProjectViewModel()
            {
                ApplicationUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == id),
                UserSkills = _context.UserSkills
                .Include(a => a.Skill)
                .Where(a => a.ApplicationUserId == id),
            };
            return View(freelancerProjectVM);
        }

        // POST: FreelancerProjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DateOfRecord,ApplicationUserId,File,AdminNotification,SpecialityId,Skills,BugetType,PurposeType,SkillCategoryType,RecruiterProjectType,DaysRequired,FreelancerProjectStatus,HideProjectOwnerDetails")] FreelancerProject freelancerProject, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                freelancerProject.DateOfRecord = DateTime.Now;
                freelancerProject.PurposeType = PurposeType.other;
                freelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Acceptingoffers;
                freelancerProject.ApplicationUserId = _userManager.GetUserId(User);
                freelancerProject.Description = freelancerProject.Description.Replace("\n", "<br/>");
                freelancerProject.File = await UserFile.UploadeNewFileAsync(freelancerProject.File,
myfile, _environment.WebRootPath, Properties.Resources.Secured);


                _context.Add(freelancerProject);
                await _context.SaveChangesAsync();

                //-----------------------------------email content-----------------------------------------
                var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
                Welcome.AppendFormat(string.Format(applicationuser.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                Welcome.Append("</h3>");
                Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                Welcome.Append("</h3>");


                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "73d74c66-3648-48f6-bf2f-9fbb69f596c3").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", freelancerProject.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل المشروع");
                MyStringBuilder.Append("</a></h2>");

                // BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationuser.Email, usercontent.ArSubject + "- رقم المشروع #[" + freelancerProject.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------


                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProject.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", freelancerProject.SpecialityId);
            return View(freelancerProject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProjectAndProposal(Guid id, int d, int p)
        {
            FreelancerReadyService freelancerReadyService = _context.FreelancerReadyServices.Include(a => a.ApplicationUser).SingleOrDefault(f => f.Id == id);
            ApplicationUser applicationUser = _context.ApplicationUsers.SingleOrDefault(f => f.Id == _userManager.GetUserId(User));
            BugetType bugetType = new BugetType();
            if (p <= 30)
            {
                bugetType = BugetType.قليل;
            }
            else if (p <= 100)
            {
                bugetType = BugetType.متوسط;
            }
            else if (p <= 300)
            {
                bugetType = BugetType.عالي;
            }
            if (p != 0 && d != 0 && id != null && applicationUser.Balance >= p)
            {
                _context.FreelancerProjects.Add(new FreelancerProject
                {
                    DateOfRecord = DateTime.Now,
                    Name = freelancerReadyService.Title,
                    Description = freelancerReadyService.Description,
                    File = null,
                    Speciality = _context.Specialities.First(),
                    SkillCategoryType = SkillCategoryType.Others,
                    Skills = freelancerReadyService.Skills,
                    BugetType = bugetType,
                    DaysRequired = d,
                    ApplicationUserId = _userManager.GetUserId(User),
                    PurposeType = PurposeType.other,
                    RecruiterProjectType = RecruiterProjectType.DontAdd,
                    FreelancerProjectStatus = FreelancerProjectStatus.FreelancerReadyServices,
                    FreelancerReadyServiceId = id,
                });
                _context.SaveChanges();

                var countOfRows = _context.FreelancerProjects.Count();
                var lastRow = _context.FreelancerProjects.Skip(countOfRows - 1).FirstOrDefault();
                _context.FreelancerProposals.Add(new FreelancerProposal
                {
                    Description = "",
                    ApplicationUserId = freelancerReadyService.ApplicationUserId,
                    IsAssigned = false,
                    Isvisible = true,
                    IsReported = false,
                    IsPrivate = true,
                    ReportedByUserId = null,
                    DaysRequired = d,
                    FreelancerProjectId = lastRow.Id,
                    DateofRecord = DateTime.Now,
                    BidAmount = p,
                    ProposalStatus = ProposalStatus.Execution,
                });



                _context.SaveChanges();

                //-----------------------------------email content-----------------------------------------
                var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
                Welcome.AppendFormat(string.Format(freelancerReadyService.ApplicationUser.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                Welcome.Append("</h3>");
                Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                Welcome.Append("</h3>");


                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "73d74c66-3648-48f6-bf2f-9fbb69f596c3").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", lastRow.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل المشروع");
                MyStringBuilder.Append("</a></h2>");

                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationuser.Email, usercontent.ArSubject + "- رقم المشروع #[" + lastRow.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------

                //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProject.ApplicationUserId);
                //ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", freelancerProject.SpecialityId);

                return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = lastRow.Id });
            }
            var countOfRs = _context.FreelancerProjects.Count();
            var lastR = _context.FreelancerProjects.Skip(countOfRs - 1).FirstOrDefault();
            return RedirectToAction("Details", "FreelancerReadyServices", new { id = freelancerReadyService.Id });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HireMePost(string id, [Bind("Id,Name,Description,DateOfRecord,ApplicationUserId,File,AdminNotification,SpecialityId,Skills,BugetType,PurposeType,SkillCategoryType,RecruiterProjectType,DaysRequired,FreelancerProjectStatus,HideProjectOwnerDetails,FixedPrice")] FreelancerProject freelancerProject, IFormFile myfile)
        {
            ApplicationUser applicationUser = _context.ApplicationUsers.SingleOrDefault(f => f.Id == id);
            if (ModelState.IsValid)
            {

                _context.FreelancerProjects.Add(new FreelancerProject
                {
                    DateOfRecord = DateTime.Now,
                    Name = freelancerProject.Name,
                    Description = freelancerProject.Description,
                    File = await UserFile.UploadeNewFileAsync(freelancerProject.File,
myfile, _environment.WebRootPath, Properties.Resources.Secured),
                    SpecialityId = freelancerProject.SpecialityId,
                    SkillCategoryType = freelancerProject.SkillCategoryType,
                    Skills = freelancerProject.Skills,
                    FixedPrice = freelancerProject.FixedPrice,
                    DaysRequired = freelancerProject.DaysRequired,
                    ApplicationUserId = _userManager.GetUserId(User),
                    PurposeType = PurposeType.other,
                    RecruiterProjectType = freelancerProject.RecruiterProjectType,
                    FreelancerProjectStatus = FreelancerProjectStatus.HireMe,
                });
                _context.SaveChanges();

                var countOfRows = _context.FreelancerProjects.Count();
                var lastRow = _context.FreelancerProjects.Skip(countOfRows - 1).FirstOrDefault();
                _context.FreelancerProposals.Add(new FreelancerProposal
                {
                    Description = "",
                    ApplicationUserId = id,
                    IsAssigned = false,
                    Isvisible = true,
                    IsReported = false,
                    IsPrivate = true,
                    ReportedByUserId = null,
                    DaysRequired = freelancerProject.DaysRequired,
                    FreelancerProjectId = lastRow.Id,
                    DateofRecord = DateTime.Now,
                    BidAmount = freelancerProject.FixedPrice,
                    ProposalStatus = ProposalStatus.Execution,
                });



                _context.SaveChanges();

                //-----------------------------------email content-----------------------------------------
                var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == id);

                StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
                Welcome.AppendFormat(string.Format(applicationuser.ArName));
                Welcome.Append("</h3>");
                Welcome.Append("</br>");

                StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                Welcome.Append("</h3>");
                Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                Welcome.Append("</h3>");


                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "73d74c66-3648-48f6-bf2f-9fbb69f596c3").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", lastRow.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل المشروع");
                MyStringBuilder.Append("</a></h2>");

                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationuser.Email, usercontent.ArSubject + "- رقم المشروع #[" + lastRow.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------

                //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProject.ApplicationUserId);
                //ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", freelancerProject.SpecialityId);

                return RedirectToAction("Details", "FreelancerProjects", new {/* routeValues, for example: */ id = lastRow.Id });
            }
            return View(applicationUser);


        }

        // GET: FreelancerProjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerProject = await _context.FreelancerProjects.SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerProject == null || FreelancerProject.GetHours(freelancerProject.DateOfRecord) > 24)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a=>a.Id==freelancerProject.ApplicationUserId), "Id", "ArName", freelancerProject.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", freelancerProject.SpecialityId);
            return View(freelancerProject);
        }

        // POST: FreelancerProjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DateOfRecord,ApplicationUserId,File,AdminNotification,SpecialityId,Skills,BugetType,PurposeType,SkillCategoryType,RecruiterProjectType,DaysRequired,FreelancerProjectStatus,HideProjectOwnerDetails,FixedPrice")] FreelancerProject freelancerProject, IFormFile myfile)
        {
            if (id != freelancerProject.Id )
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    freelancerProject.File = await UserFile.UploadeNewFileAsync(freelancerProject.File,
myfile, _environment.WebRootPath, Properties.Resources.Secured);

                    _context.Update(freelancerProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerProjectExists(freelancerProject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details" , "FreelancerProjects", new { id=freelancerProject.Id});
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerProject.ApplicationUserId);
            ViewData["SpecialityId"] = new SelectList(_context.Specialities, "Id", "EnSpecialityName", freelancerProject.SpecialityId);
            return View(freelancerProject);
        }

        // GET: FreelancerProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerProject = await _context.FreelancerProjects
                .Include(f => f.ApplicationUser)
                .Include(f => f.Speciality)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerProject == null)
            {
                return NotFound();
            }

            return View(freelancerProject);
        }

        // POST: FreelancerProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancerProject = await _context.FreelancerProjects.SingleOrDefaultAsync(m => m.Id == id);
            _context.FreelancerProjects.Remove(freelancerProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerProjectExists(int id)
        {
            return _context.FreelancerProjects.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Accept(int id)
        {
            FreelancerProposal FreelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Where(m => m.Id == id).SingleOrDefault();
            FreelancerProject FreelancerProject = _context.FreelancerProjects.Include(f => f.ApplicationUser).Where(m => m.Id == FreelancerProposal.FreelancerProjectId).SingleOrDefault();
            var FreelancerProposals = _context.FreelancerProposals.Where(a => a.FreelancerProjectId == FreelancerProject.Id);
            if (FreelancerProposal == null)
            {
                return NotFound();
            }
            if (FreelancerProject.ApplicationUser.Balance >= FreelancerProposal.BidAmount)
            {
                foreach (var item in FreelancerProposals.Where(a => a.Id != id))
                {
                    item.ProposalStatus = ProposalStatus.Execluded;
                }

                FreelancerProposal.IsAssigned = true;
                FreelancerProposal.DateOfAssignment = DateTime.Now;
                FreelancerProposal.ProposalStatus = ProposalStatus.Execution;
                FreelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Execution;

                //Add statement
                _context.Statements.Add(new Statement
                {
                    Amount = FreelancerProposal.BidAmount,
                    RecordDate = DateTime.Now,
                    Destination = false,
                    ApplicationUserId = _userManager.GetUserId(User),
                    Details = "Freelancer project #" + FreelancerProject.Id,
                    BalanceType = BalanceType.currentbalance
                });

                _context.Statements.Add(new Statement
                {
                    Amount = FreelancerProposal.BidAmount,
                    RecordDate = DateTime.Now,
                    Destination = true,
                    ApplicationUserId = FreelancerProposal.ApplicationUserId,
                    Details = "Freelancer project #" + FreelancerProject.Id,
                    BalanceType = BalanceType.holdingbalance
                });

                //Execute credit/debit
                var FreelancerProjectuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == FreelancerProject.ApplicationUserId);
                var FreelancerProposaluser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == FreelancerProposal.ApplicationUserId);

                FreelancerProjectuser.Balance = FreelancerProjectuser.Balance - FreelancerProposal.BidAmount;
                FreelancerProposaluser.HoldingBalance += FreelancerProposal.BidAmount;

                //-----------------------------------email content-----------------------------------------
                //var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                //StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
                //Welcome.AppendFormat(string.Format(applicationuser.ArName));
                //Welcome.Append("</h3>");
                //Welcome.Append("</br>");

                //StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                //Welcome.Append("</h3>");
                //Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

                //Welcome.Append("</h3>");


                //EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "4a9811a7-e626-41b1-bd21-31415bfc7f8a").SingleOrDefault();
                //StringBuilder MyStringBuilder = new StringBuilder("");
                //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", FreelancerProject.Id));
                //MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل المشروع الممنوح لكم");
                //MyStringBuilder.Append("</a></h2>");

                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(FreelancerProposaluser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

                //-----------------------------------email content-----------------------------------------

                //_context.Update(FreelancerProposal);
                //_context.Update(FreelancerProject);
                _context.SaveChanges();
                return RedirectToAction("Details/" + FreelancerProposal.FreelancerProjectId);

            }
            return RedirectToAction("Details/" + FreelancerProposal.FreelancerProjectId);
        }

        [Authorize]
        public IActionResult AcceptService(int id, string description)
        {
            FreelancerProposal FreelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).Where(m => m.Id == id).SingleOrDefault();
            FreelancerProject FreelancerProject = _context.FreelancerProjects.Include(f => f.ApplicationUser).Where(m => m.Id == FreelancerProposal.FreelancerProjectId).SingleOrDefault();
            var FreelancerProposals = _context.FreelancerProposals.Where(a => a.FreelancerProjectId == FreelancerProject.Id);
            if (FreelancerProposal == null)
            {
                return NotFound();
            }
            //if (FreelancerProject.ApplicationUser.Balance >= FreelancerProposal.BidAmount)
            //{
            foreach (var item in FreelancerProposals)
            {
                item.ProposalStatus = ProposalStatus.Execluded;
            }

            FreelancerProposal.IsAssigned = true;
            FreelancerProposal.DateOfAssignment = DateTime.Now;
            FreelancerProposal.ProposalStatus = ProposalStatus.Execution;
            FreelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Execution;

            //Add statement
            _context.Statements.Add(new Statement
            {
                Amount = FreelancerProposal.BidAmount,
                RecordDate = DateTime.Now,
                Destination = false,
                ApplicationUserId = _userManager.GetUserId(User),
                Details = "Freelancer project #" + FreelancerProject.Id,
                BalanceType = BalanceType.currentbalance
            });

            _context.Statements.Add(new Statement
            {
                Amount = FreelancerProposal.BidAmount,
                RecordDate = DateTime.Now,
                Destination = true,
                ApplicationUserId = FreelancerProposal.ApplicationUserId,
                Details = "Freelancer project #" + FreelancerProject.Id,
                BalanceType = BalanceType.holdingbalance
            });

            //Execute credit/debit
            var FreelancerProjectuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == FreelancerProject.ApplicationUserId);
            var FreelancerProposaluser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == FreelancerProposal.ApplicationUserId);

            FreelancerProjectuser.Balance = FreelancerProjectuser.Balance - FreelancerProposal.BidAmount;
            FreelancerProposaluser.HoldingBalance += FreelancerProposal.BidAmount;

            ////-----------------------------------email content-----------------------------------------
            //var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            //StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
            //Welcome.AppendFormat(string.Format(applicationuser.ArName));
            //Welcome.Append("</h3>");
            //Welcome.Append("</br>");

            //StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            //Welcome.Append("</h3>");
            //Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            //Welcome.Append("</h3>");


            //EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "4a9811a7-e626-41b1-bd21-31415bfc7f8a").SingleOrDefault();
            //StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", FreelancerProject.Id));
            //MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل المشروع الممنوح لكم");
            //MyStringBuilder.Append("</a></h2>");

            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(FreelancerProjectuser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

            ////-----------------------------------email content-----------------------------------------

            _context.Update(FreelancerProposal);
            _context.Update(FreelancerProject);
            _context.SaveChanges();
            //}
            var freelancerproposal = _context.FreelancerProposals.SingleOrDefault(f => f.Id == id);
            freelancerproposal.Description = description;
            _context.Update(freelancerproposal);
            _context.SaveChanges();



            return RedirectToAction("Details", "FreelancerProjects", new { id = freelancerproposal.FreelancerProjectId });

        }

        [Authorize]
        public IActionResult Delivered(int id)
        {
            FreelancerProposal FreelancerProposal = _context.FreelancerProposals.Where(m => m.Id == id).SingleOrDefault();
            FreelancerProject freelancerProject = _context.FreelancerProjects.Include(a => a.ApplicationUser).Where(m => m.Id == FreelancerProposal.FreelancerProjectId).SingleOrDefault();
            if (FreelancerProposal == null)
            {
                return NotFound();
            }
            FreelancerProposal.ProposalStatus = ProposalStatus.Delivered;
            freelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Delivered;
            _context.Update(FreelancerProposal);

            //-----------------------------------email content-----------------------------------------
            var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
            Welcome.AppendFormat(string.Format(applicationuser.ArName));
            Welcome.Append("</h3>");
            Welcome.Append("</br>");

            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            Welcome.Append("</h3>");
            Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            Welcome.Append("</h3>");


            EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "3b418ec1-c78b-48d3-bf5b-10bddda23811").SingleOrDefault();
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", freelancerProject.Id));
            MyStringBuilder.Append("اضغط هنا للاطلاع على المشروع ");
            MyStringBuilder.Append("</a></h2>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerProject.ApplicationUser.Email, usercontent.ArSubject + freelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

            //-----------------------------------email content-----------------------------------------

            _context.SaveChanges();
            return RedirectToAction("Details/" + FreelancerProposal.FreelancerProjectId);
        }

        [Authorize]
        public IActionResult ProjectAccept(int id)
        {
            FreelancerProject FreelancerProject = _context.FreelancerProjects.Include(a => a.ApplicationUser).Where(m => m.Id == id).SingleOrDefault();
            FreelancerProposal freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).SingleOrDefault(m => m.FreelancerProjectId == FreelancerProject.Id);
            ApplicationUser FreelancerProjectapplicationUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == FreelancerProject.ApplicationUserId);
            ApplicationUser freelancerProposalapplicationUser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == freelancerProposal.ApplicationUserId);

            if (FreelancerProject == null)
            {
                return NotFound();
            }

            //Add statement
            _context.Statements.Add(new Statement
            {
                Amount = freelancerProposal.BidAmount,
                RecordDate = DateTime.Now,
                Destination = false,
                ApplicationUserId = freelancerProposal.ApplicationUserId,
                Details = "Payment released for Freelancer project #" + FreelancerProject.Id,
                BalanceType = BalanceType.holdingbalance
            });

            _context.Statements.Add(new Statement
            {
                Amount = freelancerProposal.BidAmount,
                RecordDate = DateTime.Now,
                Destination = true,
                ApplicationUserId = freelancerProposal.ApplicationUserId,
                Details = "Payment received for Freelancer project #" + FreelancerProject.Id,
                BalanceType = BalanceType.currentbalance
            });

            //FreelancerProjectapplicationUser.Balance -= freelancerProposal.BidAmount;
            freelancerProposalapplicationUser.Balance += freelancerProposal.BidAmount;
            freelancerProposalapplicationUser.HoldingBalance -= freelancerProposal.BidAmount;
            FreelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Completed;
            _context.Update(FreelancerProject);
            // _context.Update(FreelancerProjectapplicationUser);
            _context.Update(freelancerProposalapplicationUser);
            _context.SaveChanges();

            //-----------------------------------email content-----------------------------------------
            var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
            Welcome.AppendFormat(string.Format(applicationuser.ArName));
            Welcome.Append("</h3>");
            Welcome.Append("</br>");

            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            Welcome.Append("</h3>");
            Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            Welcome.Append("</h3>");


            EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "88e07c53-d8ce-488b-bc2f-fe5e39dea879").SingleOrDefault();
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", FreelancerProject.Id));
            MyStringBuilder.Append("اضغط هنا لتقييم العمل ");
            MyStringBuilder.Append("</a></h2>");

            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(FreelancerProject.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));
            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerProposal.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

            //-----------------------------------email content-----------------------------------------


            return RedirectToAction("Details/" + FreelancerProject.Id);
        }
        [Authorize]
        public IActionResult ProjectReject(int id, string adminNotificationfromprojectowner, string notificationfromfreelancer)
        {
            FreelancerProject FreelancerProject = _context.FreelancerProjects.Include(a => a.ApplicationUser).Where(m => m.Id == id).SingleOrDefault();
            FreelancerProposal freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).SingleOrDefault(m => m.FreelancerProjectId == FreelancerProject.Id && m.IsAssigned == true);
            if (FreelancerProject == null)
            {
                return NotFound();
            }
            if (adminNotificationfromprojectowner != null)
            {
                FreelancerProject.AdminNotification += "<br/>" + FreelancerProject.ApplicationUser.ArName;
                FreelancerProject.AdminNotification += ":" + adminNotificationfromprojectowner + ".";
                FreelancerProject.FreelancerProjectStatus = FreelancerProjectStatus.Rejected;
                _context.Update(FreelancerProject);
                _context.SaveChanges();
            }
            else if (notificationfromfreelancer != null)
            {
                FreelancerProject.AdminNotification += "<br/>" + freelancerProposal.ApplicationUser.ArName;
                FreelancerProject.AdminNotification += ":" + notificationfromfreelancer + ".";
                _context.Update(FreelancerProject);
                _context.SaveChanges();
            }

            ////-----------------------------------email content-----------------------------------------
            //var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            //StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
            //Welcome.AppendFormat(string.Format(applicationuser.ArName));
            //Welcome.Append("</h3>");
            //Welcome.Append("</br>");

            //StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            //Welcome.Append("</h3>");
            //Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            //Welcome.Append("</h3>");


            //EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "199b4975-0233-4abe-b3d3-9c9934efac88").SingleOrDefault();
            //StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", FreelancerProject.Id));
            //MyStringBuilder.Append("تفاصيل المشروع ");
            //MyStringBuilder.Append("</a></h2>");

            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(FreelancerProject.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerProposal.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

            ////-----------------------------------email content-----------------------------------------


            return RedirectToAction("Details/" + FreelancerProject.Id);
        }
        public IActionResult DaysExtension(int id, int extension)
        {
            FreelancerProject FreelancerProject = _context.FreelancerProjects.Include(a => a.ApplicationUser).Where(m => m.Id == id).SingleOrDefault();
            FreelancerProposal freelancerProposal = _context.FreelancerProposals.Include(a => a.ApplicationUser).SingleOrDefault(m => m.FreelancerProjectId == FreelancerProject.Id && m.IsAssigned == true);
            if (FreelancerProject == null)
            {
                return NotFound();
            }
            freelancerProposal.DaysRequired += extension;
            _context.Update(freelancerProposal);
            _context.SaveChanges();

            ////-----------------------------------email content-----------------------------------------
            //var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            //StringBuilder Welcome = new StringBuilder("<h3 align ='right'>");
            //Welcome.AppendFormat(string.Format(applicationuser.ArName));
            //Welcome.Append("</h3>");
            //Welcome.Append("</br>");

            //StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            //Welcome.Append("</h3>");
            //Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            //Welcome.Append("</h3>");


            //EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "199b4975-0233-4abe-b3d3-9c9934efac88").SingleOrDefault();
            //StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/FreelancerProjects/Details/{0}' >", FreelancerProject.Id));
            //MyStringBuilder.Append("تفاصيل المشروع ");
            //MyStringBuilder.Append("</a></h2>");

            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(FreelancerProject.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(freelancerProposal.ApplicationUser.Email, usercontent.ArSubject + FreelancerProject.Name, Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));

            ////-----------------------------------email content-----------------------------------------


            return RedirectToAction("Details", "FreelancerProjects", new { id = FreelancerProject.Id });
        }

        public IActionResult RecruiterDelete(int id)
        {
            var freelancerRecruiter = _context.FreelancerRecruiters.SingleOrDefault(f => f.Id == id);
            _context.FreelancerRecruiters.Remove(freelancerRecruiter);
            _context.SaveChangesAsync();



            return RedirectToAction("Details/");

        }
        public IActionResult NotifyMe()
        {
            var applicationuser = _context.ApplicationUsers.SingleOrDefault(f => f.Id == _userManager.GetUserId(User));
            applicationuser.IsFreelancer = true;
            _context.Update(applicationuser);
            _context.SaveChanges();



            return RedirectToAction("Details/");

        }

        public async Task<IActionResult> AddComment(IFormFile file, string comment, int fid)
        {
            if (file != null) { 
            _context.FreelancerComments.Add(new FreelancerComment
            {
                Commentary = comment,
                File = await UserFile.UploadeNewFileAsync("", file, _environment.WebRootPath, Properties.Resources.Secured),
                ApplicationUserId = _userManager.GetUserId(User),
                DateOfRecord = DateTime.Now,
                FreelancerProjectId = fid,
            });
            }
            else { 
            _context.FreelancerComments.Add(new FreelancerComment
            {
                Commentary = comment,
                ApplicationUserId = _userManager.GetUserId(User),
                DateOfRecord = DateTime.Now,
                FreelancerProjectId = fid,
            });
            }
            _context.SaveChanges();

            return RedirectToAction("Details", "FreelancerProjects", new { id = fid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTicket(int prpsalid, int PId, int TicketCategoryId, string Subject, string Body)
        {
            if (Body != null)
            {
                Body = (System.Text.RegularExpressions.Regex.Replace(Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
            }
            if (prpsalid != 0)
            {
                FreelancerProposal freelancerProposal = _context.FreelancerProposals.SingleOrDefault(f => f.Id == prpsalid);
                freelancerProposal.IsReported = true;
                _context.Update(freelancerProposal);
                _context.SaveChanges();
            }
            _context.Tickets.Add(new Ticket
            {
                Subject = Subject,
                Body = Body,
                ApplicationUserId = _userManager.GetUserId(User),
                TicketOpenDate = DateTime.Now,
                Status = true,
                TicketCategoryId = TicketCategoryId,
            });

            _context.SaveChanges();

            //-----------------------------------email content-----------------------------------------
            //var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

            //StringBuilder Welcome = new StringBuilder("<h3 align ='right'>عزيزي ");
            //Welcome.AppendFormat(string.Format(applicationuser.ArName));
            //Welcome.Append("</h3>");
            //Welcome.Append("</br>");

            //StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
            //Welcome.Append("</h3>");
            //Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));

            //Welcome.Append("</h3>");


            //EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "510bce84-7b52-454c-8afd-6d6c2fe73204").SingleOrDefault();
            //StringBuilder MyStringBuilder = new StringBuilder("");
            //MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Tickets/Details/{0}' >", ticket.Id));
            //MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل التذكرة");
            //MyStringBuilder.Append("</a></h2>");

            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationuser.Email, usercontent.ArSubject + "- رقم التذكرة #[" + ticket.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(3));

            //-----------------------------------email content-----------------------------------------
            //var ticket = _context.TicketCategory.Last();
            //if (_context.TicketCategory.Last().NotifyEmail != null)
            //{
            //    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(_context.TicketCategory.SingleOrDefault(a => a.Id == TicketCategoryId).NotifyEmail, usercontent.ArSubject + "- رقم التذكرة #[" + ticket.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(3));
            //}
            return RedirectToAction("Details", "FreelancerProjects", new { id = PId });

        }

    }
}
