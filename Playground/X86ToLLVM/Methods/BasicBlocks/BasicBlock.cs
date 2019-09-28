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
        List<BasicBlock> prevBlocks = new List<BasicBlock>();
        public List<X86Instruction> Instructions => instructions;
        public List<BasicBlock> NextBlocks => nextBlocks;
        public List<BasicBlock> PreviousBlocks => prevBlocks;
        
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
                EndAddress = ins.Offset;
                ProcessNextBlocks(ins);
            }
        }

        public bool IsAddressInsideScope(long addr)
        {
            return addr >= StartAddress && addr <= EndAddress;
        }

        public BasicBlock SplitAtAddress(long addr)
        {
            int idx = GetIndexOfAddress(addr);
            BasicBlock res = new BasicBlock(method, addr);
            for(int i = idx; i < instructions.Count; i++)
            {
                res.instructions.Add(instructions[i]);
            }
            instructions.RemoveRange(idx, instructions.Count - idx);
            nextBlocks.Add(res);
            res.prevBlocks.Add(this);
            res.initialized = true;
            return res;
        }

        int GetIndexOfAddress(long addr)
        {
            for(int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Offset == addr)
                    return i;
            }
            return -1;
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
            long target = 0;
            BasicBlock nb = null;
            switch (ins.Mnemonic)
            {
                case X86Mnemonic.Jmp:
                    {
                        target = (long)(ulong)ins.Operand1.Value;
                        nb = method.GetOrCreateBlock(target);
                        nextBlocks.Add(nb);
                        nb.prevBlocks.Add(this);
                    }
                    break;
                case X86Mnemonic.Retn:
                    break;
                case X86Mnemonic.Je:
                case X86Mnemonic.Jne:
                    {
                        target = (long)(ulong)ins.Operand1.Value;
                        nb = method.GetOrCreateBlock(target);
                        nextBlocks.Add(nb);
                        nb.prevBlocks.Add(this);

                        target = ins.Offset + ins.ComputeSize();
                        nb = method.GetOrCreateBlock(target);
                        nextBlocks.Add(nb);
                        nb.prevBlocks.Add(this);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
