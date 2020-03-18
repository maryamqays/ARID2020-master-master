using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class UserExpressInterest
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "استمارة التسجيل")]
        public int RegistrationFormId { get; set; }
        public  RegistrationForm RegistrationForm { get; set; }

        [Display(Name = "الباحث")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        [Display(Name = "الباحث")]
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تاريخ التسجيل")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }
    }
}
