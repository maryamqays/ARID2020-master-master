using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ChapterStudyStatus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "حقل القسم مطلوب")]
        [Display(Name = "القسم")]
        public int CourseChapterContentId { get; set; }
        public CourseChapterContent CourseChapterContent { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ الانشاء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "الحالة")]
        public bool Status { get; set; }



    }
}
