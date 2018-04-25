using System;
using System.IO;

namespace cryptscript
{
    public class Interpreter
    {
        public static bool IsInteractive { get; set; }
        public static Identifier WalletAddr { get; set; } = new Identifier();
        public static bool StopExecution { get; set; } = false;
        public static string ErrorMsg { get; set; } = null;

        public static void RunConsoleInterpreter()
        {
            Lexer lexer = new Lexer();
            Parser parser = new Parser(new IdentifierGroup());

            Console.WriteLine("Type 'exit' to quit.");

            while(!StopExecution)
            {
                string prompt = !parser.doneParsing ? "... " : ">>> ";
                string input = Util.GetInput(prompt);

                if(input.Length > 0)
                {
                    IObject result = parser.Parse(new Line(lexer.Tokenize(input), -1), false);
                    if(result != null && ErrorMsg == null)
                    {
                        Console.WriteLine(result.Repr());
                    }
                }

                if(ErrorMsg != null)
                {
                    Util.WriteColor(ErrorMsg, ConsoleColor.Red);
                    ErrorMsg = null;
                }
            }
        }

        public static IdentifierGroup RunScriptFile(string filename)
        {
            Lexer lexer = new Lexer();
            IdentifierGroup ids = new IdentifierGroup();
            Parser parser = new Parser(ids);

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
                return null;
            }

            string[] lines = File.ReadAllLines(filepath);

            for(int i = 0; i < lines.Length; i++)
            {
                string code = lines[i].Trim();
                Line line = new Line(lexer.Tokenize(code), i + 1);
                if(line.Tokens.Count > 0)
                    parser.Parse(line, false);

                if(StopExecution) { break; }
            }

            if(ErrorMsg != null)
            {
                Util.WriteColor(ErrorMsg, ConsoleColor.Red);
                ErrorMsg = null;
            }

            Interpreter.StopExecution = false;

            return ids;
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
            string filename = null;
            if(args.Length > 0)
            {
                filename = args[0];
            }

            WalletAddr.Reference = new String("0x00=jU0UrZBkqPXfp8MsMoILSRylevQGaUmJRnpFbfUvcGs=7lvpCgtyWl0");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(delegate(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Interpreter.ThrowError(new Error(ErrorType.KeyboardInterrupt));
            });

            if(System.String.IsNullOrEmpty(filename))
            {
                IsInteractive = true;
                RunConsoleInterpreter();
            }
            else
            {
                IsInteractive = false;
                IdentifierGroup ids = RunScriptFile(filename);
                if(ids == null)
                {
                    Console.WriteLine("Cannot find the file specified.");
                }
            }
        }
    }
}