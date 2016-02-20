using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    abstract public class Environment : Object
    {
        protected float dampFactor;
        protected float pathWeight;

        public override void Update(GameTime gameTime)
        {

        }

        #region Testing
        public string toString()
        {
            return "";
        }
        #endregion
    }
}

