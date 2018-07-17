using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainfuckCompilerInterpreter;
using NUnit.Framework;

namespace Tests
{
    unsafe class CompilerTests
    {
        [Test]
        public void OutputOne()
        {
            var output = CompileAndExecute("-[----->+<]>--.");
            Assert.That(output, Is.EqualTo("1"));
        }

        [Test]
        public void OutputHelloWorld()
        {
            var output = CompileAndExecute("++++++++++[>+>+++>+++++++>++++++++++<<<<-]>>>++.>+.+++++++..+++.<<++.>+++++++++++++++.>.+++.------.--------.<<+.<");
            Assert.That(output, Is.EqualTo("Hello World!"));
        }
        
        [Test]
        public void OutputInput()
        {
            var output = CompileAndExecute(",.,.,.", "123");
            Assert.That(output, Is.EqualTo("123"));
        }

        [Test]
        public void OutputReverse()
        {
            var output = CompileAndExecute("+[->,----------]<[+++++++++++.<]", "123\n");
            Assert.That(output, Is.EqualTo("321"));
        }

        public string CompileAndExecute(string source, string input = "")
        {
            string output = "";
            var compiler = new Compiler();
            using (var compiled = compiler.Compile(source))
            {
                fixed (char* p = input)
                {
                    var szOutput = compiled.Invoke(p);
                    output = new String(szOutput);
                    return output;
                }
            }
        }
    }
}
