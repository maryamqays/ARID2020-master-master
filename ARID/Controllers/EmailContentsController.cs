using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ARID.Services;

namespace ARID.Controllers
{
    [Authorize(Roles = RoleName.Admins)]
    public class EmailContentsController : Controller
    {
        private readonly ARIDDbContext _context;
        private readonly IEmailSender _emailSender;

        public EmailContentsController(ARIDDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async void Broadcast(int id)
        {
            EmailContent content = _context.EmailContents.Where(m => m.Id == id).SingleOrDefault();
            if (id > 0)
            {
                List<string> emails = _context.ApplicationUsers
                    .Where(m => m.EmailConfirmed == true)
                    .Select(m => m.Email)
                    .ToList();

                for (int i = 0; i < emails.Count; i += 10)
                {
                    var temp = emails.Skip(i).Take(10);

                    foreach (var email in temp)
                    {
                        await _emailSender.SendEmailAsync(email, content.ArSubject, content.ArContent);
                    }

                    await Task.Delay(60000);
                }
            }
        }


        public async void TestBroadcast(int id)
        {
            EmailContent content = _context.EmailContents.Where(m => m.Id == id).SingleOrDefault();

            if (id > 0)
            {
                await _emailSender.SendEmailAsync("info@filspay.com", content.ArSubject, content.ArContent);
            }
        }


        // GET: EmailContents
        public async Task<IActionResult> Index()
        {
            var aRIDDbContext = _context.EmailContents.Include(e => e.Sender);
            return View(await aRIDDbContext.ToListAsync());
        }

        // GET: EmailContents/Create
        public IActionResult Create()
        {
            ViewData["SenderId"] = new SelectList(_context.Senders, "Id", "Email");
            ViewData["unquename"] = Guid.NewGuid();
            return View();
        }

        // POST: EmailContents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArContent,EnContent,ArSubject,EnSubject,SenderId,UniqueName")] EmailContent emailContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailContent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["SenderId"] = new SelectList(_context.Senders, "Id", "Email", emailContent.SenderId);
            return View(emailContent);
        }

        // GET: EmailContents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailContent = await _context.EmailContents.SingleOrDefaultAsync(m => m.Id == id);
            if (emailContent == null)
            {
                return NotFound();
            }
            ViewData["SenderId"] = new SelectList(_context.Senders, "Id", "Email", emailContent.SenderId);
            return View(emailContent);
        }

        // POST: EmailContents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArContent,EnContent,ArSubject,EnSubject,SenderId,UniqueName")] EmailContent emailContent)
        {
            if (id != emailContent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailContentExists(emailContent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["SenderId"] = new SelectList(_context.Senders, "Id", "Email", emailContent.SenderId);
            return View(emailContent);
        }

        // GET: EmailContents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailContent = await _context.EmailContents
                .Include(e => e.Sender)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (emailContent == null)
            {
                return NotFound();
            }

            return View(emailContent);
        }

        // POST: EmailContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailContent = await _context.EmailContents.SingleOrDefaultAsync(m => m.Id == id);
            _context.EmailContents.Remove(emailContent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EmailContentExists(int id)
        {
            return _context.EmailContents.Any(e => e.Id == id);
        }
    }
}
