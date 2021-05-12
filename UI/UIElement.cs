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
        public int Height;

        public UIElement(UIBuilder _ui, int height) {
            _UI = _ui;
            Height = height;
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
        public abstract void SetColor(ElementColor color);
        public abstract void Init();
        public abstract void LoadExisting(AtkResNode* node);

        public void Hide() {
            UiHelper.Hide(RootRes);
        }

        public void Show() {
            UiHelper.Show(RootRes);
        }
    }
}
