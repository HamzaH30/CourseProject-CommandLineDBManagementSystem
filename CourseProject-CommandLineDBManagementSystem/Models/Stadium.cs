using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class Stadium
    {
        // Self Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capactiy { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Navigation Properties
        public virtual ICollection<TeamStadium> TeamStadiums { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
    }
}
