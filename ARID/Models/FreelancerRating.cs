using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerRating
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        [Display(Name = "المقيم")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "المشروع")]
        public int FreelancerProjectId { get; set; }
        [Display(Name = "المشروع")]
        public FreelancerProject FreelancerProject { get; set; }

        [Display(Name = "الخدمة")]
        public Guid FreelancerReadyServiceId { get; set; }


        [Display(Name = "مرئي")]
        public bool Isvisible { get; set; }

        [Display(Name = "تاريخ التقييم")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateofRecord { get; set; }

        [Display(Name = "الوصف")]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [StringLength(1000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 1000 حرف", MinimumLength = 10)]
        public string Comment { get; set; }

        [Display(Name = "الإحترافية")]
        public int Professionalism { get; set; }

        [Display(Name = "التواصل")]
        public int Communication { get; set; }

        [Display(Name = "النوعية")]
        public int Quality { get; set; }

        [Display(Name = "الخبرة")]
        public int Experience { get; set; }

        [Display(Name = "التسليم")]
        public int Delivery { get; set; }

        [Display(Name = "التعامل معه مجددا")]
        public int RehirePossibility { get; set; }
    }
}
