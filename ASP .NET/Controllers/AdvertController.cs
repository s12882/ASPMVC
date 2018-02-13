using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ASPMVC.Models;
using System.Globalization;
using Terradue.ServiceModel.Syndication;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Web.Hosting;
using PagedList;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace ASPMVC.Controllers
{
    public class AdvertController : Controller
    {
        private IOurDbContext _ourdbcontext;

        public static List<ServiceViewModel> advList = new List<ServiceViewModel>();
        public static List<AllAdsViewModel> allAdsList = new List<AllAdsViewModel>();
        public static List<AllAdsViewModel> checkCatList = new List<AllAdsViewModel>();
        public static List<AllAdsViewModel> checkTagList = new List<AllAdsViewModel>();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdvertController()
        {
            _ourdbcontext = new OurDbContext(new AdvDbContext());
        }

        public AdvertController(IOurDbContext ourdbcontext, ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _ourdbcontext = ourdbcontext;
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public class FeedResult : ActionResult
        {
            public Encoding ContentEncoding { get; set; }
            public string ContentType { get; set; }

            private readonly SyndicationFeedFormatter _feed;
            public SyndicationFeedFormatter Feed
            {
                get { return _feed; }
            }

            public FeedResult(SyndicationFeedFormatter feed)
            {
                _feed = feed;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                var response = context.HttpContext.Response;
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

                if (ContentEncoding != null)
                    response.ContentEncoding = ContentEncoding;

                if (_feed != null)
                    using (var xmlWriter = new XmlTextWriter(response.Output))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        _feed.WriteTo(xmlWriter);
                    }
            }
        }


        #region Index

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            checkCatList.Clear();
            checkTagList.Clear();
            CreateCatAndTagList();


            Adverts(page, sortOrder, searchString, searchCategory, searchTag);
            return View();
        }

        #region Posts/AllPosts

        [ChildActionOnly]
        public ActionResult Adverts(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            advList.Clear();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSearchCategory = searchCategory;
            ViewBag.CurrentSearchTag = searchTag;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";


            var posts = _ourdbcontext.GetAds();
            foreach (var adv in posts)
            {
                var postCategories = GetAdvCategories(adv);
                var postTags = GetAdvTags(adv);
                var likes = _ourdbcontext.LikeDislikeCount("advlike", adv.Id);
                var dislikes = _ourdbcontext.LikeDislikeCount("advdislike", adv.Id);
                advList.Add(new ServiceViewModel() { Advert = adv, Modified = adv.Modified, Title = adv.Title, ShortDescription = adv.ShortDescription, PostedOn = adv.PostedOn, ID = adv.Id, AdvLikes = likes, AdvDislikes = dislikes, AdvCategories = postCategories, AdvTags = postTags, UrlSlug = adv.UrlSeo });
            }

            if (searchString != null)
            {
                advList = advList.Where(x => x.Title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            if (searchCategory != null)
            {
                List<ServiceViewModel> newlist = new List<ServiceViewModel>();
                foreach (var catName in searchCategory)
                {
                    foreach (var item in advList)
                    {
                        if (item.AdvCategories.Where(x => x.Name == catName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkCatList)
                    {
                        if (item.Category.Name == catName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                advList = advList.Intersect(newlist).ToList();
            }

            if (searchTag != null)
            {
                List<ServiceViewModel> newlist = new List<ServiceViewModel>();
                foreach (var tagName in searchTag)
                {
                    foreach (var item in advList)
                    {
                        if (item.AdvTags.Where(x => x.Name == tagName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkTagList)
                    {
                        if (item.Tag.Name == tagName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                advList = advList.Intersect(newlist).ToList();
            }

            switch (sortOrder)
            {
                case "date_desc":
                    advList = advList.OrderByDescending(x => x.PostedOn).ToList();
                    break;
                case "Title":
                    advList = advList.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    advList = advList.OrderByDescending(x => x.Title).ToList();
                    break;
                default:
                    advList = advList.OrderBy(x => x.PostedOn).ToList();
                    break;
            }

            int pageSize = 2;
            int pageNumber = (page ?? 1);

            return PartialView("Adverts", advList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AllAdverts(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            allAdsList.Clear();
            checkCatList.Clear();
            checkTagList.Clear();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSearchCategory = searchCategory;
            ViewBag.CurrentSearchTag = searchTag;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";

            var posts = _ourdbcontext.GetAds();
            foreach (var adv in posts)
            {
                var postCategories = GetAdvCategories(adv);
                var postTags = GetAdvTags(adv);
                allAdsList.Add(new AllAdsViewModel() { AdvID = adv.Id, Date = adv.PostedOn, Description = adv.ShortDescription, Title = adv.Title, AdvCategories = postCategories, AdvTags = postTags, UrlSlug = adv.UrlSeo });
            }

            if (searchString != null)
            {
                allAdsList = allAdsList.Where(x => x.Title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            CreateCatAndTagList();

            if (searchCategory != null)
            {
                List<AllAdsViewModel> newlist = new List<AllAdsViewModel>();
                foreach (var catName in searchCategory)
                {
                    foreach (var item in allAdsList)
                    {
                        if (item.AdvCategories.Where(x => x.Name == catName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkCatList)
                    {
                        if (item.Category.Name == catName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                allAdsList = allAdsList.Intersect(newlist).ToList();
            }

            if (searchTag != null)
            {
                List<AllAdsViewModel> newlist = new List<AllAdsViewModel>();
                foreach (var tagName in searchTag)
                {
                    foreach (var item in allAdsList)
                    {
                        if (item.AdvTags.Where(x => x.Name == tagName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkTagList)
                    {
                        if (item.Tag.Name == tagName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                allAdsList = allAdsList.Intersect(newlist).ToList();
            }

            switch (sortOrder)
            {
                case "date_desc":
                    allAdsList = allAdsList.OrderByDescending(x => x.Date).ToList();
                    break;
                case "Title":
                    allAdsList = allAdsList.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    allAdsList = allAdsList.OrderByDescending(x => x.Title).ToList();
                    break;
                default:
                    allAdsList = allAdsList.OrderBy(x => x.Date).ToList();
                    break;
            }

            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View("AllAdverts", allAdsList.ToPagedList(pageNumber, pageSize));

        }


        #endregion Posts/AllPosts

        #region Post

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Advert(string sortOrder, string slug)
        {
            AdvViewModel model = new AdvViewModel();
            var posts = GetAds();
            var advid = _ourdbcontext.GetAdvIdBySlug(slug);
            var adv = _ourdbcontext.GetAdvById(advid);
            var images = GetAdvImage(adv);
            var firstPostId = posts.OrderBy(i => i.PostedOn).First().Id;
            var lastPostId = posts.OrderBy(i => i.PostedOn).Last().Id;
            var nextId = posts.OrderBy(i => i.PostedOn).SkipWhile(i => i.Id != advid).Skip(1).Select(i => i.Id).FirstOrDefault();
            var previousId = posts.OrderBy(i => i.PostedOn).TakeWhile(i => i.Id != advid).Select(i => i.Id).LastOrDefault();
            model.FirstAdvId = firstPostId;
            model.LastAdvId = lastPostId;
            model.PreviousAdvSlug = posts.Where(x => x.Id == previousId).Select(x => x.UrlSeo).FirstOrDefault();
            model.NextAdvSlug = posts.Where(x => x.Id == nextId).Select(x => x.UrlSeo).FirstOrDefault();
            model.ID = adv.Id;
            model.AdvCount = posts.Count();
            model.UrlSeo = adv.UrlSeo;
            model.Images = images;
            model.Title = adv.Title;
            model.Body = adv.Body;
            model.Author = adv.Author;
            model.AdvLikes = _ourdbcontext.LikeDislikeCount("advlike", adv.Id);
            model.AdvDislikes = _ourdbcontext.LikeDislikeCount("advdislike", adv.Id);
            model.CommentViewModel = CreateCommentViewModel(adv.Id, sortOrder, adv.UrlSeo);
            return View(model);
        }


        public ActionResult UpdateAdvLike(string advid, string slug, string username, string likeordislike, string sortorder)
        {

            List<int> numlist = new List<int>();
            string likeid = "";
            int num = 0;
            var ads = _ourdbcontext.GetLikes();
            if (ads.Count != 0)
            {
                foreach (var adv in ads)
                {
                    var lid = adv.likeId;
                    Int32.TryParse(lid, out num);
                    numlist.Add(num);
                }
                numlist.Sort();
                num = numlist.Last();
                num++;
            }
            else
            {
                num = 1;
            }
            var newid = num.ToString();
            likeid = newid;

            _ourdbcontext.UpdateAdvLike(advid, username, likeordislike, likeid);
            return RedirectToAction("Advert", new { slug = slug, sortorder = sortorder });
        }

        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpGet]
        public ActionResult EditAdvertUser(string slug)
        {
            var model = CreateAdvViewModel(slug);
            return View(model);
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditAdvertUser(AdvViewModel model)
        {
            var post = _ourdbcontext.GetAdvById(model.ID);
            post.Body = model.Body; 
            post.Title = model.Title;
            post.Meta = model.Meta;
            post.UrlSeo = model.UrlSeo;
            post.ShortDescription = model.ShortDescription;
            post.Modified = DateTime.Now;
            _ourdbcontext.Save();

            return RedirectToAction("Advert", new { slug = model.UrlSeo });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditAdvert(string slug)
        {
            var model = CreateAdvViewModel(slug);
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditAdvert(AdvViewModel model)
        {
            var post = _ourdbcontext.GetAdvById(model.ID);
            post.Body = model.Body;    
            post.Title = model.Title;
            post.Meta = model.Meta;
            post.UrlSeo = model.UrlSeo;
            post.ShortDescription = model.ShortDescription;
            post.Modified = DateTime.Now;
            _ourdbcontext.Save();

            return RedirectToAction("Advert", new { slug = model.UrlSeo });
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpGet]
        public ActionResult AddImageToAdv(string advid, string slug)
        {
            AdvViewModel model = new AdvViewModel();
            model.ID = advid;
            model.UrlSeo = slug;
            return View(model);
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddImageToAdv(string advid, string slug, string Url)
        {
            CreateAdvViewModel(slug);
            _ourdbcontext.AddImageToAdv(advid, Url);
            return RedirectToAction("EditAdvertUser", new { slug = slug });
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        public ActionResult RemoveImageFromAdv(string slug, string advid, string Url)
        {
            CreateAdvViewModel(slug);
            _ourdbcontext.RemoveImageFromAdv(advid, Url);
            return RedirectToAction("EditAdvertUser", new { slug = slug });
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpGet]
        public ActionResult AddCategoryToAdv(string advid)
        {
            AdvViewModel model = new AdvViewModel();
            model.ID = advid;
            model.Categories = _ourdbcontext.GetCategories();
            return View(model);
        }

        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryToAdv(AdvViewModel model)
        {
            var post = _ourdbcontext.GetAdvById(model.ID);
            var advCats = _ourdbcontext.GetAdvCategories(post);
            List<string> pCatIds = new List<string>();
            foreach (var adCat in advCats)
            {
                pCatIds.Add(adCat.Id);
            }
            var newCats = model.Categories.Where(x => x.Checked == true).ToList();
            List<string> nCatIds = new List<string>();
            foreach (var adCat in newCats)
            {
                nCatIds.Add(adCat.Id);
            }
            if (!pCatIds.SequenceEqual(nCatIds))
            {
                foreach (var adCat in advCats)
                {
                    _ourdbcontext.RemoveAdvCategories(model.ID, adCat.Id);
                }
                foreach (var cat in model.Categories)
                {
                    AdvCategory advCategory = new AdvCategory();
                    if (cat.Checked == true)
                    {
                        advCategory.AdvID = model.ID;
                        advCategory.CatID = cat.Id;
                        advCategory.Checked = true;
                        _ourdbcontext.AddAdvCategories(advCategory);
                    }
                }
                _ourdbcontext.Save();
            }
            return RedirectToAction("EditAdvertUser", new { slug = post.UrlSeo });
        }

        [AuthorizeRoles(Role.Admin, Role.Writer)]
        public ActionResult RemoveCategoryFromAdv(string slug, string advid, string catName)
        {
            CreateAdvViewModel(slug);
            _ourdbcontext.RemoveCategoryFromAdv(advid, catName);
            return RedirectToAction("EditAdvertUser", new { slug = slug });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewCategory(string advid, bool callfrompost)
        {
            if (advid != null && callfrompost)
            {
                AdvViewModel model = new AdvViewModel();
                model.ID = advid;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewCategory(string advid, string catName, string catUrlSeo, string catDesc)
        {
            if (advid != null)
            {
                _ourdbcontext.AddNewCategory(catName, catUrlSeo, catDesc);
                return RedirectToAction("AddCategoryToAdv", new { advid = advid });
            }
            else
            {
                _ourdbcontext.AddNewCategory(catName, catUrlSeo, catDesc);
                return RedirectToAction("CategoriesAndTags", "Advert");
            }
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpGet]
        public ActionResult AddTagToAdv(string advid)
        {
            AdvViewModel model = new AdvViewModel();
            model.ID = advid;
            model.Tags = _ourdbcontext.GetTags();
            return View(model);
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTagToAdv(AdvViewModel model)
        {
            var post = _ourdbcontext.GetAdvById(model.ID);
            var advTags = _ourdbcontext.GetAdvTags(post);
            List<string> aTagIds = new List<string>();
            foreach (var aTag in advTags)
            {
                aTagIds.Add(aTag.Id);
            }
            var newTags = model.Tags.Where(x => x.Checked == true).ToList();
            List<string> nTagIds = new List<string>();
            foreach (var aTag in newTags)
            {
                nTagIds.Add(aTag.Id);
            }
            if (!aTagIds.SequenceEqual(nTagIds))
            {
                foreach (var pTag in advTags)
                {
                    _ourdbcontext.RemoveAdvTags(model.ID, pTag.Id);
                }
                foreach (var tag in model.Tags)
                {
                    AdvTag advTag = new AdvTag();
                    if (tag.Checked == true)
                    {
                        advTag.AdvID = model.ID;
                        advTag.TagId = tag.Id;
                        advTag.Checked = true;
                        _ourdbcontext.AddAdvTags(advTag);
                    }
                }
                _ourdbcontext.Save();
            }
            return RedirectToAction("EditAdvertUser", new { slug = post.UrlSeo });
        }

        [AuthorizeRoles(Role.Admin, Role.Writer)]
        public ActionResult RemoveTagFromAdv(string slug, string advid, string tagName)
        {
            CreateAdvViewModel(slug);
            _ourdbcontext.RemoveTagFromAdv(advid, tagName);
            return RedirectToAction("EditAdvertUser", new { slug = slug });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewTag(string advid, bool callfrompost)
        {
            if (advid != null && callfrompost)
            {
                AdvViewModel model = new AdvViewModel();
                model.ID = advid;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewTag(string advid, string tagName, string tagUrlSeo)
        {
            if (advid != null)
            {
                _ourdbcontext.AddNewTag(tagName, tagUrlSeo);
                return RedirectToAction("AddTagToPost", new { advid = advid });
            }
            else
            {
                _ourdbcontext.AddNewTag(tagName, tagUrlSeo);
                return RedirectToAction("CategoriesAndTags", "Advert");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CategoriesAndTags()
        {
            checkCatList.Clear();
            checkTagList.Clear();
            CreateCatAndTagList();
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCatAndTag(string[] categoryNames, string[] tagNames)
        {
            if (categoryNames != null)
            {
                foreach (var catName in categoryNames)
                {
                    var category = _ourdbcontext.GetCategories().Where(x => x.Name == catName).FirstOrDefault();
                    _ourdbcontext.RemoveCategory(category);
                }
            }
            if (tagNames != null)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = _ourdbcontext.GetTags().Where(x => x.Name == tagName).FirstOrDefault();
                    _ourdbcontext.RemoveTag(tag);
                }
            }
            return RedirectToAction("CategoriesAndTags", "Advert");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteAdv(AdvViewModel model, string advid)
        {
            model.ID = advid;
            return View(model);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdv(string advId)
        {
            _ourdbcontext.DeleteAdvandComponents(advId);
            return RedirectToAction("Index", "Advert");
        }



        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpGet]
        public ActionResult AddNewAdv()
        {
            List<int> numlist = new List<int>();
            int num = 0;
            string Author = User.Identity.GetUserName();
            var ads = _ourdbcontext.GetAds();
            if (ads.Count != 0)
            {
                foreach (var adv in ads)
                {
                    var advid = adv.Id;
                    Int32.TryParse(advid, out num);
                    numlist.Add(num);
                }
                numlist.Sort();
                num = numlist.Last();
                num++;
            }
            else
            {
                num = 1;
            }
            var newid = num.ToString();
            AdvViewModel model = new AdvViewModel();
            model.ID = newid;
            model.Author = Author;
            return View(model);
        }


        [AuthorizeRoles(Role.Admin, Role.Writer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddNewAdv(AdvViewModel model)
        {
            var adv = new Advert
            {
                Id = model.ID,
                Author = model.Author,
                Body = model.Body,
                Meta = model.Meta,
                PostedOn = DateTime.Now,
                Published = true,
                ShortDescription = model.ShortDescription,
                Title = model.Title,
                UrlSeo = model.UrlSeo
            };

               _ourdbcontext.AddNewAdvert(adv);
            
            return RedirectToAction("EditAdvertUser", "Advert", new { slug = model.UrlSeo });
        }


        #endregion Post

        #region Rss

        public ActionResult Feed()
        {
            var blogTitle = "Easy ads";
            var blogDescription = " Free adverts for everyone";
            var blogUrl = "http://esAdv.com ";

            var ads = _ourdbcontext.GetAds().Select(
                p => new SyndicationItem(
                    p.Title,
                    p.ShortDescription,
                  new Uri(blogUrl)
                )
                );

            // Create an instance of SyndicationFeed class passing the SyndicationItem collection
            var feed = new SyndicationFeed(blogTitle, blogDescription, new Uri(blogUrl), ads)
            {
                Copyright = new TextSyndicationContent(string.Format("Copyright © {0}", blogTitle)),
                Language = "en-US"
            };

            // Format feed in RSS format through Rss20FeedFormatter formatter
            var feedFormatter = new Rss20FeedFormatter(feed);

            // Call the custom action that write the feed to the response
            return new FeedResult(feedFormatter);
        }

        #endregion Rss

        [ChildActionOnly]
        public ActionResult Comments(string pageId, string sortOrder)
        {
            return PartialView();
        }

        public CommentViewModel CreateCommentViewModel(string advid, string sortOrder, string UrlSeo) 
        {
            CommentViewModel model = new CommentViewModel();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.BestSortParm = sortOrder == "Best" ? "best_desc" : "Best";

            var comments = _ourdbcontext.GetCommentsByPageId(advid).OrderByDescending(d => d.DateTime).ToList();
            foreach (var comment in comments)
            {
                var likes = LikeDislikeCount("commentlike", comment.CommentId);
                var dislikes = LikeDislikeCount("commentdislike", comment.CommentId);
                comment.NetLikeCount = likes - dislikes;
                if (comment.Replies != null) comment.Replies.Clear();
                List<CommentViewModel> replies = _ourdbcontext.GetParentReplies(comment);
                foreach (var reply in replies)
                {
                    var rep = _ourdbcontext.GetReplyById(reply.Id);
                    comment.Replies.Add(rep);
                }
            }

                model.UrlSeo = _ourdbcontext.GetAdvById(advid).UrlSeo;
                model.Id = advid;

            switch (sortOrder)
            {
                case "date_asc":
                    comments = comments.OrderBy(x => x.DateTime).ToList();
                    ViewBag.DateSortLink = "active";
                    break;
                case "Best":
                    comments = comments.OrderByDescending(x => x.NetLikeCount).ToList();
                    ViewBag.BestSortLink = "active";
                    break;
                case "best_desc":
                    comments = comments.OrderBy(x => x.NetLikeCount).ToList();
                    ViewBag.BestSortLink = "active";
                    break;
                default:
                    comments = comments.OrderByDescending(x => x.DateTime).ToList();
                    ViewBag.DateSortLink = "active";
                    break;
            }

            model.Comments = comments;
            return model;
        }

        public PartialViewResult Replies()
        {
            return PartialView();
        }

        public PartialViewResult ChildReplies()
        {
            return PartialView();
        }


        public ActionResult UpdateCommentLike(string commentid, string username, string likeordislike, string slug, string sortorder)
        {
            if (username != null)
            {
                List<int> numlist = new List<int>();
                string likeid = "";
                int num = 0;
                var ads = _ourdbcontext.GetCLikes();
                if (ads.Count != 0)
                {
                    foreach (var adv in ads)
                    {
                        var clid = adv.ClikeId;
                        Int32.TryParse(clid, out num);
                        numlist.Add(num);
                    }
                    numlist.Sort();
                    num = numlist.Last();
                    num++;
                }
                else
                {
                    num = 1;
                }
                var newid = num.ToString();
                likeid = newid;
                _ourdbcontext.UpdateCommentLike(commentid, username, likeordislike, likeid);
            }
            return RedirectToAction("Advert", new { slug = slug, sortorder = sortorder });
        }
        public ActionResult UpdateReplyLike(string replyid, string username, string likeordislike, string sortorder)
        {
            if (username != null)
            {
                List<int> numlist = new List<int>();
                string likeid = "";
                int num = 0;
                var ads = _ourdbcontext.GetRLikes();
                if (ads.Count != 0)
                {
                    foreach (var adv in ads)
                    {
                        var clid = adv.RlikeId;
                        Int32.TryParse(clid, out num);
                        numlist.Add(num);
                    }
                    numlist.Sort();
                    num = numlist.Last();
                    num++;
                }
                else
                {
                    num = 1;
                }
                var newid = num.ToString();
                likeid = newid;
                _ourdbcontext.UpdateReplyLike(replyid, username, likeordislike, likeid);
            }
            var slug = _ourdbcontext.GetAdvByReply(replyid).UrlSeo;
            return RedirectToAction("Advert", "Advert", new { slug = slug, sortorder = sortorder });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewComment(string commentBody, string comUserName, string slug, string advId)
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var comments = _ourdbcontext.GetComments().ToList();
            if (comments.Count() != 0)
            {
                foreach (var cmnt in comments)
                {
                    var comid = cmnt.CommentId;
                    Int32.TryParse(comid.Replace("cmt", ""), out num);
                    numlist.Add(num);
                }
                numlist.Sort();
                num = numlist.Last();
                num++;
            }
            else
            {
                num = 1;
            }
            var newid = "cmt" + num.ToString();
            var comment = new Comment()
            {
                CommentId = newid,
                AdvId = advId,
                DateTime = DateTime.Now,
                UserName = comUserName,
                Body = commentBody,
                NetLikeCount = 0
            };
            _ourdbcontext.AddNewComment(comment);

                return RedirectToAction("Advert", new { slug = slug });
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewParentReply(string replyBody, string comUserName, string advid, string commentid, string slug)
        {
            var comDelChck = CommentDeleteCheck(commentid);
            if (!comDelChck)
            {
                List<int> numlist = new List<int>();
                int num = 0;
                var replies = _ourdbcontext.GetReplies().ToList();
                if (replies.Count != 0)
                {
                    foreach (var rep in replies)
                    {
                        var repid = rep.ReplyId;
                        Int32.TryParse(repid.Replace("rep", ""), out num);
                        numlist.Add(num);
                    }
                    numlist.Sort();
                    num = numlist.Last();
                    num++;
                }
                else
                {
                    num = 1;
                }
                var newid = "rep" + num.ToString();
                var reply = new Reply()
                {
                    ReplyId = newid,
                    AdvID = advid,
                    CommentId = commentid,
                    ParentReplyId = null,
                    DateTime = DateTime.Now,
                    UserName = comUserName,
                    Body = replyBody,
                };
                _ourdbcontext.AddNewReply(reply);
            }
            return RedirectToAction("Advert", new { slug = slug });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewChildReply(string preplyid, string comUserName, string replyBody)
        {
            var repDelCheck = ReplyDeleteCheck(preplyid);
            var preply = _ourdbcontext.GetReplyById(preplyid);
            if (!repDelCheck)
            {
                List<int> numlist = new List<int>();
                int num = 0;
                var replies = _ourdbcontext.GetReplies().ToList();
                if (replies.Count != 0)
                {
                    foreach (var rep in replies)
                    {
                        var repid = rep.ReplyId;
                        Int32.TryParse(repid.Replace("rep", ""), out num);
                        numlist.Add(num);
                    }
                    numlist.Sort();
                    num = numlist.Last();
                    num++;
                }
                else
                {
                    num = 1;
                }
                var newid = "rep" + num.ToString();
                var advId = _ourdbcontext.GetPageIdByComment(preply.CommentId);

                var reply = new Reply()
                {
                    ReplyId = newid,
                    AdvID = advId,
                    CommentId = preply.CommentId,
                    ParentReplyId = preply.ReplyId,
                    DateTime = DateTime.Now,
                    UserName = comUserName,
                    Body = replyBody,
                };
                _ourdbcontext.AddNewReply(reply);
            }
            
                return RedirectToAction("Advert", new { slug = _ourdbcontext.GetUrlSeoByReply(preply) });
            
        }



        [HttpGet]
        public async Task<ActionResult> EditComment(CommentViewModel model, string commentid)
        {
            var user = await GetCurrentUserAsync();
            var comment = _ourdbcontext.GetCommentById(commentid);
            if (comment.UserName == user.UserName || user.UserName == "shanu")
            {
                model.Id = commentid;
                model.Body = comment.Body;
                return View(model);
            }
            else
            {
                return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == comment.AdvId).FirstOrDefault().UrlSeo });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditComment(string commentid, string commentBody)
        {
            var comment = _ourdbcontext.GetCommentById(commentid);
            comment.Body = commentBody;
            comment.DateTime = DateTime.Now;
            _ourdbcontext.Save();
            return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == comment.AdvId).FirstOrDefault().UrlSeo });
        }


        [HttpGet]
        public async Task<ActionResult> DeleteComment(CommentViewModel model, string commentid)
        {
            var user = await GetCurrentUserAsync();
            var comment = _ourdbcontext.GetCommentById(commentid);
            if (comment.UserName == user.UserName)
            {
                model.Id = commentid;
                return View(model);
            }
            else
            {
                return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == comment.AdvId).FirstOrDefault().UrlSeo });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(string commentid)
        {
            var comment = _ourdbcontext.GetCommentById(commentid);
            var advid = comment.AdvId;
            var repliesList = _ourdbcontext.GetParentReplies(comment);
            if (repliesList.Count() == 0)
            {
                _ourdbcontext.DeleteComment(commentid);
            }
            else
            {
                comment.DateTime = DateTime.Now;
                comment.Body = "<p style=\"color:red;\"><i>This comment has been deleted.</i></p>";
                comment.Deleted = true;
                _ourdbcontext.Save();
            }
            return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == advid).FirstOrDefault().UrlSeo });
        }


        [HttpGet]
        public async Task<ActionResult> EditReply(CommentViewModel model, string replyid)
        {
            var user = await GetCurrentUserAsync();
            var reply = _ourdbcontext.GetReplyById(replyid);
            if (reply.UserName == user.UserName)
            {
                model.Id = replyid;
                model.Body = reply.Body;
                return View(model);
            }
            else
            {
                return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == reply.AdvID).FirstOrDefault().UrlSeo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditReply(string replyid, string replyBody)
        {
            var reply = _ourdbcontext.GetReplyById(replyid);
            reply.Body = replyBody;
            reply.DateTime = DateTime.Now;
            _ourdbcontext.Save();
            return RedirectToAction("Advert", new { slug = _ourdbcontext.GetUrlSeoByReply(reply) });
        }

        [HttpGet]
        public async Task<ActionResult> DeleteReply(CommentViewModel model, string replyid)
        {
            var user = await GetCurrentUserAsync();
            var reply = _ourdbcontext.GetReplyById(replyid);
            if (reply.UserName == user.UserName)
            {
                model.Id = replyid;
                return View(model);
            }
            else
            {
                return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id.ToString() == reply.AdvID).FirstOrDefault().UrlSeo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReply(string replyid)
        {
            var reply = _ourdbcontext.GetReplyById(replyid);
            var repliesList = _ourdbcontext.GetChildReplies(reply);
            var advid = _ourdbcontext.GetUrlSeoByReply(reply);
            if (repliesList.Count() == 0)
            {
                _ourdbcontext.DeleteReply(replyid);
            }
            else
            {
                reply.DateTime = DateTime.Now;
                reply.Body = "<p style=\"color:red;\"><i>This comment has been deleted.</i></p>";
                reply.Deleted = true;
                _ourdbcontext.Save();
            }
            return RedirectToAction("Advert", new { slug = _ourdbcontext.GetAds().Where(x => x.Id == advid).FirstOrDefault().UrlSeo });
        }

        #region Helpers

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(User.Identity.GetUserId());
        }


        public List<CommentViewModel> GetChildReplies(Reply parentReply)
        {
            return _ourdbcontext.GetChildReplies(parentReply);
        }


        public bool CommentDeleteCheck(string commentid)
        {
            return _ourdbcontext.CommentDeleteCheck(commentid);
        }
        public bool ReplyDeleteCheck(string replyid)
        {
            return _ourdbcontext.ReplyDeleteCheck(replyid);
        }



        public static string TimePassed(DateTime postDate)
        {
            string date = null;
            double dateDiff = 0.0;
            var timeDiff = DateTime.Now - postDate;
            var yearPassed = timeDiff.TotalDays / 365;
            var monthPassed = timeDiff.TotalDays / 30;
            var dayPassed = timeDiff.TotalDays;
            var hourPassed = timeDiff.TotalHours;
            var minutePassed = timeDiff.TotalMinutes;
            var secondPassed = timeDiff.TotalSeconds;
            if (Math.Floor(yearPassed) > 0)
            {
                dateDiff = Math.Floor(yearPassed);
                date = dateDiff == 1 ? dateDiff + " year ago" : dateDiff + " years ago";
            }
            else
            {
                if (Math.Floor(monthPassed) > 0)
                {
                    dateDiff = Math.Floor(monthPassed);
                    date = dateDiff == 1 ? dateDiff + " month ago" : dateDiff + " months ago";
                }
                else
                {
                    if (Math.Floor(dayPassed) > 0)
                    {
                        dateDiff = Math.Floor(dayPassed);
                        date = dateDiff == 1 ? dateDiff + " day ago" : dateDiff + " days ago";
                    }
                    else
                    {
                        if (Math.Floor(hourPassed) > 0)
                        {
                            dateDiff = Math.Floor(hourPassed);
                            date = dateDiff == 1 ? dateDiff + " hour ago" : dateDiff + " hours ago";
                        }
                        else
                        {
                            if (Math.Floor(minutePassed) > 0)
                            {
                                dateDiff = Math.Floor(minutePassed);
                                date = dateDiff == 1 ? dateDiff + " minute ago" : dateDiff + " minutes ago";
                            }
                            else
                            {
                                dateDiff = Math.Floor(secondPassed);
                                date = dateDiff == 1 ? dateDiff + " second ago" : dateDiff + " seconds ago";
                            }
                        }
                    }
                }
            }

            return date;
        }


        public string[] NewCommentDetails(string username)
        {
            string[] newCommentDetails = new string[3];
            newCommentDetails[0] = "td" + username; //comText
            newCommentDetails[1] = "tdc" + username; //comTextdiv
            newCommentDetails[2] = "tb" + username; //comTextBtn
            return newCommentDetails;
        }

        public string[] CommentDetails(Comment comment)
        {
            string[] commentDetails = new string[17];
            commentDetails[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(comment.UserName);//username
            commentDetails[1] = "/Content/images/profile/" + commentDetails[0] + ".png?time=" + DateTime.Now.ToString();//imgUrl
            commentDetails[2] = TimePassed(comment.DateTime);//passed time
            commentDetails[3] = comment.DateTime.ToLongDateString().Replace(comment.DateTime.DayOfWeek.ToString() + ", ", "");//comment date
            commentDetails[4] = "gp" + comment.CommentId; //grandparentid
            commentDetails[5] = "mc" + comment.CommentId; //maincommentId
            commentDetails[6] = "crp" + comment.CommentId; //repliesId
            commentDetails[7] = "cex" + comment.CommentId; //commentExpid
            commentDetails[8] = "ctex" + comment.CommentId; //ctrlExpId
            commentDetails[9] = "ctflg" + comment.CommentId; //ctrlFlagId
            commentDetails[10] = "sp" + comment.CommentId; //shareParentId
            commentDetails[11] = "sc" + comment.CommentId; //shareChildId
            commentDetails[12] = "td" + comment.CommentId; //comText
            commentDetails[13] = "tdc" + comment.CommentId; //comTextdiv
            commentDetails[14] = "rpl" + comment.CommentId; //Reply
            commentDetails[15] = "cc1" + comment.CommentId; //commentControl
            commentDetails[16] = "cc2" + comment.CommentId; //commentMenu
            return commentDetails;
        }

        public string[] ReplyDetails(string replyId)
        {
            string[] replyDetails = new string[17];
            var reply = _ourdbcontext.GetReplyById(replyId);
            replyDetails[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reply.UserName);//username
            replyDetails[1] = "/Content/images/profile/" + replyDetails[0] + ".png?time=" + DateTime.Now.ToString(); //imgUrl
            replyDetails[2] = TimePassed(reply.DateTime); //passed time
            replyDetails[3] = reply.DateTime.ToLongDateString().Replace(reply.DateTime.DayOfWeek.ToString() + ", ", ""); //reply date
            replyDetails[4] = "gp" + replyId; //grandparentid
            replyDetails[5] = "rp" + replyId; //parentreplyId
            replyDetails[6] = "crp" + replyId; //repliesId
            replyDetails[7] = "cex" + replyId; //commentExpid
            replyDetails[8] = "ctex" + replyId; //ctrlExpId
            replyDetails[9] = "ctflg" + replyId; //ctrlFlagId
            replyDetails[10] = "sp" + replyId; //shareParentId
            replyDetails[11] = "sc" + replyId; //shareChildId;
            replyDetails[12] = "td" + replyId; //comText
            replyDetails[13] = "tdc" + replyId; //comTextdiv
            replyDetails[14] = "rpl" + replyId; //Reply
            replyDetails[15] = "cc1" + replyId; //commentControl
            replyDetails[16] = "cc2" + replyId; //commentMenu

            return replyDetails;
        }

        public int LikeDislikeCount(string typeAndlike, string id)
        {
            switch (typeAndlike)
            {
                case "advlike":
                    return _ourdbcontext.LikeDislikeCount("advlike", id);
                case "advdislike":
                    return _ourdbcontext.LikeDislikeCount("advdislike", id);
                case "commentlike":
                    return _ourdbcontext.LikeDislikeCount("commentlike", id);
                case "commentdislike":
                    return _ourdbcontext.LikeDislikeCount("commentdislike", id);
                case "replylike":
                    return _ourdbcontext.LikeDislikeCount("replylike", id);
                case "replydislike":
                    return _ourdbcontext.LikeDislikeCount("replydislike", id);
                default:
                    return 0;
            }
        }

        public IList<Advert> GetAds()
        {
            return _ourdbcontext.GetAds();
        }
        public IList<Category> GetAdvCategories(Advert post)
        {
            return _ourdbcontext.GetAdvCategories(post);
        }
        public IList<Tag> GetAdvTags(Advert post)
        {
            return _ourdbcontext.GetAdvTags(post);
        }
        public IList<AdvImage> GetAdvImage(Advert post)
        {
            return _ourdbcontext.GetAdvImages(post);
        }
        public void CreateCatAndTagList()
        {
            foreach (var ct in _ourdbcontext.GetCategories())
            {
                checkCatList.Add(new AllAdsViewModel { Category = ct, Checked = false });
            }
            foreach (var tg in _ourdbcontext.GetTags())
            {
                checkTagList.Add(new AllAdsViewModel { Tag = tg, Checked = false });
            }
        }

        public AdvViewModel CreateAdvViewModel(string slug)
        {
            AdvViewModel model = new AdvViewModel();
            var postid = _ourdbcontext.GetAdvIdBySlug(slug);
            var post = _ourdbcontext.GetAdvById(postid);
            model.ID = postid;
            model.Author = post.Author;
            model.Title = post.Title;
            model.Body = post.Body;
            model.Meta = post.Meta;
            model.UrlSeo = post.UrlSeo;
            model.Images = _ourdbcontext.GetAdvImages(post).ToList();
            model.AdvCategories = _ourdbcontext.GetAdvCategories(post).ToList();
            model.PostTags = _ourdbcontext.GetAdvTags(post).ToList();
            model.ShortDescription = post.ShortDescription;
            return model;
        }

        public class AuthorizeRolesAttribute : AuthorizeAttribute
        {
            public AuthorizeRolesAttribute(params string[] roles) : base()
            {
                Roles = string.Join(",", roles);
            }
        }

        public static class Role
        {
            public const string Admin = "Admin";
            public const string Writer = "Writer";
            public const string Reader = "Reader";
        }


        #endregion

    }
    #endregion
}