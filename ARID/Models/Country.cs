using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ARID.Models
{
    public partial class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ArCountryName { get; set; }

        [Required]
        [StringLength(50)]
        public string EnCountryName { get; set; }

        [StringLength(10)]
        public string CountryCode { get; set; }

        [StringLength(5, MinimumLength =2)]
        public string ShortName { get; set; }

        [StringLength(50)]
        public string Flag { get; set; }

        //public  List<City> Cities { get; set; }

        //public  List<ApplicationUser> ApplicationUsers { get; set; }

        //public  List<AcademicActivity> AcademicActivities { get; set; }
        
        //public  List<University> Universities { get; set; }
    }
}