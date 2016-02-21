using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar
{
    public class SpectreActivate : HidingPlace
    {
        public SpectreActivate(GameVector2 init_Pos, char direction)
            : base(init_Pos, direction)
        {
            
        }

        #region Testing
        /// <summary>
        /// Chris Peterson - 1/19/12
        /// </summary>
        /// <returns></returns>
        public new string toString()
        {
            return ")";
        }
        #endregion
    }
}
