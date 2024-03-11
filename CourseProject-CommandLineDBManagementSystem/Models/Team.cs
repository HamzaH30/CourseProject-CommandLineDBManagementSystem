using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class Team
    {
        // Self Properties
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<LeagueTeam> LeagueTeams { get; set; }
        public virtual Manager Manager { get; set; }
        public virtual TeamStadium TeamStadium { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
    }
}
