using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Newtonsoft.Json;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace ARID.Controllers
{
    public class FreelancerReadyServicesController : Controller
    {
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private UserManager<ApplicationUser> _userManager;



        public FreelancerReadyServicesController(UserManager<ApplicationUser> userMrg, ARIDDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
            _userManager = userMrg;

        }

        // GET: FreelancerReadyServices
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.FreelancerReadyServices.Include(f => f.ApplicationUser).OrderByDescending(f => f.DateOfRecord);
            return View(await aRIDDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTicket(Guid SId, int TicketCategoryId, string Subject, string Body)
        {
            if (Body != null)
            {
                Body = (System.Text.RegularExpressions.Regex.Replace(Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
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
            return RedirectToAction("Details", "FreelancerReadyServices", new { id = SId });

        }


        // GET: FreelancerReadyServices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            //------------------------------ratings-----------------------------
            var p = 0;
            var c = 0;
            var q = 0;
            var e = 0;
            var d = 0;
            var r = 0;

            var freelancerratings = _context.FreelancerRatings.Where(f => f.FreelancerReadyServiceId == id);
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
                p = p / freelancerratings.Count();
                c = c / freelancerratings.Count();
                q = q / freelancerratings.Count();
                e = e / freelancerratings.Count();
                d = d / freelancerratings.Count();
                r = r / freelancerratings.Count();
                ViewData["rating"] = (p + c + q + e + d + r) / 6;
            }


            //------------------------------ratings-----------------------------


            if (id == null)
            {
                return NotFound();
            }
            FreelancerReadyService FreelancerReadyService = await _context.FreelancerReadyServices
.Include(f => f.ApplicationUser)
.SingleOrDefaultAsync(m => m.Id == id);
            FreelancerReadyService.Views += 1;
            _context.Update(FreelancerReadyService);
            _context.SaveChanges();

            if (FreelancerReadyService.PricingType == PricingType.Fiver)
            {
                ViewData["price"] = 5;
            }
            else if (FreelancerReadyService.PricingType == PricingType.Tenth)
            {
                ViewData["price"] = 10;
            }
            else if (FreelancerReadyService.PricingType == PricingType.Fifteenth)
            {
                ViewData["price"] = 15;
            }
            else if (FreelancerReadyService.PricingType == PricingType.Twenty)
            {
                ViewData["price"] = 20;
            }
            else if (FreelancerReadyService.PricingType == PricingType.TwenyFive)
            {
                ViewData["price"] = 25;
            }


            FreelancerReadyServiceViewModel freelancerReadyServiceVM = new FreelancerReadyServiceViewModel()
            {
                FreelancerReadyService = await _context.FreelancerReadyServices
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id),

                FreelancerReadyServiceExtensions = _context.FreelancerReadyServiceExtensions.Where(f => f.FreelancerReadyServiceId == id).OrderByDescending(a => a.Id),
            };

            return View(freelancerReadyServiceVM);
        }

        public List<CommunityAutofillModel> getSkills()
        {
            List<CommunityAutofillModel> ls = new List<CommunityAutofillModel>();
            var sa = new JsonSerializerSettings();
            var DbSkills = _context.Skills.Select(a => new { text = a.Name, value = a.Name });

            foreach (var data in DbSkills)
            {
                CommunityAutofillModel communityAutofillModel = new CommunityAutofillModel();
                communityAutofillModel.Text = data.text;
                communityAutofillModel.Value = data.value;
                ls.Add(communityAutofillModel);
            }
            return ls;
        }


        // GET: FreelancerReadyServices/Create
        public IActionResult Create()
        {
            ViewBag.Skills = getSkills();
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: FreelancerReadyServices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Title,Skills,Description,Image,Youtube,Views,RequiredDays,DateOfRecord,PricingType,SkillCategoryType")] FreelancerReadyService freelancerReadyService, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                freelancerReadyService.Image = await UserFile.UploadeNewImageAsync(freelancerReadyService.Image,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);


                freelancerReadyService.Id = Guid.NewGuid();
                if (freelancerReadyService.Youtube != null)
                {
                    int position = freelancerReadyService.Youtube.IndexOf("=");
                    freelancerReadyService.Youtube = freelancerReadyService.Youtube.Substring(position + 1);
                }
                freelancerReadyService.ApplicationUserId = _userManager.GetUserId(User);
                _context.Add(freelancerReadyService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerReadyService.ApplicationUserId);
            return View(freelancerReadyService);
        }

        // GET: FreelancerReadyServices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerReadyService = await _context.FreelancerReadyServices.Include(f => f.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerReadyService == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers.Where(a => a.Id == freelancerReadyService.ApplicationUserId), "Id", "Id", freelancerReadyService.ApplicationUser.ArName);
            return View(freelancerReadyService);
        }

        // POST: FreelancerReadyServices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ApplicationUserId,Title,Skills,Description,Image,Youtube,Views,RequiredDays,DateOfRecord,PricingType,SkillCategoryType")] FreelancerReadyService freelancerReadyService, IFormFile myfile)
        {
            if (id != freelancerReadyService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    freelancerReadyService.Image = await UserFile.UploadeNewImageAsync(freelancerReadyService.Image,
myfile, _environment.WebRootPath, Properties.Resources.Images, 500, 500);


                    if (freelancerReadyService.Youtube != null)
                    {
                        int position = freelancerReadyService.Youtube.IndexOf("=");
                        freelancerReadyService.Youtube = freelancerReadyService.Youtube.Substring(position + 1);
                    }


                    _context.Update(freelancerReadyService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerReadyServiceExists(freelancerReadyService.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", freelancerReadyService.ApplicationUserId);
            return View(freelancerReadyService);
        }

        // GET: FreelancerReadyServices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancerReadyService = await _context.FreelancerReadyServices
                .Include(f => f.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (freelancerReadyService == null)
            {
                return NotFound();
            }

            return View(freelancerReadyService);
        }

        // POST: FreelancerReadyServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var freelancerReadyService = await _context.FreelancerReadyServices.SingleOrDefaultAsync(m => m.Id == id);
            var freelancerReadyServiceExtensions = _context.FreelancerReadyServiceExtensions.Where(f => f.FreelancerReadyServiceId == id);
            _context.FreelancerReadyServiceExtensions.RemoveRange(freelancerReadyServiceExtensions);
            _context.FreelancerReadyServices.Remove(freelancerReadyService);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerReadyServiceExists(Guid id)
        {
            return _context.FreelancerReadyServices.Any(e => e.Id == id);
        }
    }
}
