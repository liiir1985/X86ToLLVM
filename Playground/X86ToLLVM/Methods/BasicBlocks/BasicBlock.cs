using System;
using System.Collections.Generic;
using System.Text;

using AsmResolver;
using AsmResolver.X86;

using X86ToLLVM.Utils;
namespace X86ToLLVM.Methods
{
    class BasicBlock
    {
        public long StartAddress { get; private set; }
        public long EndAddress { get; private set; }
        List<X86Instruction> instructions = new List<X86Instruction>();
        List<BasicBlock> nextBlocks = new List<BasicBlock>();
        public List<X86Instruction> Instructions => instructions;
        public List<BasicBlock> NextBlocks => nextBlocks;
        
        X86Method method;
        bool initialized;
        public BasicBlock(X86Method method, long addr)
        {
            this.method = method;
            StartAddress = addr;
        }

        public void ParseInstruction()
        {
            if (!initialized)
            {
                initialized = true;
                X86Disassembler dis = method.Executable.CreateDisassemblerFromAddress(StartAddress);
                X86Instruction ins;
                bool shouldBreak = false;
                do
                {
                    ins = dis.ReadNextInstruction();
                    ProcessPendingAddress(ins);
                    instructions.Add(ins);
                }
                while (!ins.IsTerminator());
                ProcessNextBlocks(ins);
            }
        }

        void ProcessPendingAddress(X86Instruction ins)
        {
            switch (ins.Mnemonic)
            {
                case X86Mnemonic.Call:
                case X86Mnemonic.Call_Far:
                    method.Executable.RequestNewMethodAddress((long)(ulong)ins.Operand1.Value);
                    break;
            }
        }

        void ProcessNextBlocks(X86Instruction ins)
        {
            switch (ins.Mnemonic)
            {
                case X86Mnemonic.Jmp:
                    {
                        long target = (long)(ulong)ins.Operand1.Value;
                        var nb = method.GetOrCreateBlock(target);
                        nextBlocks.Add(nb);
                    }
                    break;
                case X86Mnemonic.Retn:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
