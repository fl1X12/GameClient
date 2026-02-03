using UnityEngine;
public enum GameMode
{
    PnP,
    PvB,
    PvP
}


public static class GameSetting
{
    public static GameMode currentGameMode=GameMode.PvB;
    public static int boardSide=3;
}
