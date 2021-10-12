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
- Gauges: Delirium stacks, GCDs in Blood Weapon, MP
- Buffs: Delirium, Living Shadow
- Mitigation: Living Dead, Reprisal, Dark Missionary, The Blackest Night
- Icon Display: Blood Weapon

### Warrior
- Gauges: **Inner Release Stacks**, **Storm's Eye (Surging Tempest)**
- Buffs: Inner Release
- Mitigation: Holmgang, Reprisal, Shake it Off, Nascent Flash
- Icon Display: Inner Release, **Storm's Eye (Surging Tempest)**

### Paladin
- Gauges: **Req stacks**, GCDs in Fight or Fights, **Goring Blade / Blade of Valor**
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
- Migitation: Seraph, Recitation, **Expedient**, Swiftcast
- Icon Display: Biolysis, Chain Stratagem

### White Mage
- Gauges: Dia, Prescence of Mind
- Buffs: 
- Migitation: Temperance, Benediction, **Lilybell**, Swiftcast
- Icon Display: Biolysis, Chain Stratagem

### Astrologian
- Gauges: Combust, Upgraded Earthly Star, Lightspeed
- Buffs: **Cards (no Lord/Lady)**, Divination
- Migitation: Neutral Sect, Earthly Star, **Macrocosmos**, Swiftcast
- Icon Display: Combust, Lightspeed, Earthly Star, **Macrocosmos**, **Astrodyne / Harmony of Spirit**

### Sage
- Gauges: **Eukrasian Dosis**
- Buffs: 
- Migitation: **Zoe**, **Panhaima**, **Physis II**, **Swiftcast**
- Icon Display: **Eukrasian Dosis**

### Monk

### Dragoon

### Ninja

### Samurai

### Bard

### Machinist

### Dancer

### Black Mage

### Summoner

### Red Mage

### Blue Mage

### Reaper