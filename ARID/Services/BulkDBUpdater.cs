using ARID.Data;
using ARID.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Services
{
    //public class BulkDBUpdater : IBulkDBUpdater
    //{
    //    public BulkDBUpdater()
    //    {
    //    }

        //public async Task BulkDBUpdateAsync(ARIDDbContext _context, UserManager<ApplicationUser> _userManager)
        //{
        //    await MakePasswords(_context, _userManager);
        //}

      //  private async Task MakePasswords(ARIDDbContext _context, UserManager<ApplicationUser> _userManager)
       // {
            //List<ApplicationUser> users = _context.ApplicationUsers
            //    .OrderBy(u => u.Id).ToList();
            //foreach (var user in users)
            //{
            //    var member = _context.Members
            //        .Select(m => new { m.Email, m.Password })
            //        .Where(m => m.Email == user.Email).FirstOrDefault();
            //    if (member != null)
            //    {
            //        var password = new PasswordHasher<ApplicationUser>();
            //        var hashed = password.HashPassword(user, member.Password);
            //        user.PasswordHash = hashed;

            //        var userStore = new UserStore<ApplicationUser>(_context);
            //        var result = await userStore.UpdateAsync(user);
            //        // await context.SaveChangesAsync();
            //    }
            //}
            // return Task.CompletedTask;
      //  }
    //}
}

