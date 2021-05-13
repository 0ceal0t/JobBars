using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public abstract unsafe class UIElement {
        public UIBuilder _UI;

        public UIElement(UIBuilder _ui) {
            _UI = _ui;
        }
        
        public void Setup(AtkResNode* node = null) {
            if (node == null) {
                Init();
            }
            else {
                LoadExisting(node);
            }
        }

        public AtkResNode* RootRes;
        public abstract void Init();
        public abstract void LoadExisting(AtkResNode* node);

        public void Hide() {
            UiHelper.Hide(RootRes);
        }

        public void Show() {
            UiHelper.Show(RootRes);
        }

        public abstract int GetHeight(int param);
        public abstract int GetWidth(int param);
    }
}
