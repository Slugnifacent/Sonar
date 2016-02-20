using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sonar {
    public class ThrowManager {
        public List<Throwable> throwingObjects;
        public List<Debris> debris;
        public List<Object> resetObjects;

        public ThrowManager() {
            throwingObjects = new List<Throwable>();
            debris = new List<Debris>();
            resetObjects = new List<Object>();
        }

        /// <summary>
        /// Corey - 4/8
        /// This takes care of all the update code for throwing that use to be in Map. Moved here to look cleaner.
        /// The lists associated with throwing objects and Debris were also moved here.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="map"></param>
        public void update(GameTime time, Map map) {

            //This loop is to see if the player walked over debris.
            foreach (Debris d in debris) {
                d.Update(time);
                if (d.Collide(map.getPlayer().getBox()) && d.soundCD == 0 && (map.getPlayer().positionPrevious != map.getPlayer().position)) {
                    d.soundCD = 25;
                    //GameScreen.shaders[GameScreen.visionShaderIndex].addPoint(d.position, d.getWeight() * 100, 1000f, 5f);
                    //VisionManager.visionPoints.Add(new Vector3(d.position.X, d.position.Y, d.getWeight() * 100));
                    SoundManager.GetInstance().createSound(d.position, 700f, 1000f, 5f, SoundManager.THROWABLE.GLASS_BREAKING, true);
                }
            }

            //This loop starts the updating of throwing objects on the map.
            for (int i = 0; i < throwingObjects.Count; i++) {
                throwingObjects[i].update();
                if (throwingObjects[i].Collide(map.getPlayer().getBox())) {
                    if (throwingObjects[i].readyForPickUp) {
                        map.getPlayer().obtainThrowing(throwingObjects[i]);

                        if (throwingObjects[i].GetType() == typeof(WalkieTalkie)) {
                            if (map.getPlayer().walkieTalkie != null) {
                                map.getPlayer().walkieTalkie.collected = true;
                            }
                        }
                        else {
                            SoundManager.GetInstance().playSoundFX(SoundManager.THROWABLE.GLASS_PICKUP);
                        }                        

                        throwingObjects.RemoveAt(i);
                    }
                }
            }

            //This is for the throwing objects that have landed.
            for (int i = 0; i < map.getPlayer().throwing.Count; i++) {
                //If the throwing object landed and broke, creating debris.
                if (map.getPlayer().throwing[i].landed && map.getPlayer().throwing[i].getHealth() <= 1) {
                    if (map.getPlayer().throwing[i].GetType() == typeof(WalkieTalkie)) {
                        map.getPlayer().walkieTalkie = null;
                    }
                    debris.Add(new Debris(Game1.contentManager, map.getPlayer().throwing[i].position, map.getPlayer().throwing[i].getWeight(), null));
                    // GameScreen.shaders[0].addPoint(player.throwing[i].position, (float)player.throwing[i].getWeight() * 100, 1000f, 5f);
                    // SoundManager.GetInstance().playSoundFX(player.throwing[i].position, (float)player.throwing[i].getWeight() * 300);
                    // SoundManager.GetInstance().playSoundFX(player.throwing[i].position, (float)player.throwing[i].getWeight() *300, 1000f, 5f, 1, 2000, glassBreak.CreateInstance(), true);
                    SoundManager.GetInstance().createSound(map.getPlayer().throwing[i].position, (float)map.getPlayer().throwing[i].getWeight() * 400, 2000f, 5f, SoundManager.THROWABLE.GLASS_BREAKING, true);
                    map.getPlayer().throwing.RemoveAt(i);
                    map.getPlayer().throwCooldown = false;                   
                }
                //If the throwing object landed but had enough health to survive.
                else if (map.getPlayer().throwing[i].landed) {
                    //Walkie Talkie logic
                    if (map.getPlayer().throwing[i].GetType() == typeof(WalkieTalkie)) {
                        map.getPlayer().walkieTalkie = new WalkieTalkie(map.getPlayer().throwing[i].position, Game1.contentManager, true, map.getPlayer().throwing[i].getHealth() - 1);
                        throwingObjects.Add(map.getPlayer().walkieTalkie);
                        map.getPlayer().walkieTalkie.collected = false;
                        SoundManager.GetInstance().playSoundFX(SoundManager.THROWABLE.GLASS_COLLISION);
                        //Console.Out.WriteLine("Tossed");
                    }
                    //Any other throwing item.
                    else throwingObjects.Add(new Throwable(Game1.contentManager, map.getPlayer().throwing[i].position, map.getPlayer().throwing[i].getHealth() - 1, map.getPlayer().throwing[i].getWeight(), null));
                    //SoundManager.GetInstance().playSoundFX(player.throwing[i].position, 200, 200, 1, 1, 2000, glassHit.CreateInstance());
                    // SoundManager.GetInstance().playSoundFX(player.throwing[i].position, (float)player.throwing[i].getWeight() * 300);
                    map.getPlayer().throwing.RemoveAt(i);
                    map.getPlayer().throwCooldown = false;                    
                }
            }
        }
    }
}
