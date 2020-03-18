using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommitteeAchievement
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "العضو")]
        public int CommitteeProfileId { get; set; }
        public CommitteeProfile CommitteeProfile { get; set; }


        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ الانجاز")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Required]
        [Display(Name = "نوع الانجاز")]
        public AchievementType AchievementType { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

    }
}
