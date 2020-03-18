using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerSkillVerification
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "المدقق")]
        [StringLength(450)]
        public string VerifiedByUserId { get; set; }
        public  ApplicationUser VerifiedByUser { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "التخصص")]
        public int SkillId { get; set; }
        [Display(Name = "التخصص")]
        public Skill Skill { get; set; }

        [Display(Name = "الوصف")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(5000, ErrorMessage = "هذا الحقل يجب ان يترواح بين 30 الى 5000 حرف", MinimumLength = 30)]
        public string Details { get; set; }

        [Display(Name = "هل تم التحقق")]
        public bool IsVerified { get; set; }

        [Display(Name = "هل المهارة مقدمة باللغة العربية")]
        public bool IsArabic { get; set; }

        [Display(Name = "هل المهارة مقدمة باللغة الإنكليزية")]
        public bool IsEnglish { get; set; }

        [Display(Name = "تاريخ البدء بهذه المهارة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime SinceDate { get; set; }

        [StringLength(100)]
        [Display(Name = "الشهادات")]
        public string Certificates { get; set; }

        [Url]
        [StringLength(500, ErrorMessage = "الحد الأقصى لهذا الحقل 500 حرف", MinimumLength = 0)]
        [Display(Name = "رابط يوضح إحترافيتك في هذه المهارة")]
        public string ExternalLink { get; set; }
    }
}
