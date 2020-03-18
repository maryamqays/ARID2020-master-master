using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public interface IBulkDBUpdater
    {
        Task BulkDBUpdateAsync(ARIDDbContext _context, UserManager<ApplicationUser> _userManager);
    }
}
