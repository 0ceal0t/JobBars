using System;
using FFXIVClientInterface.Client.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace FFXIVClientInterface {
    public unsafe class ClientInterface : IDisposable {
        
        private UiModule uiModule;
        public UiModule UiModule {
            get {
                if (uiModule != null && uiModule.IsValid) return uiModule;
                var fetchedUiModule = (UiModuleStruct*)Framework.Instance()->GetUiModule();
                if (fetchedUiModule != null) {
                    uiModule = new UiModule() { Data = fetchedUiModule };
                }
                return uiModule;
            }
        }

        public ClientInterface() {
        }

        public void Dispose() {
            uiModule?.Dispose();
        }
    }
}
