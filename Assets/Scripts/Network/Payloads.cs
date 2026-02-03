using System;

[Serializable]
public class Message
{
    public string type;
    public string payload;

}


[Serializable]
public class MakeMovePayload
{
    public int index;
}

[Serializable]
public class BroadcastMovePayload
{
    public int index;
    public int playerIndex;
}

[Serializable]
public class GameStartPayload
{
    public int playerIndex;
    public int turn;
}

[Serializable]
public class GameOverPayload
{
    public int winner;
}