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
- [ ] [Icon Ids](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L54)
- [ ] [Spell or Weaponskill](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L137)
- [ ] [Cooldown Group](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L138)
- [ ] [Blank Icon Id](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Data.cs#L138)
- [ ] [Texture Load Size](https://github.com/0ceal0t/JobBars/blob/main/JobBars/Helper/UiHelper.Textures.cs#L96)
- [ ] [Icon Nodes](https://github.com/0ceal0t/JobBars/blob/main/JobBars/UI/Icon/UIIconBuff.cs#L17)
	- [Timer](https://github.com/0ceal0t/JobBars/blob/main/JobBars/UI/Icon/UIIconTimer.cs#L24)

## Jobs

### Dark Knight
- Gauges: **Delirium Stacks**, GCDs in Blood Weapon, MP
- Buffs: Delirium, Living Shadow
- Mitigation: Living Dead, Reprisal, Dark Missionary, The Blackest Night
- Icon Display: Blood Weapon

### Warrior
- Gauges: **Inner Release Stacks**, **Storm's Eye (Surging Tempest)**
- Buffs: Inner Release
- Mitigation: Holmgang, Reprisal, Shake it Off, **Nascent Flash / Raw Inuition / Bloodwhetting**
- Icon Display: Inner Release, **Storm's Eye (Surging Tempest)**

### Paladin
- Gauges: **Req stacks**, Atonement Stacks, GCDs in Fight or Fights, **Goring Blade / Blade of Valor**
- Buffs: **Req**
- Mitigation: Hallowed Ground, Reprisal, Divine Veil, Passage of Arms
- Icons Display: **Goring Blade / Blade of Valor**, Fight or Flight

### Gunbreaker
- Gauges: GCDs in No Mercy
- Buffs:
- Mitigation: Superbolide, Reprisal, Heart of Light
- Icon Display: No Mercy

### Scholar
- Gauges: Excog, Biolysis
- Buffs: Chain Stratagem
- Migitation: Seraph, Recitation, **Expedient**, **Protraction**, Swiftcast
- Icon Display: Biolysis, Chain Stratagem

### White Mage
- Gauges: Dia, Prescence of Mind
- Buffs: 
- Migitation: Temperance, Benediction, **Lilybell**, Swiftcast
- Icon Display: Biolysis, Chain Stratagem, **Prescence of Mind**

### Astrologian
- Gauges: Combust, Upgraded Earthly Star, Lightspeed
- Buffs: **Cards (no Lord/Lady)**, Divination
- Migitation: Neutral Sect, Earthly Star, **Macrocosmos**, Swiftcast
- Icon Display: Combust, Lightspeed, Earthly Star, **Astrodyne / Harmony of Spirit**

### Sage
- Gauges: **Eukrasian Dosis**
- Buffs: 
- Migitation: **Zoe**, **Panhaima**, **Pneuma**, **Swiftcast**
- Icon Display: **Eukrasian Dosis**

### Monk
- Gauges: Twin Snakes, Demolish, Riddle of Earth / True North
- Buffs: Brotherhood, Riddle of Fire
- Migitation: Feint, Mantra
- Icon Display: Twin Snakes, Demolish, Riddle of Fire, **Brotherhood**

### Dragoon
- Gauges: GCDs in Lance Charge, GCDs in Dragonsight, True North
- Buffs: Battle Litany, Dragonsight, Lance Charge
- Migitation: Feint
- Icon Display: Dragonsight, Lance Charge, Disembowel, **Chaos Thrust / Chaotic Spring**

### Ninja
- Gauges: **Bunshin Stacks**, True North
- Buffs: Trick Attack, Bunshin
- Migitation: Feint
- Icon Display: Trick Attack

### Samurai
- Gauges: **Mesui Stacks**, Jinpu, Shifu, Higanbana, True North
- Buffs: **Ori Namikiri (120s)**
- Migitation: Feint
- Icon Display: Jinpu, Shifu

### Bard
- Gauges: **Straight Shot Proc**, GCDs in Raging Strikes, Caustic Bite, Stormbite
- Buffs: Battle Voice, Raging Strikes, **Radiant Finale**
- Migitation: Troubadour, Nature's Minne
- Icon Display: Caustic Bite, Stormbite, Raging Strikes

### Machinist
- Gauges: GCDs in Hypercharge, GCDs in Wildfire, Charges of Ricochet, Charges of Gauss Round
- Buffs: Wilfire
- Migitation: Tactician
- Icon Display: Wildfire

### Dancer
- Gauges: **Procs (Flourishing Symmetry, Flourishing Flow, Fan Dance 3 + 4)**
- Buffs: Technical Step, Devilment
- Migitation: Shield Samba, Improvisation
- Icon Display: Devilment

### Black Mage
- Gauges: Thunder, Fire and Thunder Procs
- Buffs: 
- Migitation: Addle
- Icon Display: Thunder, Leylines, Sharpcast

### Summoner
- Gauges: Ruin 4 (still has stacks?)
- Buffs: **Searing Light**, Summon Bahamut, **Summon Pheonix**
- Migitation: Addle
- Icon Display: 

### Red Mage
- Gauges: **Manafication Stacks**, Fire and Stone Procs, Acceleration Stacks
- Buffs: Embolden, Manafication
- Migitation: Addle, **Magick Barrier**
- Icon Display: 

### Blue Mage
- Gauges: Song of Torment, Bad Breath, Libra
- Buffs: Off-Guard, Peculiar Light
- Migitation: Addle, Angle Whisper, Swiftcast
- Icon Display: Song of Torment, Bad Breath

### Reaper
- Gauges: **Death's Design**, **Enhanced Gibbet (Ehanced Cross Reaping) / Gallows (Enhanced Void Reaping) Procs**, **Soul Reaver Stacks**, **Immortal Sacrifice Stacks**
- Buffs: **Arcane Circle**
- Migitation: **Feint**
- Icon Display: **Death's Design (Shadow of Death / Whorl of Death)**