using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم الكلية بالعربي")]
        public string ArFacultyName { get; set; }

        [StringLength(100)]
        [Display(Name = "اسم الكلية بالانجليزي")]
        public string EnFacultyName { get; set; }

        [Display(Name = "تمنح دراسات عليا؟")]
        public bool HasPostGraduation { get; set; }

        [Display(Name = "الجامعة")]
        public int UniversityId { get; set; }
               public  University University { get; set; }

        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        public int CityId { get; set; }
        public  City City { get; set; }

        public bool IsIndexed { get; set; }
    }
}
