using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public static unsafe partial class UiHelper {
        public static void Hide<T>( T* node ) where T : unmanaged => Hide( ( AtkResNode* )node );
        public static void Show<T>( T* node ) where T : unmanaged => Show( ( AtkResNode* )node );
        public static void SetVisibility<T>( T* node, bool visibility ) where T : unmanaged => SetVisibility( ( AtkResNode* )node, visibility );
    }
}
