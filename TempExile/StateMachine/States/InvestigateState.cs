using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class InvestigateState : State
    {
        MapUnit myTarg;
        int numberOfPlacesToLook = 4;
        //static Random rand = new Random();
        //int randX, randY;
        GameVector2 randVal = GameVector2.Zero;
        Condition search = new AtTargetCondition();
        int searchRange = 100;
        bool firstSearchSpot;
        float enemySpeed;
        long startTime;
        long elapsedTime;
        long waitTime = 4000; // How long you want spectre to wait at first search spot
        long targetTime; 

        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingSearchedFor = true;
            //If the spot is behind a door.
            // If the attempted path is inaccessible, Specter moves targets its current position instead
            if (spectre.GetPath() == null && firstSearchSpot) {
                myTarg = spectre.getCurrentUnit();//spectre.GetMap()[(int)spectre.getCurrentUnit().x, (int)spectre.getCurrentUnit().y];
                spectre.SetTarget(myTarg);
                spectre.ClearPath();
                numberOfPlacesToLook = 0;
            }
            else if (spectre.GetPath() == null && numberOfPlacesToLook != 0 && !firstSearchSpot) {
                numberOfPlacesToLook++;
                //randX = -1;
                //randY = -1;
                randVal.X = -1;
                randVal.Y = -1;
                myTarg = spectre.getCurrentUnit();//spectre.GetMap()[(int)spectre.getCurrentUnit().x, (int)spectre.getCurrentUnit().y];
                spectre.SetTarget(myTarg);
                spectre.ClearPath();
            }

            // Test for where you heard/last saw the player plus four other spots around that area
            // If none of these spots lead to sighting the player, go back to patrolling
            if (numberOfPlacesToLook == 0 && spectre.fleeTimer > 3) {
                spectre.finishedSearching = true;
            }
            else if (search.test(spectre, player) && numberOfPlacesToLook > 0) {
                numberOfPlacesToLook--;
                spectre.speed = 60;

                if (player.isHiding && spectre.playerBeingHeard)
                    player.Reveal();
                randVal = spectre.findValidRandomVal(spectre.getCurrPos());

                //Console.Out.WriteLine("number of places to look = " + numberOfPlacesToLook + "Spot #(" + randVal.X + ", " + randVal.Y + ")");

                elapsedTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                //Console.Out.WriteLine("Number of places to look = " + numberOfPlacesToLook);

                // This is the first place the Spectre checks after losing sight of player or hearing a noise
                if (numberOfPlacesToLook == 3) {
                    // If targetTime milliseconds have passed since the last Spectre reached position, go to next position
                    if (firstSearchSpot) {
                        targetTime = elapsedTime + waitTime;
                    }
                    firstSearchSpot = false;
                    if (elapsedTime >= targetTime) {
                        myTarg = spectre.GetMap()[(int)randVal.X / MapUnit.MAX_SIZE, (int)randVal.Y / MapUnit.MAX_SIZE];
                        spectre.SetTarget(myTarg);
                        spectre.ClearPath();
                        startTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                        spectre.isMoving = true; // Spectre is heading to search destination
                        //Console.Out.WriteLine("3 and changing target");
                    }
                    // Wait in place
                    else {
                        numberOfPlacesToLook++;
                        spectre.positionPrevious = spectre.position;
                        spectre.isMoving = false; // Spectre is waiting in place while checking search destination
                    }
                }
                // This is not the first place the Spectre is checking after losing sight of player or hearing a sound
                else {
                    myTarg = spectre.GetMap()[(int)randVal.X / MapUnit.MAX_SIZE, (int)randVal.Y / MapUnit.MAX_SIZE];
                    spectre.SetTarget(myTarg);
                    spectre.ClearPath();
                    startTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                    spectre.isMoving = true; // Spectre is heading to search destination
                }
                //Console.Out.WriteLine("Good location -- currPos = " + spectre.getCurrPos());
            }
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            spectre.active = true;
            Player.getInstance().beingSearchedFor = true;
            /*if (spectre.returningFromOpenDoor) {
                //Going to that sound he heard before.
                myTarg = spectre.investigateSpot;
                spectre.investigateSpot = null;
                spectre.SetTarget(myTarg);
                spectre.FindPath();
                spectre.returningFromOpenDoor = false;
            }*/
            if (spectre.locationOfNoise == null) {
                //Spectres coming from around corner, which doesn't have a sound source.
                myTarg = spectre.getCurrentUnit();
                spectre.ClearPath();
                spectre.goingToInvestigate = false;
            }
            else {
                //Console.Out.WriteLine("Following Sound at " + spectre.locationOfNoise + " from " + spectre.position);
                myTarg = spectre.GetMap()[(int)spectre.locationOfNoise.X / MapUnit.MAX_SIZE, (int)spectre.locationOfNoise.Y / MapUnit.MAX_SIZE];
                if (myTarg.objType.GetType() == typeof(Siren))
                {
                    MapUnit temp = myTarg;
                    do
                    {
                        myTarg = temp.neighbors[Game1.random.Next(0, 8)];
                    } while (!myTarg.isWalkable);
                }
                spectre.investigateSpot = myTarg;
                spectre.SetTarget(myTarg);
                //spectre.FindPath();
                spectre.ClearPath();
                //if (spectre.GetPath() == null) {
                    //spectre.openThatDoor = true;
                //}
            } 

            randVal = GameVector2.Zero;
            enemySpeed = spectre.baseSpeed;
            spectre.fleeTimer = 0;
            spectre.isMoving = true;
            firstSearchSpot = true;
            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; // Clock the time that the Spectre reached a destination point
        }

        public override void doExitAction(Spectre spectre, Player player)
        {
            spectre.finishedSearching = false;
            numberOfPlacesToLook = 4;
            spectre.speed = enemySpeed;
            spectre.soundHeard = null;
            spectre.lastSoundHeard = null;
        }
    }
}
