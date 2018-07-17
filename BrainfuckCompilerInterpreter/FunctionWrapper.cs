using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrainfuckCompilerInterpreter.Compiler;
using static BrainfuckCompilerInterpreter.Native;

namespace BrainfuckCompilerInterpreter
{
    public unsafe class FunctionWrapper : IDisposable
    {
        internal IntPtr buf;
        internal byte* bytePointer;
        internal CompiledFuncType ptr;
        protected virtual void Dispose(bool disposing)
        {
            VirtualFree(buf, UIntPtr.Zero, MemoryFreeType.MEM_RELEASE);
            if (disposing)
            {
                // release other disposable objects  
                // if (resource != null) resource.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public char* Invoke(char* input)
        {
            return ptr(input);
        }
    }
}
