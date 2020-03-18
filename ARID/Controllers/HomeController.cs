using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ARID.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using ARID.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using ARID.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Hangfire;
using System.Text.Encodings.Web;

namespace ARID.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        // private readonly IBulkDBUpdater _bulkDBUpdater;
        private readonly ARIDDbContext _context;
        private readonly IEmailSender _emailSender;



        public HomeController(RoleManager<IdentityRole> roleMgr,
            UserManager<ApplicationUser> userManager,
            ARIDDbContext context, IEmailSender emailSender)//,
                                                            //IBulkDBUpdater bulkDBUpdater)
        {
            _userManager = userManager;
            _context = context;
            //   _bulkDBUpdater = bulkDBUpdater;
            _roleManager = roleMgr;
            _emailSender = emailSender;
        }

        //public IActionResult SendEmailsDaily()
        //{
        //    StringBuilder Welcome;
        //    StringBuilder Footer;

        //    if (_context.Notifications.Where(u => u.ApplicationUserId == null).Count() == 0)
        //    {
        //        var notifications = _context.Notifications.Include(n => n.ApplicationUser).Where(n => n.IsRead == false && n.ApplicationUserId != null);
        //        foreach (var item in notifications)
        //        {
        //             Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
        //            Welcome.AppendFormat(string.Format(item.ApplicationUser.ArName));
        //            Welcome.Append("</h3>");
        //            Welcome.Append("</br>");

        //            Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
        //            Footer.Append("<h3 align ='right'>");
        //            Footer.AppendFormat(string.Format(item.ApplicationUser.ArName));
        //            Footer.Append("</h3>");

        //            string link = "https://portal.arid.my/ar-LY/Account/Register/" + item.ApplicationUser.Id;

        //            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.ApplicationUser.Email, item.ApplicationUser.ArName + " هذه مجموعة إشعارات اليوم", Welcome.ToString() + "" +
        //                   $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط التسجيل في منصة أُريد </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));
        //        }
        //    }
        //    else
        //    {
        //        var notifications = _context.Notifications.Include(n => n.ApplicationUser).Where(n => n.IsRead == false && n.ApplicationUserId != null);
        //        foreach (var item in notifications)
        //        {
        //            foreach (var i in _context.Notifications.Where(n => n.ApplicationUserId == null))
        //            {
        //                Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
        //                Welcome.AppendFormat(string.Format(item.ApplicationUser.ArName));
        //                Welcome.Append("</h3>");
        //                Welcome.Append("</br>");

        //                Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات</h3>");
        //                Footer.Append("<h3 align ='right'>");
        //                Footer.AppendFormat(string.Format(item.ApplicationUser.ArName));
        //                Footer.Append("</h3>");
        //            }
        //            string link = "https://portal.arid.my/ar-LY/Account/Register/" + item.ApplicationUser.Id;
        //            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(item.ApplicationUser.Email, item.ApplicationUser.ArName + " هذه مجموعة إشعارات اليوم", Welcome.ToString() + "ffffffffffffff" +
        //            $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>رابط التسجيل في منصة أُريد </a> </p></b></center> <br/>" + Footer.ToString()), TimeSpan.FromSeconds(3));
        //        }
        //    }
        //    return View();
        //}

        public JsonResult GetMessages(string id)
        {
            var messagesreplies = _context.MessageReplies.Include(m => m.Message.FromApplicationUser).Include(m => m.Message.ToApplicationUser).Where(m => m.ApplicationUserId != id && m.IsRead == false && (m.Message.ToApplicationUserId == id || m.Message.FromApplicationUserId == id));
            var messages = _context.Messages.Include(m => m.FromApplicationUser).Where(m => m.ToApplicationUserId == id).Take(7).OrderByDescending(m => m.Id).ToList();

            foreach (var item in messagesreplies)
            {
                item.Message.Content = "s";
            }

            foreach (var item in messagesreplies)
            {
                if (messages.Where(m => m.Id == item.MessageId).Count() == 0)
                {
                    messages.Add(item.Message);
                }
            }
            return Json(new { list = messages.OrderByDescending(m => m.LastActivitydate) });
        }

        public JsonResult GetNotifications(string id)
        {
            var notifications = _context.Notifications.Include(m => m.ApplicationUser).Include(m => m.Sender).Where(m => m.ApplicationUserId == id).OrderByDescending(m => m.Id).ToList();

            return Json(new { list = notifications.OrderByDescending(m => m.CreationDate) });
        }



        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            ViewData["currentUser"] = currentUser;

            //ViewData["TotalNewNotifications"] = NotificationsControllerOld.NewNotifications(_context, _userManager.GetUserId(User));
            currentUser.LastLogin = DateTime.Now;
            _context.SaveChanges();
            //return View();
            //if (currentUser.CountryId == 1 || currentUser.EmailConfirmed == false)
            //{
            //    return RedirectToAction("Index", "Manage");
            //}


            //return RedirectToAction("Index", "Communities");
            //WallViewModel wallviewmodel = await Wall();
            CommunityWallViewModel CommunityWallViewModel = await Wall();
            return View(CommunityWallViewModel);

            // return RedirectToAction("Wall", "ApplicationUsers");
            //https://stackoverflow.com/questions/7892094/how-to-redirect-to-index-from-another-controller
        }

        public async Task<CommunityWallViewModel> Wall()
        {
            try
            {


                var UserDetails = _context.Users.Include(a => a.Faculty)
                         .SingleOrDefault(m => m.Id == _userManager.GetUserId(User));

                var Following = _context.Follows.Where(a => a.UserId == _userManager.GetUserId(User)).Select(a => a.FollowedUserId);

                var wallViewModel = new CommunityWallViewModel
                {
                    //                Projects = _context.Projects
                    //               .Include(a => a.ApplicationUser)
                    //                .Where(a => Following.Contains(a.ApplicationUserId))
                    //                .OrderBy(r => Guid.NewGuid())
                    //.Take(5)
                    //.ToList(),
                    //                //According to Speciality
                    //                Publications = _context.Publications.Where(a => a.ApplicationUser.Faculty.SpecialityId == UserDetails.Faculty.SpecialityId)
                    //                .Include(a => a.ApplicationUser)
                    //.OrderBy(r => Guid.NewGuid())
                    //.Take(10)
                    //.ToList(),
                    //                //According to Speciality
                    //                Publications2 = _context.Publications.Where(a => Following.Contains(a.ApplicationUserId) || a.ApplicationUser.Summary.Contains(a.ArTitle))
                    //                .Include(a => a.ApplicationUser)
                    //.OrderBy(r => Guid.NewGuid())
                    //.Take(5)
                    //.ToList(),

                    Posts = _context.Posts.Include(a => a.Community).Include(a => a.ApplicationUser).Where(a => a.Community.CommunityType == CommunityType.Personal && a.IsHidden == false),
                    //ScientificEvents = _context.ScientificEvents.Include(a => a.Speciality).Include(a => a.ApplicationUser).Where(a => a.EventDate > DateTime.Now),
                    //SideAds = _context.SideAds.Where(a => a.IsVisible == true && a.AdsPositionType == AdsPositionType.Left)

                    //Publications = _context.Publications
                    //    .Where(a => a.ApplicationUserId == id)
                    //    .OrderByDescending(a => a.PublicationDate)
                    //    .ToList(),


                };
                return wallViewModel;
            }
            catch (Exception)
            {

                return null;
            }

        }

        [AllowAnonymous]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        public async Task<IActionResult> About()
        {
            ApplicationUser CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser != null)
            {
                ViewData["Message"] = "Your user Id is: " + CurrentUser.Id.ToString() + ", UserName: " + CurrentUser.EnName + ", Password: " + CurrentUser.PasswordHash;
            }
            ViewData["Message"] += "Your application description page.";



            string Amount = "15000.50";
            string Currency = "usd";
            string Description = "des";

            string Language = "en";
            int MerchantId = 1;
            int MerchantSiteId = 1;
            string MerchantTransactionRefNumber = "ref";
            string CalculatedMd5 = getMd5Hash(MerchantId + ":" + Amount + ":" + Currency + ":" + "16e45af4-997f-4623-b476-e6ed0b28a1d2");
            string HashCode = CalculatedMd5;

            return Redirect(string.Format("https://localhost:44399/Payments/Process?amount={0}&currency=usd&description=des&hashcode={1}&language=en&merchantid={2}&merchantsiteid={3}&trxrefnumber={4}", Amount, HashCode, MerchantId, MerchantSiteId, MerchantTransactionRefNumber));



            //  return View();
        }

        static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public IActionResult Contact()
        {
            var Admins = _userManager.GetUsersInRoleAsync(RoleName.Admins)
               .ToAsyncEnumerable();
            List<ApplicationUser> list = new List<ApplicationUser>();
            Admins.ForEach(e => e.ToList().ForEach(c => list.Add(c)));

            return View(list.OrderBy(e => e.ARID).ToList());
        }

        public IActionResult PostHidden(Guid id)
        {
            Post post = _context.Posts.SingleOrDefault(p => p.Id == id);
            post.IsHidden = true;
            _context.Update(post);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home", new { });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public void Transfere()
        {

        }
    }
}
