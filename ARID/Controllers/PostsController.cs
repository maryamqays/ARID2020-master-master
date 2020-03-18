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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ARID.AuxiliaryClasses;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Hangfire;
using ARID.Services;

namespace ARID.Controllers
{
    public class PostsController : Controller
    {
        private readonly ARIDDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;
        private readonly IEmailSender _emailSender;

        public PostsController(ARIDDbContext context, UserManager<ApplicationUser> userMrg, IHostingEnvironment environment, IEmailSender emailSender)
        {
            _context = context;
            _environment = environment;
            _userManager = userMrg;
            _emailSender = emailSender;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int id)
        {
            var aRIDDbContext = _context.Posts.Where(a => a.CommunityId == id).Include(p => p.ApplicationUser).Include(p => p.Community);
            ViewData["CommunityName"] = _context.Communities.SingleOrDefault(a => a.Id == id).Name;
            return View(await aRIDDbContext.ToListAsync());
        }


        public async Task<IActionResult> List()
        {
            var aRIDDbContext = _context.Posts.OrderByDescending(a => a.DateTime).Include(p => p.ApplicationUser).Include(a => a.Community);
            @ViewData["PageName"] = "جديد المشاركات في جميع المجتمعات العلمية";
            return View(await aRIDDbContext.ToListAsync());
        }

        public async Task<IActionResult> Featured()
        {
            var aRIDDbContext = _context.Posts.Where(a => a.IsFeatured == true).OrderByDescending(a => a.DateTime).Include(p => p.ApplicationUser).Include(a => a.Community);
            @ViewData["PageName"] = "المشاركات المميزة. ساهم بإثرائها";
            return View(await aRIDDbContext.ToListAsync());
        }

        public async Task<IActionResult> UnSolved()
        {

            var PostViewModel = new PostViewModel
            {
                PostComments = _context.PostComments.Include(p => p.Post).Include(a => a.Post.Community).Include(p => p.ApplicationUser).OrderByDescending(a => a.DateTime)
            };
            @ViewData["PageName"] = "اسئلة لم تحل. ساهم بحلها";
            return View(PostViewModel);
        }

        public async Task<IActionResult> Solved()
        {

            var PostViewModel = new PostViewModel
            {
                PostComments = _context.PostComments.Include(p => p.Post).Include(a => a.Post.Community).Include(p => p.ApplicationUser).OrderByDescending(a => a.DateTime)
            };
            @ViewData["PageName"] = "اسئلة تم حلها. ساهم بإثراءها";
            return View(PostViewModel);
        }

        // GET: Posts/Details/5
        //[Route("{Details}/{id}")]
        public async Task<IActionResult> Details(Guid? id, string t)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                if (_context.Posts.SingleOrDefault(a => a.Id == id).IsDeleted == true)
                {
                    return NotFound();
                }

                var PostItem = _context.Posts.SingleOrDefaultAsync(m => m.Id == id);


                if (_context.PostComments.Where(a => a.PostId == id & a.IsBestAnswer == true).Count() > 0)
                { ViewData["Answered"] = "AnswerAssigned"; }

                //if (_context.PostComment.Include(a=>a.Post).Include(a=>a.Post.Community).Where(a => a.Post.Community.CommunityType == CommunityType.Community).Count() > 0)
                //{ ViewData["Answered"] = "AnswerAssigned"; }

                if (_context.Posts.SingleOrDefault(a => a.Id == id).PostType != GroupPostType.QA)
                { ViewData["Answered"] = "AnswerAssigned"; }


                var postViewModel = new PostViewModel
                {
                    Post = await _context.Posts
 .Include(p => p.ApplicationUser)
 .Include(p => p.Community)
 .SingleOrDefaultAsync(m => m.Id == id),
                    PostComments = _context.PostComments.Where(a => a.PostId == id).Include(p => p.ApplicationUser),
                    PostMetrics = _context.PostMetrics.Where(a => a.PostId == id).Include(a => a.ApplicationUser),
                    CommentMetrics = _context.CommentMetrics.Where(a => a.PostComment.PostId == id).Include(a => a.ApplicationUser)
                };

                ViewData["PostWeight"] = _context.PostMetrics.Where(a => a.PostId == id).Sum(a => a.VoteValue);
                //ViewData["CommentWeight"] = _context.CommentMetric.Where(a => a.PostId == id).Sum(a=>a.VoteValue);
                if (postViewModel == null)
                {
                    return NotFound();
                }
                
