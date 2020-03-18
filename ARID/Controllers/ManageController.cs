using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ARID.Models;
using ARID.Models.ManageViewModels;
using ARID.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using ARID.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using System.IO;
using System.Security.Cryptography;
using Hangfire;

namespace ARID.Controllers
{
    [Authorize]
    //[Route("{culture}/[controller]/[action]")] // commented out to keep the culture in the route
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly ARIDDbContext _context;
        private IHostingEnvironment _environment;
        private readonly IStringLocalizer<ManageController> _localizer;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(ARIDDbContext context,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          IHostingEnvironment environment,
          IStringLocalizer<ManageController> localizer,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
            _environment = environment;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                //------------------------------------------
                RegDate = user.RegDate,
                Status = user.Status,
                ARID = user.ARID,
                ARIDDate = user.ARIDDate,
                ReferredById = user.ReferredById,
                UniversityId = user.UniversityId,
                FacultyId = user.FacultyId,
                SecondEmail = user.SecondEmail,
                ArName = user.ArName,
                EnName = user.EnName,
                OtherNames = user.OtherNames,
                DateofBirth = user.DateofBirth,
                Gender = user.Gender,
                UILanguage = user.UILanguage,
                CountryId = user.CountryId,
                CityId = user.CityId,
                ProfileImage = user.ProfileImage,
                FeaturedImage = user.FeaturedImage,
                CVURL = user.CVURL,
                Summary = user.Summary,
                ContactMeDetail = user.ContactMeDetail,
                Visitors = user.Visitors,
                DAL = user.DAL,
                DALEnabled = user.DALEnabled,
                Balance = user.Balance,
                HoldingBalance = user.HoldingBalance,
                IsFreelancer = user.IsFreelancer,
                LastLogin = user.LastLogin,
                FirmName = user.FirmName,
                IsNotUniversity = user.IsNotUniversity


                //------------------------------------------
            };

            if (model.IsEmailConfirmed == false)
            {
                StatusMessage = _localizer["RequiredEmailVerification"];
            }

