using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class EditablePropertyOption : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }

        /*
         * Delegate (pointer to a function), specifically a function that takes and returns nothing
         * Callback action with no parameters needed because the context (the property info) is captured when the instance is created.
         * The callback is expected to encapsulate all necessary context including which property it operates on
         */
        private Action _callback;

        public EditablePropertyOption(Menu menuInstance, string selectionText, Action callback)
        {
            SelectionText = selectionText;
            MenuInstance = menuInstance;
            _callback = callback;
        }

        public void PerformMenuAction()
        {
            MenuInstance.ExitMenu();
            _callback();
        }
    }
}
