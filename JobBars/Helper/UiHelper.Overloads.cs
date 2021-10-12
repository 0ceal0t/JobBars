using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public static unsafe partial class UIHelper {
        public static void Hide<T>(T* node) where T : unmanaged => Hide((AtkResNode*)node);
        public static void Show<T>(T* node) where T : unmanaged => Show((AtkResNode*)node);
        public static void SetVisibility<T>(T* node, bool visibility) where T : unmanaged => SetVisibility((AtkResNode*)node, visibility);
        public static void SetScale<T>(T* node, float? scaleX, float? scaleY) where T : unmanaged => SetScale((AtkResNode*)node, scaleX, scaleY);
        public static void SetSize<T>(T* node, int? w, int? h) where T : unmanaged => SetSize((AtkResNode*)node, w, h);
        public static void SetPosition<T>(T* node, float? x, float? y) where T : unmanaged => SetPosition((AtkResNode*)node, x, y);

        public static AtkTextNode* CloneNode(AtkTextNode* node) => (AtkTextNode*)CloneNode((AtkResNode*)node);
    }
}
