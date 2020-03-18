using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Gallary
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string OriginalImage { get; set; }

        [StringLength(500)]
        public string ThumbImage { get; set; }
    }
}
