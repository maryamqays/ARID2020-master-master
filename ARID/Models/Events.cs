using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }
        public string EventNameAr { get; set; }
        public string EventNameEn { get; set; }
        public int EventType { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string Organizers { get; set; }
        public string ApplicationUserId { get; set; }
        public string ContactDetails { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsVisible { get; set; }
        public bool IsAccepted { get; set; }
        public string Website { get; set; }
        public bool IsDecisionMaker { get; set; }


        //Event Tickets
        // Event Abstract upload , full research upload
    }
}
