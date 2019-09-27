using System;

using X86ToLLVM;

namespace X86ToLLVMCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Executables exe = new Executables();
            exe.Load(args[0], 0x400000);
        }
    }
}
