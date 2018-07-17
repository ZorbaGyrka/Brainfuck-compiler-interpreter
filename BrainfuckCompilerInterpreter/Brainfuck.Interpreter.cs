using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrainfuckCompilerInterpreter
{
    public partial class Compiler
    {
        /*
			> 	increment the data pointer (to point to the next cell to the right).
			< 	decrement the data pointer (to point to the next cell to the left).
			+ 	increment (increase by one) the byte at the data pointer.
			- 	decrement (decrease by one) the byte at the data pointer.
			. 	output the byte at the data pointer.
			, 	accept one byte of input, storing its value in the byte at the data pointer.
			[ 	if the byte at the data pointer is zero, then instead of moving the instruction pointer forward to the next command, jump it forward to the command after the matching ] command.
			] 	if the byte at the data pointer is nonzero, then instead of moving the instruction pointer forward to the next command, jump it back to the command after the matching [ command. 
		*/
        public static string Interpret(string code, string input = "")
        {
            var ip = 0;
            var dp = 0;
            var inputPosition = 0;
            var memory = new byte[100000];
            var output = new StringBuilder();
            var jumpTable = BuildJumpTable(code);
            while (ip < code.Length)
            {
                var opcode = code[ip];
                switch (opcode)
                {
                    case '>': dp++; break;
                    case '<': dp--; break;
                    case '+': memory[dp]++; break;
                    case '-': memory[dp]--; break;
                    case '.': output.Append((char)memory[dp]); break;
                    case ',':
                        if (inputPosition < input.Length)
                            memory[dp] = (byte)input[inputPosition++];
                        else
                            memory[dp] = 0;
                        break;
                    case '[': if (memory[dp] == 0) ip = jumpTable[ip] - 1; break;
                    case ']': if (memory[dp] != 0) ip = jumpTable[ip] - 1; break;
                    default: 
						// ignore unknown opcodes. Comments are put in code
                        break;
                }
                ip++;
            }
            return output.ToString();
        }

        static Dictionary<int, int> BuildJumpTable(string code)
        {
            var jumpTable = new Dictionary<int, int>();
            var jumpForwardStack = new Stack<int>();
            for (var i = 0; i < code.Length; i++)
            {
                if (code[i] == '[') jumpForwardStack.Push(i);
                if (code[i] != ']') continue;
                if (jumpForwardStack.Count == 0)
                    throw new Exception("Jump backwards ']' at position: " + i + " without matching jump forward instruction '['");
                var jumpForwardPosition = jumpForwardStack.Pop();
                jumpTable[i] = jumpForwardPosition + 1;
                jumpTable[jumpForwardPosition] = i + 1;
            }
            if (jumpForwardStack.Count != 0)
                throw new Exception("Jump forward command at position: " + jumpForwardStack.Peek() + " is not closed by matching ']'");
            return jumpTable;
        }
    }
}
