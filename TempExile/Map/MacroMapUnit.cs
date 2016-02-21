using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    class MacroMapUnit
    {
        #region Fields

        public static int MAX_SIZE = 10;

        MapUnit[,] members;
        //private List<Tuple<MapUnit, MapUnit>> entrances;

        #endregion

        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/23/12
        /// Constructor for the MacroMapUnit
        /// </summary>
        /// <param name="MicroMap"></param>
        public MacroMapUnit(MapUnit[,] MicroMap)
        {
            members = MicroMap;
            buildEntrances(true, true);
            buildEntrances(true, false);
            buildEntrances(false, true);
            buildEntrances(false, false);
        }

        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/23/12        
        /// Creates the entrances for each MacroMapUnit
        /// </summary>
        /// <param name="isHorizontal"></param>
        /// <param name="isSecond"></param>
        private void buildEntrances(bool isHorizontal, bool isSecond)
        {
            int i, j;
            bool foundStart = false, foundEnd = false;
            MapUnit start = null, end = null;
            for (i = 0; i < MAX_SIZE; i++)
            {
                //If checking the second edge
                if (!isSecond)
                    j = 0;
                else
                    j = MAX_SIZE - 1;

                //If checking horizontal edges
                if (isHorizontal)
                {
                    if (!foundStart && members[j, i].isWalkable)
                    {
                        foundStart = true;
                        start = members[j, i];
                    }
                    else if (foundStart && !foundEnd && (!members[j, i].isWalkable
                                || i == MAX_SIZE - 1))
                    {
                        end = members[j, i - 1];
                        foundEnd = true;
                        foundStart = false;
                        //entrances.Add(new Tuple<MapUnit, MapUnit>(start, end));
                    }
                }
                else
                {
                    if (!foundStart && members[i, j].isWalkable)
                    {
                        foundStart = true;
                        start = members[i, j];
                    }
                    else if (foundStart && !foundEnd && (!members[i, j].isWalkable
                                || i == MAX_SIZE - 1))
                    {
                        end = members[i, j - 1];
                        foundEnd = true;
                        foundStart = false;
                        //entrances.Add(new Tuple<MapUnit, MapUnit>(start, end));
                    }
                }
            }
        }
    }
}
