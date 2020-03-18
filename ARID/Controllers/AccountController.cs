using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ARID.Models;
using ARID.Models.AccountViewModels;
using ARID.Services;
using ARID.Data;
using ARID.Controllers;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ARID.Controllers
{
    [Authorize]
    //[Route("{culture}/[controller]/[action]")] // commented out to keep the culture in the route
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ARIDDbContext _context;//=======================

        public AccountController(
            ARIDDbContext context,
            RoleManager<IdentityRole> roleMgr,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleMgr;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = logger;
            _context = context;
            _localizer = localizer;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    //        if (DAL != null)
    //        {
    //            var applicationUser = await _context.Users
    //.SingleOrDefaultAsync(m => m.DAL == DAL);

    //            if (applicationUser == null)
    //            {
    //                return NotFound();
    //            }

    //            await _signInManager.SignInAsync(applicationUser, isPersistent: true);
    //            return RedirectToLocal(returnUrl);
    //        }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["Invalidloginattempt"]);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public JsonResult GetCitiesList(int countryid)
        {
            var cities = new SelectList(_context.Cities.Where(c => c.CountryId == countryid), "Id", "ArCityName");
            return Json(cities);
        }

        [AllowAnonymous]
        public JsonResult GetUniversitiesList(int countryid)
        {
            var universities = new SelectList(_context.Universities.Where(u => u.CountryId == countryid), "Id", "ArUniversityName");
            return Json(universities);
        }

        [AllowAnonymous]
        public JsonResult GetFacultiesList(int universityid)
        {
            var faculties = new SelectList(_context.Faculties.Where(c => c.UniversityId == universityid), "Id", "ArFacultyName");
            return Json(faculties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, _localizer["Invalidauthenticatorcode"]);
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, _localizer["Invalidrecoverycodeentered"]);
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        private void BasicDBSeeding()
        {
            if (_context.Countries.Count() == 0)
            {
                _context.Countries.Add(new Country
                {
                    ArCountryName = "أخرى",
                    EnCountryName = "Other",
                    ShortName = "OTH",
                    CountryCode = "0"
                });
                _context.SaveChanges();
            }
            if (_context.Cities.Count() == 0)
            {
                _context.Cities.Add(new City { ArCityName = "أخرى", EnCityName = "Other", CountryId = 1 });
                _context.SaveChanges();
            }
            if (_context.Universities.Count() == 0)
            {
                _context.Universities.Add(
                    new University
                    {
                        ArUniversityName = "أخرى",
                        EnUniversityName = "Other",
                        CountryId = 1,
                        Governmental = true,
                        Private = false,
                        SemiPrivate = false,
                        StaffNo = 0,
                        StudentNo = 0,
                        YearofEstablishment = 1800
                    });
                _context.SaveChanges();
            }
            if (_context.Specialities.Count() == 0)
            {
                _context.Specialities.Add(new Speciality
                {
                    Name = "أخرى",
                    EnSpecialityName = "Other"
                });
                _context.SaveChanges();
            }
            if (_context.Faculties.Count() == 0)
            {
                _context.Faculties.Add(new Faculty
                {
                    ArFacultyName = "أخرى",
                    EnFacultyName = "Other",
                    CityId = 1,
                    SpecialityId = 1,
                    UniversityId = 1,
                    HasPostGraduation = true
                });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            // BasicDBSeeding();
            if (_userManager.GetUserId(User) != null)
            {
                _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToAction("Register", "Account");
            }
               ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName"); // working
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
            //  ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    // Added by Alrshah =============================================================================================
                    ArName = model.ArName,
                    EnName = model.EnName,
                    //DateofBirth = model.DateofBirth,
                    //Gender = model.Gender,
                    UILanguage = model.UILanguage,
                    CityId = 1,
                    CountryId = model.CountryId,
                    FacultyId = 1,
                    UniversityId = 1,
                    Status = StatusType.New,
                    RegDate = DateTime.Now,
                    ARID = 0,
                    ReferredById = model.ReferredById,
                    Visitors = 0,
                    DAL = Guid.NewGuid().ToString(),
                    DALEnabled = true



                    // Added by Alrshah =============================================================================================
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString() == "05e26575-8bb0-430b-ba12-fb43e2282a65").SingleOrDefault();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content.ArSubject, content.ArContent), TimeSpan.FromSeconds(10));

                    EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "495334bf-b4d1-43ed-bfe4-f947732fa83e").SingleOrDefault();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content2.ArSubject, content2.ArContent), TimeSpan.FromDays(2));

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    //var userId = new { id = _userManager.Users.First().Id };
                    var userId = new { id = user.Id };

                    // Added by Alrshah ===================================================
                    if (!_roleManager.Roles.Any(r => r.Name == RoleName.Admins))
                        await _roleManager.CreateAsync(new IdentityRole(RoleName.Admins));
                    if (!_roleManager.Roles.Any(r => r.Name == RoleName.Members))
                        await _roleManager.CreateAsync(new IdentityRole(RoleName.Members));

                    if (_context.ApplicationUsers.Count() == 1)
                    {
                        if (_roleManager.Roles.Any(r => r.Name == RoleName.Admins))
                            await _userManager.AddToRoleAsync(user, RoleName.Admins);
                    }
                    if (_roleManager.Roles.Any(r => r.Name == RoleName.Members))
                        await _userManager.AddToRoleAsync(user, RoleName.Members);
                    // Added by Alrshah ===================================================

                    // return RedirectToAction("Index", "Manage");// Added by Alrshah ===================================================
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
                //  ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName"); // working
                //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName"); // working
                var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
                List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
                ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);

                return View(model);
            }
            else
            {
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", 0); // working
                ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName"); // working
                var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
                List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
                ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterByUser(string returnUrl = null)
        {
            // BasicDBSeeding();
            //if (_userManager.GetUserId(User) != null)
            //{
            //    _signInManager.SignOutAsync();
            //    _logger.LogInformation("User logged out.");
            //    return RedirectToAction("RegisterByUser", "Account");
            //}
            //   ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName"); // working
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName");
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
            //  ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterByUser(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    // Added by Alrshah =============================================================================================
                    ArName = model.ArName,
                    EnName = model.EnName,
                    //DateofBirth = model.DateofBirth,
                    //Gender = model.Gender,
                    UILanguage = model.UILanguage,
                    CityId = 1,
                    CountryId = model.CountryId,
                    FacultyId = 1,
                    UniversityId = 1,
                    Status = StatusType.New,
                    RegDate = DateTime.Today.Date,
                    ARID = 0,
                    ReferredById = model.ReferredById,
                    Visitors = 0,
                    DAL = Guid.NewGuid().ToString(),
                    DALEnabled = true



                    // Added by Alrshah =============================================================================================
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString() == "05e26575-8bb0-430b-ba12-fb43e2282a65").SingleOrDefault();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content.ArSubject, content.ArContent), TimeSpan.FromDays(1));

                    EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "495334bf-b4d1-43ed-bfe4-f947732fa83e").SingleOrDefault();
                    BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content2.ArSubject, content2.ArContent), TimeSpan.FromDays(2));

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    var codeDal = user.DAL;
                    var callbackUrlDal = Url.DirectAccessLink(codeDal, Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        _localizer["SendDAL"],
                       "<center>" + _localizer["DALEmailContent"] +
                        $"<h2><a style='font: bold 16px Arial;text-decoration: none;background-color: #EEEEEE;color: #333333;padding: 2px 6px 2px 6px;border-top: 1px solid #CCCCCC;border-right: 1px solid #333333;border-bottom: 1px solid #333333;border-left: 1px solid #CCCCCC;' href='{callbackUrl}'>" + _localizer["DALLink"] + "</a></h2></center>");


                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    //var userId = new { id = _userManager.Users.First().Id };
                    var userId = new { id = user.Id };

                    // Added by Alrshah ===================================================
                    if (!_roleManager.Roles.Any(r => r.Name == RoleName.Admins))
                        await _roleManager.CreateAsync(new IdentityRole(RoleName.Admins));
                    if (!_roleManager.Roles.Any(r => r.Name == RoleName.Members))
                        await _roleManager.CreateAsync(new IdentityRole(RoleName.Members));

                    if (_context.ApplicationUsers.Count() == 1)
                    {
                        if (_roleManager.Roles.Any(r => r.Name == RoleName.Admins))
                            await _userManager.AddToRoleAsync(user, RoleName.Admins);
                    }
                    if (_roleManager.Roles.Any(r => r.Name == RoleName.Members))
                        await _userManager.AddToRoleAsync(user, RoleName.Members);
                    // Added by Alrshah ===================================================

                    // return RedirectToAction("Index", "Manage");// Added by Alrshah ===================================================
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
                //  ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName"); // working
                //  ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName"); // working
                var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
                List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
                ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);

                return View(model);
            }
            else
            {
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "ArCountryName", 0); // working
                ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "ArUniversityName"); // working
                var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
                List<CultureInfo> cultureItems = ((RouteDataRequestCultureProvider)requestCulture.Provider).Options.SupportedCultures.ToList();
                ViewData["UILanguageId"] = new SelectList(cultureItems, "Name", "NativeName", model.UILanguage);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[AllowAnonymous]
        public async void ResendConfirmationEmail(string id)
        {
            ApplicationUser user = _context.ApplicationUsers.Where(m => m.Id == id).SingleOrDefault();
            if ((id != null) && (user != null))
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    ArName = "",
                    EnName = "",
                    //DateofBirth = model.DateofBirth,
                    //Gender = model.Gender,
                    UILanguage = "ar",
                    CityId = 1,
                    CountryId = 1,
                    FacultyId = 1,
                    UniversityId = 1,
                    Status = StatusType.New,
                    RegDate = DateTime.Today.Date,
                    ARID = 0,
                    ReferredById = model.ReferredById,
                    DAL = Guid.NewGuid().ToString(),
                    DALEnabled = true

                };

                EmailContent content = _context.EmailContents.Where(m => m.UniqueName.ToString() == "05e26575-8bb0-430b-ba12-fb43e2282a65").SingleOrDefault();
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content.ArSubject, content.ArContent), TimeSpan.FromDays(1));

                EmailContent content2 = _context.EmailContents.Where(m => m.UniqueName.ToString() == "495334bf-b4d1-43ed-bfe4-f947732fa83e").SingleOrDefault();
                BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(model.Email, content2.ArSubject, content2.ArContent), TimeSpan.FromDays(2));

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            //https://stackoverflow.com/questions/25405307/asp-net-identity-2-giving-invalid-token-error
            //we have generated new token and verified accordingly
            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
            ApplicationUsersController AUC = new ApplicationUsersController(_context, _userManager, null, null, null);

            if (result.Succeeded && user.ARID == 0) AUC.AssignARID(user.Id);
            AUC.Dispose();
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(
                    model.Email,
                    _localizer["ResetEmailTitle"],
                   "<center>" + _localizer["ResetEmailContent"] +
                    $"<h2><a href='{callbackUrl}'>" + _localizer["Link"] + "</a></h2></center>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendDAL()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendDAL(DALViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(UnactiveAccountExist));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = user.DAL;
                var callbackUrl = Url.DirectAccessLink(code, Request.Scheme);
                await _emailSender.SendEmailAsync(
                    model.Email,
                    _localizer["SendDAL"],
                   "<center>" + _localizer["DALEmailContent"] +
                    $"<h2><a style='font: bold 16px Arial;text-decoration: none;background-color: #EEEEEE;color: #333333;padding: 2px 6px 2px 6px;border-top: 1px solid #CCCCCC;border-right: 1px solid #333333;border-bottom: 1px solid #333333;border-left: 1px solid #CCCCCC;' href='{callbackUrl}'>" + _localizer["DALLink"] + "</a></h2></center>");
                return RedirectToAction(nameof(SendDALConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendDALConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UnactiveAccountExist()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DAL(string id)
        {
            var applicationUser = await _context.Users
.SingleOrDefaultAsync(m => m.DAL == id && m.DALEnabled == true);

            if (applicationUser == null)
            {
                return NotFound();
            }

            await _signInManager.SignInAsync(applicationUser, isPersistent: true);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
