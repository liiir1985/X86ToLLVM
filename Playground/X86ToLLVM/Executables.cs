using System;

using AsmResolver;

using X86ToLLVM.Utils;
using X86ToLLVM.Methods;
namespace X86ToLLVM
{
    public class Executables
    {
        private WindowsAssembly asm;
        private X86Method entryMethod;
        public long BaseAddress { get; private set; }
        public WindowsAssembly Assembly => asm;
        internal X86Method EntryMethod => entryMethod;
        public void Load(string path, long baseAddr)
        {
            BaseAddress = baseAddr;
            asm = WindowsAssembly.FromFile(path);
            entryMethod = this.CreateMethodFromRVA(asm.NtHeaders.OptionalHeader.AddressOfEntrypoint);
            entryMethod.Initialize();
        }
    }
}
