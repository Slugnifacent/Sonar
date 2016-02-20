using UnityEngine;
using System.Collections;

public class SoundType {

    #region Environment <Environment Sounds>
    public enum ENVIRONMENT : sbyte
    {
        ENVIRONMENT_COMPUTER_HUM,
        ENVIRONMENT_COMPUTER_TYPING,
        ENVIRONMENT_DOOR_CLOSE,
        ENVIRONMENT_DOOR_CRASH,
        ENVIRONMENT_DOOR_OPEN,
        ENVIRONMENT_DYING_PERSON,
        ENVIRONMENT_MOANING_PERSON,
        ENVIRONMENT_STATIC_MONITOR,
        ENVIRONMENT_WATER_DROPLET
    }
    #endregion Environment

    #region PLAYER <PLAYER Sounds>
    public enum PLAYER : sbyte
    {
        PLAYER_BODY_FALL,
        PLAYER_BREATHING,
        PLAYER_DEATH,
        PLAYER_FOOTSTEP_CARPET,
        PLAYER_FOOTSTEP_CONCRETE,
        PLAYER_FOOTSTEP_HARDWOOD,
        PLAYER_FOOTSTEP_TILE,
        PLAYER_HEARTBEAT,
        PLAYER_PLAYER_HIT,
    }
    #endregion PLAYER

    #region RADIO <RADIO Sounds>
    public enum RADIO : sbyte
    {
        RADIO,
        RADIO_BUTTON_ON,
        RADIO_BUTTON_OFF
    }
    #endregion RADIO

    #region SPECTRE <SPECTRE Sounds>

    public enum DUMB : sbyte
    {
        DUMB_ALERT,
        DUMB_EXCORCISED,
        DUMB_FOOTSTEP,
        DUMB_GROWL,
        DUMB_GRUNT,
        DUMB_HEAR_PLAYER,
        DUMB_POSSESSED,
        DUMB_POSSESSION_BREATHING,
        DUMB_ROAR
    }

    public enum PRIDE : sbyte
    {
        PRIDE_EXCORCISED,
        PRIDE_HEAR_PLAYER,
        PRIDE_LAUGH_WHISTPER
    }

    public enum SIREN : sbyte
    {
        SIREN_HEAR_PLAYER,
        SIREN_SCREAM,
        SIREN_SNORE,
        SIREN_WARNING
    }

    public enum STALKER : sbyte
    {
        STALKER_HEAR,
        STALKER_EXCORCISED,
        STALKER_ROAR
    }

    public enum WRATH : sbyte
    {
        WRATH_FOOTSTEP,
        WRATH_GROWL,
        WRATH_HEAR_PLAYER,
        WRATH_HIT_PLAYER,
        WRATH_IDLE,
        WRATH_PRE_SWING,
        WRATH_ROAR
    }


    #endregion SPECTRE

    #region THROWABLE <THROWABLE Sounds>
    public enum THROWABLE : sbyte
    {
        THROWABLE_GLASS_BREAKING,
        THROWABLE_GLASS_COLLISION,
        THROWABLE_GLASS_CRUNCHING,
        THROWABLE_GLASS_PICKUP
    }
    #endregion THROWABLE

    #region XENIA <XENIA Sounds>
    public enum XENIA : sbyte
    {
        XENIA_WHISPER
    }
    #endregion XENIA

    #region MISC <MISCELLANEOUS Sounds>
    public enum MISC : sbyte
    {
        MISC_MENU_OPENING,
        MISC_AMBIENCE_MACHINERY,
        MISC_STUFF_CRASHING
    }
    #endregion MISC

    #region AMBIENT <AMBIANT Sounds>
    public enum AMBIENT : sbyte
    {
        AMBIANT_PLAYER_SEEN,
        AMBIANT_SPECTRE_INVESTIGATE,
        AMBIANT_TENSION_RESOLVE
    }
    #endregion AMBIENT

    #region ELEVATOR <ELEVATOR Sounds>
    public enum ELEVATOR : sbyte
    {
        ELEVATOR_BEEP,
        ELEVATOR_CLOSE,
        ELEVATOR_DING,
        ELEVATOR_MUSIC,
        ELEVATOR_OPEN,
    }
    #endregion ELEVATOR

    #region LOCKDOWN <LOCKDOWN Sounds>
    public enum LOCKDOWN : sbyte
    {
        LOCKDOWN_LEVEL_ACCESSIBLE,
        LOCKDOWN_LEVEL_ONE,
        LOCKDOWN_LEVEL_TWO,
        LOCKDOWN_LEVEL_THREE,
        LOCKDOWN_LEVEL_FOUR,
        LOCKDOWN_LEVEL_FIVE,
        LOCKDOWN_LEVEL_SIX,
        LOCKDOWN_LEVEL_SEVEN,
        LOCKDOWN_LEVEL_EIGHT,
        LOCKDOWN_LEVEL_NINE,
        LOCKDOWN_LEVEL_TEN,
        LOCKDOWN_LEVEL_ELEVEN,
        LOCKDOWN_LEVEL_TWELVE,
        LOCKDOWN_LEVEL_THIRTEEN,
        LOCKDOWN_LEVEL_FOURTEEN,
        LOCKDOWN_LEVEL_FIFTEEN,
        LOCKDOWN_LEVEL_SIXTEEN,
        LOCKDOWN_LEVEL_SEVENTEEN,
        LOCKDOWN_LEVEL_EIGHTEEN,
        LOCKDOWN_LEVEL_NINETEEN,
        LOCKDOWN_LEVEL_TWENTY,
        LOCKDOWN_LEVEL_TWENTY_ONE,
        LOCKDOWN_LEVEL_TWENTY_TWO,
        LOCKDOWN_LEVEL_TWENTY_THREE,
        LOCKDOWN_LEVEL_TWENTY_FOUR,
        LOCKDOWN_LEVEL_TWENTY_FIVE,
        LOCKDOWN_LEVEL_TWENTY_SIX,
        LOCKDOWN_LEVEL_TWENTY_SEVEN,
        LOCKDOWN_LEVEL_TWENTY_EIGHT,
        LOCKDOWN_LEVEL_TWENTY_NINE,
        LOCKDOWN_LEVEL_THIRTY,
        LOCKDOWN_LEVEL_DEACTIVATED,
        LOCKDOWN_LEVEL_TUTORIAL
    }
    #endregion LOCKDOWN

    #region AudioCategory <Sound Categories>
    public enum AUDIOCATEGORY : sbyte
    {
        AUDIOCATEGORY_AMBIENCE,
        AUDIOCATEGORY_MENU,
        AUDIOCATEGORY_RADIO,
        AUDIOCATEGORY_SOUNDEFFECT
    }
    #endregion AudioCategory

}
