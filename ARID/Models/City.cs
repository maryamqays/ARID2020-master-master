using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ARID.Models
{
    public partial class City
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Arabic City name")]
        public string ArCityName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "English City name")]
        public string EnCityName { get; set; }

        [Required]
        public int CountryId { get; set; }
        public  Country Country { get; set; }      

       // public  List<ApplicationUser> ApplicationUsers { get; set; }

        //public  List<College> Colleges { get; set; }
        //public  List<AcademicPosition> AcademicPositions { get; set; }
    }
}