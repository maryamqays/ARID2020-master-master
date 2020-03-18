using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SentEmailRecords
    {
        public Guid Id { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int EmailContentId { get; set; }
        public EmailContent EmailContent { get; set; }
    }
}
