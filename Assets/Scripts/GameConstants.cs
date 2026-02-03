using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public static class GameConstants
{
    public const float Move_Duration=20.0f;
    
}

public static class MessageConstants
{
    public const string Find_Match="find_match";
    public const string Make_Move="make_move";
    public const string Forfeit="forfeit";

    public const string Game_Start="GAME_START";
    public const string Opp_Move="OPP_MOVE";

    public const string Game_Over="GAME_OVER";
}

public static class Gametokens
{
    public const int X=1;
    public const int O=0;
    public const int Draw=2;
}