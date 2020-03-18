using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalSpeciality
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }

        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

    }
}
