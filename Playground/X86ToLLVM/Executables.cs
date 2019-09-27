using System;

using AsmResolver;

using X86ToLLVM.Utils;
namespace X86ToLLVM
{
    public class Executables
    {
        private WindowsAssembly asm;
        public long BaseAddress { get; private set; }
        public WindowsAssembly Assembly => asm;
        public void Load(string path, long baseAddr)
        {
            BaseAddress = baseAddr;
            asm = WindowsAssembly.FromFile(path);
            X86ToLLVM.Methods.X86Method startMethod = this.CreateMethodFromRVA(asm.NtHeaders.OptionalHeader.AddressOfEntrypoint);
        }
    }
}
