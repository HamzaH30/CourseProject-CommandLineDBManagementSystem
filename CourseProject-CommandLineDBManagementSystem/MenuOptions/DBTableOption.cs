using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class DBTableOption : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }

        // Delegate (pointer to a function), specifically a function that takes in 1 string and returns nothing
        private Action<string> _callback; 

        public DBTableOption(string selectionText, Menu menuInstance, Action<string> callback)
        {
            SelectionText = selectionText;
            MenuInstance = menuInstance;
            _callback = callback;
        }

        public void PerformMenuAction()
        {
            MenuInstance.ExitMenu();

            // Invoke the callback function with the table name
            _callback(SelectionText);
        }
    }
}
