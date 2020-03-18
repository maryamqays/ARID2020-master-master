using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Badge
    {
        public int Id { get; set; }

        [Display(Name = "ArBadgeName")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArBadgeName { get; set; }

        [Display(Name = "EnBadgeName")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnBadgeName { get; set; }

        [Display(Name = "ArBadgeDesc")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(4000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArBadgeDesc { get; set; }

        [Display(Name = "EnBadgeDesc")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnBadgeDesc { get; set; }

        [Display(Name = "BadgeLogo")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string BadgeLogo { get; set; }

        [Display(Name = "EmailContentId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int EmailContentId { get; set; }
        public  EmailContent EmailContent { get; set; }

        [Display(Name = "Visibility")]
        public bool IsVisible { get; set; }
    }
}
