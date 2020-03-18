using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ExpertiseViewModel
    {
        public Expertise Expertise { get; set; }
        public IEnumerable<Expertise> Expertises { get; set; }
        public IEnumerable<UserExpertise> UserExpertise { get; set; }
    }
}
