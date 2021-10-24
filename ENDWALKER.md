# Endwalker

## Checklist
- [ ] [Sigs](https://github.com/0ceal0t/JobBars/blob/main/JobBars/JobBars.cs)
	- Double-check arguments as well
- [ ] [Helper Sigs](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.GameFunctions.cs#L22)
- [ ] [Previous Enemy Offset](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.GameFunctions.cs#L97)
- [ ] [Party Member Size](https://github.com/0ceal0t/JobBars/blob/main/JobBars/JobBars.Party.cs#L47)
	- Shouldn't change
- [ ] [Effect Header Size](https://github.com/0ceal0t/JobBars/blob/main/JobBars/JobBars.Hooks.cs#L17)
- [ ] [Actor Tick](https://github.com/0ceal0t/JobBars/blob/main/JobBars/JobBars.Hooks.cs#L101)
- [ ] [Wipe Arg](https://github.com/0ceal0t/JobBars/blob/main/JobBars/JobBars.Hooks.cs#L108)
- [ ] [Cursor Dragging](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Cursors/CursorManager.cs#L51)
- [ ] [Action Ids](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Data/Ids.Action.cs)
- [ ] [Buff Ids](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Data/Ids.Buffs.cs)
- [ ] [Job Ids](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Data/Ids.Jobs.cs)
- [ ] [Effect Enum](https://github.com/0ceal0t/JobBars/blob/main/JobBars/GameStructs/ActionEffectStructs.cs)
- [ ] [Action Bar Struct](https://github.com/0ceal0t/JobBars/blob/main/JobBars/GameStructs/AddonActionBarBase.cs)
- [ ] [Hotbar Data Struct](https://github.com/0ceal0t/JobBars/blob/main/JobBars/GameStructs/AddonHotbarNumberArray.cs)
- [ ] [Party List Struct](https://github.com/0ceal0t/JobBars/blob/main/JobBars/GameStructs/AddonPartyListNumberArray.cs)
- [ ] [Party List and Hotbar Data](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UIHelper.UIModule.cs#L9)
- [ ] [Spell or Weaponskill](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L137)
- [ ] [Cooldown Group](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L138)
- [ ] [Blank Icon Id](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L138)
- [ ] [Texture Load Size](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Textures.cs#L96)
- [ ] [Icon Nodes](https://github.com/0ceal0t/JobBars/blob/main/JobBars/UI/Icon/UIIconBuff.cs#L17)
	- [Timer](https://github.com/0ceal0t/JobBars/blob/main/JobBars/UI/Icon/UIIconTimer.cs#L24)
- [ ] Party list nodes

## Jobs

### Dark Knight
- [ ] (**NEW**) Gauge - Delirium stacks
- [ ] Gauge - GCDs in Blood Weapon
- [ ] Gauge - MP
- [ ] Buffs - Delirium
- [ ] Buffs - Living Shadow
- [ ] Mitigation - Living Dead
- [ ] Mitigation - Reprisal
- [ ] Mitigation - Dark Missionary
- [ ] Mitigation - The Blackest Night
- [ ] Icon - Blood Weapon

### Warrior
- [ ] (**NEW**) Guage - IR Stacks
- [ ] (**NEW**) Guage - Surging Tempest
- [ ] (**NEW**) Buffs - IR (30 seconds)
- [ ] (**NEW**) Mitigation - Holmgang (10 seconds)
- [ ] Mitigation - Reprisal
- [ ] Mitigation - Shake it Off
- [ ] (**NEW**) Mitigation - Nascent Flash / Raw Intuition / Bloodwhetting
- [ ] (**NEW**) Icon - IR (30 seconds max)
- [ ] (**NEW**) Icon - Surging Tempest

### Paladin
- [ ] (**NEW**) Gauge - Req Stacks
- [ ] Gauge - Atonement Stacks
- [ ] Gauge - GCDs in Fight or Flight
- [ ] (**NEW**) Guage - Goring Blade / Blade of Valor
- [ ] (**NEW**) Buffs - Req
- [ ] Mitigation - Hallowed Ground
- [ ] Mitigation - Reprisal
- [ ] Mitigation - Divine Veil
- [ ] Mitigation - Passage or Arms
- [ ] (**NEW**) Icon - Goring Blade / Blade of Valor (only display on Goring)
- [ ] Icon - Fight or Flight

### Gunbreaker
- [ ] Gauge - GCDs in No Mercy
- [ ] (**NEW**) Mitigation - Superbolide (10 seconds)
- [ ] Mitigation - Reprisal
- [ ] Mitigation - Heart of Light
- [ ] (**NEW**) Mitigation - Heart of Corundum / Hearth of Stone (25s)
- [ ] Icon - No Mercy

### Scholar
- [ ] Gauge - Excog
- [ ] Gauge - Biolysis
- [ ] Buffs - Chain
- [ ] Mitigation - Seraph
- [ ] Mitigation - Recitation
- [ ] (**NEW**) Mitigation - Expedient (20s, 120s CD)
- [ ] (**NEW**) Mitigation - Protraction (10s, 60s CD)
- [ ] Mitigation - Swiftcast
- [ ] Icon - Biolysis
- [ ] Icon - Chain

### White Mage
- [ ] Gauge - Dia
- [ ] (**NEW**) Gauge - Stacks of Lilybell
- [ ] Mitigation - Temperance
- [ ] Mitigation - Benediction
- [ ] (**NEW**) Mitigation - Lilybell (15s duration, 180s CD) (**NOTE: this might change**)
- [ ] Mitigation - Swiftcast
- [ ] Icon - Dia
- [ ] Icon - Prescence of Mind

### Astrologian
- [ ] Gauge - Combust
- [ ] Gauge - Upgraded Earthly Star
- [ ] (**NEW**) Gauge - Astrodyne / Harmony of Spirit time (15 second)
- [ ] (**NEW**) Buffs - Cards (no Lord/Lady)
- [ ] Buffs - Divination
- [ ] Mitigation - Neutral Sect
- [ ] Mitigation - Earthly Star
- [ ] (**NEW**) Mitigation - Macrocosmos (15s duration, 120s CD)
- [ ] Mitigation - Swiftcast
- [ ] Icon - Combust
- [ ] Icon - Lightspeed
- [ ] Icon - Earthly Star
- [ ] (**NEW**) Icon - Astrodyne / Harmony of Spriti (15 seconds)

### Sage
- [ ] (**NEW**) Gauge - Eukrasian Dosis I/II/III (30s)
- [ ] (**NEW**) Mitigation - Zoe (30s, 90s CD)
- [ ] (**NEW**) Mitigation - Panhaima (30s, 120s CD)
- [ ] (**NEW**) Mitigation - Pneuma (20s, 120s CD)
- [ ] (**NEW**) Mitigation - Swiftcast
- [ ] (**NEW**) Icon - Eukrasian Dosis I/II/III (30s)

### Monk
- [ ] Gauge - Twin Snakes
- [ ] Gauge - Demolish
- [ ] Gauge - Riddle of Earth / True North
- [ ] (**NEW**) Buffs - Brotherhood (120s)
- [ ] (**NEW**) Buffs - Riddle of Fire (60s)
- [ ] Mitigation - Feint
- [ ] Mitigation - Mantra
- [ ] Icon - Twin Snakes
- [ ] Icon - Demolish
- [ ] Icon - Riddle of Fire
- [ ] (**NEW**) Icon - Brotherhood

### Dragoon
- [ ] Gauge - GCDs in Lance Charge
- [ ] Gauge - GCDs in Dragon Sight
- [ ] (**NEW**) Buffs - Battle Litany (15s, 120s CD)
- [ ] Buffs - Dragon Sight
- [ ] Buffs - Lance Charge
- [ ] Mitigation - Feint
- [ ] Icon - Dragonsight
- [ ] Icon - Lance Charge
- [ ] (**NEW**) Icon - Chaos Thrust / Chaotic Spring
- [ ] (**NEW**) Icon - Power Surge (Disembowel)

### Ninja
- [ ] (**NEW**) Gauge - Bunshin Stacks
- [ ] Gauge - True North
- [ ] Buffs - Trick Attack
- [ ] Buffs - Bunshin
- [ ] Mitigation - Feint
- [ ] Icon - Trick Attack

### Samurai
- [ ] (**NEW**) Gauge - Meikyo Stacks
- [ ] Gauge - Jinpu
- [ ] Gauge - Shifu
- [ ] Gauge - Higanbana
- [ ] Gauge - True North
- [ ] (**NEW**) Ogi Namikiri Ready (30s, 120s CD)
- [ ] Mitigation - Feint
- [ ] Icon - Jinpu
- [ ] Icon - Shifu

### Reaper
- [ ] (**NEW**) Gauge - Death's Design (30s default, make 60s)
- [ ] (**NEW**) Gauge - Procs (Enhanced Gibbet / Ehanced Cross Reaping , Enhanced Gallows / Enhanced Void Reaping)
- [ ] (**NEW**) Gauge - Soul Reaver stacks (2 max)
- [ ] (**NEW**) Gauge - Immortal Sacrifice Stacks (8 max)
- [ ] (**NEW**) Buffs - Arcane Circle (20s, 120s CD)
- [ ] (**NEW**) Mitigation - Feint
- [ ] (**NEW**) Icon - Death's Design (Shadow of Death / Whorl of Death)

### Bard
- [ ] (**NEW**) Gauge - Straight Shot Proc
- [ ] (**NEW**) Gauge - Blootletter Charges (2 charges, 15s)
- [ ] (**NEW**) Gauge - GCDs in Raging Strikes (20 seconds)
- [ ] Gauge - Caustic Bite
- [ ] Gauge - Stormbite
- [ ] (**NEW**) Buffs - Battle Voice (15s, 120s CD)
- [ ] (**NEW**) Buffs - Raging Strikes (20s, 60s CD)
- [ ] (**NEW**) Buffs - Barrage (10s, 120s CD)
- [ ] (**NEW**) Buffs - Radiant Finale (15s, 90s CD)
- [ ] (**NEW**) Mitigation - Troubadour (15s, 90s CD)
- [ ] Mitigation - Nature's Minne
- [ ] (**NEW**) Icon - Raging Strikes (20s)
- [ ] Icon - Caustic Bite
- [ ] Icon - Stormbite

### Machinist
- [ ] Gauge - GCDs in Hypercharge
- [ ] Gauge - GCDs in Wildfire
- [ ] Gauge - Charges of Ricochet
- [ ] Gauge - Charges of Gauss Round
- [ ] Buffs - Wildfire
- [ ] (**NEW**) Mitigation - Tactician (15s, 90s CD)
- [ ] Icon - Wildfire

### Dancer
- [ ] (**NEW**) Gauge - Procs (Flourishing Symmetry, Flourishing Flow, Fan Dance 3)
- [ ] Buffs - Technical Step
- [ ] Buffs - Devilment
- [ ] (**NEW**) Mitigation - Shield Samba (15s, 90s CD)
- [ ] Mitigation - Improvisation
- [ ] Icon - Devilment

### Black Mage
- [ ] (**NEW**) Gauge - Procs (Fire = 30s, Thunder = 21s)
- [ ] Guage - Thunder 3 + 4
- [ ] Mitigation - Addle
- [ ] Icon - Thunder
- [ ] Icon - Leylines

### Summoner
- [ ] (**NEW**) Buffs - Searing Light (30s, 120s CD)
- [ ] Buffs - Summon Bahamut
- [ ] (**NEW**) Buffs - Summon Pheonix (20s, 60s CD) / new ability + buff name
- [ ] Mitigation - Addle

### Red Mage
- [ ] (**NEW**) Gauges - Manafication Stacks (6)
- [ ] Gauges - Fire and Stone Procs
- [ ] Gauges - Acceleration Stacks
- [ ] Buffs - Embolden
- [ ] Buffs - Manafication
- [ ] Mitigation - Addle
- [ ] (**NEW**) Mitigation - Magick Barrier (10s, 120s CD)

### Blue Mage
- [ ] Gauges - Song of Torment
- [ ] Gauges - Bad Breath
- [ ] Gauges - Libra
- [ ] Buffs - Off-Guard
- [ ] Buffs - Peculiar Light
- [ ] Mitigation - Addle
- [ ] Mitigation - Angel Whisper
- [ ] Mitigation - Swiftcast
- [ ] Icon - Song of Torment
- [ ] Icon - Bad Breath