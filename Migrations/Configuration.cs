namespace shanuMVCUserRoles.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<shanuMVCUserRoles.Models.AdvDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(shanuMVCUserRoles.Models.AdvDbContext context)
        {
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat1", Name = "Lorem", UrlSeo = "Lorem", Description = "Lorem Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat2", Name = "Duis", UrlSeo = "Duis", Description = "Duis Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat3", Name = "Nulla", UrlSeo = "Nulla", Description = "Nulla Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat4", Name = "Ipsum", UrlSeo = "Ipsum", Description = "Ipsum Category" });

            context.Adverts.AddOrUpdate(new Models.Advert { Id = "1", Title = "Lorem", Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus nec dolor metus. Nullam risus nisi, posuere eget consequat ac, lacinia ac arcu. Nulla facilisi. Nunc nec tristique sem, eu pulvinar augue. Morbi at risus eget tortor pharetra cursus eu at ligula. Mauris eu commodo nisl, ac lobortis lectus. Ut rhoncus rutrum elit sed fringilla. Etiam in accumsan purus. Maecenas orci diam, consequat a tellus at, pellentesque ullamcorper elit. Sed quis consequat turpis. Proin lacinia est sit amet felis imperdiet, sit amet convallis nulla imperdiet. Nunc sit amet justo sapien. Nulla pulvinar mi quis dapibus commodo.", ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus nec dolor metus. Nullam risus nisi, posuere eget consequat ac, lacinia ac arcu. Nulla facilisi. Nunc nec tristique sem, eu pulvinar augue. Morbi at risus eget tortor pharetra cursus eu at ligula.", PostedOn = DateTime.Now, Meta = "Lorem", UrlSeo = "Lorem", Published = true });
            context.Adverts.AddOrUpdate(new Models.Advert { Id = "2", Title = "Duis", Body = "Duis sed bibendum risus, nec porta velit. Proin commodo lectus ut nibh blandit tincidunt ut non nibh. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean pretium, felis eget sollicitudin pharetra, arcu ante commodo eros, ut mattis lacus neque sed augue. Fusce laoreet libero eros, sit amet iaculis mauris tempor sit amet. Donec sollicitudin bibendum sem. Nullam a ligula placerat velit rutrum finibus. Vivamus dapibus diam vel nisi pellentesque, et iaculis tellus commodo. Donec efficitur sapien eget arcu cursus bibendum. Morbi risus risus, pellentesque ac sem a, tempor tristique elit.", ShortDescription = "Duis sed bibendum risus, nec porta velit. Proin commodo lectus ut nibh blandit tincidunt ut non nibh. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.", PostedOn = DateTime.Now, Meta = "Duis", UrlSeo = "Duis", Published = true });
            context.Adverts.AddOrUpdate(new Models.Advert { Id = "3", Title = "Nulla", Body = "Nulla mattis mi in elementum elementum. Aliquam dictum quam id nibh fermentum maximus. Curabitur facilisis neque eget lorem scelerisque vestibulum. Sed et pulvinar turpis, eu convallis urna. Vivamus consectetur vel lorem ut dictum. Aliquam ac ante eu tortor pharetra efficitur. Fusce mattis lacinia arcu, vel dignissim leo fermentum ac. Donec tincidunt pellentesque tristique. Pellentesque porta faucibus scelerisque. Ut ante mi, iaculis eleifend augue vel, fringilla sodales felis. Pellentesque vehicula metus sapien, eget sagittis augue eleifend ut. Ut sit amet nulla est. Sed vel turpis quis dui lobortis accumsan a a mi. Donec nec sagittis urna.", ShortDescription = "Nulla mattis mi in elementum elementum. Aliquam dictum quam id nibh fermentum maximus. Curabitur facilisis neque eget lorem scelerisque vestibulum.", PostedOn = DateTime.Now, Meta = "Nulla", UrlSeo = "Nulla", Published = true });

            context.AdvCategories.AddOrUpdate(new Models.AdvCategory { AdvID = "1", CatID = "cat1" });
            context.AdvCategories.AddOrUpdate(new Models.AdvCategory { AdvID = "1", CatID = "cat4" });
            context.AdvCategories.AddOrUpdate(new Models.AdvCategory { AdvID = "2", CatID = "cat2" });
            context.AdvCategories.AddOrUpdate(new Models.AdvCategory { AdvID = "3", CatID = "cat3" });


            context.AdvImages.AddOrUpdate(new Models.AdvImage { ID = 1, AdvId = "1", SiteName = "YouTube", ImageUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", Imagehumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.AdvImages.AddOrUpdate(new Models.AdvImage { ID = 2, AdvId = "2", SiteName = "YouTube", ImageUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", Imagehumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.AdvImages.AddOrUpdate(new Models.AdvImage { ID = 3, AdvId = "3", SiteName = "YouTube", ImageUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", Imagehumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.AdvImages.AddOrUpdate(new Models.AdvImage { ID = 4, AdvId = "1", SiteName = "YouTube", ImageUrl = "https://www.youtube.com/embed/XzAHGhMhl7o", Imagehumbnail = "http://img.youtube.com/vi/XzAHGhMhl7o/0.jpg" });


            context.Comments.AddOrUpdate(new Models.Comment { CommentId = "cmt1", AdvId = "1", Body = "Vivamus hendrerit commodo pulvinar. In convallis nunc nec scelerisque sodales. Curabitur aliquam neque dolor, hendrerit cursus felis ultricies ac. Mauris ac justo vel lorem condimentum malesuada. Vivamus porttitor vestibulum lorem a luctus. Suspendisse in eleifend orci.", DateTime = DateTime.Now, UserName = "devsone" });
            context.Comments.AddOrUpdate(new Models.Comment { CommentId = "cmt2", AdvId = "1", Body = "Suspendisse egestas risus eget posuere egestas. Nunc facilisis ligula et vestibulum pretium. Suspendisse potenti. Nulla facilisi. Integer mi lorem, efficitur quis viverra in, sollicitudin id urna. Maecenas scelerisque, tellus eget rutrum pulvinar, velit erat pulvinar risus, vitae convallis quam mi ut leo. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Ut posuere lacus ex, eget tristique quam eleifend molestie.", DateTime = DateTime.Now, UserName = "mario" });
            context.Comments.AddOrUpdate(new Models.Comment { CommentId = "cmt3", AdvId = "1", Body = "Integer lacinia ipsum magna, ac sodales libero porttitor at. Donec at enim felis. Ut bibendum lorem odio, quis ultricies elit eleifend ut. Fusce erat tellus, eleifend at gravida in, fermentum et tellus. Fusce at massa vehicula, maximus diam nec, lacinia lectus.", DateTime = DateTime.Now, UserName = "natash" });


            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep1", AdvID = "1", CommentId = "cmt3", DateTime = DateTime.Now, UserName = "flint", Body = "Cras sodales justo sit amet libero placerat consectetur. Duis hendrerit facilisis tempor. Nullam ut nisl nec neque posuere semper. Praesent vestibulum id purus quis maximus." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep9", AdvID = "1", CommentId = "cmt3", DateTime = DateTime.Now, UserName = "anyav", Body = "Cras sodales justo sit amet libero placerat consectetur. Duis hendrerit facilisis tempor. Nullam ut nisl nec neque posuere semper. Praesent vestibulum id purus quis maximus." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep2", AdvID = "1", CommentId = "cmt3", ParentReplyId = "rep1", DateTime = DateTime.Now, UserName = "devsone", Body = "Mauris pulvinar tristique libero id ornare. Quisque sit amet accumsan leo. Vestibulum dapibus elit sed lorem lacinia suscipit. In hac habitasse platea dictumst. Vivamus egestas leo eu nulla faucibus cursus." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep3", AdvID = "1", CommentId = "cmt3", ParentReplyId = "rep2", DateTime = DateTime.Now, UserName = "kingkong", Body = "Suspendisse consequat dolor urna, sit amet accumsan lectus luctus eget. Vestibulum maximus ante vel placerat cursus. Nulla luctus augue ac vulputate aliquet." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep4", AdvID = "1", CommentId = "cmt3", ParentReplyId = "rep3", DateTime = DateTime.Now, UserName = "hanley", Body = "Donec aliquam, sem a tincidunt tincidunt, orci velit mollis magna, vel auctor arcu augue nec risus. Integer luctus enim ac viverra luctus." });

            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep5", AdvID = "1", CommentId = "cmt2", DateTime = DateTime.Now, UserName = "taylor", Body = "Cras sodales justo sit amet libero placerat consectetur. Duis hendrerit facilisis tempor. Nullam ut nisl nec neque posuere semper. Praesent vestibulum id purus quis maximus." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep6", AdvID = "1", CommentId = "cmt2", ParentReplyId = "rep5", DateTime = DateTime.Now, UserName = "devsone", Body = "Mauris pulvinar tristique libero id ornare. Quisque sit amet accumsan leo. Vestibulum dapibus elit sed lorem lacinia suscipit." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep7", AdvID = "1", CommentId = "cmt2", ParentReplyId = "rep6", DateTime = DateTime.Now, UserName = "hanley", Body = "Suspendisse consequat dolor urna, sit amet accumsan lectus luctus eget. Vestibulum maximus ante vel placerat cursus. Nulla luctus augue ac vulputate aliquet." });
            context.Replies.AddOrUpdate(new Models.Reply { ReplyId = "rep8", AdvID = "1", CommentId = "cmt2", ParentReplyId = "rep7", DateTime = DateTime.Now, UserName = "taylor", Body = "Donec aliquam, sem a tincidunt tincidunt, orci velit mollis magna, vel auctor arcu augue nec risus. Integer luctus enim ac viverra luctus." });

        }
    }
}
