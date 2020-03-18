using ARID.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Collections.Generic;

namespace ARID.Models
{
    public class SeedingUsers
    {
      //  public static async Task InitializeAsync(ARIDDbContext context, UserManager<ApplicationUser> userManager)
       // {
            //await TransferMembers(context, userManager);
            //int x = 500;
            //List<ApplicationUser> users = context.ApplicationUsers.OrderBy(u => u.Id).ToList();
            ////.Skip(5*x).Take(500)

            //foreach (var user in users)
            //{
            //    var member = context.Members.Select(m => new { m.Email, m.Password}).Where(m => m.Email == user.Email).FirstOrDefault();
            //    if (member != null)
            //    {
            //        var password = new PasswordHasher<ApplicationUser>();
            //        var hashed = password.HashPassword(user, member.Password);
            //        user.PasswordHash = hashed;

            //        var userStore = new UserStore<ApplicationUser>(context);
            //        var result = await userStore.UpdateAsync(user);                    
            //       // await context.SaveChangesAsync();
            //    }
            //}
        //}

        //public static async Task TransferMembers(ARIDDbContext context, UserManager<ApplicationUser> userManager)
        //{
            //List<Member> members = context.Members.OrderBy(m => m.Dor).ToList();
            //foreach (var oldmember in members)
            //{
            //    try
            //    {
            //        if (!context.Users.Any(u => u.UserName == oldmember.Email))
            //        {
            //            var user = new ApplicationUser();

            //            if (!string.IsNullOrEmpty(oldmember.ReferalName) && !string.IsNullOrWhiteSpace(oldmember.ReferalName))
            //            {
            //                var ru = context.ApplicationUsers
            //                               .Where(c => c.ArName == oldmember.ReferalName)
            //                               .FirstOrDefault();
            //                if (ru != null)
            //                    user.ReferredById = ru.Id;
            //                else
            //                    user.ReferredById = null;
            //            }
            //            else
            //            {
            //                user.ReferredById = null;
            //            }
            //            user.ArName = oldmember.Name;
            //            user.EnName = oldmember.EnName;
            //            user.OtherNames = oldmember.Othernames;

            //            user.DateofBirth = DateTime.ParseExact(oldmember.Dob + " 00:00:00.000", "M/d/yyyy HH:mm:ss.fff", CultureInfo.CurrentCulture);

            //            if (oldmember.Sex != null)
            //            {
            //                if (oldmember.Sex == true)
            //                    user.Gender = GenderType.Male;
            //                else
            //                    user.Gender = GenderType.Female;
            //            }
            //            else
            //                user.Gender = GenderType.Unknown;

            //            user.SecondEmail = "";
            //            user.RegDate = Convert.ToDateTime(oldmember.Dor);

            //            if ((oldmember.AccountNumber.Contains("-")) && (oldmember.AccountNumber.Length == 9))
            //                user.ARID = Convert.ToInt32(oldmember.AccountNumber.Substring(0, 4) + oldmember.AccountNumber.Substring(5, 4));
            //            else
            //                user.ARID = 0;

            //            user.ARIDDate = oldmember.ResearcherIdDor;

            //            if ((oldmember.AccountNumber.Contains("-")) && (oldmember.AccountNumber.Length == 9))
            //                user.Status = StatusType.Active;
            //            else
            //                user.Status = StatusType.New;

            //            user.UILanguage = "ar";
            //            user.ProfileImage = oldmember.Profilepic;
            //            user.FeaturedImage = oldmember.Featuredpic;
            //            user.CVURL = oldmember.Cvfile;
            //            user.Summary = oldmember.Summary;
            //            user.ContactMeDetail = oldmember.Contactmedetails;

            //            user.CountryId = context.Countries
            //                               .Where(c => c.ArCountryName == oldmember.CountryNameAra)
            //                               .FirstOrDefault().Id;
            //            user.CityId = 1;

            //            if (oldmember.UniversityName != null)
            //                user.UniversityId = context.Universities
            //                               .Where(c => c.ArUniversityName == oldmember.UniversityName)
            //                               .FirstOrDefault().Id;
            //            else
            //                user.UniversityId = 1;

            //            if (oldmember.Collegename != null)
            //                user.FacultyId = context.Faculties
            //                               .Where(c => c.ArFacultyName == oldmember.Collegename)
            //                               .FirstOrDefault().Id;
            //            else
            //                user.FacultyId = 1;

            //            user.Email = oldmember.Email;
            //            user.NormalizedEmail = oldmember.Email.ToUpper();
            //            user.UserName = oldmember.Email;
            //            user.NormalizedUserName = oldmember.Email.ToUpper();
            //            user.PhoneNumber = oldmember.Mobile;

            //            if (oldmember.EmailVerification != null)
            //                user.EmailConfirmed = (bool)oldmember.EmailVerification;
            //            else
            //                user.EmailConfirmed = false;

            //            user.PhoneNumberConfirmed = false;
            //            user.SecurityStamp = Guid.NewGuid().ToString("D");

            //            if (!context.Users.Any(u => u.UserName == user.UserName))
            //            {
            //                //var password = new PasswordHasher<ApplicationUser>();
            //                //var hashed = password.HashPassword(user, "secret");
            //                //user.PasswordHash = hashed;

            //                // await userManager.AddPasswordAsync(user, oldmember.Password);

            //                var userStore = new UserStore<ApplicationUser>(context);
            //                var result = userStore.CreateAsync(user);
            //            }

            //            // await userManager.AddToRoleAsync(user, RoleName.Members);
            //            await context.SaveChangesAsync();
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
        //}
    }
}
