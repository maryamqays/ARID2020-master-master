using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseInstructor
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "الترتيب")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [Range(1, 1000, ErrorMessage = "RangeError")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "حقل الدورة مطلوب ")]
        [Display(Name = "الدورة")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب ")]
        [Display(Name = "نبذة عن المحاضر")]
        public string Description { get; set; }
        
        [Display(Name = "المحاضر")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }
    }
}
