using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommitteeProfile
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "العضو")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "رئيس اللجنة")]
        public bool IsCommitteeAdmin { get; set; }

        [Display(Name = "تاريخ الالتحاق")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Display(Name = "اللجنة")]
        public int CommitteeId { get; set; }
        [Display(Name = "اللجنة")]
        public Committee Committee { get; set; }


        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "الصفة")]
        public string Task { get; set; }

    }
}
