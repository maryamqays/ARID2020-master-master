using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class RegistrationFormViewModel
    {
        public RegistrationForm RegistrationForm { get; set; }
        public UserExpressInterest ExpressInterest { get; set; }
        public IEnumerable<UserExpressInterest> UserExpressInterests { get; set; }
        //public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
    }
}
