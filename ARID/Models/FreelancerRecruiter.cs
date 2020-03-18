using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerRecruiter
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "المكلف")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "المشروع")]
        public int FreelancerProjectId { get; set; }
        [Display(Name = "المشروع")]
        public FreelancerProject FreelancerProject { get; set; }

        [Display(Name = "تاريخ البدء ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "حالة المكلف بالمشروع")]
        public RecruiterStatus RecruiterStatus { get; set; }

        [Display(Name = "التقرير النهائي للإدارة")]
        [StringLength(2000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 2000 حرف", MinimumLength = 10)]
        public string ReportToAdmin { get; set; }
    }
}