                //if (string.IsNullOrWhiteSpace(t))
                //{
                //    t = _context.Posts.First(p => p.Id == id).Title.Replace("/", "-");
                //    t = t.Replace("%09", "-");
                //    t = t.Replace(@"\", "-");
                //    t = t.Replace(@" ", "-");
                //    t = t.Replace("  ", "-");
                //    t = t.Replace("--", "-");

                //    return RedirectToAction("Details", new { id = id, t = RemoveSpecialChars(t.Replace(" ", "-")) });
                //}
                //else
                //{
                    if (HttpContext.Session.GetString("BlogId") != id.ToString())
                    {
                        _context.Posts.SingleOrDefaultAsync(m => m.Id == id).Result.Reads++;
                        HttpContext.Session.SetString("BlogId", id.ToString());

                        if (_context.Posts.SingleOrDefaultAsync(m => m.Id == id).Result.Reads == 99) {

                            //-----------------------------------email content-----------------------------------------

                            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
                            Welcome.AppendFormat(string.Format(_context.Posts.Include(a=>a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id).Result.ApplicationUser.ArName));
                            Welcome.Append("</h3>");
                            Welcome.Append("</br>");

                            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                            Welcome.Append("</h3>");
                            Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة أُريد "));

                            Welcome.Append("</h3>");

                            EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "8afef0ad-ae04-4787-9954-8c3cb307852e").SingleOrDefault();
                            StringBuilder MyStringBuilder = new StringBuilder("");
                            MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Post/{0}' >", id));
                            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل الموضوع");
                            MyStringBuilder.Append("</a></h2>");


                            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(PostItem.Result.ApplicationUser.Email, "موضوع مميز / " + PostItem.Result.Title, Welcome.ToString() + usercontent.ArContent + MyStringBuilder.ToString() + Footer.ToString()), TimeSpan.FromSeconds(1));

                            //-----------------------------------email content-----------------------------------------

                        }

                        if (_context.Posts.SingleOrDefaultAsync(m => m.Id == id).Result.Reads == 49)
                        {

                            //-----------------------------------email content-----------------------------------------

                            StringBuilder Welcome = new StringBuilder("<h3 align ='right'>سعادة ");
                            Welcome.AppendFormat(string.Format(_context.Posts.Include(a => a.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id).Result.ApplicationUser.ArName));
                            Welcome.Append("</h3>");
                            Welcome.Append("</br>");

                            StringBuilder Footer = new StringBuilder("<h3 align ='right'>أطيب التحيات ");
                            Welcome.Append("</h3>");
                            Footer.AppendFormat(string.Format("<h3 align ='right'>فريق منصة أُريد "));

                            Welcome.Append("</h3>");

                            EmailContent usercontent = _context.EmailContents.Where(m => m.UniqueName.ToString() == "fb8275f9-52b9-4183-aca0-b1ac92990a20").SingleOrDefault();
                            StringBuilder MyStringBuilder = new StringBuilder("");
                            MyStringBuilder.AppendFormat(string.Format("<h2 align ='center'><a href='https://portal.arid.my/ar-LY/Post/{0}' >", id));
                            MyStringBuilder.Append("اضغط هنا للاطلاع على تفاصيل الموضوع");
                            MyStringBuilder.Append("</a></h2>");


                            BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(PostItem.Result.ApplicationUser.Email, "موضوع مميز / " + PostItem.Result.Title, Welcome.ToString() + usercontent.ArContent + MyStringBuilder.ToString() + Footer.ToString()), TimeSpan.FromSeconds(1));

