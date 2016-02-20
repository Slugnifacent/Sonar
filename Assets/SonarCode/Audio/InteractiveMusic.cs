using UnityEngine;
using System.Collections;


namespace Sonar
{ 
    public class InteractiveMusic {

        static InteractiveMusic interactiveMusic;
        object gObject;
        Sound PlayerBeingChased;
        Sound PlayerBeingSearchedFor;
        Sound TensionResolve;


        public InteractiveMusic(object gObject)
        {

            PlayerBeingChased = SoundManager.getCue(gObject, SoundType.AMBIENT.AMBIANT_PLAYER_SEEN.ToString());
            PlayerBeingSearchedFor = SoundManager.getCue(gObject, SoundType.AMBIENT.AMBIANT_SPECTRE_INVESTIGATE.ToString());
            TensionResolve = SoundManager.getCue(gObject, SoundType.AMBIENT.AMBIANT_TENSION_RESOLVE.ToString());
        }

        public void Stop()
        {
            PlayerBeingChased.STOP();
            PlayerBeingSearchedFor.STOP();
            TensionResolve.STOP();
        }

        public Sound getSound(string Name)
        {
            switch (Name)
            {
                case "AMBIANT_PLAYER_SEEN":
                    return PlayerBeingChased;
                case "AMBIANT_SPECTRE_INVESTIGATE":
                    return PlayerBeingSearchedFor;
                case "AMBIANT_TENSION_RESOLVE":
                    return TensionResolve;
                default:
                    return null;
            }
        }

    }
}
