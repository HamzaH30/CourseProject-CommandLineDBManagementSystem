using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class ReadOption : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }
        public bool ContinueToReadMenu { get; private set; }

        public ReadOption(Menu menuInstance)
        {
            SelectionText = "Read entries of a Table";
            ContinueToReadMenu = false;
            MenuInstance = menuInstance;
        }

        public void PerformMenuAction()
        {
            ContinueToReadMenu = true;
            MenuInstance.ExitMenu();
        }
    }
}
