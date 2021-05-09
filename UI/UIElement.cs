using FFXIVClientStructs.FFXIV.Component.GUI;
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

        public UIElement(UIBuilder _ui, int height, AtkResNode* node = null) {
            _UI = _ui;
            Height = height;
            if (node == null) {
                Init();
            }
            else {
                Pickup(node);
            }
        }

        public AtkResNode* RootRes;
        public abstract void SetColor(ElementColor color);
        public abstract void Init();
        public abstract void Pickup(AtkResNode* node);
    }
}
