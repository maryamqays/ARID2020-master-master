using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerReadyServiceExtension
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "الاضافة")]
        [StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "السعر")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        public PricingType PricingType { get; set; }

        [Display(Name = "الأيام المستغرقة لأكمال الإضافة")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        public RequiredDays RequiredDays { get; set; }


        [Display(Name = "الخدمة")]
        public Guid FreelancerReadyServiceId { get; set; }
        public FreelancerReadyService FreelancerReadyService { get; set; }
    }
}
