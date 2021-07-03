using System;
using System.ComponentModel.DataAnnotations;

namespace Wulikunkun.Web.Models
{
    public class Article
    {
        public int? Id { get; set; }
        [MaxLength(256)]
        public string Title { get; set; }
        public string MarkContent { get; set; }
        public string HtmlContent { get; set; }
        public DateTime PublishTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public byte Tag { get; set; }
        public int ViewTimes { get; set; }
        public ArticleStatus? Status { get; set; }
        public virtual string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    /* 已删除，审核中，已通过，审核未通过 */
    public enum ArticleStatus
    {
        Deleted,
        Audit,
        Allowed,
        NotAllowed
    }
}