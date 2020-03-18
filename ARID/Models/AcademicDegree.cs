using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class AcademicDegree
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ArDegreeName { get; set; }

        [Required]
        [StringLength(100)]
        public string EnDegreeName { get; set; }

        [Range(1, 100)]
        public int Indx { get; set; }
    }
}
