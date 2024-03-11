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
        Menu MenuToReferTo { get; init; }

        public Exit(string selectionText, Menu menuToReferTo)
        {
            SelectionText = selectionText;
            MenuToReferTo = menuToReferTo;
        }

        public void PerformMenuAction()
        {
            Console.WriteLine("Exiting...");
            MenuToReferTo.ExitMenu();
        }
    }

}
