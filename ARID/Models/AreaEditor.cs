using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class AreaEditor
    {
        public int Id { get; set; }

        [Display(Name = "العضوية")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }

        [Required(ErrorMessage = "الوصف ضروري")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "الوصف ")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "الدور ")]
        public Role RoleId { get; set; }

        [Display(Name = "تاريخ الالتحاق")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

    }
}
