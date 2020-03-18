using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Expertise
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        [StringLength(450)]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

     
    }
}
