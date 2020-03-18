using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ProfileViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public FreelancerRating FreelancerRating { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public List<UserExpertise> UserExpertises { get; set; }
        public List<Post> Posts { get; set; }
        public List<UserSkill> UserSkills { get; set; }
        public List<AcademicActivity> AcademicActivities { get; set; }
        public List<AcademicPosition> AcademicPositions { get; set; }
        public List<EducationalLevel> EducationalLevels { get; set; }
        public List<ProfileLink> ProfileLinks { get; set; }
        public List<Project> Projects { get; set; }
        public List<Publication> Publications { get; set; }
        public List<TeachingExperience> TeachingExperiences { get; set; }
        public List<UserBadge> UserBadges { get; set; }
        public List<Follow> Followings { get; set; }
        public List<Follow> Followers { get; set; }
        public List<Follow> UserFollowers { get; set; }
        public IEnumerable<FreelancerPortfolio> FreelancerPortfolios { get; set; }
        public IEnumerable<FreelancerRating> FreelancerRatings { get; set; }
        public int RegisteredByYou { get; set; }
        public int Likes { get; set; }
        public int SameUniversityCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
        public int postsCount { get; set; }
    }
}
