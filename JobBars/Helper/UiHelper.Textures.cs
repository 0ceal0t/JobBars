using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public unsafe partial class UiHelper {
        public static void UpdatePart( AtkUldPartsList* partsList, int partIdx, ushort u, ushort v, ushort width, ushort height ) {
            partsList->Parts[partIdx].U = u;
            partsList->Parts[partIdx].V = v;
            partsList->Parts[partIdx].Width = width;
            partsList->Parts[partIdx].Height = height;
        }
    }
}
