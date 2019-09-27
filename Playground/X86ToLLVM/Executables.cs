using System;
using System.Collections.Generic;

using AsmResolver;

using X86ToLLVM.Utils;
using X86ToLLVM.Methods;
namespace X86ToLLVM
{
    public class Executables
    {
        private WindowsAssembly asm;
        private X86Method entryMethod;
        HashSet<long> pendingAddresses = new HashSet<long>();
        Dictionary<long, X86Method> methods = new Dictionary<long, X86Method>();
        public long BaseAddress { get; private set; }
        public WindowsAssembly Assembly => asm;
        internal X86Method EntryMethod => entryMethod;
        public void Load(string path, long baseAddr)
        {
            BaseAddress = baseAddr;
            asm = WindowsAssembly.FromFile(path);
            entryMethod = this.CreateMethodFromRVA(asm.NtHeaders.OptionalHeader.AddressOfEntrypoint);
            methods[entryMethod.Address] = entryMethod;
            entryMethod.Initialize();

            while (pendingAddresses.Count > 0)
            {
                var emu = pendingAddresses.GetEnumerator();
                emu.MoveNext();
                long addr = emu.Current;
                var method = this.CreateMethodFromAddress(addr);
                methods[addr] = method;
                method.Initialize();
                pendingAddresses.Remove(addr);
            }
        }

        public void RequestNewMethodAddress(long address)
        {
            if (!methods.ContainsKey(address))
                pendingAddresses.Add(address);
        }
    }
}
