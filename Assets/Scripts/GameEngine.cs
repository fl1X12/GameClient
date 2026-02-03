//GameEngine.cs

using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameEngine
{
    public int[,] gameMatrix;

    int side=0;
    int gridLen=0;

    public int currTurn=0;
    private int playerIndex=0;
    int movesPlayed=0;
    
    
    bool modeBot=false;
    
    public void InitializeEngine(int side)
    {
        if (GameSetting.currentGameMode == GameMode.PvB)
        {
            modeBot = true;
            Debug.Log("Game Mode: Player vs Bot");
        }
        else if(GameSetting.currentGameMode == GameMode.PnP)
        {
            modeBot = false;
            Debug.Log("Game Mode: Player vs Player");
        }
        
        this.side=side;
        this.gridLen=side*side;
        this.gameMatrix=new int[side,side];
        initMatrix(side);
    }

    void initMatrix(int side)
    {
        int k=0;
        for(int i = 0; i < side; i++)
        {
            for(int j = 0; j < side; j++,k++)
            {
                gameMatrix[i,j]=-1;
            }
        }
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    public void SetPlayerIndex(int index)
    {
        playerIndex=index;
    }

    public void SetElementInMatrix(int index, int currPiece)
    {
        int x = GetRow(index);
        int y = GetCol(index);
        Debug.Log(x+" "+y);
        gameMatrix[x, y] = currPiece;    
    }

    public int GetElementFromMatrix(int index)
    {
        int x = GetRow(index);
        int y = GetCol(index);
        return gameMatrix[x, y];
    }
    bool IsEmpty(int index)
    {
        if(GetElementFromMatrix(index) == -1)
        {
            return true;
        }
        return false;
    }

    public int GetCurrTurn()
    {
        return currTurn;
    }
    
    public void SetCurrTurn(int turn)
    {
        currTurn=turn;
    }
    public int ValidateMove(int index)
    {
        
        if(index<0 || index >= gridLen)
        {
            Debug.Log("index out of bounds");
            return -1;
        }
        if (!IsEmpty(index))
        {
            Debug.Log("tile not empty");
            return -1;
        }

        bool isOnline = (GameSetting.currentGameMode == GameMode.PvP);

        if((modeBot||isOnline) && currTurn != playerIndex)
        {
            Debug.Log("move out of turn");
            return -1;
        }
        return currTurn;
    }

    public int PerformMove(int index,int turn)
    {
        SetElementInMatrix(index,turn);
        movesPlayed++;
        int res=CheckGame(index,turn);
        if (res != -1)
        {
            return res;
        }
        UpdateTurn();      
        return res;  

    }

    public void UpdateTurn()
    {
        currTurn=1-currTurn;
    }

    public bool IsPlayerTurn()
    {
        return currTurn == playerIndex;
    }

    public int CheckGame(int index, int currTurn)
    {
        int x = GetRow(index);
        int y = GetCol(index);
        if(checkRow(x, currTurn) || checkCol(y, currTurn) || checkDiags(x, y, currTurn)) return currTurn;
       
        if(movesPlayed==gridLen) return Gametokens.Draw;
        return -1;
    }
    
    bool checkRow(int x, int currTurn)
    {
        for(int i = 0; i < side; i++)
            if (gameMatrix[x, i] != currTurn) return false;
        return true;
    }

    bool checkCol(int y, int currTurn)
    {
        for(int i = 0; i < side; i++)
            if (gameMatrix[i, y] != currTurn) return false;
        return true;
    }

    bool checkDiags(int x, int y, int currTurn)
    {
        bool OnMainDiag = (x == y);
        bool OnAntiDiag = (x + y == side - 1);

        if(!OnMainDiag && !OnAntiDiag) return false;

        if (OnMainDiag)
        {
            bool mainDiagWin = true;
            for(int i = 0; i < side; i++)
                if (gameMatrix[i, i] != currTurn) { mainDiagWin = false; break; }
            if (mainDiagWin) return true;
        }
    
        if (OnAntiDiag)
        {
            bool antiDiagWin = true;
            for(int i = 0; i < side; i++)
                if (gameMatrix[i, side - 1 - i] != currTurn) { antiDiagWin = false; break; }
            if (antiDiagWin) return true;
        }
        return false;
    }

    //helper methods
    public int GetRow(int index) => index / side;
    public int GetCol(int index) => index % side;


}
