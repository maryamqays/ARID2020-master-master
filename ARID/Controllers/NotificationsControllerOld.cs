//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using ARID.Data;
//using ARID.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using ARID.AuxiliaryClasses;

//namespace ARID.Controllers
//{
//    [Authorize]
//    public class NotificationsControllerOld : Controller
//    {
//        private readonly ARIDDbContext _context;
//        private UserManager<ApplicationUser> _userManager;
//        private IHostingEnvironment _environment;

//        public NotificationsControllerOld(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment)
//        {
//            _context = context;
//            _userManager = userMrg;
//            _environment = environment;
//        }

//        public static int NewNotifications(ARIDDbContext context,string UserId)
//        {
//            int TotalUnreadNotification = 0;
//            if (context != null)
//            {
//                TotalUnreadNotification = 1;

//                //TotalUnreadNotification = context.Notifications.Count() - 
//                //    context.Notificationlogs.Select(m => m.ApplicationUserId == UserId).Count();
//            }

//            return TotalUnreadNotification;
//        }

//        // GET: Notifications
//        public async Task<IActionResult> Index()
//        {
//            var aRIDDbContext = _context.Notifications.Include(n => n.ApplicationUser).OrderByDescending(n => n.Id);
//            return View(await aRIDDbContext.ToListAsync());
//        }

//        public async Task<IActionResult> AdminIndex()
//        {
//            var aRIDDbContext = _context.Notifications.Include(n => n.ApplicationUser).OrderByDescending(n => n.Id);
//            return View(await aRIDDbContext.ToListAsync());
//        }

//        // GET: Notifications/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notification = await _context.Notifications
//                .Include(n => n.ApplicationUser)
//                .SingleOrDefaultAsync(m => m.Id == id);
//            if (notification == null)
//            {
//                return NotFound();
//            }

//            return View(notification);
//        }

//        public IActionResult NotificationDetails(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notification = _context.Notifications
//                .Include(n => n.ApplicationUser)
//                .SingleOrDefault(m => m.Id == id);

//            if (notification == null)
//            {
//                return NotFound();
//            }

//            NotificationLogsControllerOld.LogNotificationRead(_context, notification.Id, _userManager.GetUserId(User));

//            return View(notification);
//        }

//        // GET: Notifications/Create
//        public IActionResult Create()
//        {
//            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
//            return View();
//        }

//        // POST: Notifications/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Title,Content,Date,ApplicationUserId,Image,ExternalLink")] Notification notification, IFormFile myfile)
//        {
//            if (ModelState.IsValid)
//            {
//                notification.Image = await UserFile.UploadeNewImageAsync(notification.Image,
//                    myfile, _environment.WebRootPath, Properties.Resources.NotificationsPhoto, 300, 300);

//                notification.Date = DateTime.Today.Date;
//                notification.ApplicationUserId = _userManager.GetUserId(User);
//                _context.Add(notification);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            return View(notification);
//        }

//        // GET: Notifications/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notification = await _context.Notifications.SingleOrDefaultAsync(m => m.Id == id);
//            if (notification == null)
//            {
//                return NotFound();
//            }
//            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", notification.ApplicationUserId);
//            return View(notification);
//        }

//        // POST: Notifications/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Date,ApplicationUserId,Image,ExternalLink")] Notification notification, IFormFile myfile)
//        {
//            if (id != notification.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {

//                    notification.Image = await UserFile.UploadeNewImageAsync(notification.Image,
//                 myfile, _environment.WebRootPath, Properties.Resources.NotificationsPhoto, 300, 300);

//                    notification.ApplicationUserId = _userManager.GetUserId(User);
//                    _context.Update(notification);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!NotificationExists(notification.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(notification);
//        }

//        // GET: Notifications/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notification = await _context.Notifications
//                .Include(n => n.ApplicationUser)
//                .SingleOrDefaultAsync(m => m.Id == id);
//            if (notification == null)
//            {
//                return NotFound();
//            }

//            return View(notification);
//        }

//        // POST: Notifications/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var notification = await _context.Notifications.SingleOrDefaultAsync(m => m.Id == id);
//            _context.Notifications.Remove(notification);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool NotificationExists(int id)
//        {
//            return _context.Notifications.Any(e => e.Id == id);
//        }
//    }
//}
