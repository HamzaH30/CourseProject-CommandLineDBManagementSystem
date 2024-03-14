using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuClasses
{
    public class Exit : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }

        public Exit(string selectionText, Menu menuToReferTo)
        {
            SelectionText = selectionText;
            MenuInstance = menuToReferTo;
        }

        public void PerformMenuAction()
        {
            Console.WriteLine("Exiting...");
            MenuInstance.ExitMenu();
        }
    }

}
