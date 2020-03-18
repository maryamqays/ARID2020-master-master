using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseSponsor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ألاسم مطلوب")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "الاسم ")]
        public string Name { get; set; }

        [Display(Name = "الترتيب")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [Range(1, 1000, ErrorMessage = "RangeError")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "حقل الدورة مطلوب")]
        [Display(Name = "الدورة")]
        public int CourseId { get; set; }
        public  Course Course { get; set; }

        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الشعار")]
        public string Logo { get; set; }

        [Url]
        [Display(Name = "الرابط")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Link { get; set; }

    }
}
