using UnityEngine;
using System.Collections;

public partial class Audio {

    protected GameObject gObject;
    protected uint ID;

    protected Audio(GameObject GameObj, string Name)
    {
        gObject = GameObj;
        ID = AkSoundEngine.GetIDFromString(Name);
    }

    protected void PLAY()
    {
        AkSoundEngine.PostEvent(ID, gObject); 
    }

    protected virtual void STOP()
    {
        AKRESULT result = AkSoundEngine.ExecuteActionOnEvent(ID, AkActionOnEventType.AkActionOnEventType_Stop);
    }

    protected virtual void PUASE()
    {
        AKRESULT result = AkSoundEngine.ExecuteActionOnEvent(ID, AkActionOnEventType.AkActionOnEventType_Stop);
    }

    protected static void PostEvent()
    {

    }

    private static void LoadSoundBank(string SoundBankName)
    {
        AkBankManager.LoadBank(SoundBankName);
    }

    private static void UnloadSoundBank(string SoundBankName)
    {
        AkBankManager.UnloadBank(SoundBankName);
    }
}
