using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Publication
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "PublicationType")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(PublicationType.Journal)]
        public PublicationType PublicationType { get; set; }

        [Display(Name = "ArTitle")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArTitle { get; set; }

        [Display(Name = "EnTitle")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnTitle { get; set; }

        [Display(Name = "ArAuthors")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArAuthors { get; set; }

        [Display(Name = "EnAuthors")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnAuthors { get; set; }

        [Display(Name = "ArAbstract")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(4000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArAbstract { get; set; }

        [Display(Name = "EnAbstract")]
        [StringLength(4000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnAbstract { get; set; }

        [Display(Name = "PublicationDate")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Publisher")]
        //[Required(ErrorMessage = "RequiredFieldError")]        
        [StringLength(300, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Publisher { get; set; }

        [Display(Name = "VolumeNo")]
        [Range(0, 10000, ErrorMessage = "RangeError")]
        public int? VolumeNo { get; set; }

        [Display(Name = "IssueNo")]
        [Range(0, 10000, ErrorMessage = "RangeError")]
        public int? IssueNo { get; set; }

        [Display(Name = "ISSN/ISBN")]
        [StringLength(10, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ISSN { get; set; }

        [Display(Name = "DOI")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string DOI { get; set; }

        [Display(Name = "Pages")]
        [StringLength(10, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Pages { get; set; }

        [Display(Name = "FileLink")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string FileLink { get; set; }

        [Url]
        [Display(Name = "ExternalLink")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ExternalLink { get; set; }

        [Display(Name = "DownloadHits")]
        [Range(0, 1000000, ErrorMessage = "RangeError")]
        public int DownloadHits { get; set; }

        [Display(Name = "Keywords")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Keywords { get; set; }

    }
}
