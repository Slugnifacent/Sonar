using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class WrathAtkIdleState : State
    {
        MapUnit myTarg;
        int numberOfPlacesToLook = 3;
        //static Random rand = new Random();
        GameVector2 randVal = GameVector2.Zero;
        Condition search = new AtTargetCondition();
        int searchRange = 60;
        float enemySpeed;

        public override void doAction(Spectre spectre, Player player)
        {
            //After attacking the player will stop and breathe heavily.
            if (spectre.fleeTimer < 3) {
                spectre.SetTarget(spectre.getCurrentUnit());
            }
            else {
                // Wanders a bit before attacking the player again.
                if (spectre.GetPath() == null && numberOfPlacesToLook != 0) {
                    numberOfPlacesToLook++;
                    randVal.X = -1;
                    randVal.Y = -1;
                    myTarg = spectre.getCurrentUnit();//spectre.GetMap()[(int)spectre.getCurrentUnit().x, (int)spectre.getCurrentUnit().y];
                    spectre.SetTarget(myTarg);
                    spectre.ClearPath();
                }
                //spectre.SetTarget(myTarg);
                //spectre.ClearPath();

                // Test for where you heard/last saw the player plus four other spots around the area
                // If none of these spots lead to sighting the player, go back to patroling
                if (numberOfPlacesToLook == 0) {
                    spectre.finishedSearching = true;
                }
                else if (search.test(spectre, player) && numberOfPlacesToLook > 0) {
                    numberOfPlacesToLook--;
                    spectre.speed = 80;

                    randVal = spectre.findValidRandomVal(spectre.getCurrPos(), searchRange);
                    /*while ((randX < 0) || (randY < 0) ||
                           (randX >= spectre.GetMap().GetUpperBound(1) * MapUnit.MAX_SIZE) || (randY >= spectre.GetMap().GetUpperBound(0) * MapUnit.MAX_SIZE))
                    {
                        //Console.Out.WriteLine("While 1--------------------------------------------------1");
                        randX = Game1.random.Next((int)spectre.getCurrPos().X - searchRange, (int)spectre.getCurrPos().X + searchRange);
                        randY = Game1.random.Next((int)spectre.getCurrPos().Y - searchRange, (int)spectre.getCurrPos().Y + searchRange);
                    }
                    while (!spectre.GetMap()[randX / MapUnit.MAX_SIZE, randY / MapUnit.MAX_SIZE].isWalkable)
                    { // Ensures that next place to search is walkable and in the map
                        randX = Game1.random.Next((int)spectre.getCurrPos().X - searchRange, (int)spectre.getCurrPos().X + searchRange);
                        randY = Game1.random.Next((int)spectre.getCurrPos().Y - searchRange, (int)spectre.getCurrPos().Y + searchRange);
                        //Console.Out.WriteLine("While 2__________________________________________________2");

                        if (randX < 0)
                        {
                            randX = 0;
                        }
                        if (randY < 0)
                        {
                            randY = 0;
                        }
                    }*/

                    //Console.Out.WriteLine("number of places to look = " + numberOfPlacesToLook + "Spot #(" + randVal.X + ", " + randVal.Y + ")");
                    myTarg = spectre.GetMap()[(int)randVal.X / MapUnit.MAX_SIZE, (int)randVal.Y / MapUnit.MAX_SIZE];
                    spectre.SetTarget(myTarg);
                    spectre.ClearPath();
                    //Console.Out.WriteLine("Good location -- currPos = " + spectre.getCurrPos());
                    /*randX = Game1.random.Next((int)spectre.getCurrPos().X - 100, (int)spectre.getCurrPos().X + 100);
                    randY = Game1.random.Next((int)spectre.getCurrPos().Y - 100, (int)spectre.getCurrPos().Y + 100);*/
                }
            }
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            Metrics.getInstance().addMetric("Wrath Attack Idle", Player.getInstance().gethealth(), null, spectre.position);
            myTarg = spectre.getCurrentUnit();
            //spectre.SetTarget(spectre.GetMap()[(int)playerPos.X / MapUnit.MAX_SIZE, (int)playerPos.Y / MapUnit.MAX_SIZE]);
            spectre.SetTarget(myTarg);
            spectre.ClearPath();

            /*randX = 0;
            randY = 0;*/
            randVal = GameVector2.Zero;
            enemySpeed = spectre.speed;
            player.facing = spectre.orientation;
            player.isKnockedBack = true;
            player.speed *= 2;
            player.LoseHealth(25);
            Game1.camera.Shake(30, 2);
            spectre.fleeTimer = 0;

            // Animate Wrath's attack
            if (spectre.GetType() == typeof(Wrath)) {
                Wrath wrath = (Wrath)spectre;
                wrath.Attack();
            }
        }

        public override void doExitAction(Spectre spectre, Player player)
        {
            spectre.finishedSearching = false;
            numberOfPlacesToLook = 3;
            spectre.speed = enemySpeed;
            spectre.soundHeard = null;
            spectre.lastSoundHeard = null;
        }
    }
}
