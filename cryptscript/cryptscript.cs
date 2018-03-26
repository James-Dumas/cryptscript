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

        public static void Start(string filename)
        {
            WalletAddr.Reference = new String("0x00=jU0UrZBkqPXfp8MsMoILSRylevQGaUmJRnpFbfUvcGs=7lvpCgtyWl0");

            IdentifierGroup globals = new IdentifierGroup();
            Parser parser = new Parser(globals);

            if(System.String.IsNullOrEmpty(filename))
            {
                // interactive console interpreter

                IsInteractive = true;

                Console.WriteLine("Type 'exit' to quit.");

                while(!StopExecution)
                {
                    string prompt = !parser.doneParsing ? "... " : ">>> ";
                    string input = Util.GetInput(prompt);

                    if(input.Length > 0)
                    {
                        Lexer lexer = new Lexer(input);
                        IObject result = parser.Parse(lexer.Tokenize(), false);
                        if(result != null && ErrorMsg == null)
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
                if(Path.IsPathRooted(filename))
                {
                    filepath = filename;
                }
                else
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                }

                if(!File.Exists(filepath))
                {
                    Console.WriteLine("Cannot find the file specified.");
                    return;
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

        public static void Main(string[] args)
        {
            string arg = null;
            if(args.Length > 0)
            {
                arg = args[0];
            }

            Start(arg);
        }
    }
}