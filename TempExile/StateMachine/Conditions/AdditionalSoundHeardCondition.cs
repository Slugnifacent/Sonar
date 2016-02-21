using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AdditionalSoundHeardCondition : Condition
    {
        //This determines if another sound is heard while in investigate so the spectre will go after the new sound.
        public override bool test(Spectre spectre, Player player)
        {
                if ((spectre.objectHeard || spectre.playerBeingHeard) && (GameVector2.Distance(spectre.lastLocationOfNoise, spectre.locationOfNoise) > 100))
                /*if ((spectre.objectHeard || spectre.playerBeingheard) && (spectre.getLastSoundHeard() != spectre.getSoundHeard()) &&
                    (GameVector2.Distance (spectre.locationOfNoise, spectre.getCurrPos()) > GameVector2.Distance (spectre.lastLocationOfNoise, spectre.getCurrPos())))*/
                {
                    //Console.Out.WriteLine("Louder Sound Heard******************************");
                    return true;
                }
                //Console.Out.WriteLine("Sound is not louder");
            //}
            //Console.Out.WriteLine("Equal");
            return false;
        }
    }
}
