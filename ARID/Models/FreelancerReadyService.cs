using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerReadyService
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(450)]
        [Display(Name = "العضو")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(200, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 100 حرف", MinimumLength = 10)]
        [Display(Name = "العنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المهارات")]
        [StringLength(2000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 2000 حرف", MinimumLength = 0)]
        public string Skills { get; set; }

        [Display(Name = "الوصف")]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [StringLength(4000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 5000 حرف", MinimumLength = 10)]
        public string Description { get; set; }
      
        [StringLength(100)]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [Display(Name = "صورة")]
        public string Image { get; set; }

        [StringLength(50)]
        [Display(Name = "يوتيوب")]
        public string Youtube { get; set; }

        [Display(Name = "عدد المشاهدات")]
        public int Views { get; set; }

        [Display(Name = "الأيام المستغرقة لأكمال العمل")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        public int RequiredDays { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "التسعير")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        public PricingType PricingType { get; set; }


        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "فئة مهارات المشروع")]
        public SkillCategoryType SkillCategoryType { get; set; }

    }
}


    

