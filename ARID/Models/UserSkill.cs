using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class UserSkill
    {
        public int Id { get; set; }

        [Display(Name = "SkillId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int SkillId { get; set; }
        public  Skill Skill { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }
    }
}
