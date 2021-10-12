using ImGuiNET;
using JobBars.Helper;

namespace JobBars.Data {
    public abstract class PerJobManagerNested<T> : PerJobManagerGeneric<T[]> where T : class {
        protected T SettingsItemSelected = null;

        public PerJobManagerNested(string id) : base(id) { }

        protected override void DrawLeftColumn() {
            foreach(var entry in JobToValue) {
                if (entry.Key == JobIds.OTHER) continue;

                var rowId = UIHelper.Localize(entry.Key) + _ID;
                if (ImGui.CollapsingHeader(rowId)) {
                    SelectedJob = entry.Key;
                    ImGui.Indent();

                    foreach(var item in entry.Value) {
                        var itemId = ItemToString(item) + _ID;
                        if(ImGui.Selectable(itemId, item == SettingsItemSelected)) {
                            SettingsItemSelected = item;
                        }
                    }

                    ImGui.Unindent();
                }
            }
        }

        protected override void DrawRightColumn() {
            if (SettingsItemSelected == null) {
                ImGui.Text("Select an item...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                DrawItem(SettingsItemSelected);
                ImGui.EndChild();
            }
        }

        protected abstract string ItemToString(T item);

        protected abstract void DrawItem(T item);
    }
}
