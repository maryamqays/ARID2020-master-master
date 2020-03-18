using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class TicketCategory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "القسم")]
        public string Name { get; set; }

        [StringLength(50)]
        public string NotifyEmail { get; set; }

        public bool Status { get; set; }
    }
}
