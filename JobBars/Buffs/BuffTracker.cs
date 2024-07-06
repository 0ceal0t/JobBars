using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public class BuffTracker {
        public enum BuffState {
            None, // hidden
            Running, // bright, show countdown
            OffCD, // bright, no text
            OnCD_Hidden,
            OnCD_Visible // dark, show countdown
        }

        public readonly BuffConfig Config;

        private BuffState State = BuffState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;
        private float Percent;

        private BuffNode Node;

        public BuffState CurrentState => State;
        public uint Id => ( uint )Config.Icon;
        public bool Enabled => ( State == BuffState.Running || State == BuffState.OffCD || State == BuffState.OnCD_Visible );
        public bool Active => State == BuffState.Running;
        public bool Highlight => Active && Config.PartyListHighlight;
        public bool ShowPartyText => Config.ShowPartyText;
        public bool ApplyToTarget => Config.ApplyToTarget;
        public string Text => ( ( int )Math.Round( TimeLeft ) ).ToString();

        public BuffTracker( BuffConfig config ) {
            Config = config;
        }

        public void ProcessAction( Item action ) {
            if( Config.Triggers.Contains( action ) ) SetActive( action );
        }

        private void SetActive( Item trigger ) {
            State = BuffState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick( Dictionary<Item, Status> buffDict ) {
            if( State != BuffState.Running && AtkHelper.CheckForTriggers( buffDict, Config.Triggers, out var trigger ) ) SetActive( trigger );

            if( State == BuffState.Running ) {
                TimeLeft = AtkHelper.TimeLeft( Config.Duration, buffDict, LastActiveTrigger, LastActiveTime );
                if( TimeLeft <= 0 ) { // Buff over
                    Percent = 1f;
                    TimeLeft = 0;

                    State = Config.CD <= 0 ? BuffState.None : // doesn't have a cooldown, just make it invisible
                        Config.CD <= JobBars.Configuration.BuffDisplayTimer ? BuffState.OnCD_Visible : BuffState.OnCD_Hidden;
                }
                else { // Still running
                    Percent = 1.0f - ( float )( TimeLeft / Config.Duration );
                }
            }
            else if( State == BuffState.OnCD_Hidden || State == BuffState.OnCD_Visible ) {
                TimeLeft = ( float )( Config.CD - ( DateTime.Now - LastActiveTime ).TotalSeconds );

                if( TimeLeft <= 0 ) {
                    State = BuffState.OffCD;
                }
                else if( TimeLeft <= JobBars.Configuration.BuffDisplayTimer ) { // CD low enough to be visible
                    State = BuffState.OnCD_Visible;
                    Percent = TimeLeft / Config.CD;
                }
            }
        }

        public void TickUi( BuffNode node ) {
            if( node == null ) {
                Dalamud.Log( "here" );
                return;
            }

            if( Node != node || Node?.IconId != Config.Icon ) {
                Node = node;
                SetupUI();
            }

            Node.IsVisible = true;
            Node.SetColor( Config.Color );

            if( State == BuffState.Running ) {
                Node.SetOffCd();
                Node.SetPercent( Percent );
                Node.SetText( Text );
            }
            else if( State == BuffState.OffCD ) {
                Node.SetOffCd();
                Node.SetPercent( 0 );
                Node.SetText( "" );
            }
            else if( State == BuffState.OnCD_Visible ) {
                Node.SetOnCd();
                Node.SetPercent( Percent );
                Node.SetText( Text );
            }
        }

        private void SetupUI() {
            Node.LoadIcon( Config.Icon );
        }

        public void Reset() {
            State = BuffState.None;
        }
    }
}
