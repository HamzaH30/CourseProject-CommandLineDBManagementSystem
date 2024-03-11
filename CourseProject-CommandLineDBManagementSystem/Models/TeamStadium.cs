using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    /// <summary>
    /// This is a Join Table between Team and Stadium
    /// </summary>
    internal class TeamStadium
    {
        // Self & Foreign Properties
        public int TeamId { get; set; }
        public int StadiumId { get; set; }

        // Naviation Properties
        public virtual Team Team { get; set; }
        public virtual Stadium Stadium { get; set; }
    }
}
