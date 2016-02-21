using UnityEngine;
using System.Collections;


namespace Sonar
{
    public class GameRectangle
    {
        Bounds bound;

        public GameRectangle(Bounds Bound)
        {
            bound = Bound;
        }

        public float X
        {
            get { return bound.center.x - bound.extents.x; }
        }


        public float Y
        {
            get { return bound.center.y - bound.extents.y; }
            set { value = value; }
        }

        public float Width
        {
            get { return bound.size.x; }
        }

        public float Height
        {
            get { return bound.size.y; }
        }
    }
}
