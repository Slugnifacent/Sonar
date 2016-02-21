
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class MapUnit
    {
        #region Fields

        public static int MAX_SIZE = 48;

        /*
            Neighbors start in Top-Left and go Clockwise
         */
        public List<MapUnit> neighbors { get; set; }
        public Object objType { get; set; }
        public int weight { get; set; }
        public MapUnit parent { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        #region Pathvariables
        public int f { get; set; }
        public int g { get; set; }
        public int h { get; set; }
        #endregion
        private bool drawable { get; set; }
        public bool isWalkable { get; set; }
        public bool isSeeThrough { get; set; }
        public bool isExit { get; set; }
        public bool isHideable { get; set; }
        object/*dynamic*/ landmarkEnumeration;
        int landmarkTimer;
        int landmarkTimerMax;
        public bool marked;
        #endregion

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Constructor for a MapUnit
        /// </summary>
        /// <param name="new_Obj"></param>
        public MapUnit(Object newObj, int newWeight, int new_x, int new_y)
        {
            objType = newObj;
            weight = newWeight;
            neighbors = new List<MapUnit>();
            x = new_x;
            y = new_y;
        }

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Initializer for the MapUnit class
        /// </summary>
        /// <param name="maxSize"></param>
        private void Initialize(int maxSize)
        {

        }

        public object getObject()
        {
            return objType;
        }

        public GameVector2 GetPosition()
        {
            return new GameVector2((x * MAX_SIZE) + (MAX_SIZE / 2), (y * MAX_SIZE) + (MAX_SIZE / 2));

        }

        /// <summary>
        /// Derrick
        /// Resets weight/cost values
        /// </summary>
        public void resetFGH()
        {
            f = g = h = 0;
            parent = null;
        }

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Updates the state of the current MapUnit
        /// </summary>
        /// <param name="time"></param>
        public void Update()
        {
            //landmarkTimer--;
            //if (landmarkEnumeration != null)
            //{
            //    landmarkTimer = (int)Util.getInstance().wrap(landmarkTimer, 0, landmarkTimerMax);
            //    if (landmarkTimer == landmarkTimerMax) SoundManager.createSound(Util.getInstance().PositionCoordinates(new GameVector2(x, y)), 100, 100, 1, landmarkEnumeration, false);
            //}
        }

        public void createLandmarkSound(object/*dynamic*/ SoundEnumeration, int Timer) {
            landmarkEnumeration = SoundEnumeration;
            landmarkTimer = landmarkTimerMax = Timer;
        }

        /// <summary>
        /// Chris Peterson - 1/18/12
        /// Draws the current state of the MapUnit to the screen
        /// </summary>
        /// <param name="batch"></param>
        [Obsolete("Obsolete for Unity Port")]
        public void Draw()
        {
            //if (GameVector2.Distance(GetPosition(), Player.getInstance().position) < 300) 
            //objType.Draw(batch);
        }

        #region Testing
        public string toString()
        {
            string s = "X:" + x + " Y: " + y;
            return s;
            //return objType.toString();
        }
        #endregion
    }
}

