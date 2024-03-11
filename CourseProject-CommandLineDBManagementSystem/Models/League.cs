using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class League
    {
        // Self Properties
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<LeagueTeam> LeagueTeams { get; set; }
    }
}
