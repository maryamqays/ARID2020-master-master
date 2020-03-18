using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapterExam
    {
        [key]
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(300, ErrorMessage = "عدد الحروف يجب أن يكون بين 5 و 300 حرف", MinimumLength = 5)]
        [Display(Name = "السؤال")]
        public string Question { get; set; }

        [Display(Name = "الفصل")]
        public Guid CourseChapterId { get; set; }
        public CourseChapter CourseChapter { get; set; }

        [Display(Name = "الترتيب")]
        public int Indx { get; set; }

        [Display(Name = "مخفي")]
        public bool IsHidden { get; set; }

        [StringLength(100, ErrorMessage = "عدد الحروف يجب أن يكون أقل من 100 حرف")]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "صورة")]
        public string Imgurl { get; set; }
       
    }
}
