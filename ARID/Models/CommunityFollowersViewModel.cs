using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
	public class CommunityFollowersViewModel
	{
		public CommunityFollower CommunityFollower { get; set; }
		public Community Community { get; set; }
		public ApplicationUser ApplicationUser { get; set; }
		public IEnumerable<CommunityFollower> CommunityFollowers { get; set; }
		public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
	}
}
