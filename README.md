# JobBars
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

Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

https://user-images.githubusercontent.com/18051158/130377508-ee88e07f-b41f-4a39-83db-4b9cc79a47b0.mp4

https://user-images.githubusercontent.com/18051158/130377516-5c299fb5-9a3a-4b47-bb5f-b03297c3ea6f.mp4

https://user-images.githubusercontent.com/18051158/130377606-2490ab26-1c2b-43fa-93f3-80e6c95e9fff.mp4

https://user-images.githubusercontent.com/18051158/130377610-86fb7e17-9780-4827-81df-0739908bd709.mp4

https://user-images.githubusercontent.com/18051158/130377598-2398d33a-9c0c-4d0c-8fd7-4187451a7e56.mp4

## Usage
To open the settings menu, use `/jobbars`. When changing icon positions on your hotbars, you may need to switch to a different job and then back to update the icon displays.

## Why?
The goal of this project is to augment the existing UI by displaying information in a more convenient format. It is not meant to be a complete overhaul, or to replace existing job gauges. If you personally only find certain parts useful, every gauge/buff/mitigation/etc. can be enabled and disabled individually.

## Jobs

**Feel like something is missing? Open an [issue](https://github.com/0ceal0t/JobBars/issues)**

### <img src="assets/job_icons/DRK.png" height="20px" width="20px"> Dark Knight

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Delirium          </td><td> Delirium            </td><td> Living Dead          </td><td> Delirium           </td></tr>
  <tr><td> GCDs in Blood Weapon      </td><td> Living Shadow       </td><td> Reprisal             </td><td> Blood Weapon       </td></tr>
  <tr><td> MP                        </td><td>                     </td><td> Dark Missionary      </td><td>                    </td></tr>
  <tr><td>                           </td><td>                     </td><td> The Blackest Night   </td><td>                    </td></tr>
</table>


### <img src="assets/job_icons/WAR.png" height="20px" width="20px"> Warrior

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Inner Release     </td><td> Inner Release       </td><td> Holmgang             </td><td> Inner Release      </td></tr>
  <tr><td> Storm's Eye               </td><td>                     </td><td> Reprisal             </td><td> Storm's Eye        </td></tr>
  <tr><td>                           </td><td>                     </td><td> Shake it Off         </td><td>                    </td></tr>
  <tr><td>                           </td><td>                     </td><td> Nascent Flash        </td><td>                    </td></tr>
</table>


### <img src="assets/job_icons/PLD.png" height="20px" width="20px"> Paladin

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Requiescat        </td><td>                     </td><td> Hallowed Ground      </td><td> Requiescat         </td></tr>
  <tr><td> GCDs in Fight or Flight   </td><td>                     </td><td> Reprisal             </td><td> Goring Blade       </td></tr>
  <tr><td> Goring Blade              </td><td>                     </td><td> Divine Veil          </td><td> Fight or Flight    </td></tr>
  <tr><td>                           </td><td>                     </td><td> Passage of Arms      </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/GNB.png" height="20px" width="20px"> Gunbreaker

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in No Mercy          </td><td>                     </td><td> Superbolide          </td><td> No Mercy           </td></tr>
  <tr><td>                           </td><td>                     </td><td> Reprisal             </td><td>                    </td></tr>
  <tr><td>                           </td><td>                     </td><td> Heart of Light       </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/SCH.png" height="20px" width="20px"> Scholar

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Excog                     </td><td> Chain Stratagem     </td><td> Seraph               </td><td> Biolysis           </td></tr>
  <tr><td> Biolysis                  </td><td>                     </td><td> Deployment Tactics   </td><td> Chain Stratagem    </td></tr>
  <tr><td>                           </td><td>                     </td><td> Recitation           </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/WHM.png" height="20px" width="20px"> White Mage

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Dia                       </td><td> Cards               </td><td> Temperance           </td><td> Dia                </td></tr>
  <tr><td>                           </td><td> Divination          </td><td> Benedictiob          </td><td> Presence of Mind   </td></tr>
  <tr><td>                           </td><td>                     </td><td> Asylum               </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/AST.png" height="20px" width="20px"> Astrologian

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Combust                   </td><td> Cards               </td><td> Netural Sect            </td><td> Combust            </td></tr>
  <tr><td> Upgraded Earthly Star     </td><td> Divination          </td><td> Celestial Opposition    </td><td> Lightspeed         </td></tr>
  <tr><td> Lightspeed                </td><td>                     </td><td> Collective Unconscious  </td><td>                    </td></tr>
  <tr><td>                           </td><td>                     </td><td> Earthly Star            </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/MNK.png" height="20px" width="20px"> Monk

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Twin Snakes                  </td><td> Brotherhood         </td><td> Feint                </td><td> Twin Snakes        </td></tr>
  <tr><td> Demolish                     </td><td> Riddle of Fire      </td><td> Mantra               </td><td> Demolish           </td></tr>
  <tr><td> Riddle of Earth / True North </td><td>                     </td><td>                      </td><td> Riddle of Fire     </td></tr>
</table>

### <img src="assets/job_icons/DRG.png" height="20px" width="20px"> Dragoon

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Lance Charge      </td><td> Battle Litany       </td><td> Feint                </td><td> Dragonsigh         </td></tr>
  <tr><td> GCDs in Dragonsight       </td><td> Dragonsight         </td><td>                      </td><td> Lance Charge       </td></tr>
  <tr><td> True North                </td><td> Lance Charge        </td><td>                      </td><td> Disembowel         </td></tr>
  <tr><td>                           </td><td>                     </td><td>                      </td><td> Chaos Thrust       </td></tr>
</table>

### <img src="assets/job_icons/NIN.png" height="20px" width="20px"> Ninja

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Bunshin           </td><td> Trick Attack        </td><td> Feint                </td><td> Trick Attack        </td></tr>
  <tr><td> True North                </td><td> Bunshin             </td><td>                      </td><td>                     </td></tr>
</table>

### <img src="assets/job_icons/SAM.png" height="20px" width="20px"> Samurai

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Jinpu                     </td><td> Double Midare       </td><td> Feint                </td><td> Jinpu              </td></tr>
  <tr><td> Shifu                     </td><td>                     </td><td>                      </td><td> Shifu              </td></tr>
  <tr><td> Higanbana                 </td><td>                     </td><td>                      </td><td>                    </td></tr>
  <tr><td> True North                </td><td>                     </td><td>                      </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/BRD.png" height="20px" width="20px"> Bard

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Raging Strikes    </td><td> Battle Voice        </td><td> Troubadour           </td><td> Caustic Bite       </td></tr>
  <tr><td> Caustic Bite              </td><td> Raging Strikes      </td><td> Nature's Minne       </td><td> Stormbite          </td></tr>
  <tr><td> Stormbite                 </td><td> Barrage             </td><td>                      </td><td> Raging Strikes     </td></tr>
</table>

### <img src="assets/job_icons/MCH.png" height="20px" width="20px"> Machinist

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Hypercharge       </td><td> Wildfire            </td><td> Tactician            </td><td> Wildfire           </td></tr>
  <tr><td> GCDs in Wildfire          </td><td> Reassemble          </td><td>                      </td><td>                    </td></tr>
  <tr><td> Charges of Ricochet       </td><td>                     </td><td>                      </td><td>                    </td></tr>
  <tr><td> Charges of Gauss Round    </td><td>                     </td><td>                      </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/DNC.png" height="20px" width="20px"> Dancer

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Procs                     </td><td> Technical Step      </td><td> Shield Samba         </td><td> Devilment          </td></tr>
  <tr><td>                           </td><td> Devilment           </td><td> Improvisation        </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/BLM.png" height="20px" width="20px"> Black Mage

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Thunder                   </td><td>                     </td><td> Addle                </td><td> Thunder            </td></tr>
  <tr><td> Fire and Thunder Procs    </td><td>                     </td><td>                      </td><td> Leylines           </td></tr>
  <tr><td>                           </td><td>                     </td><td>                      </td><td> Sharpcast          </td></tr>
</table>

### <img src="assets/job_icons/SMN.png" height="20px" width="20px"> Summoner

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Ruin 4 Stacks             </td><td> Devotion            </td><td> Addle                </td><td> Bio                </td></tr>
  <tr><td> Bio                       </td><td> Summon Bahamut      </td><td>                      </td><td> Miasma             </td></tr>
  <tr><td> Miasma                    </td><td> Firebird Trance     </td><td>                      </td><td>                    </td></tr>
  <tr><td> Wyrmwave/Scarlet Flame    </td><td>                     </td><td>                      </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/RDM.png" height="20px" width="20px"> Red Mage

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> GCDs in Manafication      </td><td> Embolden            </td><td> Addle                </td><td> Manafication       </td></tr>
  <tr><td> Fire and Stone Procs      </td><td> Manafication        </td><td>                      </td><td>                    </td></tr>
  <tr><td> Acceleration Stacks       </td><td>                     </td><td>                      </td><td>                    </td></tr>
</table>

### <img src="assets/job_icons/BLU.png" height="20px" width="20px"> Blue Mage

<table>
  <tr><th width="200px">Gauges</th> <th width="200px">Buffs</th> <th width="200px">Mitigation</th> <th width="200px">Icon Display</th> </tr>

  <tr><td> Song of Torment           </td><td> Off-Guard           </td><td> Addle                </td><td> Song of Torment    </td></tr>
  <tr><td> Bad Breath                </td><td> Peculiar Light      </td><td> Angel Whisper        </td><td> Bad Breath         </td></tr>
  <tr><td> Condensed Libra           </td><td>                     </td><td>                      </td><td>                    </td></tr>
</table>

## TODO
- [ ] Vertical gauges
- [ ] Custom text spacing
- [ ] Text above/below gauges
- [ ] Completely custom gauges/buffs/cds (requires big rework)
- [ ] Support crossbar better for icon replacement
- [ ] Hide based on level
- [ ] How many people got hit by buffs
- [ ] Split up party buffs and personal buffs
- [ ] SMN DoTs at 6 seconds [see this issue](https://github.com/0ceal0t/JobBars/issues/9)
- [x] ~~Support crossbar better for icon replacement~~
- [x] ~~Proc order changing~~
- [x] ~~GCD timer~~ (cursor)
- [x] ~~DoT tick timer / MP timer~~ (cursor)
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
