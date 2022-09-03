# JobBars
[![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FJobBars)](https://github.com/0ceal0t/JobBars)

A plugin for [XIVQuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) which provides extra job gauges, a party buff tracker, and more.

- Number of GCDs under buffs (Fight or Flight, Inner Release)
- DoT tracker (Dia, Miasma)
- Proc display (Verfire/Verstone Ready)
- Number of charges (Ricochet, Gauss Round)
- Number of stacks (Ruin IV)
- Icon replacement (time until DoT refresh, duration left on buffs)
- Party buffs coming off of cooldown
- Mitigation tracker
- Cursor displays (cast time, GCD timer, MP tick, DoT tick)

Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152). Feel like something is missing? Open an [issue](https://github.com/0ceal0t/JobBars/issues)

https://user-images.githubusercontent.com/18051158/130377508-ee88e07f-b41f-4a39-83db-4b9cc79a47b0.mp4

https://user-images.githubusercontent.com/18051158/130377516-5c299fb5-9a3a-4b47-bb5f-b03297c3ea6f.mp4

https://user-images.githubusercontent.com/18051158/130377606-2490ab26-1c2b-43fa-93f3-80e6c95e9fff.mp4

https://user-images.githubusercontent.com/18051158/130377610-86fb7e17-9780-4827-81df-0739908bd709.mp4

https://user-images.githubusercontent.com/18051158/130377598-2398d33a-9c0c-4d0c-8fd7-4187451a7e56.mp4

## Usage
To open the settings menu, use `/jobbars`. When changing icon positions on your hotbars, you may need to switch to a different job and then back to update the icon displays.

## Why?
The goal of this project is to augment the existing UI by displaying information in a more convenient format. It is not meant to be a complete overhaul, or to replace existing job gauges. If you personally only find certain parts useful, every gauge/buff/mitigation/etc. can be enabled and disabled individually.

## TODO
- [ ] Update README tables
- [ ] PVP specific gauges/cooldowns/buffs
- [ ] Style MP bars like actual MP
- [ ] Alternate text style
- [ ] Custom text spacing
- [ ] Completely custom gauges/buffs/cds (requires big rework)
- [ ] Hide based on level
- [ ] How many people got hit by buffs
- [ ] Split up party buffs and personal buffs