using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class Manager
    {
        // Self Properties
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }

        // Foreign Properties
        public int TeamId { get; set; }
        
        // Navigation properties
        public virtual Team Team { get; set; }
    }
}
