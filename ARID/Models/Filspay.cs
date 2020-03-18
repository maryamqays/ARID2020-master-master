using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Filspay
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "المبلغ")]
        [Range(20, 1000)]
        public decimal UserAmount { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public bool Status { get; set; }

        [StringLength(450)]
        public string trn_id { get; set; }

    }
}
