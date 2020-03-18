using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerComment
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "المشروع")]
        public int FreelancerProjectId { get; set; }
        [Display(Name = "المشروع")]
        public FreelancerProject FreelancerProject { get; set; }

        [StringLength(450)]
        [Display(Name = "المعلق")]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "التعليق")]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [StringLength(2000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 5000 حرف", MinimumLength = 10)]
        public string Commentary { get; set; }

        [Display(Name = "تاريخ التعليق")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "تحميل ملف")]
        [StringLength(500)]
        public string File { get; set; }

    }
}
