using Dalamud.Interface.Internal;
using ImGuiNET;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Data {
    public class ItemSelector {
        private readonly List<ItemData> Data;
        public IDalamudTextureWrap Icon;

        private ItemData Selected = new() {
            Icon = 0,
            Name = "",
            Data = new Item( ( ActionIds )0 )
        };
        private ItemData SearchSelected = new() {
            Icon = 0,
            Name = "",
            Data = new Item( ( ActionIds )0 )
        };

        private string Text = "[NONE]";
        private string SearchText = "";

        private List<ItemData> _Searched = null;
        private List<ItemData> Searched => _Searched ?? Data;

        private readonly string Label;
        private readonly string Id;

        public ItemSelector( string label, string id, List<ItemData> data ) {
            Label = label;
            Id = id;
            Data = data;
        }

        public bool Draw() {
            var ret = false;
            if( ImGui.BeginCombo( Id, Text, ImGuiComboFlags.HeightLargest ) ) {
                var resetScroll = false;
                if( ImGui.InputText( $"Search{Id}", ref SearchText, 100 ) ) {
                    _Searched = SearchText.Length == 0 ? null : Data.Where( x => x.Name.Contains( SearchText, StringComparison.CurrentCultureIgnoreCase ) ).ToList();
                    resetScroll = true;
                }

                ImGui.BeginChild( $"Select{Id}", new Vector2( ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X, 200 ), true );

                DisplayVisible( Searched.Count, out var preItems, out var showItems, out var postItems, out var itemHeight );
                if( resetScroll ) { ImGui.SetScrollHereY(); };
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );

                var idx = 0;
                foreach( var item in Searched ) {
                    if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                    if( ImGui.Selectable( $"{item.Name}{Id}{item.Data.Id}", item.Data == SearchSelected.Data ) ) {
                        SearchSelected = item;
                        try {
                            Icon = Dalamud.TextureProvider.GetIcon( item.Icon > 0 ? item.Icon : 0 );
                        }
                        catch( Exception ) {
                            Icon = Dalamud.TextureProvider.GetIcon( 0 );
                        }
                    }
                    idx++;
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
                ImGui.EndChild();

                if( SearchSelected.Data.Id != 0 ) {
                    if( Icon != null ) {
                        ImGui.Image( Icon.ImGuiHandle, new Vector2( 24, 24 ) );
                        ImGui.SameLine();
                    }

                    if( ImGui.Button( "Select" + Id ) ) {
                        Selected = SearchSelected;
                        Text = Selected.Name;
                        ret = true;
                    }
                }

                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text( Label );

            return ret;
        }

        private static void DisplayVisible( int count, out int preItems, out int showItems, out int postItems, out float itemHeight ) {
            float childHeight = 200;
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = ( int )Math.Floor( scrollY / itemHeight );
            showItems = ( int )Math.Ceiling( childHeight / itemHeight );
            postItems = count - showItems - preItems;
        }

        public ItemData GetSelected() => Selected;

        public void Dispose() {
            Icon?.Dispose();
            Icon = null;
        }
    }
}
