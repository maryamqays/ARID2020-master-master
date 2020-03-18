using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class TransferCreditViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        [EmailAddress(ErrorMessage = "EmailError")]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

               [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }
    }
}
