using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC.Models
{
    public class OurDbContext : IOurDbContext, IDisposable
    {

        private AdvDbContext _context;

        public OurDbContext(AdvDbContext context)
        {
            _context = context;
        }

        #region 1
        public IList<Advert> GetAds()
        {
            return _context.Adverts.ToList();
        }

        public IList<AdvLike> GetLikes()
        {
            return _context.AdvLikes.ToList();
        }

        public IList<CommentLike> GetCLikes()
        {
            return _context.CommentLikes.ToList();
        }

        public IList<ReplyLike> GetRLikes()
        {
            return _context.ReplyLikes.ToList();
        }

        public IList<Tag> GetTags()
        {
            return _context.Tags.ToList();
        }
        public IList<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public IList<Category> GetAdvCategories(Advert adv)
        {
            var categoryIds = _context.AdvCategories.Where(p => p.AdvID == adv.Id).Select(p => p.CatID).ToList();
            List<Category> categories = new List<Category>();
            foreach (var catId in categoryIds)
            {
                categories.Add(_context.Categories.Where(p => p.Id == catId).FirstOrDefault());
            }
            return categories;
        }
        public IList<Tag> GetAdvTags(Advert adv)
        {
            var tagIds = _context.AdvTags.Where(p => p.AdvID == adv.Id).Select(p => p.TagId).ToList();
            List<Tag> tags = new List<Tag>();
            foreach (var tagId in tagIds)
            {
                tags.Add(_context.Tags.Where(p => p.Id == tagId).FirstOrDefault());
            }
            return tags;
        }

        public int LikeDislikeCount(string typeAndlike, string id)
        {
            switch (typeAndlike)
            {
                case "advlike":
                    return _context.AdvLikes.Where(p => p.advId == id && p.Like == true).Count();
                case "advdislike":
                    return _context.AdvLikes.Where(p => p.advId == id && p.Dislike == true).Count();
                case "commentlike":
                    return _context.CommentLikes.Where(p => p.CommentId == id && p.Like == true).Count();
                case "commentdislike":
                    return _context.CommentLikes.Where(p => p.CommentId == id && p.Dislike == true).Count();
                case "replylike":
                    return _context.ReplyLikes.Where(p => p.ReplyId == id && p.Like == true).Count();
                case "replydislike":
                    return _context.ReplyLikes.Where(p => p.ReplyId == id && p.Dislike == true).Count();
                default:
                    return 0;
            }
        }
        #endregion

        #region 2
        public Advert GetAdvById(string id)
        {
            return _context.Adverts.Find(id);
        }

        public string GetAdvIdBySlug(string slug)
        {
            try
            {
                return _context.Adverts.Where(x => x.UrlSeo == slug).FirstOrDefault().Id;
            }
            catch
            {
                return _context.Adverts.Where(x => x.UrlSeo == slug).FirstOrDefault().Id;
            }
            
        }

        public void UpdateCommentLike(string commentid, string username, string likeordislike, string likeid)
        {
            var commentLike = _context.CommentLikes.Where(x => x.Username == username && x.CommentId == commentid).FirstOrDefault();
            if (commentLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (commentLike.Like == false) { commentLike.Like = true; commentLike.Dislike = false; }
                        else commentLike.Like = false;
                        break;
                    case "dislike":
                        if (commentLike.Dislike == false) { commentLike.Dislike = true; commentLike.Like = false; }
                        else commentLike.Dislike = false;
                        break;
                }
                if (commentLike.Like == false && commentLike.Dislike == false) _context.CommentLikes.Remove(commentLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        commentLike = new CommentLike() { ClikeId = likeid, CommentId = commentid, Username = username, Like = true, Dislike = false };
                        _context.CommentLikes.Add(commentLike);
                        break;
                    case "dislike":
                        commentLike = new CommentLike() { ClikeId = likeid, CommentId = commentid, Username = username, Like = false, Dislike = true };
                        _context.CommentLikes.Add(commentLike);
                        break;
                }
            }
            var comment = _context.Comments.Where(x => x.CommentId == commentid).FirstOrDefault();
            comment.NetLikeCount = LikeDislikeCount("commentlike", commentid) - LikeDislikeCount("commentdislike", commentid);
            Save();
        }

        public void UpdateAdvLike(string advid, string username, string likeordislike, string likeid)
        {
            var advLike = _context.AdvLikes.Where(x => x.Username == username && x.advId == advid).FirstOrDefault();
           
            if (advLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (advLike.Like == false) { advLike.Like = true; advLike.Dislike = false; }
                        else advLike.Like = false;
                        break;
                    case "dislike":
                        if (advLike.Dislike == false) { advLike.Dislike = true; advLike.Like = false; }
                        else advLike.Dislike = false;
                        break;
                }
                if (advLike.Like == false && advLike.Dislike == false) _context.AdvLikes.Remove(advLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        advLike = new AdvLike() { likeId = likeid, advId = advid.ToString(), Username = username, Like = true, Dislike = false };
                        _context.AdvLikes.Add(advLike);
                        break;
                    case "dislike":
                        advLike = new AdvLike() { likeId = likeid, advId = advid.ToString(), Username = username, Like = false, Dislike = true };
                        _context.AdvLikes.Add(advLike);
                        break;
                }
            }
            var adv = _context.Adverts.Where(x => x.Id == advid).FirstOrDefault();
            adv.LikeCount = LikeDislikeCount("advlike", advid) - LikeDislikeCount("advdislike", advid);
            Save();
        }

        public IList<AdvImage> GetAdvImages(Advert adv)
        {
            var advUrls = _context.AdvImages.Where(p => p.AdvId == adv.Id).ToList();
            List<AdvImage> images = new List<AdvImage>();
            foreach (var url in advUrls)
            {
                images.Add(url);
            }
            return images;
        }

        public void AddImageToAdv(string advid, string Url)
        {
            List<int> numlist = new List<int>();
            int num = 1;
            string siteName = null;
            string thumbUrl = null;
            var check = _context.AdvImages.Where(x => x.AdvId == advid && x.ImageUrl == Url).Any();
            if (!check)
            {
                while (_context.AdvImages.Where(x => x.ID == num).Any())
                {
                    num++;
                }

                if (Url.Contains("youtube.com") || Url.Contains("youtu.be"))
                {
                    int pos = Url.LastIndexOf("/") + 1;
                    var result = Url.Substring(pos, Url.Length - pos);
                    thumbUrl = "https://img.youtube.com/vi/" + result + "/0.jpg";
                    siteName = "YouTube";
                }
                else
                {
                    thumbUrl = Url;
                }
            

                var img = new AdvImage { ID = num, AdvId = advid, ImageUrl = Url, SiteName = siteName, Imagehumbnail = thumbUrl };
                _context.AdvImages.Add(img);
                Save();
            }

        }

        public void RemoveImageFromAdv(string advid, string Url)
        {
            var img = _context.AdvImages.Where(x => x.AdvId == advid && x.ImageUrl == Url).FirstOrDefault();
            _context.AdvImages.Remove(img);
            Save();
        }

        public void AddAdvCategories(AdvCategory advCategory)
        {
            _context.AdvCategories.Add(advCategory);
        }

        public void RemoveAdvCategories(string advid, string categoryid)
        {
            AdvCategory postCategory = _context.AdvCategories.Where(x => x.AdvID == advid && x.CatID == categoryid).FirstOrDefault();
            _context.AdvCategories.Remove(postCategory);
            Save();
        }
        public void RemoveCategoryFromAdv(string advid, string catName)
        {
            var catid = _context.Categories.Where(x => x.Name == catName).Select(x => x.Id).FirstOrDefault();
            var cat = _context.AdvCategories.Where(x => x.AdvID == advid && x.CatID == catid).FirstOrDefault();
            _context.AdvCategories.Remove(cat);
            Save();
        }
        public void AddNewCategory(string catName, string catUrlSeo, string catDesc)
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var categories = _context.Categories.ToList();
            foreach (var cat in categories)
            {
                var catid = cat.Id;
                Int32.TryParse(catid.Replace("cat", ""), out num);
                numlist.Add(num);
            }
            numlist.Sort();
            num = numlist.Last();
            num++;
            var newid = "cat" + num.ToString();
            var category = new Category { Id = newid, Name = catName, Description = catDesc, UrlSeo = catUrlSeo, Checked = false };
            _context.Categories.Add(category);
            Save();
        }
        public void RemoveTagFromAdv(string advid, string tagName)
        {
            var tagid = _context.Tags.Where(x => x.Name == tagName).Select(x => x.Id).FirstOrDefault();
            var tag = _context.AdvTags.Where(x => x.AdvID == advid && x.TagId == tagid).FirstOrDefault();
            _context.AdvTags.Remove(tag);
            Save();
        }
        public void AddAdvTags(AdvTag advTag)
        {
            _context.AdvTags.Add(advTag);
        }

        public void RemoveAdvTags(string advid, string tagid)
        {
            AdvTag postTag = _context.AdvTags.Where(x => x.AdvID == advid && x.TagId == tagid).FirstOrDefault();
            _context.AdvTags.Remove(postTag);
            Save();
        }
        public void AddNewTag(string tagName, string tagUrlSeo)
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var tags = _context.Tags.ToList();
            if (tags.Count() != 0)
            {
                foreach (var tg in tags)
                {
                    var tagid = tg.Id;
                    Int32.TryParse(tagid.Replace("tag", ""), out num);
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
            var newid = "tag" + num.ToString();
            var tag = new Tag { Id = newid, Name = tagName, UrlSeo = tagUrlSeo, Checked = false };
            _context.Tags.Add(tag);
            Save();
        }
        public void RemoveCategory(Category category)
        {
            var advCategories = _context.AdvCategories.Where(x => x.CatID == category.Id).ToList();
            foreach (var advCat in advCategories)
            {
                _context.AdvCategories.Remove(advCat);
            }
            _context.Categories.Remove(category);
            Save();
        }
        public void RemoveTag(Tag tag)
        {
            var advTags = _context.AdvTags.Where(x => x.TagId == tag.Id).ToList();
            foreach (var postTag in advTags)
            {
                _context.AdvTags.Remove(postTag);
            }
            _context.Tags.Remove(tag);
            Save();
        }
        public void DeleteAdvandComponents(string advid)
        {
            var advCategories = _context.AdvCategories.Where(p => p.CatID == advid).ToList();
            var advLikes = _context.AdvLikes.Where(p => p.advId == advid).ToList();
            var advTags = _context.AdvTags.Where(p => p.AdvID == advid).ToList();
            var advImages = _context.AdvImages.Where(p => p.AdvId == advid).ToList();
            var advComments = _context.Comments.Where(p => p.AdvId == advid).ToList();
            var advReplies = _context.Replies.Where(p => p.AdvID == advid).ToList();
            var advs = _context.Adverts.Find(advid);
            foreach (var pc in advCategories) _context.AdvCategories.Remove(pc);
            foreach (var pl in advLikes) _context.AdvLikes.Remove(pl);
            foreach (var pt in advTags) _context.AdvTags.Remove(pt);
            foreach (var pv in advImages) _context.AdvImages.Remove(pv);
            foreach (var pcom in advComments)
            {
                var commentLikes = _context.CommentLikes.Where(x => x.CommentId == pcom.CommentId).ToList();
                foreach (var cl in commentLikes) _context.CommentLikes.Remove(cl);
                _context.Comments.Remove(pcom);
            }
            foreach (var pr in advReplies)
            {
                var replyLikes = _context.ReplyLikes.Where(x => x.ReplyId == pr.ReplyId).ToList();
                foreach (var rl in replyLikes) _context.ReplyLikes.Remove(rl);
                _context.Replies.Remove(pr);
            }
            _context.Adverts.Remove(advs);
            Save();
        }
        public void AddNewAdvert(Advert advert)
        {
            _context.Adverts.Add(advert);
            Save();
        }
        #endregion

        public IList<Comment> GetAdvComments(Advert advert)
        {
            return _context.Comments.Where(p => p.AdvId == advert.Id).ToList();
        }

        public List<CommentViewModel> GetParentReplies(Comment comment)
        {
            var parentReplies = _context.Replies.Where(p => p.CommentId == comment.CommentId && p.ParentReplyId == null).ToList();
            List<CommentViewModel> parReplies = new List<CommentViewModel>();
            foreach (var pr in parentReplies)
            {
                var chReplies = GetChildReplies(pr);
                parReplies.Add(new CommentViewModel() { Body = pr.Body, ParentReplyId = pr.ParentReplyId, DateTime = pr.DateTime, Id = pr.ReplyId, UserName = pr.UserName, ChildReplies = chReplies });
            }
            return parReplies;
        }

        public List<CommentViewModel> GetChildReplies(Reply parentReply)
        {
            List<CommentViewModel> chldReplies = new List<CommentViewModel>();
            if (parentReply != null)
            {
                var childReplies = _context.Replies.Where(p => p.ParentReplyId == parentReply.ReplyId).ToList();
                foreach (var reply in childReplies)
                {
                    var chReplies = GetChildReplies(reply);
                    chldReplies.Add(new CommentViewModel() { Body = reply.Body, ParentReplyId = reply.ParentReplyId, DateTime = reply.DateTime, Id = reply.ReplyId, UserName = reply.UserName, ChildReplies = chReplies });
                }
            }
            return chldReplies;
        }


        public Reply GetReplyById(string id)
        {
            return _context.Replies.Where(p => p.ReplyId == id).FirstOrDefault();
        }


        public bool CommentDeleteCheck(string commentid)
        {
            return _context.Comments.Where(x => x.CommentId == commentid).Select(x => x.Deleted).FirstOrDefault();
        }
        public bool ReplyDeleteCheck(string replyid)
        {
            return _context.Replies.Where(x => x.ReplyId == replyid).Select(x => x.Deleted).FirstOrDefault();
        }


        

        public void UpdateReplyLike(string replyid, string username, string likeordislike, string rlikeid)
        {
            var replyLike = _context.ReplyLikes.Where(x => x.Username == username && x.ReplyId == replyid).FirstOrDefault();
            if (replyLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (replyLike.Like == false) { replyLike.Like = true; replyLike.Dislike = false; }
                        else replyLike.Like = false;
                        break;
                    case "dislike":
                        if (replyLike.Dislike == false) { replyLike.Dislike = true; replyLike.Like = false; }
                        else replyLike.Dislike = false;
                        break;
                }
                if (replyLike.Like == false && replyLike.Dislike == false) _context.ReplyLikes.Remove(replyLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        replyLike = new ReplyLike() { RlikeId = rlikeid, ReplyId = replyid, Username = username, Like = true, Dislike = false };
                        _context.ReplyLikes.Add(replyLike);
                        break;
                    case "dislike":
                        replyLike = new ReplyLike() { RlikeId = rlikeid, ReplyId = replyid, Username = username, Like = false, Dislike = true };
                        _context.ReplyLikes.Add(replyLike);
                        break;
                }
            }
            Save();
        }

        public Advert GetAdvByReply(string replyid)
        {
            var advid = _context.Replies.Where(x => x.ReplyId == replyid).Select(x => x.AdvID).FirstOrDefault();
            return _context.Adverts.Where(x => x.Id == advid).FirstOrDefault();
        }

        public string GetPageIdByComment(string commentId)
        {
            return _context.Comments.Where(x => x.CommentId == commentId).Select(x => x.AdvId).FirstOrDefault();
        }

        public string GetUrlSeoByReply(Reply reply)
        {
            var postId = _context.Comments.Where(x => x.CommentId == reply.CommentId).Select(x => x.AdvId).FirstOrDefault();
            return _context.Adverts.Where(x => x.Id == postId).Select(x => x.UrlSeo).FirstOrDefault();
        }

        public IList<Comment> GetComments()
        {
            return _context.Comments.ToList();
        }

        public IList<Comment> GetCommentsByPageId(string pageId)
        {
            return _context.Comments.Where(p => p.AdvId == pageId).ToList();
        }

        public IList<Reply> GetReplies()
        {
            return _context.Replies.ToList();
        }

        public void AddNewComment(Comment comment)
        {
            _context.Comments.Add(comment);
            Save();
        }
        public void AddNewReply(Reply reply)
        {
            _context.Replies.Add(reply);
            Save();
        }



        public Comment GetCommentById(string id)
        {
            return _context.Comments.Where(p => p.CommentId == id).FirstOrDefault();
        }

        public void DeleteComment(string commentid)
        {
            var comment = _context.Comments.Where(x => x.CommentId == commentid).FirstOrDefault();
            var commentLikes = _context.CommentLikes.Where(x => x.CommentId == comment.CommentId).ToList();
            var commentReplies = _context.Replies.Where(x => x.CommentId == comment.CommentId).ToList();
            foreach (var cl in commentLikes) _context.CommentLikes.Remove(cl);
            foreach (var cr in commentReplies) _context.Replies.Remove(cr);
            _context.Comments.Remove(comment);
            Save();
        }
        public void DeleteReply(string replyid)
        {
            var reply = _context.Replies.Where(x => x.ReplyId == replyid).FirstOrDefault();
            var retlyLikes = _context.ReplyLikes.Where(x => x.ReplyId == reply.CommentId).ToList();
            foreach (var rl in retlyLikes) _context.ReplyLikes.Remove(rl);
            _context.Replies.Remove(reply);
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        #region dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion dispose

    }
}