using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapterContent
    {
        [key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "العنوان ")]
        public string Title { get; set; }

        [Required(ErrorMessage = "وصف المحتوى مطلوب ")]
        [Display(Name = "وصف المحتوى")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError")]
        public string Description { get; set; }

        [Display(Name = "الترتيب")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "حقل القسم مطلوب")]
        [Display(Name = "القسم")]
        public Guid CourseChapterId { get; set; }
        public CourseChapter CourseChapter { get; set; }

        [Required]
        [Display(Name = "نوع المحتوى")]
        public ContentType ContentType { get; set; }


        [Display(Name = "مسار الملف")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        public string FilePath { get; set; }

        [Display(Name = "المدة")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError")]
        public string Duration { get; set; }

        [Display(Name = "مجاني")]
        public bool IsFree { get; set; }

        [Display(Name = "متاح للتنزيل")]
        public bool IsDownloadable { get; set; }

        [Required]
        [Display(Name = "محذوفة")]
        public bool IsDeleted { get; set; }

        [Required]
        [Display(Name = "مخفية")]
        public bool Ishidden { get; set; }

        [Required]
        [Display(Name = "مبلغ عنه")]
        public bool IsReported { get; set; }


    }
}
