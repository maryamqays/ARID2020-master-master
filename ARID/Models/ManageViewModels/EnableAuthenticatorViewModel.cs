using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(7, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [ReadOnly(true)]
        [Display(Name = "SharedKey")]
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}
