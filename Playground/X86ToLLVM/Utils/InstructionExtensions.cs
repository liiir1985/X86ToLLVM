using System;
using System.Collections.Generic;
using System.Text;

using AsmResolver;
using AsmResolver.X86;

using X86ToLLVM.Methods;
namespace X86ToLLVM.Utils
{
    static class InstructionExtensions
    {
        public static bool IsTerminator(this X86Instruction ins)
        {
            switch(ins.Mnemonic)
            {
                case X86Mnemonic.Jmp:
                case X86Mnemonic.Jmp_Far:
                case X86Mnemonic.Ja:
                case X86Mnemonic.Jb:
                case X86Mnemonic.Jbe:
                case X86Mnemonic.Je:
                case X86Mnemonic.Jecxz:
                case X86Mnemonic.Jg:
                case X86Mnemonic.Jge:
                case X86Mnemonic.Jl:
                case X86Mnemonic.Jle:
                case X86Mnemonic.Jnb:
                case X86Mnemonic.Jne:
                case X86Mnemonic.Jno:
                case X86Mnemonic.Jns:
                case X86Mnemonic.Jo:
                case X86Mnemonic.Jpe:
                case X86Mnemonic.Jpo:
                case X86Mnemonic.Js:
                case X86Mnemonic.Retn:
                case X86Mnemonic.Retf:
                    return true;
                default:
                    return false;
            }
        }
    }
}