            if (CultureInfo.CurrentCulture.Name.Contains("ar"))
            {
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", model.CountryId);
                ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == model.CountryId), "Id", "ArCityName", model.CityId);
                ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u => u.CountryId == model.CountryId && u.IsIndexed == true), "Id", "ArUniversityName", model.UniversityId);
                ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == model.UniversityId), "Id", "ArFacultyName", model.FacultyId);
                ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", model.ReferredById);

            }
            else
            {
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "EnCountryName", model.CountryId);
                ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == model.CountryId), "Id", "EnCityName", model.CityId);
                ViewData["UniversityId"] = new SelectList(_context.Universities.Where(u => u.CountryId == model.CountryId && u.IsIndexed == true), "Id", "EnUniversityName", model.UniversityId);
                ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == model.UniversityId), "Id", "EnFacultyName", model.FacultyId);
                ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", model.ReferredById);
            }

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
            ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);
            return View(model);
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





        public IActionResult PaymentSuccessfully(Guid trn_id, Guid trxRefNumber, string VerificationString)
        {
            var FPay = _context.Filspays.SingleOrDefault(m => m.Id == trxRefNumber);
            string Amount = FPay.UserAmount.ToString();
            string Currency = "usd";
            //string Description = "Dxn Products";
            //string Language = "en";
            int MerchantId = 2;
            int MerchantSiteId = 2;
            string MerchantTransactionRefNumber = (FPay.Id).ToString();

            string MerchantVerificationString = getMd5Hash(MerchantId + ":" + trn_id + ":" + MerchantSiteId + ":" + Currency + ":" + "en" + ":" + "6dc023e3-9916-45dd-b7a1-885aab63bae6");

            if (MerchantVerificationString == VerificationString && FPay.Status == false)
            {
                FPay.Status = true;
                FPay.trn_id = trn_id.ToString();
                _context.Update(FPay);



                _context.Statements.Add(new Statement
                {
                    Amount = FPay.UserAmount - ((Math.Truncate((decimal)0.05 * FPay.UserAmount))),
                    RecordDate = DateTime.Now,
                    Destination = true,
                    ApplicationUserId = _userManager.GetUserId(User),
                    Details = "FilsPay - Payment #" + FPay.Id,
                    BalanceType = BalanceType.currentbalance
                });

                var user = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));
                user.Balance = user.Balance + (FPay.UserAmount - ((Math.Truncate((decimal)0.05 * FPay.UserAmount))));

                _context.SaveChanges();
                ViewData["PaymentStatus"] = "تمت تعبئة الحساب بنجاح";
                return View();
            }

            else
            {
                ViewData["PaymentStatus"] = "حصل خلل أثناء الدفع. الرجاء التواصل مع الادارة عبر الرقم 0060196714305 او البريد info@filspay.com";
            }
            return View();


        }

        //public IActionResult PaymentSuccessfully()
        //{

        //    return View();
        //}

        public IActionResult Filspay()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filspay(decimal UserAmount)
        {
            //var Invoice = _context.Invoices.SingleOrDefault(m => m.Id == id);

            if (UserAmount.Equals(null) || UserAmount == 0)
            {
                return View();
            }
            else
            {
                string Amount = ((Math.Truncate((decimal)0.05 * UserAmount)) + UserAmount).ToString();
                decimal AmountDecimalType = (Math.Truncate((decimal)0.05 * UserAmount)) + UserAmount;
                string Currency = "usd";
                int MerchantId = 2;
                int MerchantSiteId = 2;

                string CalculatedMd5 = getMd5Hash(MerchantId + ":" + Amount + ":" + Currency + ":" + "6dc023e3-9916-45dd-b7a1-885aab63bae6");
                string HashCode = CalculatedMd5;

                Filspay FPay = new Filspay();
                FPay.UserAmount = AmountDecimalType;
                FPay.ApplicationUserId = _userManager.GetUserId(User);
                FPay.Status = false;
                FPay.CreationDate = DateTime.Now;

                _context.Add(FPay);
                _context.SaveChanges();

                //_context.Filspays.Add(new Filspay
                //{
                //    UserAmount = UserAmount,
                //    ApplicationUserId = _userManager.GetUserId(User),
                //    Status = false,
                //    CreationDate = DateTime.Now,
                //});


                return Redirect(string.Format("https://filspay.com/Payments/Process?amount={0}&currency=usd&description=des&hashcode={1}&language=en&merchantid={2}&merchantsiteid={3}&trxrefnumber={4}", Amount, HashCode, MerchantId, MerchantSiteId, FPay.Id));
            }
        }


        public IActionResult TransferCredit()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferCredit(string Email, decimal Amount)
        {

            var fromuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));
            var touser = _context.ApplicationUsers.SingleOrDefault(a => a.Email == Email);
            try
            {
                if (fromuser.Balance >= Amount)
                {
                    _context.Statements.Add(new Statement
                    {
                        Amount = Amount,
                        RecordDate = DateTime.Now,
                        Destination = false,
                        ApplicationUserId = _userManager.GetUserId(User),
                        Details = "Transfer to " + Email,
                        BalanceType = BalanceType.currentbalance
                    });

                    _context.Statements.Add(new Statement
                    {
                        Amount = Amount,
                        RecordDate = DateTime.Now,
                        Destination = true,
                        ApplicationUserId = touser.Id,
                        Details = "Payment from " + fromuser.Email,
                        BalanceType = BalanceType.currentbalance
                    });


                    fromuser.Balance -= Amount;
                    touser.Balance += Amount;

                    _context.SaveChanges();

                    //-----------------------------------email content-----------------------------------------
                    var applicationuser = _context.ApplicationUsers.SingleOrDefault(a => a.Id == _userManager.GetUserId(User));

                    StringBuilder Welcomefromuser = new StringBuilder("<h3 align ='right'>");
                    Welcomefromuser.AppendFormat(string.Format(applicationuser.ArName));
                    Welcomefromuser.Append("</h3>");
                    Welcomefromuser.Append("</br>");

                    StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                    Footer.Append("</h3>");
                    Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة اريد "));
                    Footer.Append("</h3>");



                    StringBuilder Welcometouser = new StringBuilder("<h3 align ='right'>");
                    Welcometouser.AppendFormat(string.Format(applicationuser.ArName));
                    Welcometouser.Append("</h3>");
                    Welcometouser.Append("</br>");





                    EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "4e5a33b7-5291-4a66-9ca4-894e39eda2b1").SingleOrDefault();
                    StringBuilder MyStringBuilder = new StringBuilder("");
                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format("المبلغ  : $" + Amount));
                    MyStringBuilder.Append("</h2>");
                    MyStringBuilder.Append("<hr/>");
                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format("التاريخ  : " + DateTime.Now));
                    MyStringBuilder.Append("</h2>");
                    MyStringBuilder.Append("<hr/>");
                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format("المرسل  : " + fromuser.ArName));
                    MyStringBuilder.Append("</h2>");

                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format(fromuser.Email));
                    MyStringBuilder.Append("</h2>");
                    MyStringBuilder.Append("<hr/>");
                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format("المستلم  : " + touser.ArName));

                    MyStringBuilder.AppendFormat(string.Format("<h2 align ='right'>"));
                    MyStringBuilder.AppendFormat(string.Format(touser.Email));
                    MyStringBuilder.Append("</h2>");

                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(fromuser.Email, "ارسال رصيد في منصة اريد بمقدار " + Amount + "$ وبتاريخ " + DateTime.Now.ToLongDateString(), Welcomefromuser.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(10));
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(touser.Email, "استقبال رصيد في منصة اريد بمقدار " + Amount + "$ وبتاريخ " + DateTime.Now.ToLongDateString(), Welcometouser.ToString() + usercontent.ArContent + MyStringBuilder + Footer.ToString()), TimeSpan.FromSeconds(5));

                    //-----------------------------------email content-----------------------------------------

                    ViewData["PaymentStatus"] = "تمت تعبئة الحساب بنجاح";
                }
                else
                { ViewData["PaymentStatus"] = "ليس لديكم رصيد كافي لاتمام هذه الحوالة"; }
            }
            catch (Exception)
            {
                ViewData["PaymentStatus"] = "التفاصيل غير صحيحة";
                // throw;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model, IFormFile myfile1, IFormFile myfile2)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {

                if (CultureInfo.CurrentCulture.Name.Contains("ar"))
                {
                    ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", model.CountryId);
                    ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == model.CountryId), "Id", "ArCityName", model.CityId);
                    ViewData["UniversityId"] = new SelectList(_context.Universities.Where(c => c.CountryId == model.CountryId && c.IsIndexed == true), "Id", "ArUniversityName", model.UniversityId);
                    ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == model.UniversityId), "Id", "ArFacultyName", model.FacultyId);
                    ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", model.ReferredById);
                }
                else
                {
                    ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "EnCountryName", model.CountryId);
                    ViewData["CityId"] = new SelectList(_context.Cities.Where(c => c.CountryId == model.CountryId), "Id", "EnCityName", model.CityId);
                    ViewData["UniversityId"] = new SelectList(_context.Universities.Where(c => c.CountryId == model.CountryId && c.IsIndexed == true), "Id", "EnUniversityName", model.UniversityId);
                    ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(c => c.UniversityId == model.UniversityId), "Id", "EnFacultyName", model.FacultyId);
                    ViewData["ReferredById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", model.ReferredById);
                }
                var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
                List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
                ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);
                return View(model);
            }



            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            if (model.IsNotUniversity == true)
            {
                user.UniversityId = 1;
                user.FacultyId = 1;
            }
            else
            {
                user.UniversityId = model.UniversityId;
                user.FacultyId = model.FacultyId;
            }

            //------------------------------------------
            //user.RegDate = model.RegDate;
            //user.Status = model.Status;
            //user.ARID = model.ARID;
            //user.ARIDDate = model.ARIDDate;
            //user.ReferredById = model.ReferredById;
            if (user.ARID == 0)
            {
                int maxARID = _context.ApplicationUsers.Max(a => a.ARID);
                user.ARID = maxARID + 1;
                user.ARIDDate = DateTime.Now;
                user.Status = StatusType.Active;
            }

            user.SecondEmail = model.SecondEmail;
            user.ArName = model.ArName;
            user.EnName = model.EnName;
            user.OtherNames = model.OtherNames;
            user.DateofBirth = model.DateofBirth;
            user.Gender = model.Gender;
            user.UILanguage = model.UILanguage;
            user.CountryId = model.CountryId;
            user.CityId = model.CityId;
            user.FeaturedImage = model.FeaturedImage;
            user.Summary = model.Summary;
            user.ContactMeDetail = model.ContactMeDetail;
            model.ProfileImage = await UserFile.UploadeNewImageAsync(user.ProfileImage, myfile1, _environment.WebRootPath, Properties.Resources.ProfileImageFolder, 400, 300);
            user.ProfileImage = model.ProfileImage;
            model.CVURL = await UserFile.UploadeNewFileAsync(user.CVURL, myfile2, _environment.WebRootPath, Properties.Resources.CVFileFolder);
            user.CVURL = model.CVURL;
            //user.Visitors = model.Visitors;
            user.DAL = model.DAL;
            user.DALEnabled = true;
            user.IsFreelancer = model.IsFreelancer;
            user.LastLogin = DateTime.Now;
            user.FirmName = model.FirmName;
            user.IsNotUniversity = model.IsNotUniversity;

            await _userManager.UpdateAsync(user);
            _context.SaveChanges();
            //------------------------------------------

            StatusMessage = _localizer["Updated"];
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> DownloadCV(string filename)
        //{
        //    if (filename == null)
        //        return Content("filename not present");

        //    var path = Path.Combine(_environment.WebRootPath,
        //        Properties.Resources.CVFileFolder, filename);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }

        //    memory.Position = 0;
        //    return File(memory, UserFile.GetContentType(path), Path.GetFileName(path));
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = _localizer["Verificationemailsent"];
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            //http://findnerd.com/list/view/Reset-Password-without-taking-old-password-as-parameter-in-Asp-Net-Identity/17915/
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.NewPassword);

            // var changePasswordResult = await _userManager.ChangePasswordAsync(user, user.PasswordHash, model.NewPassword);
            //var changePasswordResult = await _userManager.pass(user, model.OldPassword, model.NewPassword);

            //if (!changePasswordResult.Succeeded)
            //{
            //    AddErrors(changePasswordResult);
            //    return View(model);
            //}

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(_localizer["LoggingMessage"]);
            StatusMessage = _localizer["ChangeMessage"];

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = _localizer["Set"];

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = _localizer["Added"];
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            UserFile.DeleteOldImage(_environment.WebRootPath, Properties.Resources.ProfileImageFolder, user.ProfileImage);

            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", _localizer["Invalid"]);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DirectAccessLink()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.DALEnabled)
            {

                throw new ApplicationException($"Cannot generate Direct Access Link DAL for user with ID '{user.Id}' as they do not have DAL enabled.");
            }
            var recoveryCodes = user.DAL;
            ViewData["DAL"] = recoveryCodes;
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("ARID"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }



        #endregion
    }
}
