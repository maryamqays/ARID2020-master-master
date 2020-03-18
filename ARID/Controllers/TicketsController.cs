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
using System.Text;
using Hangfire;
using ARID.Services;
using Microsoft.AspNetCore.Authorization;

namespace ARID.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private int PagSize = 100;
        private readonly IEmailSender _emailSender;


        public TicketsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userMrg;
            _emailSender = emailSender;

        }

        // GET: Tickets
        public IActionResult Index(int productPage = 1)
        {
            var ARIDDbContext = _context.Tickets.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).Include(t => t.ApplicationUser).Include(a=>a.TicketCategory).OrderByDescending(a => a.Id);

            TicketViewModel ticketVM = new TicketViewModel()
            {
                Tickets = _context.Tickets.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(a => a.Id).Include(a => a.ApplicationUser).Include(a => a.TicketCategory),
            };

            var count = ticketVM.Tickets.Count();
            ticketVM.Tickets = ticketVM.Tickets.OrderByDescending(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            ticketVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };


            return View(ticketVM);


        }

        public IActionResult List(string ss, int productPage = 1)
        {
            TicketViewModel ticketVM = new TicketViewModel();
            if (ss == "closed")
            {
                ticketVM = new TicketViewModel()
                {
                    Tickets = _context.Tickets.Where(a => a.Status == false).Include(a => a.ApplicationUser),
                };
            }
            else if (ss == "open")
            {
                ticketVM = new TicketViewModel()
                {
                    Tickets = _context.Tickets.Where(a => a.Status == true).Include(a => a.ApplicationUser),
                };
            }
            else if (ss != null)
            {
                ticketVM = new TicketViewModel()
                {
                    Tickets = _context.Tickets.OrderByDescending(a => a.TicketOpenDate).Include(a => a.ApplicationUser).Include(a => a.TicketCategory).Where(a => a.Subject.Contains(ss) || a.Body.Contains(ss) || a.ApplicationUser.ArName.Contains(ss) || a.ApplicationUser.EnName.Contains(ss)),
                };
            }
            else
            {
                ticketVM = new TicketViewModel()
                {
                    Tickets = _context.Tickets.Where(a => a.Status == true).OrderByDescending(a => a.TicketOpenDate).Include(a => a.ApplicationUser).Include(a => a.TicketCategory),
                };
            }

            var count = ticketVM.Tickets.Count();
            ticketVM.Tickets = ticketVM.Tickets.OrderBy(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            ticketVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };

            return View(ticketVM);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Ticket = _context.Tickets.Where(a => a.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Admins")).SingleOrDefault(m => m.Id == id);
            if (Ticket.Id != 0)
            {
                TicketViewModel ticketVM = new TicketViewModel()
                {
                    TicketReplys = _context.TicketReplies.Where(a => a.TicketId == id).OrderByDescending(a => a.Id).Include(a => a.SupportUser),
                    Ticket = _context.Tickets.Where(a => a.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Admins")).Include(t => t.ApplicationUser).SingleOrDefault(m => m.Id == id),
                };
                return View(ticketVM);


            }
            else
                return RedirectToAction(nameof(Index));
        }

        // GET: Tickets/Create
        public IActionResult Create(int id)
        {
            ViewData["TicketCategoryId"] = new SelectList(_context.TicketCategory, "Id", "Name", id);
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,Body,ApplicationUserId,TicketOpenDate,TicketCloseDate,Status,TicketCategoryId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.ApplicationUserId = _userManager.GetUserId(User);
                ticket.TicketOpenDate = DateTime.Now;

                if (ticket.Body != null)
                {
                    ticket.Body = (System.Text.RegularExpressions.Regex.Replace(ticket.Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                }
                ticket.Status = true;
                _context.Add(ticket);
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


                EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "510bce84-7b52-454c-8afd-6d6c2fe73204").SingleOrDefault();
                StringBuilder MyStringBuilder = new StringBuilder("");
                MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Tickets/Details/{0}' >", ticket.Id));
                MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل التذكرة");
                MyStringBuilder.Append("</a></h2>");

                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(applicationuser.Email, usercontent.ArSubject + "- رقم التذكرة #[" + ticket.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(3));

                //-----------------------------------email content-----------------------------------------

                if (_context.TicketCategory.SingleOrDefault(a=>a.Id== ticket.TicketCategoryId).NotifyEmail != null) {
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(_context.TicketCategory.SingleOrDefault(a => a.Id == ticket.TicketCategoryId).NotifyEmail, usercontent.ArSubject + "- رقم التذكرة #[" + ticket.Id + "]", Welcome.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(3));
                                    }
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.SingleOrDefaultAsync(m => m.Id == id & m.ApplicationUserId == _userManager.GetUserId(User));
            ViewData["TicketCategoryId"] = new SelectList(_context.TicketCategory, "Id", "Name", ticket.TicketCategoryId);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Subject,Body,ApplicationUserId,TicketOpenDate,TicketCloseDate,Status,TicketCategoryId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.ApplicationUserId = _userManager.GetUserId(User);
                    ticket.TicketOpenDate = DateTime.Now;
                    ticket.TicketCloseDate = DateTime.Now;
                    ticket.Status = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id & m.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Admins"));
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.SingleOrDefaultAsync(m => m.Id == id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
