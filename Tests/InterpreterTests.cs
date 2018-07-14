﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BrainfuckCompilerInterpreter;

namespace Tests
{
    class InterpreterTests
    {
        [Test]
        public void OutputOne()
        {
            var output = Brainfuck.Interpret("-[----->+<]>--.");
            Assert.That(output, Is.EqualTo("1"));
        }

        [Test]
        public void OutputHelloWorld()
        {
            var output = Brainfuck.Interpret("++++++++++[>+>+++>+++++++>++++++++++<<<<-]>>>++.>+.+++++++..+++.<<++.>+++++++++++++++.>.+++.------.--------.<<+.<");
            Assert.That(output, Is.EqualTo("Hello World!"));
        }

        [Test]
        public void OutputInput()
        {
            var output = Brainfuck.Interpret(",.,.,.", "123");
            Assert.That(output, Is.EqualTo("123"));
        }


        [Test]
        public void OutputReverse()
        {
            var output = Brainfuck.Interpret("+[->,----------]<[+++++++++++.<]", "123\n");
            Assert.That(output, Is.EqualTo("321"));
        }
    }
}
