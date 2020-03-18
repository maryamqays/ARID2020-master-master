using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseRegisteration
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "حقل الدورة مطلوب ")]
        [Display(Name = "الدورة")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Display(Name = "تاريخ الالتحاق")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تجاوز الاختبار")]
        public bool IsPassedExam { get; set; }

        [Display(Name = "تاريخ الشهادة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime PassedDate { get; set; }

    }
}
