using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ARID.Services
{
    public static class BulkDBWorkService
    {
        public static Task BulkDBUpdateAsync(this IBulkDBUpdater bulkDBUpdater, 
            ARIDDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            return bulkDBUpdater.BulkDBUpdateAsync(_context, _userManager);
        }
    }
}