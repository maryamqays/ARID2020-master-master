using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class RegisterFromOutsideViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        [EmailAddress(ErrorMessage = "EmailError")]
        [Display(Name = "البريد الاكتروني")]
        public string Email { get; set; }

        [StringLength(450)]
        [Display(Name = "تم اضافته بواسطة")]
        public string ReferredById { get; set; }

        // Added by Alrshah ===========================================================================================
        //Added by  saif https://stackoverflow.com/questions/12518689/regular-expression-not-to-allow-numbers-just-arabic-letters/12518814
        [Required(ErrorMessage = "الاسم مطلوب لاكمال التسجيل")]
        [RegularExpression(@"^[\u0621-\u064A\040]+$", ErrorMessage = "الاسم يجب ان يكون باللغة العربية")]
        [StringLength(50, ErrorMessage = "يجب كتابة الاسم الثلاثي", MinimumLength = 10)]
        [Display(Name = "الإسم باللغة العربية")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(50)]
        [Display(Name = "الإسم باللغة الإنجليزية")]
        public string EnName { get; set; }

        [Required(ErrorMessage = "اللغة")]
        [Display(Name = "UILanguage")]
        public string UILanguage { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [Display(Name = "البلد")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [Display(Name = "الجامعة")]
        public int UniversityId { get; set; }

    }
}
