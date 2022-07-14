using Dalamud.Logging;
using ImGuiNET;
using JobBars.Helper;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Data {
    public class ItemSelector {
        private readonly List<ItemData> Data;
        public ImGuiScene.TextureWrap Icon;

        private ItemData Selected = new() {
            Icon = 0,
            Name = "",
            Data = new Item((ActionIds)0)
        };
        private ItemData SearchSelected = new() {
            Icon = 0,
            Name = "",
            Data = new Item((ActionIds)0)
        };

        private string Text = "[NONE]";
        private string SearchText = "";

        private List<ItemData> _Searched = null;
        private List<ItemData> Searched => _Searched ?? Data;

        private readonly string Label;
        private readonly string Id;

        public ItemSelector(string label, string id, List<ItemData> data) {
            Label = label;
            Id = id;
            Data = data;
        }

        public bool Draw() {
            var ret = false;
            if (ImGui.BeginCombo(Id, Text, ImGuiComboFlags.HeightLargest)) {
                var resetScroll = false;
                if (ImGui.InputText($"Search{Id}", ref SearchText, 100)) {
                    _Searched = SearchText.Length == 0 ? null : Data.Where(x => x.Name.ToLower().Contains(SearchText.ToLower())).ToList();
                    resetScroll = true;
                }

                ImGui.BeginChild($"Select{Id}", new Vector2(ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X, 200), true);

                DisplayVisible(Searched.Count, out int preItems, out int showItems, out int postItems, out float itemHeight);
                if (resetScroll) { ImGui.SetScrollHereY(); };
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + preItems * itemHeight);

                int idx = 0;
                foreach (var item in Searched) {
                    if (idx < preItems || idx > (preItems + showItems)) { idx++; continue; }
                    if (ImGui.Selectable($"{item.Name}{Id}{item.Data.Id}", item.Data == SearchSelected.Data)) {
                        SearchSelected = item;
                        LoadIcon(item.Icon);
                    }
                    idx++;
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + postItems * itemHeight);
                ImGui.EndChild();

                if (SearchSelected.Data.Id != 0) {
                    if (Icon != null) {
                        ImGui.Image(Icon.ImGuiHandle, new Vector2(24, 24));
                        ImGui.SameLine();
                    }

                    if (ImGui.Button("Select" + Id)) {
                        Selected = SearchSelected;
                        Text = Selected.Name;
                        ret = true;
                    }
                }

                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text(Label);

            return ret;
        }

        private void LoadIcon(ushort iconId) {
            Icon?.Dispose();
            Icon = null;
            if (iconId > 0) {
                TexFile tex;
                try {
                    tex = JobBars.DataManager.GetIcon(iconId);
                }
                catch (Exception) {
                    tex = JobBars.DataManager.GetIcon(0);
                }
                Icon = JobBars.PluginInterface.UiBuilder.LoadImageRaw(BGRA_to_RGBA(tex.ImageData), tex.Header.Width, tex.Header.Height, 4);
            }
        }

        private static byte[] BGRA_to_RGBA(byte[] data) {
            byte[] ret = new byte[data.Length];
            for (int i = 0; i < data.Length / 4; i++) {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }

        private static void DisplayVisible(int count, out int preItems, out int showItems, out int postItems, out float itemHeight) {
            float childHeight = 200;
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = (int)Math.Floor(scrollY / itemHeight);
            showItems = (int)Math.Ceiling(childHeight / itemHeight);
            postItems = count - showItems - preItems;
        }

        public ItemData GetSelected() => Selected;

        public void Dispose() {
            Icon?.Dispose();
            Icon = null;
        }
    }
}
