using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapter
    {
        [key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "إسم القسم ")]
        public string Name { get; set; }

        [Display(Name = "الترتيب")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "حقل الدورة مطلوب ")]
        [Display(Name = "الدورة")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        [Display(Name = "محذوفة")]
        public bool IsDeleted { get; set; }

        [Required]
        [Display(Name = "مخفية")]
        public bool Ishidden { get; set; }


    }
}
