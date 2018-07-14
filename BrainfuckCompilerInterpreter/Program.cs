using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckCompilerInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 && (args.Length != 3 || args.Length == 3 && args[1] != "-input"))
            {
                PrintHelp();
                return;
            }

            var code = args[0];
            var input = args.Length == 3 ? args[2] + "\n" : "";
            var output = Brainfuck.Interpret(code, input);
            Console.WriteLine(output);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: brainfuck \"code\" [-input \"some input\"]");
        }
    }
}
