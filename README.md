# JobBars
A plugin for [XIVQuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) which provides extra job gauges and a party buff tracker
- Number of GCDs under buffs (Fight or Flight, Inner Release)
- DoT tracker (Dia, Miasma)
- Proc display (Verfire/Verstone Ready)
- Number of charges (Ricochet, Gauss Round)
- Number of stacks (Ruin IV)
- Party buffs coming off of cooldown

https://user-images.githubusercontent.com/18051158/123529151-a4f9e100-d6bb-11eb-9197-b16f7b827689.mp4

**Feel like something is missing? Open an [issue](https://github.com/0ceal0t/JobBars/issues)**

## Usage
To open the settings menu, use `/jobbars`

> Note: you must have your HP/MP visible for this plugin to work properly

## Jobs

### <img src="Assets/JobIcons/DRK.png" height="20px" width="20px"> DRK
+ **Gauges**: GCDS used in Delirium, GCDS used in Blood Weapon
+ **Buffs**: Delerium, Living Shadow

### <img src="Assets/JobIcons/WAR.png" height="20px" width="20px"> WAR
+ **Gauges**: GCDS used in Inner Release, Storm's Eye
+ **Buffs**: Inner Release

### <img src="Assets/JobIcons/PLD.png" height="20px" width="20px"> PLD
+ **Gauges**: GCDS used in Requiescat, GCDS used in Fight or Flight, Goring Blade

### <img src="Assets/JobIcons/GNB.png" height="20px" width="20px"> GNB
+ **Gauges**: GCDS used in No Mercy

### <img src="Assets/JobIcons/SCH.png" height="20px" width="20px"> SCH
+ **Gauges**: Excog, Biolysis
+ **Buffs**: Chain Stratagem

### <img src="Assets/JobIcons/WHM.png" height="20px" width="20px"> WHM
+ **Gauges**: Dia

### <img src="Assets/JobIcons/AST.png" height="20px" width="20px"> AST
+ **Gauges**: Combust, Upgraded Earthly Star, Lightspeed
+ **Buffs**: Cards, Divination

### <img src="Assets/JobIcons/MNK.png" height="20px" width="20px"> MNK
+ **Gauges**: Twin Snakes, Demolish, Charges + time left on Riddle of Earth / True North
+ **Buffs**: Brotherhood, Riddle of Fire

### <img src="Assets/JobIcons/DRG.png" height="20px" width="20px"> DRG
+ **Gauges**: GCDS used in Lance Charge, GCDS used in Dragonsight, Charges + time left on True North
+ **Buffs**: Battle Litany, Dragonsight, Lance Charge

### <img src="Assets/JobIcons/NIN.png" height="20px" width="20px"> NIN
+ **Gauges**: GCDS used in Bunshin, Charges + time left on True North
+ **Buffs**: Trick Attack, Bunshin

### <img src="Assets/JobIcons/SAM.png" height="20px" width="20px"> SAM
+ **Gauges**: Jinpu, Shifu, Higanbana, Charges + time left on True North
+ **Buffs**: Double Midare

### <img src="Assets/JobIcons/BRD.png" height="20px" width="20px"> BRD
+ **Gauges**: GCDS used in Raging Strikes, Caustic Bite, Stormbite
+ **Buffs**: Battle Voice, Raging Strikes, Barrage

### <img src="Assets/JobIcons/MCH.png" height="20px" width="20px"> MCH
+ **Gauges**: GCDS used in Hypercharge, GCDS used in Wildfire, Charges of Ricochet, Charges of Gauss Round
+ **Buffs**: Wildfire, Reassemble

### <img src="Assets/JobIcons/DNC.png" height="20px" width="20px"> DNC
+ **Gauges**: Procs
+ **Buffs**: Technical Step, Devilment

### <img src="Assets/JobIcons/BLM.png" height="20px" width="20px"> BLM
+ **Gauges**: Thunder 3+4, Fire and Thunder procs

### <img src="Assets/JobIcons/SMN.png" height="20px" width="20px"> SMN
+ **Gauges**: Ruin 4, Bio, Miasma, Wyrmwave and Scarlet Flame
+ **Buffs**: Devotion, Summon Bahamut, Firebird Trance

### <img src="Assets/JobIcons/RDM.png" height="20px" width="20px"> RDM
+ **Gauges**: GCDS used in Manification, Fire and Stone procs, Acceleration stacks
+ **Buffs**: Embolden, Manafication

### <img src="Assets/JobIcons/BLU.png" height="20px" width="20px"> BLU
+ **Gauges**: Song of Torment, Bad Breath, Condensed Libra
+ **Buffs**: Off-guard, Peculiar Light

## TODO
- [ ] Hide based on level
- [ ] How many people got hit by buffs
- [ ] Glow around gauges
- [ ] Replicate or attach to existing gauges
- [ ] Split up party buffs and personal buffs
- [ ] GCD timer
- [ ] DoT tick timer
- [ ] SMN DoTs at 6 seconds [see this issue](https://github.com/0ceal0t/JobBars/issues/9)
- [x] ~~Add invert counter option to GCDs~~
- [x] ~~Better bar placement options~~
- [x] ~~Animate gauge movement~~ (kinda)
- [x] ~~Hide all gauges options~~
- [x] ~~Hide all buffs options~~
- [x] ~~MCH number of charges of Gauss Round / Richochet~~
- [x] ~~Track DoTs based on target~~
- [x] ~~AST upgraded star~~
- [x] ~~Move gauges independently~~
- [x] ~~Sound effect when DoTs are low~~
- [x] ~~Red text when DoTs are low~~
- [x] ~~Remove buffs on instance end~~
- [x] ~~Remove buffs on party change~~
- [x] ~~Dia messes up battle stance icon~~
- [x] ~~Border around buffs~~