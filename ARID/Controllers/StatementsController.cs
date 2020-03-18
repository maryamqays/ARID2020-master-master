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

namespace ARID.Controllers
{
    public class StatementsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private int PagSize = 100;

        public StatementsController(ARIDDbContext context,UserManager<ApplicationUser> userMrg)
        {
            _context = context;
            _userManager = userMrg;
        }

        // GET: Statements
        public IActionResult Index()
        {
            IEnumerable<Statement> applicationDbContext;
          
                applicationDbContext = _context.Statements.Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(a=>a.RecordDate);
           
            return View(applicationDbContext);
        }


        public IActionResult Search(string ss)
        {
            IEnumerable<Statement> applicationDbContext;
            if (ss == null)
            {
                applicationDbContext = _context.Statements.Include(s => s.ApplicationUser);
            }
            else
            {
                applicationDbContext = _context.Statements.Where(a => a.Details.Contains(ss) || a.ApplicationUser.ArName.Contains(ss) || a.ApplicationUser.EnName.Contains(ss)).Include(s => s.ApplicationUser);
            }

            //ViewData["Balance"] = 
            return View(applicationDbContext);
        }

        public async Task<IActionResult> UserStatements(string id)
        {
            ViewData["userid"] = id;
            var application = _context.ApplicationUsers.SingleOrDefault(a => a.Id == id);
            ViewData["username"] = application.ArName;
            var applicationDbContext = _context.Statements.Where(a => a.ApplicationUserId == id).Include(s => s.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }


        public IActionResult MemberStatements(int productPage = 1)
        {
            StatementViewModel statementVM = new StatementViewModel()
            {
                Statements = _context.Statements.Where(a => a.ApplicationUserId ==_userManager.GetUserId(User)).OrderByDescending(a=>a.Id),
            };
            var count = statementVM.Statements.Count();
            statementVM.Statements = statementVM.Statements.OrderByDescending(p => p.Id)
                .Skip((productPage - 1) * PagSize)
                .Take(PagSize).ToList();


            statementVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PagSize,
                TotalItem = count
            };


            return View(statementVM);
        }

        // GET: Statements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statement = await _context.Statements
                .Include(s => s.ApplicationUser)

                .FirstOrDefaultAsync(m => m.Id == id);
            if (statement == null)
            {
                return NotFound();
            }

            return View(statement);
        }

        // GET: Statements/Create
        public IActionResult Create(string id)
        {
            ViewData["userid"] = id;
            //ViewData["FromApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            //ViewData["ToApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Statements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string uid, [Bind("RecordDate,Details,Amount,Destination,ApplicationUserId,BalanceType")] Statement statement)
        {
            if (ModelState.IsValid)
            {
                if (statement.Details != null) { 
                statement.Details = statement.Details.Replace("\n", "<br/>");
                                    }

                _context.Add(statement);

                var user = _context.ApplicationUsers.SingleOrDefault(a => a.Id == statement.ApplicationUserId);

                if (statement.Destination == true)
                {
                    user.Balance = user.Balance + statement.Amount;
                    _context.Update(user);
                }
                else
                {
                    user.Balance = user.Balance - statement.Amount;
                    _context.Update(user);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserStatements),new { id= uid });
            }
            //ViewData["FromApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", statement.ApplicationUser);

            return View(statement);
        }

        // GET: Statements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statement = await _context.Statements.FindAsync(id);
            if (statement == null)
            {
                return NotFound();
            }
            ViewData["FromApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", statement.ApplicationUser);

            return View(statement);
        }

        // POST: Statements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RecordDate,Details,Amount,Destination,ApplicationUserId,BalanceType")] Statement statement)
        {
            if (id != statement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatementExists(statement.Id))
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
         //   ViewData["FromApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", statement.ApplicationUser);

            return View(statement);
        }

        // GET: Statements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statement = await _context.Statements
                .Include(s => s.ApplicationUser)

                .FirstOrDefaultAsync(m => m.Id == id);
            if (statement == null)
            {
                return NotFound();
            }

            return View(statement);
        }

        // POST: Statements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statement = await _context.Statements.FindAsync(id);
            _context.Statements.Remove(statement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatementExists(int id)
        {
            return _context.Statements.Any(e => e.Id == id);
        }
    }
}
