using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class StatisticsViewModel
    {
        public Country Country { get; set; }
        public University University { get; set; }
        public City City { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<University> Universities { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
    }
}
