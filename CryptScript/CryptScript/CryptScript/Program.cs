using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptScript
{
    class Program
    {
        public static bool IsRunning { get; set; } = true;
        public static bool IsMining { get; set; } = false;

        static void Main(string[] args)
        {
            // Initialize the display
            Display.Update(DisplayMode.Main);

            while(IsRunning)
            {
                // Print the display
                Display.PrintLayout();
                Display.PrintMessages();

                // Parse user input into a command sequence
                Display.Input.Parse(Display.GetInput());
            }
        }
    }
}
