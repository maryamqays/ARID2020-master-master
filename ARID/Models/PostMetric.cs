using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PostMetric
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public  Post Post { get; set; }

        [Display(Name = "المستخدم")]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "قيمة التصويت")] // 0 visit  1 like -1 dislike
        public int VoteValue { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateTime { get; set; }

        [Display(Name = "الاشعار")]
        public bool NotifyMe { get; set; }

        [Display(Name = "نوع البلاغ")]
        public ReportType ReportType { get; set; }

    }
}
