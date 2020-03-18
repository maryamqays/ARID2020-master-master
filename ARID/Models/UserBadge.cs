using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public partial class UserBadge
    {
        public int Id { get; set; }

        [Display(Name = "UserId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Display(Name = "BadgeId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int BadgeId { get; set; }
        public  Badge Badge { get; set; }
        
        [Display(Name = "DateofRequest")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime DateofRequest { get; set; }

        [Display(Name = "DateofGrant")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime? DateofGrant { get; set; }

        [Display(Name = "IsRejected")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(false)]
        public bool IsRejected { get; set; }

        [Display(Name = "RejectCount")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [Range(0, 255, ErrorMessage = "RangeError")]
        [DefaultValue(0)]
        public byte RejectCount { get; set; }

        [Display(Name = "التفاصيل")]
        [StringLength(2000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string RejectReason { get; set; }

        [Display(Name = "IsGranted")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(false)]
        public bool IsGranted { get; set; }

        [Display(Name = "Visibility")]
        public bool IsVisible { get; set; }

        [NotMapped]
        [Display(Name = "Disabled")]
        public bool Disabled
        {
            get
            {
                if (RejectCount >= 5)
                    return true;
                else
                    return false;
            }

            set
            {
                if (value == false)
                    this.RejectCount = 0;
            }

        }
    }
}
