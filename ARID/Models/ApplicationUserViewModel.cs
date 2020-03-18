using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
	public class ApplicationUserViewModel
	{
		public ApplicationUser ApplicationUser { get; set; }
		public PagingInfo PagingInfo { get; set; }
		public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
		public string StatusMessage { get; set; }
	}
}
