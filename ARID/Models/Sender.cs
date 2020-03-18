using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Sender
    {
        public int Id { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Required]
        public string Email { get; set; }
                
        [Required]
        [StringLength(100)]
        public string ArDescription { get; set; }

        [Required]
        [StringLength(100)]
        public string EnDescription { get; set; }

        //public virtual List<EmailContent> EmailContents { get; set; }
    }
}
