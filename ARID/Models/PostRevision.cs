using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PostRevision
    {

        public Guid Id { get; set; }

        [Display(Name = "المحرر")] //New
        [StringLength(450)]
        public string EditorUserId { get; set; }
        [Display(Name = "الناشر")]
        public  ApplicationUser EditorUser { get; set; }

        [Display(Name = "تاريخ التحرير")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EditorDateTime { get; set; }

        [Display(Name = "الموضوع")]
        public Guid PostId { get; set; }
        [Display(Name = "الموضوع")]
        public  Post Post { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "العنوان")]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        [Display(Name = "المحتوى")]
        public string Body { get; set; }
    }
}
