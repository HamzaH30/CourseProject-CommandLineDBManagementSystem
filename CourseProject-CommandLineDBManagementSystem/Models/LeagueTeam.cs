using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    /// <summary>
    /// This is a join table between League and Team
    /// </summary>
    internal class LeagueTeam
    {
        // Self Properties
        public int LeaguePosition { get; set; }

        // Self & Foreign Properties
        public int LeagueId { get; set; }
        public int TeamId { get; set; }

        // Navigation Propertis
        public virtual Team Team { get; set; }
        public virtual League League { get; set; }
    }
}
