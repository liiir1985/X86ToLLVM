using System;

using AsmResolver;
namespace X86ToLLVM
{
    public class Executables
    {
        WindowsAssembly asm;
        public void Load(string path)
        {
            asm = WindowsAssembly.FromFile(path);
            
        }
    }
}
