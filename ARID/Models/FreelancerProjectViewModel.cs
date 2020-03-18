using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerProjectViewModel
    {
        public FreelancerProject FreelancerProject { get; set; }
        public Ticket Ticket { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public FreelancerProposal FreelancerProposal { get; set; }
        public FreelancerComment FreelancerComment { get; set; }
        public FreelancerRating FreelancerRating { get; set; }
        public IEnumerable<FreelancerProject> FreelancerProjects { get; set; }
        public IEnumerable<FreelancerProject> AllFreelancerProjects { get; set; }
        public IEnumerable<FreelancerRating> FreelancerRatings { get; set; }
        public IEnumerable<FreelancerProposal> FreelancerProposals { get; set; }
        public IEnumerable<FreelancerComment> FreelancerComments { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public IEnumerable<FreelancerRecruiter> FreelancerRecruiters { get; set; }
        public IEnumerable<FreelancerReadyService> FreelancerReadyServices { get; set; }
        public IEnumerable<UserSkill> UserSkills { get; set; }
    }
}
