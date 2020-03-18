using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("NewPassword", ErrorMessage = "ConfirmpasswordError")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
