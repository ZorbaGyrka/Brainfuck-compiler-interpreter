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
            try
            {
                var output = CompileAndExecute("-[----->+<]>--.");
                Assert.That(output, Is.EqualTo("1"));
            }
            catch (DllNotFoundException e)
            {

            }
        }

        [Test]
        public void OutputHelloWorld()
        {
            try
            {
                var output = CompileAndExecute("++++++++++[>+>+++>+++++++>++++++++++<<<<-]>>>++.>+.+++++++..+++.<<++.>+++++++++++++++.>.+++.------.--------.<<+.<");
                Assert.That(output, Is.EqualTo("Hello World!"));
            }
            catch (DllNotFoundException e)
            {

            }
        }

        [Test]
        public void OutputInput()
        {
            try
            {
                var output = CompileAndExecute(",.,.,.", "123");
                Assert.That(output, Is.EqualTo("123"));
            }
            catch (DllNotFoundException e)
            {

            }
        }

        [Test]
        public void OutputReverse()
        {
            try
            {
                var output = CompileAndExecute("+[->,----------]<[+++++++++++.<]", "123\n");
                Assert.That(output, Is.EqualTo("321"));
            }
            catch (DllNotFoundException e)
            {

            }
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
