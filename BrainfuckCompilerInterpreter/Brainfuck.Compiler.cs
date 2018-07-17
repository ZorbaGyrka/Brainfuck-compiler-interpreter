using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static BrainfuckCompilerInterpreter.Native;

namespace BrainfuckCompilerInterpreter
{
    public unsafe partial class Compiler
    {

        public static bool Is64BitProc()
        {
            return (IntPtr.Size == 8);
        }

        //[SuppressUnmanagedCodeSecurity]
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate char* CompiledFuncType(char* a);
        public FunctionWrapper Compile(string source)
        {
            List<byte> bytecode = new List<byte> {
0x55, //                      push   ebp
0x89, 0xe5, // mov    ebp,esp
0x89, 0xc8, //                   mov    eax,ecx
//0x8b, 0x45, 0x08, //             mov eax, DWORD PTR[ebp + 0x8]
//0xb8, 0x0a, 0x00, 0x00, 0x00,//          mov eax,0xa
           //0x89, 0xd0,//                   mov    eax,edx
// 0x0f, 0xb6, 0x00, //             movzx eax, BYTE PTR[eax]
//0x0f, 0xbe, 0xc0,    //            movsx eax, al
0xc9,// leave
0xc3,// ret



//0x55, //                      push ebp
//0x89, 0xe5,// mov    ebp,esp
 //           0x89, 0xd0,//                   mov    eax,edx
//0x67, 0x8b, 0x02, //mov    eax,DWORD PTR [edx] 

//0x67, 0x8b, 0x45, 0x04, //             mov eax, DWORD PTR[ebp + 0x8]
//0x67, 0x0f, 0xb6, 0x00, //             movzx eax, BYTE PTR[eax]
//0x0f, 0xbe, 0xc0,    //            movsx eax, al
            //0xb8, // MOV EAX,
            //42, 0, 0, 0, // 42


//0x89, 0xec,// mov    esp,ebp

//0x5d,//                      pop ebp
//0xc3 //ret
/*
                //0x67, 0x8b, 0x02, //                   mov    eax,DWORD PTR [edx]
                //0x89, 0x02, //                   mov    DWORD PTR [edx],eax
                //0x0F, 0xB7, 0x01,
            0xb8, // MOV EAX,
            0, 0, 0, 0, // 42
            //    0x8a, 0x02, //                   mov    al,BYTE PTR [edx]
            //0x83, 0xC0, 0x05, // add    eax,0x5 
            0xc3, // RET
*/
            };



            var buf = VirtualAlloc(IntPtr.Zero, (IntPtr)300000, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

            if (Is64BitProc()) // (Environment.Is64BitProcess)
            {
                bytecode = new List<byte> {
                    //0x55, //                      push   ebp
                    //0x89, 0xe5, // mov    ebp,esp
                    //0x89, 0xc8, //                   mov    eax,ecx
                    0x48, 0x89, 0xc8, //                mov    rax,rcx
                    //0xc9,// leave
                    0xc3,// ret
                };
            }
            else
            {
                // EDX = input
                // EAX = data pointer - memory
                // ECX = current output
                bytecode = new List<byte> {
                    0x55, //                      push   ebp
                    0x89, 0xe5, // mov    ebp,esp
                    //0x8b, 0x45, 0x08, //             mov eax, DWORD PTR[ebp + 0x8]
                    0x8b, 0x55, 0x08, //                mov edx, DWORD PTR [ebp+0x8]
                };
                var outputAddress = (int)buf + 100000;
                // mov ecx, outputAddress
                bytecode.Add(0xb9);
                bytecode.Add((byte)(outputAddress & 0xff));
                bytecode.Add((byte)((outputAddress >> 8) & 0xff));
                bytecode.Add((byte)((outputAddress >> 16) & 0xff));
                bytecode.Add((byte)((outputAddress >> 24) & 0xff));

                var memoryAddress = (int)buf + 200000;
                // mov eax, memoryAddress
                bytecode.Add(0xb8);
                bytecode.Add((byte)(memoryAddress & 0xff));
                bytecode.Add((byte)((memoryAddress >> 8) & 0xff));
                bytecode.Add((byte)((memoryAddress >> 16) & 0xff));
                bytecode.Add((byte)((memoryAddress >> 24) & 0xff));
                var jumpTable = BuildJumpTable(source);
                var sourcePositionToCompiledPosition = new List<int>();
                var compiledPositionToSourcePosition = new Dictionary<int, int>();
                var jumpsToFill = new List<int>();
                var ip = 0;
                while (ip < source.Length)
                {
                    var opcode = source[ip];
                    sourcePositionToCompiledPosition.Add(bytecode.Count);
                    compiledPositionToSourcePosition[bytecode.Count] = ip;
                    switch (opcode)
                    {
                        case '>':
                            bytecode.Add(0x40);       //                   inc    eax
                            break;
                        case '<':
                            bytecode.Add(0x48);       //                   dec    eax
                            break;
                        case '+': 
                            bytecode.AddRange(new List<byte> {
                                0xfe, 0x00, //                  inc BYTE PTR[eax]
                            });
                            break;
                        case '-':
                            bytecode.AddRange(new List<byte> {
                                0xfe, 0x08, //                   dec BYTE PTR[eax]
                            });
                            break;

                        case ',':
                            bytecode.AddRange(new List<byte> {
                                // read from input (EDX) and write to data pointer - memory (EAX)
                                0x8a, 0x1a, //                   mov    bl,BYTE PTR [edx]
                                0x42,       //                   inc    edx
                                0x42,       //                   inc    edx
                                0x88, 0x18, //                   mov    BYTE PTR [eax],bl
                    });
                            break;
                        case '.':
                            bytecode.AddRange(new List<byte> {
                                // read from data pointer - memory (EAX) and write to current output (ECX)
                                0x8a, 0x18,//                   mov    bl,BYTE PTR [eax]
                                0x88, 0x19,//                   mov    BYTE PTR [ecx],bl
                                0x41,      //                   inc    ecx
                                0x41,      //                   inc    ecx
                            });
                            break;
                        case '[':
                                jumpsToFill.Add(bytecode.Count);
                                bytecode.AddRange(new List<byte> {
                                    0x80, 0x38, 0x00, //                cmp    BYTE PTR [eax],0x0
                                    0x0f, 0x84, 0x74, 0x56, 0x34, 0x12, //      je     1234567d <_main+0x1234567d>
                                });
                            break;
                        case ']':
                            jumpsToFill.Add(bytecode.Count);
                            bytecode.AddRange(new List<byte> {
                                    0x80, 0x38, 0x00, //                cmp    BYTE PTR [eax],0x0
                                    0x0f, 0x85, 0x74, 0x56, 0x34, 0x12, //      jne     1234567d <_main+0x1234567d>
                                });
                            break;
                    }
                    ip++;
                }
                sourcePositionToCompiledPosition.Add(bytecode.Count);

                foreach (var jump in jumpsToFill)
                {
                    var sourcePosition = compiledPositionToSourcePosition[jump];
                    var jumpAddressInSource = jumpTable[sourcePosition];
                    var jumpAddressInCompiled = sourcePositionToCompiledPosition[jumpAddressInSource];
                    var relativeJumpOffset = jumpAddressInCompiled - jump - 9; // cmp + jne size
                    bytecode[jump + 5] = ((byte)(relativeJumpOffset & 0xff));
                    bytecode[jump + 6] = ((byte)((relativeJumpOffset >> 8) & 0xff));
                    bytecode[jump + 7] = ((byte)((relativeJumpOffset >> 16) & 0xff));
                    bytecode[jump + 8] = ((byte)((relativeJumpOffset >> 24) & 0xff));
                }
               
                // mov eax, outputAddress
                bytecode.Add(0xb8);
                bytecode.Add((byte)(outputAddress & 0xff));
                bytecode.Add((byte)((outputAddress >> 8) & 0xff));
                bytecode.Add((byte)((outputAddress >> 16) & 0xff));
                bytecode.Add((byte)((outputAddress >> 24) & 0xff));

                bytecode.AddRange(new List<byte> {
                    0xc9,// leave
                    0xc3,// ret
                });
            }

            Marshal.Copy(bytecode.ToArray(), 0, buf, bytecode.Count);
            var ptr = (CompiledFuncType)Marshal.GetDelegateForFunctionPointer(buf, typeof(CompiledFuncType));
            var bytePointer = (byte*)buf.ToPointer();
            var func = new FunctionWrapper { buf = buf, ptr = ptr, bytePointer = bytePointer };
            return func;
        }
    }
}
