using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class UpdateOption : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }
        public bool ContinueToUpdateMenu { get; private set; }

        public UpdateOption(Menu menuInstance)
        {
            SelectionText = "Update the entries of a Table";
            ContinueToUpdateMenu = false;
            MenuInstance = menuInstance;
        }

        public void PerformMenuAction()
        {
            ContinueToUpdateMenu = true;
            MenuInstance.ExitMenu();
        }
    }
}
