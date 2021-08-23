using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FFXIVClientInterface.Client.UI.Misc;
using FFXIVClientInterface.Misc;

namespace FFXIVClientInterface.Client.UI {
    
    public unsafe class UiModule : StructWrapper<UiModuleStruct>, IDisposable {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* GetModuleDelegate(UiModuleStruct* @this);
        
        public static implicit operator UiModuleStruct*(UiModule module) => module.Data;
        public static explicit operator ulong(UiModule module) => (ulong) module.Data;
        public static explicit operator UiModule(UiModuleStruct* @struct) => new() { Data = @struct };
        public static explicit operator UiModule(void* ptr) => new() { Data = (UiModuleStruct*) ptr};
        

        private readonly Dictionary<Type, IStructWrapper> structWrappers = new();
        
        private T GetModuleSingleton<T>(IntPtr getterAddr) where T : IStructWrapper, new() {
            if (structWrappers.ContainsKey(typeof(T))) {
                if (structWrappers[typeof(T)].IsValid) {
                    return (T) structWrappers[typeof(T)];
                }
            }
            var getter = Marshal.GetDelegateForFunctionPointer<GetModuleDelegate>(getterAddr);
            var module = getter(this);
            if (module == null) return default;
            var wrapper = new T();
            wrapper.SetData(module);
            structWrappers.Add(typeof(T), wrapper);
            return wrapper;
        }
        
        public RaptureHotbarModule RaptureHotbarModule => GetModuleSingleton<RaptureHotbarModule>(Data->vtbl->GetRaptureHotbarModule);
        
        public override void Dispose() {
            foreach (var m in structWrappers) {
                m.Value.Dispose();
            }
        }
    }
    
    
    [StructLayout(LayoutKind.Explicit, Size = 0xDE750)]
    public unsafe struct UiModuleStruct {
        [FieldOffset(0x00000)] public UiModuleVtbl* vtbl;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x658)]
    public unsafe struct UiModuleVtbl {
        public void* vf0; // dtor
        public void* vf1;
        public void* vf2;
        public void* vf3;
        public void* vf4;
        public void* vf5;
        public void* vf6;
        public void* vf7;
        public void* vf8;
        public IntPtr GetRaptureShellModule;
        public void* vf10;
        public void* vf11;
        public IntPtr GetRaptureMacroModule;
        public IntPtr GetRaptureHotbarModule;
        public IntPtr GetRaptureGearsetModule;
        public void* vf15;
        public IntPtr GetItemOrderModule;
        public IntPtr GetItemFinderModule;
        public IntPtr GetConfigModule;
        public void* vf19;
        public void* vf20;
        public void* vf21;
        public void* vf22;
        public void* vf23;
        public void* vf24;
        public void* vf25;
        public void* vf26;
        public void* vf27;
        public void* vf28;
        public void* vf29;
        public void* vf30;
        public void* vf31;
        public void* vf32;
        public void* vf33;
        public IntPtr GetAgentModule;
    }
}
