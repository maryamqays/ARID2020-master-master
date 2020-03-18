using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class EmailContent
    {
        public int Id { get; set; }

        public Guid UniqueName { get; set; }

        [Required]
        //[StringLength(4000)]
        public string ArContent { get; set; }

        [Required]
        [StringLength(4000)]
        public string EnContent { get; set; }

        [Required]
        [StringLength(100)]
        public string ArSubject { get; set; }

        [Required]
        [StringLength(100)]
        public string EnSubject { get; set; }

        [Required]
        public int SenderId { get; set; }
        public  Sender Sender { get; set; }   
    }
}
