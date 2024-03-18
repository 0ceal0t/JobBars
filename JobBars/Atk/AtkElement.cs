using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Atk {
    public abstract unsafe class AtkElement {
        public AtkResNode* RootRes;

        public void SetVisible( bool value ) {
            if( value ) Show();
            else Hide();
        }

        public virtual void Hide() {
            AtkHelper.Hide( RootRes );
        }

        public virtual void Show() {
            AtkHelper.Show( RootRes );
        }

        public virtual void SetPosition( Vector2 pos ) {
            AtkHelper.SetPosition( RootRes, pos.X, pos.Y );
        }

        public virtual void SetScale( float scale ) {
            AtkHelper.SetScale( RootRes, scale, scale );
        }

        public abstract void Dispose();
    }
}
