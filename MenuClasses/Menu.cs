using System;
using System.Collections.Generic;

namespace MenuClasses
{
    public class Menu
    {
        private List<IMenuItem> _menuItems = new();
        private string _welcomeText { get; init; }
        public string Prompt { get; init; }
        public bool IsMenuCurrentlyBeingUsed { get; set; }
        public bool UserRequestedExit { get; private set; }


        public Menu(string welcomeText, string prompt)
        {
            IsMenuCurrentlyBeingUsed = true;
            _welcomeText = welcomeText;
            Prompt = prompt;
            UserRequestedExit = false;

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
                string userInput = Console.ReadLine();
                HandleUserInput(userInput);
            }
        }

        public void HandleUserInput(string input)
        {
            if (int.TryParse(input, out int inputInt) && inputInt > 0 && inputInt <= _menuItems.Count)
            {
                if (inputInt == _menuItems.Count)
                {
                    // User wants to exit the menu
                    inputInt = 0;
                    UserRequestedExit = true;
                }

                SelectMenuElement(_menuItems[inputInt]);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number corresponding to the menu options.");
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
