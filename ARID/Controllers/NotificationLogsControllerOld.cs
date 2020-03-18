//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using ARID.Data;
//using ARID.Models;

//namespace ARID.Controllers
//{
//    public class NotificationLogsControllerOld : Controller
//    {
//        private readonly ARIDDbContext _context;

//        public NotificationLogsControllerOld(ARIDDbContext context)
//        {
//            _context = context;
//        }

//        public static void LogNotificationRead(ARIDDbContext context, int notificationId, string userId)
//        {
//            if (!HasBeenRead(context, notificationId, userId))
//            {
//                context.Notificationlogs.Add(
//                    new NotificationLog
//                    {
//                        NotificationId = notificationId,
//                        ReadDate = DateTime.Today.Date,
//                        ApplicationUserId = userId
//                    });
//                context.SaveChangesAsync();
//            }
//        }

//        public static bool HasBeenRead(ARIDDbContext context, int notificationId, string userId)
//        {
//            int notificationLogItem = context.Notificationlogs
//                .Where(m => m.NotificationId == notificationId && m.ApplicationUserId == userId).Count();

//            if (notificationLogItem == 0)
//            {
//                return false;
//            }
//            return true;
//        }


      

//        // GET: NotificationLogs
//        public async Task<IActionResult> Index()
//        {
//            var aRIDDbContext = _context.Notificationlogs.OrderByDescending(n => n.Id).Include(n => n.ApplicationUser).Include(n => n.Notification);
//            return View(await aRIDDbContext.ToListAsync());
//        }

//        // GET: NotificationLogs/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notificationLog = await _context.Notificationlogs
//                .Include(n => n.ApplicationUser)
//                .Include(n => n.Notification)
//                .SingleOrDefaultAsync(m => m.Id == id);
//            if (notificationLog == null)
//            {
//                return NotFound();
//            }

//            return View(notificationLog);
//        }

//        // GET: NotificationLogs/Create
//        public IActionResult Create()
//        {
//            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
//            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "Id");
//            return View();
//        }

//        // POST: NotificationLogs/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,NotificationId,ApplicationUserId,ReadDate")] NotificationLog notificationLog)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(notificationLog);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", notificationLog.ApplicationUserId);
//            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "Id", notificationLog.NotificationId);
//            return View(notificationLog);
//        }

//        // GET: NotificationLogs/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notificationLog = await _context.Notificationlogs.SingleOrDefaultAsync(m => m.Id == id);
//            if (notificationLog == null)
//            {
//                return NotFound();
//            }
//            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", notificationLog.ApplicationUserId);
//            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "Id", notificationLog.NotificationId);
//            return View(notificationLog);
//        }

//        // POST: NotificationLogs/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,NotificationId,ApplicationUserId,ReadDate")] NotificationLog notificationLog)
//        {
//            if (id != notificationLog.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(notificationLog);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!NotificationLogExists(notificationLog.Id))
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
//            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", notificationLog.ApplicationUserId);
//            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "Id", notificationLog.NotificationId);
//            return View(notificationLog);
//        }

//        // GET: NotificationLogs/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var notificationLog = await _context.Notificationlogs
//                .Include(n => n.ApplicationUser)
//                .Include(n => n.Notification)
//                .SingleOrDefaultAsync(m => m.Id == id);
//            if (notificationLog == null)
//            {
//                return NotFound();
//            }

//            return View(notificationLog);
//        }

//        // POST: NotificationLogs/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var notificationLog = await _context.Notificationlogs.SingleOrDefaultAsync(m => m.Id == id);
//            _context.Notificationlogs.Remove(notificationLog);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool NotificationLogExists(int id)
//        {
//            return _context.Notificationlogs.Any(e => e.Id == id);
//        }
//    }
//}
