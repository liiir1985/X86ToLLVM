using System;
using System.Collections.Generic;
using System.Text;

using AsmResolver;
using AsmResolver.X86;

namespace X86ToLLVM.Methods
{
    class X86Method
    {
        X86Disassembler dis;
        Executables exe;
        long address;
        public X86Method(Executables exe, X86Disassembler dis, long address)
        {
            this.exe = exe;
            this.dis = dis;
            this.address = address;


            var ins = dis.ReadNextInstruction();
        }
    }
}
