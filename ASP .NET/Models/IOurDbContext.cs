using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPMVC.Models
{
    public interface IOurDbContext : IDisposable
    {
        #region 1
        IList<Advert> GetAds();
        IList<AdvLike> GetLikes();
        IList<ReplyLike> GetRLikes();
        IList<CommentLike> GetCLikes();
        IList<Category> GetAdvCategories(Advert adv);
        IList<AdvImage> GetAdvImages(Advert adv);
        IList<Tag> GetAdvTags(Advert adv);
        int LikeDislikeCount(string typeAndlike, string id);
        IList<Tag> GetTags();
        IList<Category> GetCategories();
        #endregion

        #region 2
        Advert GetAdvById(string advid);
        string GetAdvIdBySlug(string slug);
        void UpdateAdvLike(string advid, string username, string likeordislike, string likeid);
        void AddImageToAdv(string advid, string Url);
        void RemoveImageFromAdv(string advid, string Url);
        void AddAdvCategories(AdvCategory advCategory);
        void RemoveAdvCategories(string advid, string categoryid);
        void RemoveCategoryFromAdv(string advid, string catName);
        void AddNewCategory(string catName, string catUrlSeo, string catDesc);
        void RemoveTagFromAdv(string advid, string tagName);
        void AddAdvTags(AdvTag advTag);
        void RemoveAdvTags(string advid, string tagid);
        void AddNewTag(string tagName, string tagUrlSeo);
        void RemoveCategory(Category category);
        void RemoveTag(Tag tag);
        void DeleteAdvandComponents(string advid);
        void AddNewAdvert(Advert adv);
        #endregion

        IList<Comment> GetAdvComments(Advert post);
        IList<Comment> GetCommentsByPageId(string pageId);
        string GetPageIdByComment(string commentId);
        string GetUrlSeoByReply(Reply reply);
        List<CommentViewModel> GetParentReplies(Comment comment);
        List<CommentViewModel> GetChildReplies(Reply parentReply);
        Reply GetReplyById(string id);
        bool CommentDeleteCheck(string commentid);
        bool ReplyDeleteCheck(string replyid);
        void UpdateCommentLike(string commentid, string username, string likeordislike, string likeid);
        void UpdateReplyLike(string replyid, string username, string likeordislike, string clikeid);
        Advert GetAdvByReply(string replyid);
        IList<Comment> GetComments();
        IList<Reply> GetReplies();
        void AddNewComment(Comment comment);
        void AddNewReply(Reply reply);
        Comment GetCommentById(string id);
        void DeleteComment(string commentid);
        void DeleteReply(string replyid);
        void Save();

    }
}
