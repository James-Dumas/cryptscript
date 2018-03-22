using System.Collections.Generic;

namespace CryptScript
{
    public static class Display
    {
        public static UserInput Input { get; set; } = new UserInput();

        private static DisplayMode Mode { get; set; } = DisplayMode.Main;

        private static Dictionary<DisplayMode, List<List<Command>>> ModeCommands = new Dictionary<DisplayMode, List<List<Command>>>()
        {
            { DisplayMode.Main, new List<List<Command>> { Data.MainCommands } },
            { DisplayMode.Mining, new List<List<Command>> { Data.MainCommands, Data.MiningCommands } }
        };
        

        public static void Update(DisplayMode mode)
        {
            // Reset user input commands
            Input = new UserInput();
            if (ModeCommands.ContainsKey(mode))
            {
                foreach (List<Command> CommandList in ModeCommands[mode])
                {
                    Util.SubscribeCommands(CommandList, Input);
                }
            }
        }
    }

    public enum DisplayMode
    {
        Main,
        Mining
    }
}
