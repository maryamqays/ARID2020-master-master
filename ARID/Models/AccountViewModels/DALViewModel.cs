﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models.AccountViewModels
{
    public class DALViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        [EmailAddress(ErrorMessage = "EmailError")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
