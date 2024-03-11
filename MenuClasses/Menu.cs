using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuClasses
{
    public class Menu
    {
        private List<IMenuItem> _menuItems = new();
        private string _welcomeText { get; init; }
        public string Prompt { get; init; }

        public bool IsMenuCurrentlyBeingUsed { get; set; }

        public Menu(string welcomeText, string prompt)
        {
            IsMenuCurrentlyBeingUsed = true;
            _welcomeText = welcomeText;
            Prompt = prompt;

            // Adding an exit menu option
            _menuItems.Add(new Exit("Exit Menu", this));
        }

        public void ExitMenu()
        {
            IsMenuCurrentlyBeingUsed = false;
        }

        public void Display()
        {
            while (IsMenuCurrentlyBeingUsed)
            {
                Console.WriteLine(_welcomeText);
                Console.WriteLine(Prompt);

                for (int i = 1; i < _menuItems.Count; i++)
                {
                    Console.WriteLine($"{i}: {_menuItems[i].SelectionText}");
                }

                // Printing out the exit menu option
                Console.WriteLine($"{_menuItems.Count}: {_menuItems.First().SelectionText}");

                Console.Write("Option: ");
                HandleUserInput(Console.ReadLine());
            }
        }

        public void HandleUserInput(string input)
        {
            if (int.TryParse(input, out int inputInt))
            {
                if (inputInt == _menuItems.Count)
                {
                    // User wants to exit the menu
                    inputInt = 0;
                }

                SelectMenuElement(_menuItems[inputInt]);
            }
            else
            {
                throw new Exception("Please enter a number");
            }
        }

        private void SelectMenuElement(IMenuItem item)
        {
            item.PerformMenuAction();
            Console.WriteLine();
        }

        public void AddToMenu(IMenuItem item)
        {
            _menuItems.Add(item);
        }
    }
}
