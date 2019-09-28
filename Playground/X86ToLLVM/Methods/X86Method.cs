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
        Dictionary<long, BasicBlock> basicBlockMapping = new Dictionary<long, BasicBlock>();
        List<BasicBlock> basicBlocks = new List<BasicBlock>();
        BasicBlock entryBlock;
        public List<BasicBlock> BasicBlocks => basicBlocks;
        public BasicBlock EntryBlock => entryBlock;
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
                basicBlockMapping[address] = entryBlock;
                basicBlocks.Add(entryBlock);
                entryBlock.ParseInstruction();
                initialized = true;
            }
        }

        public BasicBlock GetOrCreateBlock(long addr)
        {
            if (!basicBlockMapping.TryGetValue(addr, out var block))
            {
                bool found = false;
                foreach(var i in basicBlocks)
                {
                    if (i.IsAddressInsideScope(addr))
                    {
                        block = i.SplitAtAddress(addr);
                        basicBlockMapping[addr] = block;
                        basicBlocks.Add(block);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    block = new BasicBlock(this, addr);
                    basicBlockMapping[addr] = block;
                    basicBlocks.Add(block);
                    block.ParseInstruction();
                }
            }
            return block;
        }
    }
}
