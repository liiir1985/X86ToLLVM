using System;
using System.Collections.Generic;
using System.Text;

using AsmResolver;
using AsmResolver.X86;

namespace X86ToLLVM.Methods
{
    class X86Method
    {
        Executables exe;
        long address;
        bool initialized = false;
        Dictionary<long, BasicBlock> basicBlocks = new Dictionary<long, BasicBlock>();
        BasicBlock entryBlock;
        public Dictionary<long, BasicBlock> BasicBlocks => basicBlocks;
        public Executables Executable => exe;
        public long Address => address;

        public X86Method(Executables exe, long address)
        {
            this.exe = exe;
            this.address = address;
        }

        public void Initialize()
        {
            if (!initialized)
            {
                entryBlock = new BasicBlock(this, address);
                basicBlocks[address] = entryBlock;
                entryBlock.ParseInstruction();
                initialized = true;
            }
        }

        public BasicBlock GetOrCreateBlock(long addr)
        {
            if (!basicBlocks.TryGetValue(addr, out var block))
            {
                block = new BasicBlock(this, addr);
                basicBlocks[addr] = block;
                block.ParseInstruction();                
            }
            return block;
        }
    }
}
