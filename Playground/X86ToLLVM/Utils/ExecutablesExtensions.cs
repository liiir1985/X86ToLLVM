using System;
using System.Collections.Generic;
using System.Text;

using AsmResolver;
using AsmResolver.X86;

using X86ToLLVM.Methods;
namespace X86ToLLVM.Utils
{
    static class ExecutablesExtensions
    {
        public static X86Method CreateMethodFromRVA(this Executables exe, long rva)
        {
            return new X86Method(exe, exe.BaseAddress + rva);
        }

        public static X86Method CreateMethodFromAddress(this Executables exe, long address)
        {
            return new X86Method(exe, address);
        }

        public static X86Disassembler CreateDisassemblerFromRVA(this Executables exe, long rva)
        {
            var asm = exe.Assembly;
            var reader = asm.ReadingContext.Reader.CreateSubReader(asm.RvaToFileOffset(rva));
            var sec = asm.GetSectionHeaderByRva(rva);
            var dis = new AsmResolver.X86.X86Disassembler(reader, exe.BaseAddress + sec.VirtualAddress - sec.PointerToRawData);
            return dis;
        }

        public static X86Disassembler CreateDisassemblerFromAddress(this Executables exe, long address)
        {
            return CreateDisassemblerFromRVA(exe, address - exe.BaseAddress);
        }
    }
}
