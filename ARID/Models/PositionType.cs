using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PositionType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ArPositionName { get; set; }

        [StringLength(100)]
        public string EnPositionName { get; set; }

        [Required]
        public bool IsApproved { get; set; }
    }
}
