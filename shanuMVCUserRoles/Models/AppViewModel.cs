using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPMVC.Models
{
    [Table("Advert")]
    public class Advert
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "ShortDescription")]
        public string ShortDescription { get; set; }
        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }
        [Required]
        [Display(Name = "Meta")]
        public string Meta { get; set; }
        [Required]
        [Display(Name = "UrlSeo")]
        public string UrlSeo { get; set; }
        public bool Published { get; set; }
        [DefaultValue(0)]
        public int LikeCount { get; set; }
        public string Author { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime? Modified { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reply> Replies { get; set; }
        public ICollection<AdvCategory> AdvCategories { get; set; }
        public ICollection<AdvTag> AdvTags { get; set; }
        public ICollection<AdvImage> AdvImages { get; set; }
        public ICollection<AdvLike> AdvLikes { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "UrlSeo")]
        public string UrlSeo { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool Checked { get; set; }
        public ICollection<AdvCategory> AdvCategories { get; set; }
    }

    public class AdvCategory
    {
        [Key]
        [Column(Order = 0)]
        public string AdvID { get; set; }

        [Key]
        [Column(Order = 1)]
        public string CatID { get; set; }

        public bool Checked { get; set; }
        public Advert Advert { get; set; }
        public Category Category { get; set; }

    }

    public class AdvLike
    {
        [Key]
        public string likeId { get; set; }
        [ForeignKey("Advert")]
        public string advId { get; set; }
        public string Username { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
        public Advert Advert { get; set; }
    }

    public class Comment
    {
        [Key]
        public string CommentId { get; set; }
        public string AdvId { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Comment can`t be empty. ")]
        public string Body { get; set; }
        [DefaultValue(0)]
        public int NetLikeCount { get; set; }
        [DefaultValue(false)]
        public bool Deleted { get; set; }
        public Advert Advert { get; set; }
        public ICollection<Reply> Replies { get; set; }
        public ICollection<CommentLike> CommentLikes { get; set; }
    }

    public class AdvImage
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "ImgUrl")]
        public string ImageUrl { get; set; }
        public string Imagehumbnail { get; set; }
        public string AdvId { get; set; }
        public string SiteName { get; set; }
    
        public Advert Advert { get; set; }
    }

    public class Reply
    {
        [Key]
        public string ReplyId { get; set; }
        public string AdvID { get; set; }
        public string CommentId { get; set; }
        public string ParentReplyId { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Comment can`t be empty. ")]
        public string Body { get; set; }
        [DefaultValue(false)]
        public bool Deleted { get; set; }
        public Advert Advert { get; set; }
        public Comment Comment { get; set; }
        public ICollection<ReplyLike> ReplyLikes { get; set; }
    }

    public class CommentLike
    {
        [Key]
        public string ClikeId { get; set; }
        [ForeignKey("Comment")]
        public string CommentId { get; set; }
        public string Username { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
        public Comment Comment { get; set; }
    }

    public class ReplyLike
    {
        [Key]
        public string RlikeId { get; set; }
        [ForeignKey("Reply")]
        public string ReplyId { get; set; }
        public string Username { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
        public Reply Reply { get; set; }
    }

    public class Tag
    {
        [Key]
        public string Id { get; set; }
        [Required(ErrorMessage = "Giva a Tag name ")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Giva a Url ")]
        [Display(Name = "UrlSeo")]
        public string UrlSeo { get; set; }

        public bool Checked { get; set; }
        public ICollection<AdvTag> AdvTags { get; set; }
    }

    public class AdvTag
    {
        [Key]
        [Column(Order = 0)]
        public string AdvID { get; set; }
        [Key]
        [Column(Order = 1)]
        public string TagId { get; set; }
        public bool Checked { get; set; }
        public Advert Advert { get; set; }
        public Tag Tag { get; set; }
    }

   

    public class AllAdsViewModel
    {
        public IList<Category> AdvCategories { get; set; }
        public IList<Tag> AdvTags { get; set; }
        public string AdvID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public bool Checked { get; set; }
        public Tag Tag { get; set; }
        public string UrlSlug { get; set; }
    }

    public class AdvViewModel
    {
        public string Body { get; set; }
        public string FirstAdvId { get; set; }
        public string ID { get; set; }
        public string LastAdvId { get; set; }
        public string NextAdvSlug { get; set; }
        public string PreviousAdvSlug { get; set; }
        public int AdvCount { get; set; }
        public int AdvDislikes { get; set; }
        public int AdvLikes { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public IList<AdvImage> Images { get; set; }
        public IList<Tag> PostTags { get; set; }
        public string Meta { get; set; }
        public string UrlSeo { get; set; }
        public IList<Category> AdvCategories { get; set; }
        public string ShortDescription { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<Tag> Tags { get; set; }
        public IList<Comment> Comments { get; set; }

        public CommentViewModel CommentViewModel { get; set; }
    }

    public class CommentViewModel
    {
        public CommentViewModel() { }
        public CommentViewModel(Comment comment)
        {
            Comment = comment;
        }
        public IList<Comment> Comments { get; set; }
        public Comment Comment { get; set; }
        public string UrlSeo { get; set; }
        public DateTime DateTime { get; set; }
        public IList<CommentViewModel> ChildReplies { get; set; }
        public string Body { get; set; }
        public string Id { get; set; }
        public string ParentReplyId { get; set; }
        public string UserName { get; set; }

    }

    public class ServiceViewModel
    {
        public DateTime PostedOn { get; set; }
        public DateTime? Modified { get; set; }
        public IList<Tag> Tag { get; set; }
        public int AdvDislikes { get; set; }
        public int AdvLikes { get; set; }
        public int TotalAverts { get; set; }
        public List<string> Category { get; set; }
        public Advert Advert { get; set; }
        public string ID { get; set; }
        public string ShortDescription { get; set; }
        public string Title { get; set; }
        public IList<Category> AdvCategories { get; set; }
        public IList<Tag> AdvTags { get; set; }
        public string UrlSlug { get; set; }

        public CommentViewModel CommentViewModel { get; set; }
        public PagedList.IPagedList<ServiceViewModel> PagedServiceViewModel { get; set; }
    }
}