                            //-----------------------------------email content-----------------------------------------

                        }


                    }
                  
                //}
                await _context.SaveChangesAsync();
                return View(postViewModel);

            }
            catch (Exception)
            {
                t = _context.Posts.First(p => p.Id == id).Tags;
                return RedirectToAction("Details", new { id = id });
                // throw;
            }

        }

        public static string RemoveSpecialChars(string str)
        {
            // Create  a string array and add the special characters you want to remove
            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]", "09%" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "-");
                }
            }
            return str;
        }

        //public static string StripHtmlTags(this string source)
        //{
        //    return Regex.Replace(source, "<.*?>|&.*?;", string.Empty);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("Id,Comment,DateTime,IsHidden,IsFeatured,IsDeleted,PostId,ApplicationUserId")] PostComment postComment, Guid PostId)
        {
            if (ModelState.IsValid)
            {
                postComment.ApplicationUserId = _userManager.GetUserId(User);
                postComment.DateTime = DateTime.Now;
                postComment.IsDeleted = false;
                postComment.IsFeatured = false;
                postComment.IsHidden = false;
                postComment.PostId = PostId;
                postComment.Id = Guid.NewGuid();
                _context.Add(postComment);

                _context.ScoreLogs.Add(new ScoreLog
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userManager.GetUserId(User),
                    PostId = PostId,
                    Date = DateTime.Now,
                    ScoreValueId = 3
                });
                //addnotification{
                var post = _context.Posts.Include(p=>p.ApplicationUser).SingleOrDefault(p => p.Id == PostId);
                var commenter = _context.Users.SingleOrDefault(u=>u.Id==_userManager.GetUserId(User));
                _context.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = post.ApplicationUserId,
                    Content = "علق "+ commenter.ArName + " على منشورك",
                    Url = "https://portal.arid.my/ar-LY/Posts/Details/"+ PostId +"",
                    CreationDate = DateTime.Now,
                    SenderId=_userManager.GetUserId(User),
                    IsDeleted=false,
                    IsRead=false,
                });

                //}
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = PostId });
            }

            return RedirectToAction("Details", new { id = PostId });
        }

        public JsonResult Skills_Read(string text)
        {
            var result = GetSkills();
            var result1 = GetExpertises();
            var result2 = GetSpeciality();

            if (!string.IsNullOrEmpty(text))
            {
                result = result.Where(p => p.Name.Contains(text)).ToList();
                result1 = result1.Where(p => p.Name.Contains(text)).ToList();
                result2 = result2.Where(p => p.Name.Contains(text) || p.EnSpecialityName.Contains(text)).ToList();
            }

            // var empIds = result.Select(x => x.Name).Concat(result1.Select(x => x.Name));

            IEnumerable<object> c = result.Cast<object>()
                          .Concat(result1.Cast<object>()).Concat(result2.Cast<object>());


            //var sb = new StringBuilder();
            //sb.Append(result);
            //sb.Append(result1);

            // return Json(sb.ToString());
            // return Json(new { result, result1 });
            return Json(c);
        }



        private IEnumerable<Skill> GetSkills()
        {
            var result = _context.Skills;
            return result;
        }

        private IEnumerable<Expertise> GetExpertises()
        {
            var result = _context.Expertises;

            return result;
        }

        private IEnumerable<Speciality> GetSpeciality()
        {
            var result = _context.Specialities;
            return result;
        }

        // GET: Posts/Create
        public IActionResult Create(int cid)
        {
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == cid), "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? cid, [Bind("Id,Title,Body,DateTime,IsCommentsAllowed,Image,File,IsApproved,IsHidden,IsFeatured,Reads,IsDeleted,CommunityId,ApplicationUserId,Tags,PostType,IsPublishRequest,PublishRequestStatus,IsGifted,GiftType")] Post post, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                post.Image = await UserFile.UploadeNewImageAsync(post.Image,
    myfile, _environment.WebRootPath, Properties.Resources.Community, 500, 500);
                post.File = await UserFile.UploadeNewFileAsync(post.File,
          myfile1, _environment.WebRootPath, Properties.Resources.Community);
                post.ApplicationUserId = _userManager.GetUserId(User);
                post.DateTime = DateTime.Now;
                post.Reads = 0;
                post.IsApproved = false;
                post.IsDeleted = false;
                post.IsHidden = false;
                post.IsFeatured = false;
                post.IsGifted = false;
                //if (_context.Community.SingleOrDefault(a => a.Id == cid).CommunityType == CommunityType.Community)
                //{
                post.PostType = GroupPostType.QA;
                //}
                //else
                //{ post.PostType = PostType.Article; }

                post.Id = Guid.NewGuid();
                _context.Add(post);
                _context.ScoreLogs.Add(new ScoreLog
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userManager.GetUserId(User),
                    PostId = post.Id,
                    Date = DateTime.Now,
                    ScoreValueId = 2
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = post.Id });
            }
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == post.CommunityId), "Id", "Name", cid);
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult CreateGroupPost(int cid)
        {
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == cid), "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroupPost(int? cid, [Bind("Id,Title,Body,DateTime,IsCommentsAllowed,Image,File,IsApproved,IsHidden,IsFeatured,Reads,IsDeleted,CommunityId,ApplicationUserId,Tags,PostType,IsPublishRequest,PublishRequestStatus,IsGifted,GiftType")] Post post, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                post.Image = await UserFile.UploadeNewImageAsync(post.Image,
    myfile, _environment.WebRootPath, Properties.Resources.Secured, 700, 500);
                post.File = await UserFile.UploadeNewFileAsync(post.File,
          myfile1, _environment.WebRootPath, Properties.Resources.Secured);
                post.ApplicationUserId = _userManager.GetUserId(User);
                post.DateTime = DateTime.Now;
                post.Reads = 0;
                post.IsApproved = true;
                post.IsDeleted = false;
                post.IsHidden = false;
                post.IsFeatured = false;
                post.IsGifted = false;
                //if (_context.Community.SingleOrDefault(a => a.Id == cid).CommunityType == CommunityType.Community)
                //{
                post.PostType = GroupPostType.QA;
                //}
                //else
                //{ post.PostType = PostType.Article; }

                post.Id = Guid.NewGuid();
                _context.Add(post);
                _context.ScoreLogs.Add(new ScoreLog
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userManager.GetUserId(User),
                    PostId = post.Id,
                    Date = DateTime.Now,
                    ScoreValueId = 1
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = post.Id });
            }
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == post.CommunityId), "Id", "Name", cid);
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult CreateBlog(int cid)
        {
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == cid), "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(int? cid, [Bind("Id,Title,Body,DateTime,IsCommentsAllowed,Image,File,IsApproved,IsHidden,IsFeatured,Reads,IsDeleted,CommunityId,ApplicationUserId,Tags,PostType,IsPublishRequest,PublishRequestStatus,IsGifted,GiftType")] Post post, IFormFile myfile, IFormFile myfile1)
        {
            if (ModelState.IsValid)
            {
                post.Image = await UserFile.UploadeNewImageAsync(post.Image,
    myfile, _environment.WebRootPath, Properties.Resources.Community, 500, 500);
                post.File = await UserFile.UploadeNewFileAsync(post.File,
          myfile1, _environment.WebRootPath, Properties.Resources.Community);
                post.ApplicationUserId = _userManager.GetUserId(User);
                post.DateTime = DateTime.Now;
                post.Reads = 0;
                post.IsApproved = true;
                post.IsDeleted = false;
                post.IsHidden = false;
                post.IsFeatured = false;
                post.IsGifted = false;
                post.Body = (System.Text.RegularExpressions.Regex.Replace(post.Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                //post.Title = RemoveSpecialChars(post.Title.Replace("/", " "));
              //  post.Title = post.Title.Replace("/", "");
              //  post.Title = post.Title.Replace(@"\", string.Empty);
                //if (_context.Community.SingleOrDefault(a => a.Id == cid).CommunityType == CommunityType.Community)
                //{
                post.PostType = GroupPostType.QA;
                //}
                //else
                //{ post.PostType = PostType.Article; }

                post.Id = Guid.NewGuid();
                _context.Add(post);
                _context.ScoreLogs.Add(new ScoreLog
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userManager.GetUserId(User),
                    PostId = post.Id,
                    Date = DateTime.Now,
                    ScoreValueId = 2
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = post.Id });
            }
            ViewData["CommunityId"] = new SelectList(_context.Communities.Where(a => a.Id == post.CommunityId), "Id", "Name", cid);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> EditBlog(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            //ViewData["Body"] = (post.Body).Replace("<br/>", "\n");
            ViewData["CommunityId"] = new SelectList(_context.Set<Community>(), "Id", "Name", post.CommunityId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(Guid id, [Bind("Id,Title,Body,DateTime,IsCommentsAllowed,Image,File,IsApproved,IsHidden,IsFeatured,Reads,IsDeleted,CommunityId,ApplicationUserId,Tags,PostType,IsPublishRequest,PublishRequestStatus,IsGifted,GiftType")] Post post, IFormFile myfile, IFormFile myfile1)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                   // var postitem = _context.Posts.SingleOrDefault(m => m.Id == id);
                    post.Image = await UserFile.UploadeNewImageAsync(post.Image,
myfile, _environment.WebRootPath, Properties.Resources.Community, 500, 500);

                    post.File = await UserFile.UploadeNewFileAsync(post.File,
              myfile1, _environment.WebRootPath, Properties.Resources.Community);
                    post.ApplicationUserId = _userManager.GetUserId(User);
                    post.DateTime = DateTime.Now;
                    post.IsHidden = post.IsHidden;
                    post.Body = (System.Text.RegularExpressions.Regex.Replace(post.Body, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty)).Replace("\n", "<br/>");
                  //  post.Title = post.Title.Replace("/", "-");

                    _context.Update(post);
                     _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = post.Id });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", post.ApplicationUserId);
            ViewData["CommunityId"] = new SelectList(_context.Set<Community>(), "Id", "BgImage", post.CommunityId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.ApplicationUser)
                .Include(p => p.Community)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            //var PostComments = _context.PostComment.Where(m => m.PostId == id);
            //var Scorelog = _context.ScoreLog.Where(m => m.PostId == id);
            //var PostMetric =  _context.PostMetric.SingleOrDefault(m => m.PostId == id);
            _context.Posts.SingleOrDefault(m => m.Id == id).IsDeleted = true;

            //_context.PostComment.RemoveRange(PostComments);
            //_context.ScoreLog.RemoveRange(Scorelog);
            //_context.PostMetric.Remove(PostMetric);
            //_context.Post.Remove(post);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index), "Blogs");
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [Authorize]
        public IActionResult Follow(Guid id)
        {
            Post post = _context.Posts.Where(m => m.Id == id).SingleOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                _context.PostMetrics.Add(new PostMetric
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = currentuserid,
                    PostId = id,
                    NotifyMe = true,
                    DateTime = DateTime.Now
                });

            }

            else
            {
                _context.PostMetrics.SingleOrDefault(a => a.PostId == id && a.ApplicationUserId == currentuserid).NotifyMe = true;
            }
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult UnFollow(Guid id)
        {
            Post post = _context.Posts.Where(m => m.Id == id).SingleOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            string currentuserid = _userManager.GetUserId(User);
            if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid && f.NotifyMe == true).Count() > 0)
            {
                _context.PostMetrics.SingleOrDefault(a => a.PostId == id & a.ApplicationUserId == currentuserid).NotifyMe = false;
                _context.SaveChanges();
            }
            //ViewData["Count"] = _context.PostMetric.Count(a => a.PostId == id);
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult Up(Guid id)
        {
            Post post = _context.Posts.Where(m => m.Id == id).SingleOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);

            if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.Add(new PostMetric
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = currentuserid,
                        PostId = id,
                        NotifyMe = false,
                        VoteValue = 1
                    });

                    if (_context.PostMetrics.Where(a => a.PostId == id).Sum(a => a.VoteValue) > 5)
                    {
                        post.IsFeatured = true;
                    }

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 4
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = post.ApplicationUserId,
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 5
                    });
                }
            }
            else if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid && f.VoteValue == 0).Count() > 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.SingleOrDefault(a => a.PostId == id & a.ApplicationUserId == currentuserid).VoteValue = 1;

                    if (_context.PostMetrics.Where(a => a.PostId == id).Sum(a => a.VoteValue) > 5)
                    {
                        post.IsFeatured = true;
                    }
                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 4
                    });


                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = post.ApplicationUserId,
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 5
                    });
                }
            }

            else if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid && f.VoteValue == -1).Count() > 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.SingleOrDefault(a => a.PostId == id & a.ApplicationUserId == currentuserid).VoteValue = 1;

                    if (_context.PostMetrics.Where(a => a.PostId == id).Sum(a => a.VoteValue) > 5)
                    {
                        post.IsFeatured = true;
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Details/" + id);
        }
        [Authorize]
        public IActionResult Down(Guid id)
        {
            Post post = _context.Posts.Where(m => m.Id == id).SingleOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);

            if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid).Count() == 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.Add(new PostMetric
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = currentuserid,
                        PostId = id,
                        NotifyMe = false,
                        VoteValue = -1
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 7
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = post.ApplicationUserId,
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 8
                    });
                }

            }
            else if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid && f.VoteValue == 0).Count() > 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.SingleOrDefault(a => a.PostId == id & a.ApplicationUserId == currentuserid).VoteValue = -1;

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 7
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = post.ApplicationUserId,
                        PostId = post.Id,
                        Date = DateTime.Now,
                        ScoreValueId = 8
                    });
                }
            }

            else if (_context.PostMetrics.Where(f => f.PostId == id && f.ApplicationUserId == currentuserid && f.VoteValue == 1).Count() > 0)
            {
                if (currentuserid != post.ApplicationUserId)
                {
                    _context.PostMetrics.SingleOrDefault(a => a.PostId == id & a.ApplicationUserId == currentuserid).VoteValue = -1;
                }
            }
            _context.SaveChanges();


            return RedirectToAction("Details/" + id);
        }


        [Authorize]
        public IActionResult CommentUp(Guid id)
        {

            PostComment PostComment = _context.PostComments.Where(m => m.Id == id).SingleOrDefault();

            if (PostComment == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid != PostComment.ApplicationUserId)
            {
                if (_context.CommentMetrics.Where(f => f.PostCommentId == id && f.ApplicationUserId == currentuserid).Count() == 0)
                {
                    _context.CommentMetrics.Add(new CommentMetric
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = currentuserid,
                        PostCommentId = id,
                        VoteValue = 1,
                        Date = DateTime.Now,
                        ReportType = ReportType.None
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = PostComment.PostId,
                        Date = DateTime.Now,
                        ScoreValueId = 9
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = PostComment.ApplicationUserId,
                        PostId = PostComment.PostId,
                        Date = DateTime.Now,
                        ScoreValueId = 10
                    });


                }
                else if (_context.CommentMetrics.Where(f => f.PostCommentId == id && f.ApplicationUserId == currentuserid && f.VoteValue == -1).Count() > 0)
                {
                    _context.CommentMetrics.SingleOrDefault(a => a.PostCommentId == id & a.ApplicationUserId == currentuserid).VoteValue = 1;
                }

                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }

        [Authorize]
        public IActionResult CommentDown(Guid id)
        {

            PostComment PostComment = _context.PostComments.Where(m => m.Id == id).SingleOrDefault();

            if (PostComment == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);
            if (currentuserid != PostComment.ApplicationUserId)
            {
                if (_context.CommentMetrics.Where(f => f.PostCommentId == id && f.ApplicationUserId == currentuserid).Count() == 0)
                {
                    _context.CommentMetrics.Add(new CommentMetric
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = currentuserid,
                        PostCommentId = id,
                        VoteValue = -1,
                        Date = DateTime.Now,
                        ReportType = ReportType.None
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = _userManager.GetUserId(User),
                        PostId = PostComment.PostId,
                        Date = DateTime.Now,
                        ScoreValueId = 11
                    });

                    _context.ScoreLogs.Add(new ScoreLog
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = PostComment.ApplicationUserId,
                        PostId = PostComment.PostId,
                        Date = DateTime.Now,
                        ScoreValueId = 12
                    });
                }
                else if (_context.CommentMetrics.Where(f => f.PostCommentId == id && f.ApplicationUserId == currentuserid && f.VoteValue == 1).Count() > 0)
                {
                    _context.CommentMetrics.SingleOrDefault(a => a.PostCommentId == id & a.ApplicationUserId == currentuserid).VoteValue = -1;
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Details/" + id);
        }
        public IActionResult Best(Guid id)
        {

            PostComment PostComment = _context.PostComments.Where(m => m.Id == id).SingleOrDefault();

            if (PostComment == null)
            {
                return NotFound();
            }

            string currentuserid = _userManager.GetUserId(User);

            if (PostComment.IsBestAnswer == true)
            {
                _context.PostComments.SingleOrDefault(a => a.Id == id).IsBestAnswer = false;
            }
            else
            { _context.PostComments.SingleOrDefault(a => a.Id == id).IsBestAnswer = true; }
            //_context.PostComment.SingleOrDefault(a => a.Id == id).IsBestAnswer = true;
            //if (_context.PostComment.Where(f => f.Id == id && f.ApplicationUserId == currentuserid).Count() == 0)
            //{
            //    PostComment.IsBestAnswer = true;

            //}
            //else if (_context.PostComment.Where(f => f.Id == id && f.ApplicationUserId == currentuserid && f.IsBestAnswer == true).Count() > 0)
            //{
            //    _context.PostComment.SingleOrDefault(a => a.Id == id & a.ApplicationUserId == currentuserid).IsBestAnswer = false;
            //}

            _context.ScoreLogs.Add(new ScoreLog
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = _userManager.GetUserId(User),
                PostId = PostComment.PostId,
                Date = DateTime.Now,
                ScoreValueId = 13
            });

            _context.ScoreLogs.Add(new ScoreLog
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = PostComment.ApplicationUserId,
                PostId = PostComment.PostId,
                Date = DateTime.Now,
                ScoreValueId = 15
            });
            _context.SaveChanges();
            //return RedirectToAction("Details/" + id);
            return RedirectToAction("Details", new { id = id });
        }

    }
}
