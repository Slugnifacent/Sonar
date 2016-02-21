using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Sonar {
    public class Map {
        #region Fields

        private MapUnit[,] terrain;

        private object player;
        public List<object> spectres;
        private List<object> radios;
        private object lockdownSwitch;
        private List<object> doors;
        private List<object> landmarks;
        private List<object> exits;
        private List<object> hidingPlaces;
        private List<GameVector2> landmarkPositions;
        private List<object> resetRadios;
        private GameVector2 startPos;
        private int height;
        private int width;
        private int levelNum;
        private int x2 = 0;
        private int y2 = 0;
        private bool playerAtDoor;
        private int collisionSoundTimer;
        private int collisionSoundTimerReset = 10;
        private int autoUnlockTimer;

        private bool hasRun = false;

        #region Tutorial Level

        const byte WALK_TALK_TUTORIAL = 0;       //1
        const byte BASIC_DOOR_TUTORIAL = 1;      //2
        const byte PASSWORD_DOOR_TUTORIAL = 2;   //3 
        const byte RADIO_OBJECT_TUTORIAL = 3;    //4
        const byte SNEAK_TUTORIAL = 4;           //5
        const byte POSSESSION_TUTORIAL = 5;      //6
        const byte STALKER_TUTORIAL = 6;          //7         
        const byte LOCKDOWN_SWITCH_TUTORIAL = 7; //8

        #endregion

        #region Save Data
        public LevelSaveData saveData;
        public int currentDeaths;
        public int currentAlerts;
        public float currentTime;
        #endregion

        private ThrowManager throwManager; //All throw code goes in here now.

        Stopwatch watch;
        TimeSpan span;
        
        private bool isTutorial;        
        private SpriteFont font;
        TutorialString tutString;

        #endregion

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Map Constructor
        /// </summary>
        /// <param name="filename">Filename for the current level</param>
        /// <param name="levelIndex">Which level the player is currently on</param>
        public Map(int levelIndex, Player p, bool isTutorial) {

            LoadingScreen.setCurrentActionString("Loading Level " + levelIndex);

            this.isTutorial = isTutorial;            
            levelNum = levelIndex;            
            throwManager = new ThrowManager();
            font = Game1.contentManager.Load<SpriteFont>("Fonts/Gothic");
            landmarkPositions = new List<GameVector2>();
            resetRadios= new List<Radio>();

            #region Walkie-Talkie
            //walkieTalkies = new List<WalkieTalkie>();
            #endregion

            #region Doors
            doors = new List<Door>();
            #endregion

            #region Landmarks
            landmarks = new List<Landmark>();
            #endregion

            #region Hiding Places
            hidingPlaces = new List<HidingPlace>();
            #endregion Hiding Places

            #region Level SetUp
            levelIndex += 1;
            int folderIndex = levelIndex / 10;
            folderIndex *= 10;
            int max = folderIndex + 9;
            if (!isTutorial)
                LoadFromFile("Content\\Levels\\" + folderIndex + "-" + max + "\\level_" + levelIndex + "\\level" + levelIndex + ".txt");
            else
                LoadFromFile("Content\\Levels\\Tutorial\\Tutorial_" + levelIndex + "\\tutorial" + levelIndex + ".txt");
            MapUnit temp = null;
            // Place the proper room tile under the cooresponding Landmark for each Landmark
            foreach (MapUnit mU in terrain)
            {
                if (mU.getObject().GetType() == typeof (Landmark)) {
                    temp = new MapUnit (AssignRoomTile(mU.x, mU.y), 1, mU.x, mU.y);
                    temp.isExit = mU.isExit;
                    temp.isHideable = mU.isHideable;
                    temp.isWalkable = mU.isWalkable;
                    temp.isSeeThrough = mU.isSeeThrough;

                    if (temp.getObject() != null)
                    {
                        terrain[mU.x, mU.y] = temp;
                    }
                }
            }

            // Place the proper room tile under the cooresponding Landmark for each Landmark
            foreach (MapUnit mU in terrain)
            {
                if (mU.getObject().GetType() == typeof(Landmark))
                {
                    temp = new MapUnit(AssignRoomTile(mU.x, mU.y), 1, mU.x, mU.y);
                    temp.isExit = mU.isExit;
                    temp.isHideable = mU.isHideable;
                    temp.isWalkable = mU.isWalkable;

                    if (temp.getObject() != null)
                    {
                        terrain[mU.x, mU.y] = temp;
                    }
                }
            }

            string password = SynchJanitorToPDoor();

            // Apply proper floor tile under each door
            foreach (Door d in doors)
            {
                //d.ApplyFloorTexture (this);

                if (d.getPassword() != null)
                {
                    d.SetPassword(password);
                }
            }

            foreach (Landmark l in landmarks) {
                if (l.GetType() == typeof(Janitor))
                {
                    Janitor janitor = (Janitor)l;
                    janitor.SetPassword(password);
                }
            }

            setGridNeighbors();
            //Console.WriteLine("Level Index: " + levelIndex);
            Exit.getDoorCenter(ref exits);
            #endregion

            #region Spectres
            spectres = new List<Spectre>();
            player = p;
            makeSpectres(levelIndex, isTutorial);
            #endregion

            #region MacroLevel
            //M_terrain = new MacroMapUnit[height / MacroMapUnit.MAX_SIZE, 
            //                            width / MacroMapUnit.MAX_SIZE];
            //buildClusters();
            #endregion

             watch = new Stopwatch();
             span = new TimeSpan();
            collisionSoundTimer = 0;
            saveData = new LevelSaveData();
            tutString = new TutorialString();
        }

        #region LoadFile

        #region LoadMap
        /// <summary>
        /// Chris Peterson - 1/19/12
        /// Takes in a filename and reads the file, producing a new map
        /// </summary>
        /// <param name="filename"></param>
        public void LoadFromFile(String filename) {
            radios = new List<Radio>();
            exits = new List<Exit>();
            Stream fileStream = TitleContainer.OpenStream(filename);
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream)) {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null) {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }
            height = lines.Count();
            //Allocate the tile grid.
            terrain = new MapUnit[width, lines.Count];

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    char tileType = lines[y][x];
                    terrain[x, y] = LoadMapUnit(tileType, x, y, lines);
                }
            }
            fileStream.Close();
        }

        /// <summary>
        ///  Chris Peterson - 1/19/12
        ///  Contains switch statement for file reading
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private MapUnit LoadMapUnit(char tileType, int x, int y, List<string> lines) {
            MapUnit new_Unit;
            Landmark temp = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox);
            switch (tileType) {
                //Wall
                case 'X':
                    new_Unit = new MapUnit(new Wall(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), false), 200, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                //Floor
                case '-': // Default
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                 Sonar.Floor.FloorType.Default), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '~': // Carpet
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                 Sonar.Floor.FloorType.Carpet), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '.': // Lab
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                 Sonar.Floor.FloorType.Lab), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '_': // Bathroom
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                 Sonar.Floor.FloorType.Bathroom), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case ',': // Kitchen
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                    Sonar.Floor.FloorType.Kitchen), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case ':': // Hardwood
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                    Sonar.Floor.FloorType.Hardwood), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case ';': // Hardwood2
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                    Sonar.Floor.FloorType.Hardwood2), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '"': // Concrete
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                    Sonar.Floor.FloorType.Concrete), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                /*case '*': // Door mat
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),
                                                                    Sonar.Floor.FloorType.DoorMat), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                //Basic Door
                case 'D':
                    Door door = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'd') {
                        y2 = y - 1;
                        x2 = x;
                        door = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'R', "D", new GameVector2(x2, y2));
                    }
                    if (lines[y + 1][x] == 'd') {
                        y2 = y + 1;
                        x2 = x;
                        door = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'L', "D", new GameVector2(x2, y2));
                    }
                    if (lines[y][x - 1] == 'd') {
                        y2 = y;
                        x2 = x - 1;
                        door = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'U', "D", new GameVector2(x2, y2));
                    }
                    if (lines[y][x + 1] == 'd') {
                        y2 = y;
                        x2 = x + 1;
                        door = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'D', "D", new GameVector2(x2, y2));
                    }
                    new_Unit = new MapUnit(door, 2, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isSeeThrough = false;
                    terrain[x2, y2] = new_Unit;
                    doors.Add(door);
                    return new_Unit;
                case 'd':
                    Door doorB = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'D') {
                        y2 = y - 1;
                        x2 = x;
                        doorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'L', "d", new GameVector2(x2, y2));
                    }
                    if (lines[y + 1][x] == 'D') {
                        y2 = y + 1;
                        x2 = x;
                        doorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'R', "d", new GameVector2(x2, y2));
                    }
                    if (lines[y][x - 1] == 'D') {
                        y2 = y;
                        x2 = x - 1;
                        doorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'D', "d", new GameVector2(x2, y2));
                    }
                    if (lines[y][x + 1] == 'D') {
                        y2 = y;
                        x2 = x + 1;
                        doorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), null, 'U', "d", new GameVector2(x2, y2));
                    }
                    new_Unit = new MapUnit(doorB, 2, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isSeeThrough = false;
                    //terrain[x2, y2] = new_Unit;
                    //doors.Add(doorB);
                    return new_Unit;
                //Passworded Door
                case 'P':
                    Door pwdDoor = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'p') {
                        y2 = y - 1;
                        x2 = x;
                        pwdDoor = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'R', "P", new GameVector2(x2, y2));
                    }
                    if (lines[y + 1][x] == 'p') {
                        y2 = y + 1;
                        x2 = x;
                        pwdDoor = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'L', "P", new GameVector2(x2, y2));
                    }
                    if (lines[y][x - 1] == 'p') {
                        y2 = y;
                        x2 = x - 1;
                        pwdDoor = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'U', "P", new GameVector2(x2, y2));
                    }
                    if (lines[y][x + 1] == 'p') {
                        y2 = y;
                        x2 = x + 1;
                        pwdDoor = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'D', "P", new GameVector2(x2, y2));
                    }
                    pwdDoor.SetPassword(SynchPDoorToJanitor());
                    new_Unit = new MapUnit(pwdDoor, 2, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isSeeThrough = false;
                    terrain[x2, y2] = new_Unit;
                    doors.Add(pwdDoor);
                    return new_Unit;
                case 'p':
                    Door pwdDoorB = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'P') {
                        y2 = y - 1;
                        x2 = x;
                        pwdDoorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'L', "p", new GameVector2(x2, y2));
                    }
                    if (lines[y + 1][x] == 'P') {
                        y2 = y + 1;
                        x2 = x;
                        pwdDoorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'R', "p", new GameVector2(x2, y2));
                    }
                    if (lines[y][x - 1] == 'P') {
                        y2 = y;
                        x2 = x - 1;
                        pwdDoorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'D', "p", new GameVector2(x2, y2));
                    }
                    if (lines[y][x + 1] == 'P') {
                        y2 = y;
                        x2 = x + 1;
                        pwdDoorB = new Door(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), new GameVector2(x, y), "-", 'U', "p", new GameVector2(x2, y2));
                    }
                    pwdDoorB.SetPassword(SynchPDoorToJanitor());
                    new_Unit = new MapUnit(pwdDoorB, 2, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isSeeThrough = false;
                    //terrain[x2, y2] = new_Unit;
                    //doors.Add(pwdDoorB);
                    return new_Unit;
                //Exit
                //Travis Carlson
                //2/7//12
                case 'E':
                    Exit tempExit = new Exit(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE));
                    exits.Add(tempExit);
                    new_Unit = new MapUnit(tempExit, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = true;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                //HidingPlace Front
                case 'H':
                    HidingPlace temp1 = new HidingPlace(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'F');
                    hidingPlaces.Add(temp1);
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = true;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                //HidingPlace Left
                case 'h':
                    HidingPlace temp2 = new HidingPlace(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'L');
                    hidingPlaces.Add(temp2);
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = true;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                //HidingPlace Right
                case '#':
                    HidingPlace temp3 = new HidingPlace(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'R');
                    hidingPlaces.Add(temp3);
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = true;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                /*case 'h':
                    SpectreActivate tempActive = new SpectreActivate(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE));
                    hidingPlaces.Add(tempActive);
                    new_Unit = new MapUnit(tempActive, 1, x, y);//new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = true;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                //Off Radio
                case 'R':
                    Radio radio = new Radio(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE));
                    radios.Add (radio);
                    // Account for ramdom type of radio being chosen
                    if (radio.radioNum == 1)
                    {
                        landmarks.Add(new Landmark(radio.getCurrPos(), Landmark.Type.deskNorm));
                    }
                    else
                    {
                        landmarks.Add(new Landmark(radio.getCurrPos(), Landmark.Type.deskNormSide));
                    }
                    //resetRadios.Add(new Radio(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE)));
                    new_Unit = new MapUnit(new Landmark (new GameVector2 (x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                //On Radio
                case 'r':
                    Radio tempRadio = new Radio(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE));
                    tempRadio.DefualtON();
                    radios.Add(tempRadio);
                    // Account for ramdom type of radio being chosen
                    if (tempRadio.radioNum == 1)
                    {
                        landmarks.Add(new Landmark(tempRadio.getCurrPos(), Landmark.Type.deskNorm));
                    }
                    else
                    {
                        landmarks.Add(new Landmark(tempRadio.getCurrPos(), Landmark.Type.deskNormSide));
                    }
                    //resetRadios.Add(tempRadio);
                    //resetRadios.Add (new Radio (new GameVector2 (x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE)));
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                //Throwable
                case 'G':
                    //throwManager.throwingObjects.Add(new Throwable(Game1.contentManager, new GameVector2((x * MapUnit.MAX_SIZE) + MapUnit.MAX_SIZE / 2 + 1, (y * MapUnit.MAX_SIZE) + MapUnit.MAX_SIZE / 2 + 1), 1, 4, null));
                    //throwManager.resetObjects.Add(new Throwable(Game1.contentManager, new GameVector2((x * MapUnit.MAX_SIZE) + MapUnit.MAX_SIZE / 2 + 1, (y * MapUnit.MAX_SIZE) + MapUnit.MAX_SIZE / 2 + 1), 1, 4, null));
                    landmarks.Add(new Landmark(new GameVector2 (x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.deskLab));
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                // Player Spawn
                case 'S':
                    //Set Player Spawn Position
                    startPos = new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE);
                    new_Unit = new MapUnit(new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE),0), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                //Walkie talkie
                case 'W':
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    //throwManager.throwingObjects.Add(new WalkieTalkie(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Game1.contentManager));
                    //throwManager.resetObjects.Add(new WalkieTalkie(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Game1.contentManager));
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                
                // Landmarks

                case 'B': // Dead Body
                    GameTexture tempSprite;
                    if (Game1.random.Next(0, 9) % 2 == 0) tempSprite = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Bodies/dead1");
                    else tempSprite = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Bodies/dead2");
                    Landmark tempBody = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.body);
                    landmarks.Add(tempBody);
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'J': // Janitor
                    Janitor janitorCorpse = new Janitor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.janitor);
                    janitorCorpse.SetPassword(SynchJanitorToPDoor());
                    landmarks.Add(janitorCorpse);
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'j': // Car basic
                    Landmark carBasic = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'Ã')
                    {
                        y2 = y - 1;
                        x2 = x;
                        carBasic = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicB);
                    }
                    if (lines[y + 1][x] == 'Ã')
                    {
                        y2 = y + 1;
                        x2 = x;
                        carBasic = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicF);
                    }
                    if (lines[y][x - 1] == 'Ã')
                    {
                        y2 = y;
                        x2 = x - 1;
                        carBasic = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicR);
                    }
                    if (lines[y][x + 1] == 'Ã')
                    {
                        y2 = y;
                        x2 = x + 1;
                        carBasic = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicL);
                    }
                    landmarks.Add(carBasic);
                    new_Unit = new MapUnit(carBasic, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ã': //                   Car basic Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                /*case '‡': // Car basic right
                    Landmark carBasicR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicR);
                    landmarks.Add(carBasicR);
                    new_Unit = new MapUnit(carBasicR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '†': //                   Car basic right Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                case '‰': // Car sports
                    Landmark carSports = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'Æ')
                    {
                        y2 = y - 1;
                        x2 = x;
                        carSports = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsB);
                    }
                    if (lines[y + 1][x] == 'Æ')
                    {
                        y2 = y + 1;
                        x2 = x;
                        carSports = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsF);
                    }
                    if (lines[y][x - 1] == 'Æ')
                    {
                        y2 = y;
                        x2 = x - 1;
                        carSports = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsR);
                    }
                    if (lines[y][x + 1] == 'Æ')
                    {
                        y2 = y;
                        x2 = x + 1;
                        carSports = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsL);
                    }
                    landmarks.Add(carSports);
                    new_Unit = new MapUnit(carSports, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Æ': //                   Car sports Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                /*case 'Š': // Car sports back
                    Landmark carSportsB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsB);
                    landmarks.Add(carSportsB);
                    new_Unit = new MapUnit(carSportsB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'È': //                   Car sports back Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Œ': // Car sports left
                    Landmark carSportsL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsL);
                    landmarks.Add(carSportsL);
                    new_Unit = new MapUnit(carSportsL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Ç': //                   Car sports left Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Ž': // Car sports right
                    Landmark carSportsR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carSportsR);
                    landmarks.Add(carSportsR);
                    new_Unit = new MapUnit(carSportsR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'É': //                   Car basic right Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                case '™': // Car van
                    Landmark carVan = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'Ê')
                    {
                        y2 = y - 1;
                        x2 = x;
                        carVan = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanB);
                    }
                    if (lines[y + 1][x] == 'Ê')
                    {
                        y2 = y + 1;
                        x2 = x;
                        carVan = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanF);
                    }
                    if (lines[y][x - 1] == 'Ê')
                    {
                        y2 = y;
                        x2 = x - 1;
                        carVan = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanR);
                    }
                    if (lines[y][x + 1] == 'Ê')
                    {
                        y2 = y;
                        x2 = x + 1;
                        carVan = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanL);
                    }
                    landmarks.Add(carVan);
                    new_Unit = new MapUnit(carVan, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ê': //                   Car van Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                /*case 'š': // Car van back
                    Landmark carVanB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanB);
                    landmarks.Add(carVanB);
                    new_Unit = new MapUnit(carVanB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Ë': //                   Car van back Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'œ': // Car van left
                    Landmark carVanL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanL);
                    landmarks.Add(carVanL);
                    new_Unit = new MapUnit(carVanL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Ì': //                   Car van left Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'ž': // Car van right
                    Landmark carVanR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carVanR);
                    landmarks.Add(carVanR);
                    new_Unit = new MapUnit(carVanR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Í': //                   Car van right Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                case 'C': // Chair front
                    Landmark chairF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.chairF);
                    landmarks.Add(chairF);
                    new_Unit = new MapUnit(chairF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'b': // Chair back
                    Landmark chairB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.chairB);
                    landmarks.Add(chairB);
                    new_Unit = new MapUnit(chairB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'c': // Chair left
                    Landmark chairL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.chairL);
                    landmarks.Add(chairL);
                    new_Unit = new MapUnit(chairL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'q': // Chair right
                    Landmark chairR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.chairR);
                    landmarks.Add(chairR);
                    new_Unit = new MapUnit(chairR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case ')': // Couch front
                    Landmark couchF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.couchF);
                    landmarks.Add(couchF);
                    new_Unit = new MapUnit(couchF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '=': // Couch left
                    Landmark couchL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.couchL);
                    landmarks.Add(couchL);
                    new_Unit = new MapUnit(couchL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '+': // Couch right
                    Landmark couchR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.couchR);
                    landmarks.Add(couchR);
                    new_Unit = new MapUnit(couchR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '?': // Couch back
                    Landmark couchB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.couchB);
                    landmarks.Add(couchB);
                    new_Unit = new MapUnit(couchB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'T': // Table
                    Landmark table = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.table);
                    landmarks.Add(table);
                    new_Unit = new MapUnit(table, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'N': // Desk normal
                    Landmark deskNorm = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskNorm);
                    landmarks.Add(deskNorm);
                    new_Unit = new MapUnit(deskNorm, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'n': // Desk normal side
                    Landmark deskNormS = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskNormSide);
                    landmarks.Add(deskNormS);
                    new_Unit = new MapUnit(deskNormS, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Y': // Desk lab
                    Landmark deskLab = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskLab);
                    landmarks.Add(deskLab);
                    new_Unit = new MapUnit(deskLab, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'y': // Desk lab side
                    Landmark deskLabS = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskLabSide);
                    landmarks.Add(deskLabS);
                    new_Unit = new MapUnit(deskLabS, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'I': // Desk office
                    Landmark deskOffice = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskOffice);
                    landmarks.Add(deskOffice);
                    new_Unit = new MapUnit(deskOffice, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'i': // Desk office side
                    Landmark deskOfficeS = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.deskOfficeSide);
                    landmarks.Add(deskOfficeS);
                    new_Unit = new MapUnit(deskOfficeS, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'F': // Fridge front
                    Landmark fridgeF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fridgeF);
                    landmarks.Add(fridgeF);
                    new_Unit = new MapUnit(fridgeF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'f': // Fridge left
                    Landmark fridgeL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fridgeL);
                    landmarks.Add(fridgeL);
                    new_Unit = new MapUnit(fridgeL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'g': // Fridge right
                    Landmark fridgeR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fridgeR);
                    landmarks.Add(fridgeR);
                    new_Unit = new MapUnit(fridgeR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ÿ': // Gym bench
                    Landmark gymBench = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y + 1][x] == 'Î')
                    {
                        y2 = y + 1;
                        x2 = x;
                        gymBench = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymBench);
                    }
                    landmarks.Add(gymBench);
                    new_Unit = new MapUnit(gymBench, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Î': //                   Gym bench Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '¢': // Gym bike
                    Landmark gymBike = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y + 1][x] == 'Ï')
                    {
                        y2 = y + 1;
                        x2 = x;
                        gymBike = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymBike);
                    }
                    landmarks.Add(gymBike);
                    new_Unit = new MapUnit(gymBike, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ï': //                   Gym bike Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '£': // Gym row
                    Landmark gymRow = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y + 1][x] == 'Ð')
                    {
                        y2 = y + 1;
                        x2 = x;
                        gymRow = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymRow);
                    }
                    landmarks.Add(gymRow);
                    new_Unit = new MapUnit(gymRow, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ð': //                   Gym row Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '¤': // Gym tread
                    Landmark gymTread = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymTread);
                    y2 = 0;
                    x2 = 0;
                    if (lines[y + 1][x] == 'Ñ')
                    {
                        y2 = y + 1;
                        x2 = x;
                        gymTread = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymTread);
                    }
                    landmarks.Add(gymTread);
                    new_Unit = new MapUnit(gymTread, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ñ': //                   Gym tread Pt. 2
                    Landmark carBasicR2 = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.carBasicR);
                    new_Unit = new MapUnit(carBasicR2, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '¥': // Gym weights
                    Landmark gymWeights = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.gymWeights);
                    landmarks.Add(gymWeights);
                    new_Unit = new MapUnit(gymWeights, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '|': // Bed
                    Landmark bed = null;
                    y2 = 0;
                    x2 = 0;
                    if (lines[y - 1][x] == 'Ò')
                    {
                        y2 = y - 1;
                        x2 = x;
                        bed = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bedB);
                    }
                    if (lines[y + 1][x] == 'Ò')
                    {
                        y2 = y + 1;
                        x2 = x;
                        bed = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type. bedF);
                    }
                    if (lines[y][x - 1] == 'Ò')
                    {
                        y2 = y;
                        x2 = x - 1;
                        bed = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bedR);
                    }
                    if (lines[y][x + 1] == 'Ò')
                    {
                        y2 = y;
                        x2 = x + 1;
                        bed = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bedL);
                    }
                    landmarks.Add(bed);
                    new_Unit = new MapUnit(bed, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Ò': //                   Bed Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                /*case 'Ô': //                   Bed left Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '!': // Bed right
                    Landmark bedR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bedR);
                    landmarks.Add(bedR);
                    new_Unit = new MapUnit(bedR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Õ': //                   Bed right Pt. 2
                    new_Unit = new MapUnit(temp, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;*/
                case '€': // Stove front
                    Landmark stoveF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.stoveF);
                    landmarks.Add(stoveF);
                    new_Unit = new MapUnit(stoveF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Ä': // Stove back
                    Landmark stoveB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.stoveB);
                    landmarks.Add(stoveB);
                    new_Unit = new MapUnit(stoveB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'ƒ': // Stove left
                    Landmark stoveL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.stoveL);
                    landmarks.Add(stoveL);
                    new_Unit = new MapUnit(stoveL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Å': // Stove right
                    Landmark stoveR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.stoveR);
                    landmarks.Add(stoveR);
                    new_Unit = new MapUnit(stoveR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'O': // Toilet front
                    Landmark toiletF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.toiletF);
                    landmarks.Add(toiletF);
                    new_Unit = new MapUnit(toiletF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '>': // Toilet left
                    Landmark toiletL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.toiletL);
                    landmarks.Add(toiletL);
                    new_Unit = new MapUnit(toiletL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '[': // Toilet right
                    Landmark toiletR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.toiletR);
                    landmarks.Add(toiletR);
                    new_Unit = new MapUnit(toiletR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case ']': // Toilet back
                    Landmark toiletB = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.toiletB);
                    landmarks.Add(toiletB);
                    new_Unit = new MapUnit(toiletB, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'K': // Plant
                    Landmark plant = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.plant);
                    landmarks.Add(plant);
                    new_Unit = new MapUnit(plant, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'k': // Plant fallen 1
                    Landmark plantFallen1 = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.plantFallen1);
                    landmarks.Add(plantFallen1);
                    new_Unit = new MapUnit(plantFallen1, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'V': // Plant fallen 2
                    Landmark plantFallen2 = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.plantFallen2);
                    landmarks.Add(plantFallen2);
                    new_Unit = new MapUnit(plantFallen2, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'U': // Sink front
                    Landmark sinkF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.sinkF);
                    landmarks.Add(sinkF);
                    new_Unit = new MapUnit(sinkF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'u': // Sink left
                    Landmark sinkL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.sinkL);
                    landmarks.Add(sinkL);
                    new_Unit = new MapUnit(sinkL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'v': // Sink right
                    Landmark sinkR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.sinkR);
                    landmarks.Add(sinkR);
                    new_Unit = new MapUnit(sinkR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'M': // File cabinetF
                    Landmark cabinetF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fileCabinetF);
                    landmarks.Add(cabinetF);
                    new_Unit = new MapUnit(cabinetF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'm': // File cabinetL
                    Landmark cabinetL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fileCabinetL);
                    landmarks.Add(cabinetL);
                    new_Unit = new MapUnit(cabinetL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '/': // File cabinetR
                    Landmark cabinetR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fileCabinetR);
                    landmarks.Add(cabinetR);
                    new_Unit = new MapUnit(cabinetR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'Z': // File cabinet fallen left
                    Landmark cabinetFallenL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fileCabinetFallenL);
                    landmarks.Add(cabinetFallenL);
                    new_Unit = new MapUnit(cabinetFallenL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'z': // File cabinet fallen right
                    Landmark cabinetFallenR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.fileCabinetFallenR);
                    landmarks.Add(cabinetFallenR);
                    new_Unit = new MapUnit(cabinetFallenR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case 'w': // Cardboard box
                    Landmark box = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.cardboardBox);
                    landmarks.Add(box);
                    new_Unit = new MapUnit(box, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case 'Q': // Bookshelf front
                    Landmark bookshelfF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfF);
                    landmarks.Add(bookshelfF);
                    new_Unit = new MapUnit(bookshelfF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '@': // Bookshelf left
                    Landmark bookshelfL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfL);
                    landmarks.Add(bookshelfL);
                    new_Unit = new MapUnit(bookshelfL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '$': // Bookshelf right
                    Landmark bookshelfR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfR);
                    landmarks.Add(bookshelfR);
                    new_Unit = new MapUnit(bookshelfR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '%': // Bookshelf fallen front
                    Landmark bookshelfFallenF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfFallenF);
                    landmarks.Add(bookshelfFallenF);
                    new_Unit = new MapUnit(bookshelfFallenF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '&': // Bookshelf fallen left
                    Landmark bookshelfFallenL = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfFallenL);
                    landmarks.Add(bookshelfFallenL);
                    new_Unit = new MapUnit(bookshelfFallenL, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '(': // Bookshelf fallen right
                    Landmark bookshelfFallenR = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.bookshelfFallenR);
                    landmarks.Add(bookshelfFallenR);
                    new_Unit = new MapUnit(bookshelfFallenR, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '{': // Water Tank
                    Landmark waterTank = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.waterTank);
                    landmarks.Add(waterTank);
                    new_Unit = new MapUnit(waterTank, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                case '}': // Generator
                    Landmark generator = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.generator);
                    landmarks.Add(generator);
                    new_Unit = new MapUnit(generator, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                case '<': // Server front
                    Landmark serverF = new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Sonar.Landmark.Type.serverF);
                    landmarks.Add(serverF);
                    new_Unit = new MapUnit(serverF, 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = false;
                    return new_Unit;
                // Lockdown Switch front
                case 'L':
                    LockdownSwitch ldSwitchF = new LockdownSwitch(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'F');
                    lockdownSwitch = ldSwitchF;
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                // Lockdown Switch back
                case '*':
                    LockdownSwitch ldSwitchB = new LockdownSwitch(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'B');
                    lockdownSwitch = ldSwitchB;
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                // Lockdown Switch left
                case 'Ó':
                    LockdownSwitch ldSwitchL = new LockdownSwitch(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'L');
                    lockdownSwitch = ldSwitchL;
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                // Lockdown Switch right
                case '^':
                    LockdownSwitch ldSwitchR = new LockdownSwitch(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), 'R');
                    lockdownSwitch = ldSwitchR;
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = false;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    return new_Unit;
                // Water Droplets
                case '1':
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    new_Unit.createLandmarkSound(SoundManager.ENVIRONMENT.WATER_DROPLET, 100);
                    landmarkPositions.Add(new GameVector2(x, y));
                    return new_Unit;
                // Moaning Person
                case '2':
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    new_Unit.createLandmarkSound(SoundManager.ENVIRONMENT.MOANING_PERSON, 100);
                    landmarkPositions.Add(new GameVector2(x, y));
                    return new_Unit;
                // Computer Hum
                case '3':
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    new_Unit.createLandmarkSound(SoundManager.ENVIRONMENT.COMPUTER_HUM, 100);
                    landmarkPositions.Add(new GameVector2(x, y));
                    return new_Unit;
                // Static Monitor
                case '4':
                    new_Unit = new MapUnit(new Landmark(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), Landmark.Type.cardboardBox), 1, x, y);
                    new_Unit.isWalkable = true;
                    new_Unit.isExit = false;
                    new_Unit.isHideable = false;
                    new_Unit.isSeeThrough = true;
                    new_Unit.createLandmarkSound(SoundManager.ENVIRONMENT.STATIC_MONITOR, 100);
                    landmarkPositions.Add(new GameVector2(x, y));
                    return new_Unit;
                default:
                    return new MapUnit(new Wall(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), false), 1, x, y); //Wall
            }
        }

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Prints the map to the Console
        /// </summary>
        public void PrintMap_Console() {
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    Console.Write(terrain[i, j].toString());
                }
                Console.Write('\n');
            }
        }

        public void setGridNeighbors() {
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (i == 0 || i == width - 1
                     || j == 0 || j == height - 1) {
                        continue;
                    }
                    else {
                        terrain[i, j].neighbors.Add(terrain[i - 1, j - 1]);
                        terrain[i, j].neighbors.Add(terrain[i - 1, j]);
                        terrain[i, j].neighbors.Add(terrain[i - 1, j + 1]);
                        terrain[i, j].neighbors.Add(terrain[i, j + 1]);
                        terrain[i, j].neighbors.Add(terrain[i + 1, j + 1]);
                        terrain[i, j].neighbors.Add(terrain[i + 1, j]);
                        terrain[i, j].neighbors.Add(terrain[i + 1, j - 1]);
                        terrain[i, j].neighbors.Add(terrain[i, j - 1]);
                    }
                }
            }
        }
        #endregion

        #region SpectrePath
        /// <summary>
        /// Chris Peterson - 1/24/12
        /// Reads in the paths of the Spectres, and creates them at initial position
        /// 
        /// File format being spectre_# (# = Level number)
        /// </summary>
        /// <param name="level">The level the player is currently on</param>
        public void makeSpectres(int level, bool isTutorial) {
            int folderIndex = level / 10;
            folderIndex *= 10;
            int max = folderIndex + 9;
            String filename;
            if (!isTutorial)
                filename = string.Format("Content\\Levels\\" + folderIndex + "-" + max + "\\level_" + level + "\\spectre_" + level + ".txt", level);
            else
                filename = string.Format("Content\\Levels\\Tutorial\\" + "tutorial_" + level + "\\spectre_" + level + ".txt", level);
            Stream fileStream = TitleContainer.OpenStream(filename);
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream)) {
                string line;
                List<MapUnit> patrolPaths = new List<MapUnit>();
                line = reader.ReadLine();
                int id = 1;
                while (line != null) {
                    int x = 0, y = 0;
                    int space = line.IndexOf(",");
                    x = Int32.Parse(line.Substring(0, space - 1));
                    y = Int32.Parse(line.Substring(space + 1));
                    patrolPaths.Add(terrain[x, y]);
                    line = reader.ReadLine();
                    switch (line) {
                        case "Siren":
                            Siren temp = new Siren(terrain, patrolPaths, doors, player, id);
                            spectres.Add(temp);//new Siren(terrain, patrolPaths, doors, player, id));
                            terrain[patrolPaths[0].x, patrolPaths[0].y].isWalkable = false;
                            terrain[patrolPaths[0].x, patrolPaths[0].y].isSeeThrough = true;
                            temp.SetDrawTile(AssignRoomTile (patrolPaths[0].x, patrolPaths[0].y)/*terrain[patrolPaths[0].x, patrolPaths[0].y].getObject() as Floor*/);
                            terrain[patrolPaths[0].x, patrolPaths[0].y].objType = temp;
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        case "Dumb":
                            spectres.Add(new Dumb(terrain, patrolPaths, doors, player, id));
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        case "Stalker":
                            spectres.Add(new Stalker(terrain, patrolPaths, doors, player, id));
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        case "Pride":
                            spectres.Add(new Pride(terrain, patrolPaths, doors, player, id));
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        case "Wrath":
                            spectres.Add(new Wrath(terrain, patrolPaths, doors, player, id));
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        case "HideWrath":
                            spectres.Add(new HidingTutorialWrath(terrain, patrolPaths, doors, player, id));
                            patrolPaths.Clear();
                            line = reader.ReadLine();
                            id++;
                            break;
                        default:
                            break;
                    }

                    /*
                    if (line == "" || line == null)
                    {
                        spectres.Add(new Siren(terrain, patrolPaths, player));                        
                        foreach (MapUnit m in patrolPaths)
                        {
                            //Console.WriteLine(m.toString());
                        }
                        patrolPaths.Clear();
                        line = reader.ReadLine();
                    }*/
                }
                patrolPaths.Clear();
            }
            fileStream.Close();
        }
        #endregion

        #endregion

        #region MacroMapUnit


        /// <summary>
        /// MARKED FOR DELETION
        /// Chris Peterson / Derrick Huey - 1/23/12
        /// Creates and Populates the MacroMapUnit array
        /// NOTE: MAY NEED CHANGING
        /// </summary>
        /*private void buildClusters()
        {
            MapUnit[,] temp = new MapUnit[MacroMapUnit.MAX_SIZE,MacroMapUnit.MAX_SIZE];

            for (int i = 0; i < height / MacroMapUnit.MAX_SIZE; i ++) {
                for (int j = 0; j < width / MacroMapUnit.MAX_SIZE; j ++) {
                    for (int k = 0; k < MacroMapUnit.MAX_SIZE; k++)
                    {
                        for (int l = 0; l < MacroMapUnit.MAX_SIZE; l++)
                        {
                            temp[k, l] = terrain[(i * MacroMapUnit.MAX_SIZE) + k,
                                                 (j * MacroMapUnit.MAX_SIZE) + l];
                        }
                    }
                    M_terrain[i, j] = new MacroMapUnit(temp);
                }
            }
        }*/

        #endregion

        #region Collision Detection
        /// <summary>
        /// Ryan Anderson - 1/24/12
        /// Determines if player has collided with a terrain object, and if so, moves player to a location where they are no longer colliding.
        /// </summary>
        private void collide() {
            GameVector2 pos = player.getCurrPos();
            GameVector2 spos = new GameVector2(pos.X, pos.Y);
            GameRectangle rect = player.getBox();
            int X = (int)Util.getInstance().GridCoordinates(pos).X;
            int Y = (int)Util.getInstance().GridCoordinates(pos).Y;


            float lAdj = MapUnit.MAX_SIZE * (X) + rect.Width / 2 - pos.X + 1;
            float rAdj = MapUnit.MAX_SIZE * (X + 1) - rect.Width / 2 - pos.X;
            float tAdj = MapUnit.MAX_SIZE * (Y) + rect.Height / 2 - pos.Y + 1;
            float bAdj = MapUnit.MAX_SIZE * (Y + 1) - rect.Height / 2 - pos.Y;

            if (collisionSoundTimer >= 0) collisionSoundTimer--;

            if (terrain[X, Y].isWalkable) {
                bool side = true;
                if (!terrain[X, (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable) {
                    pos.Y += tAdj;
                    side = false;
                    player.setPos(pos);
                    checkMoving();
                    if (tAdj != 0 && collisionSoundTimer < 0) {
                        VisionManager.addVisionPoint(spos, 150f, false);
                        collisionSoundTimer = collisionSoundTimerReset;
                    }
                }
                if (!terrain[X, (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                    //pos.Y = (float)(MapUnit.MAX_SIZE * (Y + 1) -1);
                    pos.Y += bAdj;
                    side = false;
                    player.setPos(pos);
                    checkMoving();
                    if (bAdj != 0 && collisionSoundTimer < 0) {
                        VisionManager.addVisionPoint(spos, 150f, false);
                        collisionSoundTimer = collisionSoundTimerReset;
                    }
                }
                if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), Y].isWalkable) {
                    //pos.X = (float)(MapUnit.MAX_SIZE * (X + 1) -1);
                    pos.X += lAdj;
                    side = false;
                    player.setPos(pos);
                    checkMoving();
                    if (lAdj != 0 && collisionSoundTimer < 0) {
                        VisionManager.addVisionPoint(spos, 150f, false);
                        collisionSoundTimer = collisionSoundTimerReset;
                    }
                }

                if (!terrain[(int)(rect.Right / MapUnit.MAX_SIZE), Y].isWalkable) {
                    //pos.X = (float)(MapUnit.MAX_SIZE * (X + 1) -1);
                    pos.X += rAdj+1;
                    side = false;
                    player.setPos(pos);
                    checkMoving();
                    if (rAdj != 0 && collisionSoundTimer < 0) {
                        VisionManager.addVisionPoint(spos, 150f, false);
                        collisionSoundTimer = collisionSoundTimerReset;
                    }
                }

                if (side) {
                    if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable) {
                        if (Math.Abs(tAdj) > Math.Abs(lAdj)) pos.X += lAdj;
                        else pos.Y += tAdj;
                        player.setPos(pos);
                        checkMoving();
                        if (lAdj != 0 && tAdj != 0 && collisionSoundTimer < 0) {
                            VisionManager.addVisionPoint(spos, 150f, false);
                            collisionSoundTimer = collisionSoundTimerReset;
                        }
                    }
                    if (!terrain[(int)(rect.Right / MapUnit.MAX_SIZE), (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable) {
                        if (Math.Abs(tAdj) > Math.Abs(rAdj)) pos.X += rAdj;
                        else pos.Y += tAdj;
                        player.setPos(pos);
                        checkMoving();
                        if (rAdj != 0 && tAdj != 0 && collisionSoundTimer < 0) {
                            VisionManager.addVisionPoint(spos, 150f, false);
                            collisionSoundTimer = collisionSoundTimerReset;
                        }
                    }
                    if (!terrain[(int)(rect.Right / MapUnit.MAX_SIZE), (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                        if (Math.Abs(bAdj) > Math.Abs(rAdj)) pos.X += rAdj;
                        else pos.Y += bAdj;
                        player.setPos(pos);
                        checkMoving();
                        if (rAdj != 0 && bAdj != 0 && collisionSoundTimer < 0) {
                            VisionManager.addVisionPoint(spos, 150f, false);
                            collisionSoundTimer = collisionSoundTimerReset;
                        }
                    }
                    if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                        if (Math.Abs(bAdj) > Math.Abs(lAdj)) pos.X += lAdj;
                        else pos.Y += bAdj;
                        player.setPos(pos);
                        checkMoving();
                        if (lAdj != 0 && bAdj != 0 && collisionSoundTimer < 0) {
                            VisionManager.addVisionPoint(spos, 150f, false);
                            collisionSoundTimer = collisionSoundTimerReset;
                        }
                    }
                }
            }
        }

        private void checkMoving()
        {
            if (player.getCurrPos() == player.getPrevPos())
            {
                player.isMoving = false;
            }
            else { player.isMoving = true; }
        }


        /// <summary>
        /// Ryan Anderson - 2/10/12
        /// Determines if a throwable object has collided with a wall, and if so, sets its speed to 0,
        /// and adjusts its position to a location where it is no longer colliding.
        /// </summary>

        private void throwCollide() {
            for (int i = 0; i < player.throwing.Count; i++) {
                if (player.throwing[i].getFlightTime() > 0) {
                    GameVector2 pos = player.throwing[i].getCurrPos();
                    GameRectangle rect = player.throwing[i].getBox();
                    int X = (int)Util.getInstance().GridCoordinates(pos).X;
                    int Y = (int)Util.getInstance().GridCoordinates(pos).Y;
                    float xAdj = (MapUnit.MAX_SIZE * (X + .5f) + 1) - pos.X;
                    float yAdj = (MapUnit.MAX_SIZE * (Y + .5f) + 1) - pos.Y;


                    if (terrain[X, Y].isWalkable) {
                        bool side = true;
                        if (player.throwing[i].orientation.X != 0) {
                            if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), Y].isWalkable || !terrain[(int)(rect.Right / MapUnit.MAX_SIZE), Y].isWalkable) {
                                //pos.X = (float)(MapUnit.MAX_SIZE * (X + 1) -1);
                                pos.X += xAdj;
                                side = false;
                                player.throwing[i].setPos(pos);
                                player.throwing[i].setSpeed(0);
                            }
                        }
                        if (player.throwing[i].orientation.Y != 0) {
                            if (!terrain[X, (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable || !terrain[X, (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                                //pos.Y = (float)(MapUnit.MAX_SIZE * (Y + 1) -1);
                                pos.Y += yAdj;
                                side = false;
                                player.throwing[i].setPos(pos);
                                player.throwing[i].setSpeed(0);
                            }
                        }
                        /*if (!terrain[X, (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable || !terrain[X, (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                            //pos.Y = (float)(MapUnit.MAX_SIZE * (Y + 1) -1);
                            pos.Y += yAdj;
                            side = false;
                            player.throwing[i].setPos(pos);
                            player.throwing[i].setSpeed(0);
                        }
                        if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), Y].isWalkable || !terrain[(int)(rect.Right / MapUnit.MAX_SIZE), Y].isWalkable) {
                            //pos.X = (float)(MapUnit.MAX_SIZE * (X + 1) -1);
                            pos.X += xAdj;
                            side = false;
                            player.throwing[i].setPos(pos);
                            player.throwing[i].setSpeed(0);
                        }*/

                        /*if (side) {
                            if (!terrain[(int)(rect.Left / MapUnit.MAX_SIZE), (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable ||
                                !terrain[(int)(rect.Right / MapUnit.MAX_SIZE), (int)(rect.Top / MapUnit.MAX_SIZE)].isWalkable ||
                                !terrain[(int)(rect.Right / MapUnit.MAX_SIZE), (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable ||
                                !terrain[(int)(rect.Left / MapUnit.MAX_SIZE), (int)(rect.Bottom / MapUnit.MAX_SIZE)].isWalkable) {
                                //Console.WriteLine(xAdj + " " + yAdj);
                                if (Math.Abs(xAdj) > Math.Abs(yAdj)) {
                                    pos.Y += yAdj;
                                    player.throwing[i].setSpeed(0);

                                }
                                else {
                                    pos.X += xAdj;
                                    player.throwing[i].setSpeed(0);
                                }
                                player.throwing[i].setPos(pos);

                            }
                        }*/
                        if (player.throwing[i].positionPrevious == player.throwing[i].position) {
                            player.throwing[i].collideWithWall = true;
                        }
                    }
                }
            }
        }

        ///<sumary>
        /// Travis Carlson - 2/7/12
        /// Check if the player has collided with the exit
        /// Based on Chris's collision method
        /// </sumary>
        /// <param name="pos"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Boolean exited(GameVector2 pos, GameRectangle rect) {
            int X = (int)Util.getInstance().GridCoordinates(rect.Center).X;
            int Y = (int)Util.getInstance().GridCoordinates(rect.Center).Y;
            if (terrain[X, Y].isExit) {
                player.pressedLockdown = false;
                return true;
            }
            return false;
        }

        ///<sumary>
        /// Travis Carlson - 2/27/12
        /// Check if the player is at a tile she can hide in
        /// esentially a copy and paste of the exited class.
        /// </sumary>
        /// <param name="pos"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Boolean canHide(GameVector2 pos, GameRectangle rect) {
            /*int X = (int)Util.getInstance().GridCoordinates(rect.Center).X;
            int Y = (int)Util.getInstance().GridCoordinates(rect.Center).Y;
            if (terrain[X, Y].isHideable) {
                HidingPlace temp = null;
                foreach (HidingPlace hSpot in hidingPlaces)
                {
                    if (hSpot.getCurrPos().X == X * MapUnit.MAX_SIZE && hSpot.getCurrPos().Y == Y * MapUnit.MAX_SIZE)
                    {
                        temp = hSpot;
                    }
                }
                //HidingPlace temp = terrain[X, Y].getObject() as HidingPlace;
                if (temp.getBox().Height > 0 || Player.getInstance().isHiding)*/
            foreach (HidingPlace hSpot in hidingPlaces)
            {
                if (hSpot.Collide(Player.getInstance().getBox()))
                {
                    return true;
                }
            }
            //}
            return false;
        }
        #endregion

        /// <summary>
        /// Steven Ekejiuba
        /// 4/6/2012
        /// Returns the password set for the current level's passworded door.  If door has no password, create one.
        /// </summary>
        public string SynchJanitorToPDoor() {
            string temp = null;
            VoiceEngine engine = VoiceEngine.getInstance();
            //Random rand = Game1.random;
            //int randVal = rand.Next(1, engine.GetDictionary().Count);

            // Find the password for the the current level's passworded door (that of the last passworded door in doors list)
            foreach (Door d in doors) {
                if (d.getPassword() != "-") {
                    temp = d.getPassword();
                }
            }

            // If there was a passworded door found, use it's password
            if (temp != null) {
                return temp;
            }
            // If a passworded door was not found, randomly choose a string from VoiceEngine
            return engine.PasswordPhrase();
        }

        /// <summary>
        /// Steven Ekejiuba
        /// 4/6/2012
        /// Returns the password set for the current level's janitor.  If janitor has no password, create one.
        /// </summary>
        public string SynchPDoorToJanitor() {
            Janitor janitor = null;
            VoiceEngine engine = VoiceEngine.getInstance();
            Random rand = Game1.random;
            int randVal = rand.Next(1, engine.GetDictionary().Count);

            // Find the password that the janitor of the current level has (that of the last janitor in the bodies list)
            foreach (Landmark obj in landmarks) {
                if (obj.GetType() == typeof(Janitor)) {
                    janitor = (Janitor)obj;
                }
            }

            // If there was a janitor corpse found, use it's password
            if (janitor != null) {
                return janitor.GetPassword();
            }

            // If a janitor corpse was not found, randomly choose a string from VoiceEngine
            return engine.GetDictionary().ElementAt(randVal);
        }

        /// <summary>
        /// Steven Ekejiuba
        /// Returns the proper floor tile for the current room
        /// </summary>
        public Floor AssignRoomTile (int x, int y)
        {
            Floor roomTile = null;
            Floor tempTile = null;
            if (levelNum == 0 && !isTutorial)
            {
                Console.Out.WriteLine();
            }
            // Check above, to the right, below, and to the left of current position.  If a tile exists,
            // this is the tile for the current room
            if (terrain.GetUpperBound (1) >= y+1)
            {
                if (terrain[x, y + 1].getObject() != null)
                {
                    if (terrain[x, y + 1].getObject().GetType() == typeof(Floor))
                    {
                        tempTile = (Floor)terrain[x, y + 1].getObject();

                        if (roomTile != null)
                        {
                            // If more than one of a tile type was found, return a new tile of the same type
                            if (roomTile.GetFloorType() == tempTile.GetFloorType())
                            {
                                roomTile = new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), roomTile.GetFloorType());
                                return roomTile;
                            }
                        }
                        roomTile = tempTile;
                    }
                }
            }

            if (terrain.GetUpperBound (0) >= x+1)
            {
                if (terrain[x + 1, y].getObject() != null)
                {
                    if (terrain[x + 1, y].getObject().GetType() == typeof(Floor))
                    {
                        tempTile = (Floor)terrain[x + 1, y].getObject();

                        if (roomTile != null)
                        {
                            // If more than one of a tile type was found, return a new tile of the same type
                            if (roomTile.GetFloorType() == tempTile.GetFloorType())
                            {
                                roomTile = new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), roomTile.GetFloorType());
                                return roomTile;
                            }
                        }
                        roomTile = tempTile;
                    }
                }
            }

            if (terrain.GetLowerBound (1) <= y-1)
            {
                if (terrain[x, y - 1].getObject() != null)
                {
                    if (terrain[x, y - 1].getObject().GetType() == typeof(Floor))
                    {
                        tempTile = (Floor)terrain[x, y - 1].getObject();

                        if (roomTile != null)
                        {
                            // If more than one of a tile type was found, return a new tile of the same type
                            if (roomTile.GetFloorType() == tempTile.GetFloorType())
                            {
                                roomTile = new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), roomTile.GetFloorType());
                                return roomTile;
                            }
                        }
                        roomTile = tempTile;
                    }
                }
            }

            if (terrain.GetLowerBound (0) <= x-1)
            {
                if (terrain[x - 1, y].getObject() != null)
                {
                    if (terrain[x - 1, y].getObject().GetType() == typeof(Floor))
                    {
                        tempTile = (Floor)terrain[x - 1, y].getObject();

                        if (roomTile != null)
                        {
                            // If more than one of a tile type was found, return a new tile of the same type
                            if (roomTile.GetFloorType() == tempTile.GetFloorType())
                            {
                                roomTile = new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), roomTile.GetFloorType());
                                return roomTile;
                            }
                        }
                        roomTile = tempTile;
                    }
                }
            }

            // If only one tile of its kind was found around original position, return it
            // If no tiles were found, which should not be the case, return null
            if (roomTile != null)
            {
                roomTile = new Floor(new GameVector2(x * MapUnit.MAX_SIZE, y * MapUnit.MAX_SIZE), roomTile.GetFloorType());
                return roomTile;
            }
            return roomTile;
        }

        /// <summary>
        /// Travis Carlson
        /// 2/15/12
        /// When the player dies, all spectres in this level reset to their original positions and states
        /// </summary>
        /// <param name="player"></param>
        public void ResetSpectres() {
            foreach (Spectre s in spectres) {
                s.resetPosition();
            }
        }

        /// <summary>
        /// Chris Peterson
        /// 5/9/12
        /// Resets all the tutorial strings
        /// Useful when the player restarts the tutorials
        /// </summary>
        public void ResetAllTutorialStrings() {
            tutString.resetAll();
        }

        /// <summary>
        /// Chris Peterson
        /// 5/9/12
        /// Resets only the tutorialStrings in the current level
        /// </summary>
        public void ResetThisLevelTutStrings() {
            switch (levelNum) {                              
                case WALK_TALK_TUTORIAL:
                    tutString.resetAtIndex(0);
                    tutString.resetAtIndex(1);
                    tutString.resetAtIndex(2);
                    tutString.resetAtIndex(3);
                    tutString.resetAtIndex(17);
                    tutString.resetAtIndex(18);
                    tutString.resetAtIndex(19);
                    break;
                case BASIC_DOOR_TUTORIAL:
                    tutString.resetAtIndex(4);
                    break;
                case PASSWORD_DOOR_TUTORIAL:
                    tutString.resetAtIndex(5);
                    tutString.resetAtIndex(6);
                    tutString.resetAtIndex(7);
                    break;
                case RADIO_OBJECT_TUTORIAL:
                    tutString.resetAtIndex(8);
                    tutString.resetAtIndex(9);
                    tutString.resetAtIndex(10);
                    break;
                case SNEAK_TUTORIAL:
                    tutString.resetAtIndex(11);
                    tutString.resetAtIndex(12);
                    break;
                case POSSESSION_TUTORIAL:
                    tutString.resetAtIndex(13);
                    tutString.resetAtIndex(14);
                    tutString.resetAtIndex(15);
                    tutString.resetAtIndex(16);
                    tutString.resetAtIndex(21);
                    tutString.resetAtIndex(22);
                    break;
                case STALKER_TUTORIAL:
                    
                    tutString.resetAtIndex(20);
                    
                    /*
                    tutString.resetAtIndex(16);
                    tutString.resetAtIndex(17);
                    tutString.resetAtIndex(18);
                    tutString.resetAtIndex(19);
                    */
                    break;
                case LOCKDOWN_SWITCH_TUTORIAL:
                    //tutString.resetAtIndex(20);
                    //tutString.resetAtIndex(21);
                    //tutString.resetAtIndex(22);
                    break;
            }
        }

        /// <summary>
        /// Devon Wyland
        /// 3/6/2012
        /// Resets the location of the throwable objects and clears the screen of debris
        /// </summary>
        public void ResetObjects() {
            throwManager.throwingObjects.Clear();
            throwManager.debris.Clear();
            /*radios.Clear();
            Radio newRadio;
            foreach (Radio r in resetRadios) {
                newRadio = new Radio(r.position);
                if(r.DoesItStartOn()) {
                    newRadio.DefualtON();
                }
                radios.Add(newRadio);
            }*/
            foreach (Radio r in radios)
            {
                r.Reset();
            }
            foreach (Throwable o in throwManager.resetObjects) {
                if (o.GetType() == typeof(WalkieTalkie)) {
                    throwManager.throwingObjects.Add(new WalkieTalkie(o.position, Game1.contentManager));
                }
                else {
                    throwManager.throwingObjects.Add(new Throwable(Game1.contentManager, o.position, o.getHealth(), o.getWeight(), null));
                }
            }
        }

        /// <summary>
        /// Reset all the doors in the game.
        /// </summary>
        public void ResetDoors() {
            foreach (Door d in doors) {
                d.reset();
            }
        }

        /// <summary>
        /// Steven Ekejiuba
        /// 4/17/2012
        /// Resets the state of Lockdown Switches and Elevator
        /// </summary>
        public void ResetExit() {
            lockdownSwitch.Reset();
            Exit.closeDoors(ref exits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time, Player player)
        {
            #region Tutorial
            //if (isTutorial && levelNum != LOCKDOWN_SWITCH_TUTORIAL)
            //    player.pressedLockdown = true;


            #region Password Door
            if (isTutorial && levelNum == PASSWORD_DOOR_TUTORIAL && !tutString.getShownStatus(5)) {
                tutString.setIndex(5);
                tutString.isActive = true;
            }
            #endregion

            #region Radios Tut
            if (isTutorial && levelNum == RADIO_OBJECT_TUTORIAL && !tutString.getShownStatus(8)) {
                tutString.setIndex(8);
                tutString.isActive = true;
            }
            else if (isTutorial && levelNum == RADIO_OBJECT_TUTORIAL && !tutString.getShownStatus(9) && getCurrentUnit().X > 9) {
                tutString.setIndex(9);
                tutString.isActive = true;
            }
            else if (isTutorial && levelNum == RADIO_OBJECT_TUTORIAL && !tutString.getShownStatus(10) && getCurrentUnit().X > 16) {
                tutString.setIndex(10);
                tutString.isActive = true;
            }
            #endregion

            #region Hiding Tut

            if (isTutorial  && levelNum == SNEAK_TUTORIAL 
                            && !tutString.getShownStatus(11) 
                            && getCurrentUnit().X >= 2) {
                tutString.setIndex(11);
                tutString.isActive = true;
            }

            if (isTutorial && levelNum == SNEAK_TUTORIAL
                          && !tutString.getShownStatus(12)
                          && lockdownSwitch.IsPressed()) {
                tutString.setIndex(12);
                tutString.isActive = true;
            }

            #region Tutorial 7

            /*
            if (isTutorial && levelNum == HIDING_TUTORIAL && !tutString.getShownStatus(15) && getCurrentUnit().X >= 3)
            {
                tutString.setIndex(15);
                tutString.isActive = true;
            }
            else if (isTutorial && levelNum == HIDING_TUTORIAL && !tutString.getShownStatus(16) && getCurrentUnit().Y >= 5)
            {
                tutString.setIndex(16);
                tutString.isActive = true;
            }
            else if (isTutorial && levelNum == HIDING_TUTORIAL && !tutString.getShownStatus(17) && getCurrentUnit().X >= 14)
            {
                tutString.setIndex(17);
                tutString.isActive = true;
            }
                /*
            else if (isTutorial && levelNum == HIDING_TUTORIAL && !tutString.getShownStatus(19) && tutString.getShownStatus(18) && player.throwCooldown == true) {
                tutString.setIndex(19);
                tutString.isActive = true;
            }*/

            #endregion

            #endregion

            #region Possession Tut

            if (isTutorial  && levelNum == POSSESSION_TUTORIAL 
                            && !tutString.getShownStatus(13) 
                            && player.GetPossessor() != Player.possessor.none) {
                tutString.setIndex(13);
                tutString.isActive = true;
            }

            if (isTutorial && levelNum == POSSESSION_TUTORIAL
                          && !tutString.getShownStatus(22)) {
                tutString.setIndex(22);
                tutString.isActive = true;
            }                          


            if (isTutorial && levelNum == STALKER_TUTORIAL
                            && !tutString.getShownStatus(20)) 
            {
                tutString.setIndex(20);
                tutString.isActive = true;
            }                            
                /*
            else if (isTutorial && levelNum == POSSESSION_TUTORIAL 
                                && tutString.getShownStatus(13) 
                                && !tutString.getShownStatus(15) 
                                && player.GetPossessor() == Player.possessor.none)
            {
                tutString.setIndex(15);
                tutString.isActive = true;
            }*/
            /*
            if (isTutorial && levelNum == POSSESSION_TUTORIAL
                           && !tutString.getShownStatus(14)
                           && this.lockdownSwitch.IsPressed()
                           && getCurrentUnit().X < 8)                
            {
                tutString.setIndex(14);
                tutString.isActive = true;
            }*/

            if (isTutorial && levelNum == POSSESSION_TUTORIAL
                          && !tutString.getShownStatus(16)                         
                          && getCurrentUnit().X >= 4) {
                tutString.setIndex(16);
                tutString.isActive = true;
            }

            if (isTutorial && levelNum == POSSESSION_TUTORIAL
                          && !tutString.getShownStatus(21)
                          && getCurrentUnit().X >= 7
                          && getCurrentUnit().Y >= 5 ) {
                tutString.setIndex(21);
                tutString.isActive = true;
            }
            
            #endregion
            
            #endregion

            collide();
            throwCollide();
            foreach (GameVector2 location in landmarkPositions) {
                terrain[(int)location.X, (int)location.Y].Update(time);
            }

            //saveData.time += (float)time.ElapsedGameTime.TotalSeconds;
            currentTime += (float)time.ElapsedGameTime.TotalSeconds;

            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = false;
            Player.getInstance().safe = true;

            Player.player.getCurrPos();
            foreach (Spectre s in spectres) {
                s.updateSound();
                s.Update(time, player);
                //Loops through the the debris to see if any spectres step over it.
                foreach (Debris d in throwManager.debris) {
                    if (d.Collide(s.getBox()) && d.soundCD == 0 && (s.positionPrevious != s.position)) {
                        d.soundCD = 25;
                        VisionManager.addVisionPoint(d.position, 1000f, false);
                    }
                }
                if (Player.getInstance().beingSearchedFor || Player.getInstance().beingChased)
                {
                    Player.getInstance().safe = false;
                }
            }



            if (Player.getInstance().beingChased)
            {
                SoundManager.PlayerChase();
            }
            else
            {
                if (Player.getInstance().beingSearchedFor)
                {
                    SoundManager.PlayerSearch();
                }
                else {
                    if (Player.getInstance().safe) SoundManager.PlayerResolve();
                }
            }
        

            #region Radio
            player.radio = null;  
            foreach (Radio r in radios) {
                r.Update(time);
                //player.radio = null;
                //if (player.getBox().Contains(r.getBox().Center))
                if (r.Collide(player.getBox()))
                {
                    player.radio = r;
                }
            }
            #endregion

            #region HidingSpot
            foreach (HidingPlace h in hidingPlaces)
            {
                h.Update(time);

                //if (CheckIfCloseby(player.getBox(), h.getBox()))
                if (h.Collide(player.getBox()))
                {
                    #region TutorialStrings Hiding
                    /*if (isTutorial && levelNum == HIDING_THROW_TUTORIAL && !tutString.getShownStatus(18)) {
                        tutString.setIndex(18);
                        tutString.isActive = true;
                    }*/
                    #endregion
                }                
            }
            #endregion

            #region Throwing Objects
            throwManager.update(time, this);

           /* if(isTutorial){               
                if(levelNum == 6 && tutString.getIndex() != 12){
                    tutString.setIndex(11);
                    tutString.isActive = true;
                }                
            }*/
            
            #endregion

            #region Doors
            playerAtDoor = false;

            if (isTutorial && 
                levelNum == PASSWORD_DOOR_TUTORIAL && 
                !tutString.getShownStatus(6) && 
                getCurrentUnit().X > 4)
            {
                tutString.setIndex(6);
                tutString.isActive = true;
            }


            foreach (Door d in doors) {
                if (d.toString() == "D" || d.toString() == "P") {
                    d.Update(time);
                    if (d.opening) {
                        terrain[(int)d.getIndex().X, (int)d.getIndex().Y].isWalkable = true;
                        terrain[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isWalkable = true;
                        terrain[(int)d.getIndex().X, (int)d.getIndex().Y].isSeeThrough = true;
                        terrain[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isSeeThrough = true;
                        d.opening = false;
                        d.isOpen = true;
                    }
                    else if (!d.isOpen) {
                        terrain[(int)d.getIndex().X, (int)d.getIndex().Y].isWalkable = false;
                        terrain[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isWalkable = false;
                        terrain[(int)d.getIndex().X, (int)d.getIndex().Y].isSeeThrough = false;
                        terrain[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isSeeThrough = false;
                    }
                    if (CheckIfClosebyDoor (player.getBox(), d))
                    {
                        #region TutorialStrings Door
                        if (isTutorial && d.getPassword() == null && levelNum == BASIC_DOOR_TUTORIAL && !tutString.getShownStatus(4)) {
                            tutString.setIndex(4);
                            tutString.isActive = true;
                        }

                        //X: 4
                        #endregion

                        //Console.Out.WriteLine("Collision");
                        playerAtDoor = true;
                        if (player.door == null || player.door != d) {
                            player.door = d;
                            //playerAtDoor = true;
                            autoUnlockTimer = 0;
                        }
                        else {
                            //player.door = null;

                            // If player is at a door that requires a password and has the password, will then do check.
                            // Checks to see if player said the right word and if they are give them time before it auto opens.
                            if (player.door.getPassword() != null && player.carriedPassword != null) {
                                autoUnlockTimer += 1;
                                if (((VoiceEngine.getInstance().SpokenWord() == player.door.getPassword()) &&
                                    VoiceEngine.getInstance().getTimeSinceSpoken() != 0) || autoUnlockTimer > 300) {
                                    if (player.door.locked == true) {
                                        // SoundManager.playSoundFX(player.door.position, 100, 100, 1f, 1, 300, VoiceEngine.getInstance().soundEffect().CreateInstance());
                                        player.door.locked = false;
                                    }
                                }
                            }
                        }
                    }                    

                    // Check spectres to see if they are in the doorway or they have to open/break a door.
                    foreach (Spectre s in spectres) {
                        if (s.behindDoor) {
                            if (s.brokeADoor) {
                                if (d.getIndex().X == s.theDoor.x && d.getIndex().Y == s.theDoor.y ||
                                    d.getOtherHalfOfDoorIndex().X == s.theDoor.x && d.getOtherHalfOfDoorIndex().Y == s.theDoor.y) {
                                    d.Break();
                                    s.brokeADoor = false;
                                    s.theDoor = null;
                                    s.behindDoor = false;
                                }
                            }
                            else if (s.nextToDoor) {
                                if (d.getIndex().X == s.theDoor.x && d.getIndex().Y == s.theDoor.y ||
                                    d.getOtherHalfOfDoorIndex().X == s.theDoor.x && d.getOtherHalfOfDoorIndex().Y == s.theDoor.y) {
                                    d.Interact();
                                    s.nextToDoor = false;
                                    s.theDoor = null;
                                    s.behindDoor = false;
                                }
                            }
                        }
                        if (s.GetType() != typeof(Siren)) {
                            if (CheckIfCloseby(s.getBox(), d.getBox())) {
                                if (!s.isChasing && !s.possessing) {
                                    if (!d.isOpen && !d.locked) {
                                        d.Interact();
                                    }
                                }
                            }
                        }
                        if (s.getBox().Intersects(d.getBox())) {
                            d.isOccupied = true;
                        }
                        /*else {
                            d.isOccupied = false;
                        }*/
                    }
                    if (player.getBox().Intersects(d.doorThreshold)) {
                        d.isOccupied = true;
                    }
                    else {
                        d.isOccupied = false;
                    }
                }
            }

            #region TutorialString Possession
            if (isTutorial) {
                if (player.exorcismPhrase.ACTIVE == true && !tutString.getShownStatus(13) && levelNum == POSSESSION_TUTORIAL) {
                    tutString.setIndex(13);
                    tutString.isActive = true;
                }                
            }
            #endregion

            if (!playerAtDoor) {
                player.door = null;
            }
            #endregion

            #region LockdownSwitches

            if (isTutorial && levelNum == WALK_TALK_TUTORIAL) {
                /*if (!lockdownSwitch.IsPressed() && !tutString.getShownStatus(20))
                {
                    tutString.setIndex(20);
                    tutString.isActive = true;
                }*/
                /*else*/ if(lockdownSwitch.IsPressed() && !tutString.getShownStatus(19)) { //
                    tutString.setIndex(19);
                    tutString.isActive = true;
                }
            }
            // LockdownSwitch            
            if (CheckIfCloseby(player.getBox(), lockdownSwitch.getBox())) {
                player.ldSwitch = lockdownSwitch;
                if (isTutorial && levelNum == WALK_TALK_TUTORIAL)
                {
                    if (!lockdownSwitch.IsPressed() && !tutString.getShownStatus(18)) //
                    {
                        tutString.setIndex(18);
                        tutString.isActive = true;
                    }                    
                }                                
            }
            else {
                player.ldSwitch = null;
            }
            #endregion

            #region TUT_SPEECH

            if (isTutorial && levelNum == WALK_TALK_TUTORIAL && Game1.camera.Zoom.X == 1)
            {
                Console.Write("");
                if(getCurrentUnit().X < 4 && !tutString.getShownStatus(0)){
                    tutString.setIndex(0);
                    tutString.isActive = true;
                }
                else if (getCurrentUnit().X > 4 && !tutString.getShownStatus(1)) {
                    tutString.setIndex(1);
                    tutString.isActive = true;
                }
                else if (getCurrentUnit().Y > 10 && !tutString.getShownStatus(2)){
                    tutString.setIndex(2);                
                    tutString.isActive = true;
                }
                else if (getCurrentUnit().Y > 14){
                    if (!tutString.getShownStatus(17)) //
                    {
                        tutString.setIndex(17);
                        tutString.isActive = true;
                    }
                }
            }

            #endregion            

            #region Janitors
            Janitor janitor;
            foreach (Landmark obj in landmarks)
            {                
                if (obj.GetType() == typeof(Janitor) && CheckIfCloseby(player.getBox(), obj.getBox()))
                {                    
                    janitor = (Janitor)obj;
                    if (player.carriedPassword == null) {
                        player.carriedPassword = janitor.GetPassword();
                        player.passPosition = player.position - Game1.camera.topLeft();
                        player.startDrawingPass = true;
                    }
                    if (isTutorial && levelNum == PASSWORD_DOOR_TUTORIAL && !tutString.getShownStatus(7))
                    {
                        tutString.setIndex(7);
                        tutString.isActive = true;
                    }         
                }
            }

            #endregion

            #region Exit

            // If the player has pressed the lockdown switch, the elevator entrance becomes walkable
            foreach (MapUnit mU in terrain) {
                if (mU.getObject().GetType() == typeof(Exit)) {
                    if (player.pressedLockdown) {
                        mU.isWalkable = true;
                    }
                    else {
                        mU.isWalkable = false;
                    }                    
                }
            }
            #endregion

            Exit.Update(time, player.position, ref exits, player.pressedLockdown);

            //Fixes lingering text after death
            if (player.isDead() && isTutorial) {
                tutString.isActive = false;
                ResetThisLevelTutStrings();
            }
            //GameScreen.shadows.PlaceExitLight(exits[0], exits[1]);
            tutString.Update(time);

        }

        public void SpectreCollisionWithWalkieTalkie(object spectre) {
            //// If Spectre collides with walkie-talkie that is on and making noise, destroy walkie-talkie
            //List<int> removeList = new List<int>();
            //for (int i = 0; i < throwManager.throwingObjects.Count; i++) {
            //    if (throwManager.throwingObjects[i].GetType() == typeof(WalkieTalkie)) {
            //        WalkieTalkie wT = (WalkieTalkie)throwManager.throwingObjects[i];
            //        if (spectre.Collide(wT.getBox(), Sonar.Spectre.Collidable.throwable) && (wT.on) && wT.makingNoise) {                        
            //            removeList.Add(i);
            //        }
            //    }
            //}
            //foreach (int i in removeList) {
            //    throwManager.debris.Add(new Debris(Game1.contentManager, throwManager.throwingObjects.ElementAt(i).position, throwManager.throwingObjects.ElementAt(i).getWeight(), null));
            //    throwManager.throwingObjects.RemoveAt(i);
            //}
            //removeList = null;
        }

        /// <summary>
        /// Steven Ekejiuba
        /// 4/7/2012
        /// Determines if two given boxes are near each other
        /// </summary>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        /// <returns></returns>
        public bool CheckIfCloseby(GameRectangle box1, GameRectangle box2) {
            if (box1.Intersects(new GameRectangle(box2.X - box2.Width / 3,
                                              box2.Y - box2.Height / 3,
                                              (int)(box2.Width + box2.Width / 1.5),
                                              (int)(box2.Height + box2.Height / 1.5)))) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Corey DiMiceli
        /// 5/29/2012
        /// Determines if player is in the hitbox for opening/closing a door. It goes from door mat to door mat.
        /// </summary>
        /// <param name="box1">Intersecting Agent</param>
        /// <param name="box2">The door in question</param>
        /// <returns></returns>
        public bool CheckIfClosebyDoor(GameRectangle box1, Door d) {
            if (box1.Intersects(d.doorHitBox)) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Corey DiMiceli
        /// 5/29/2012
        /// Determines if player is in the hitbox to close the door.
        /// </summary>
        /// <param name="box1">Intersecting Agent</param>
        /// <param name="box2">The door in question</param>
        /// <returns></returns>
        public bool CheckIfInDoor(GameRectangle box1, Door d) {
            if (d.direction == 'U' || d.direction == 'D') {
                d.doorThreshold = new GameRectangle((int)(d.getBox().X),
                                              d.getBox().Y + MapUnit.MAX_SIZE / 2,
                                              (int)(d.getBox().Width),
                                              (int)(d.getBox().Height / 8));
                if (box1.Intersects(d.doorThreshold)) {
                    return true;
                }
            }
            else {
                d.doorThreshold = new GameRectangle(d.getBox().X + MapUnit.MAX_SIZE / 2,
                                              (int)(d.getBox().Y),
                                              (int)(d.getBox().Width / 4),
                                              (int)(d.getBox().Height));
                if (box1.Intersects(d.doorThreshold)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(object batch, object graphics) {
            //batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.camera.viewMatrix);
            //graphics.GraphicsDevice.Clear(GameColor.Black);

            //for (int i = 0; i < width; i++) {
            //    for (int j = 0; j < height; j++) {
            //        terrain[i, j].Draw(batch);
            //    }
            //}

            ////Janitor janitor;
            //foreach (Landmark obj in landmarks) {
            //    obj.Draw(batch);

            //    /*if (obj.GetType() == typeof(Janitor) && CheckIfCloseby(player.getBox(), obj.getBox())) {
            //        //Console.Out.WriteLine("Player pos: " + player.getCurrPos() + "\nBody Pos: " + body.getCurrPos());
            //        janitor = (Janitor)obj;
            //        batch.DrawString(font, janitor.GetPassword(), janitor.getCurrPos() - new GameVector2(16, 16), GameColor.White);                    
            //    }*/
            //}
            //foreach (Door d in doors)
            //{
            //    d.DrawDoorMats (batch);
            //    //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), d.doorThreshold, GameColor.White);
            //}

            //foreach (Spectre s in spectres) {
            //    s.Draw(batch, graphics);
            //}
            //foreach (Radio r in radios) {
            //    r.Draw(batch);
            //}
            //foreach (Exit e in exits) {
            //    // Ensure that entities can move through an open Exit but not a closed one
            //    if (e.underTile == null)
            //    {
            //        e.underTile = AssignRoomTile((int)e.getIndex().X, (int)e.getIndex().Y).GetTexture();
            //    }
            //    /*if (e.GetIsLocked())
            //    {
            //        terrain[(int)e.getIndex().X, (int)e.getIndex().Y].isWalkable = true;
            //    }
            //    else
            //    {
            //        terrain[(int)e.getIndex().X, (int)e.getIndex().Y].isWalkable = false;
            //    }*/
            //    e.Draw(batch);                
            //}

            //lockdownSwitch.Draw(batch);
            //batch.End();
            //batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.camera.viewMatrix);
            //foreach (HidingPlace hSpot in hidingPlaces)
            //{
            //    hSpot.Draw(batch);
            //}
            //batch.End();
            //batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.camera.viewMatrix);
            //foreach (Throwable t in throwManager.throwingObjects) {
            //    t.Draw(batch);
            //}

            //foreach (Debris d in throwManager.debris) {
            //    d.Draw(batch);
            //}
            //tutString.Draw(batch);

            

            //batch.End();
        }

        /// <summary>
        /// Steven Ekejiuba
        /// 4/9/2012
        /// Display text informing player to find lockdown switch
        /// </summary>
        [Obsolete("Obsolete for Unity Port")]
        public void DrawExitText(object batch) {
            //// Player is near exit
            //if (CheckIfCloseby(player.getBox(), exits.ElementAt(0).getBox())) {
            //    exits.ElementAt(0).playerNear = true;
            //}
            //// Player is not near exit
            //else {
            //    exits.ElementAt(0).playerNear = false;
            //}
            ////exits.ElementAt(0).DrawText(batch);
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawText(object batch) {            
            tutString.Draw(batch);
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawWalls(object batch, object graphics) {
            //for (int i = 0; i < width; i++) {
            //    for (int j = 0; j < height; j++) {
            //        if (terrain[i, j].getObject().GetType() == typeof(Wall))
            //            terrain[i, j].Draw(batch);
            //    }
            //}
            //foreach (Exit e in exits) {
            //    e.Draw(batch);
            //}

            //foreach (Door d in doors)
            //{
            //    d.DrawDoorOnly(batch);
            //}
        }
        [Obsolete("Obsolete for Unity Port")]
        public void DrawDoors(object batch, object graphics) {
            //foreach (Door d in doors) {
            //    //d.Draw(batch);
            //}
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawDoorOutlines(object batch, object graphics, object color)
        {
            //foreach (Door d in doors)
            //{
            //    d.DrawOutline(batch, color);
            //}
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawExitOutlines(object batch, object graphics, object color)
        {
            //foreach (Exit e in exits)
            //{
            //    e.DrawOutline(batch);
            //}
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawRadioOutlines(object batch, object graphics, object color)
        {
            //foreach (Radio r in radios)
            //{
            //    r.DrawOutline(batch, color);
            //}
        }

        [Obsolete("Obsolete for Unity Port")]
        public void DrawHidingPlaceOutlines(object batch, object graphics, object color)
        {
            //foreach (HidingPlace h in hidingPlaces)
            //{
            //    h.DrawOutline(batch, color);
            //}
        }

        public GameVector2 getDimensions() {
            return new GameVector2(width, height);
        }

        public void stopSounds() {
            foreach (Spectre spectre in spectres)
                spectre.stopAudio();
            foreach (Radio radio in radios)
                radio.Stop();
        }

        public void pauseSounds() {
            foreach (Spectre spectre in spectres)
                spectre.pauseAudio();
            foreach (Radio radio in radios)
                radio.Pause();
        }
        public void unpauseSounds() {
            foreach (Spectre spectre in spectres)
                spectre.unpauseAudio();
            foreach (Radio radio in radios)
                radio.unPause();
        }
        public MapUnit[,] getTerrain() {
            return terrain;
        }

        /// <summary>
        /// Corey 4/8
        /// For the newly created ThrowManager.
        /// </summary>
        /// <returns></returns>
        public Player getPlayer() {
            return player;
        }

        public LevelSaveData GetSaveData()
        {
            foreach (Spectre s in spectres)
                saveData.alerts += s.alertedCount;
            return saveData;
        }

        public void UpdateSaveData(bool fromPause)
        {
            if (!saveData.completed)
            {
                foreach (Spectre s in spectres)
                    saveData.alerts += s.alertedCount;
                saveData.deaths = currentDeaths;
                saveData.time = currentTime;
            }
            else if (!fromPause)
            {
                foreach (Spectre s in spectres)
                    currentAlerts += s.alertedCount;
                if (currentTime < saveData.time)
                    saveData.time = currentTime;
                if (currentDeaths < saveData.deaths)
                    saveData.deaths = currentDeaths;
                if (currentAlerts < saveData.alerts)
                    saveData.alerts = currentAlerts;
            }
            if (!fromPause)
            {
                currentAlerts = 0;
                currentDeaths = 0;
                currentTime = 0;
                foreach (Spectre s in spectres)
                    s.alertedCount = 0;
            }
        }

        public void SetSaveData(LevelSaveData data)
        {
            saveData = data;
            if (!saveData.completed)
            {
                currentAlerts = saveData.alerts;
                currentDeaths = saveData.deaths;
                currentTime = saveData.time;
            }
        }

        /// <summary>
        /// Corey 4/8
        /// For the newly created ThrowManager.
        /// </summary>
        /// <returns></returns>
        public bool getIsTutorial() {
            return isTutorial;
        }

        public GameVector2 getCurrentUnit()
        {
            return new GameVector2((int)(player.getCurrPos().X) / MapUnit.MAX_SIZE, (int)(player.getCurrPos().Y) / MapUnit.MAX_SIZE);            
        }
        public List<Spectre> getSpectres()
        {
            return this.spectres;
        }
    }
}

