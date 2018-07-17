using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BrainfuckCompilerInterpreter
{
    unsafe class Program
    {
        //[MethodImpl(MethodImplOptions.NoOptimization)]
        static void Main(string[] args)
        {
            string output = "";
            var compiler = new Compiler();
            using (var compiled = compiler.Compile("-[----->+<]>--."))//,>,<++.>."))
            {
                byte[] arr = new byte[300000];
                Marshal.Copy(compiled.buf, arr, 0, 300000);
                File.WriteAllBytes(@"C:\projects\c#\BrainfuckCompiler\compiled.bin", arr);
                fixed (char* p = "123456")
                {
                    var szOutput = compiled.Invoke(p);
                    output = new String(szOutput);
                    Console.WriteLine("Result: " + output);
                }
            }
           
            //Console.WriteLine(result);
            Console.WriteLine("Done");
            return;

            if (args.Length != 1 && (args.Length != 3 || args.Length == 3 && args[1] != "-input"))
            {
                PrintHelp();
                return;
            }

            var code = args[0];
            var input = args.Length == 3 ? args[2] + "\n" : "";
            output = Compiler.Interpret(code, input);
            Console.WriteLine(output);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: brainfuck \"code\" [-input \"some input\"]");
        }
    }
}
