using System;
using System.IO;

namespace cryptscript
{
    public class Interpreter
    {
        public static bool IsInteractive { get; set; }
        public static int LineNumber { get; set; } = 0;
        public static Identifier WalletAddr { get; set; } = new Identifier();
        public static bool StopExecution { get; set; } = false;
        public static string ErrorMsg { get; set; } = null;

        public static void Main(string[] args)
        {
            IdentifierGroup globals = new IdentifierGroup();
            Parser parser = new Parser(globals);

            if(args.Length == 0)
            {
                // interactive console interpreter

                IsInteractive = true;

                Console.WriteLine("Type 'exit' to quit.");

                while(!StopExecution)
                {
                    string prompt = parser.parsingLoop || parser.parsingFunc ? "... " : ">>> ";
                    string input = Util.GetInput(prompt);

                    if(input.Length > 0)
                    {
                        Lexer lexer = new Lexer(input);
                        IObject result = parser.Parse(lexer.Tokenize(), false);
                        if(result != null)
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }

                    if(ErrorMsg != null)
                    {
                        Util.WriteColor(ErrorMsg, ConsoleColor.Red);
                        ErrorMsg = null;
                    }
                }
            }
            else
            {
                // run script file

                IsInteractive = false;

                string filepath;
                if(Path.IsPathRooted(args[0]))
                {
                    filepath = args[0];
                }
                else
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
                }

                string[] lines = File.ReadAllLines(filepath);
                foreach(string line in lines)
                {
                    LineNumber++;
                    string code = line.Trim();
                    if(code.Length > 0)
                    {
                        Lexer lexer = new Lexer(line);
                        parser.Parse(lexer.Tokenize(), false);
                    }

                    if(StopExecution) { break; }
                }

                if(ErrorMsg != null)
                {
                    Util.WriteColor(ErrorMsg, ConsoleColor.Red);
                }
            }
        }

        public static void ThrowError(Error e)
        {
            ErrorMsg = (string) e.Value;
            if(!IsInteractive)
            {
                StopExecution = true;
            }
        }
    }
}