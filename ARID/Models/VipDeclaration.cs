using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class VipDeclaration
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "أسم الشخص")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        [Display(Name = "أسم الشخص")]
        public ApplicationUser ApplicationUser { get; set; }



        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ التصريح")]
        [Required]
        public DateTime DeclarationDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ التكوين")]
        [Required]
        public DateTime CreationDate { get; set; }

        [Display(Name = "نص التصريح")]
        [Required]
        [StringLength(500 ,ErrorMessage = "النص طويل")]
        public string DeclarationBody { get; set; }


        [Display(Name = "مناسبة التصريح")]
        [StringLength(75, ErrorMessage = "النص طويل")]
        public string DeclarationOccasion { get; set; }

    }
}
