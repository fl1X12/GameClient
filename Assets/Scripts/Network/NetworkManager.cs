using System;
using System.Collections.Concurrent;
using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    ClientWebSocket ws=new ClientWebSocket();
    //Uri serverUri=new Uri("wss://gameserver-k0ap.onrender.com/ws");

    Uri serverUri=new Uri("ws://localhost:8080/ws");
    ConcurrentQueue<Action> mainThreadQueue= new ConcurrentQueue<Action>();


    //defining events 
    public event Action<GameStartPayload> OnGameStart;
    public event Action<BroadcastMovePayload> OnOppMove;
    public event Action<GameOverPayload> OnGameEnd;
    

    void Awake()
    {
        if(Instance==null) {
            Instance=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        { 
            Destroy(gameObject);
        }
    }

    void Update()
    {
        while(mainThreadQueue.TryDequeue(out var action)) action.Invoke();
    }

    public async Task ConnectAsync()
    {
        if(ws.State == WebSocketState.Open) return;

        if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted || ws.State == WebSocketState.CloseSent)
        {
            Debug.Log("Recreating WebSocket connection...");
            ws.Dispose();
            ws = new ClientWebSocket();
        }

        try
        {
            await ws.ConnectAsync(serverUri,CancellationToken.None);
            Debug.Log("Connected");
            _=ListenForMessages();
        }
        catch (Exception e)
        {
            Debug.LogError("connection Failed : "+e.Message);
        }
    }

    public void FindMatch()
    {
        SendJson(MessageConstants.Find_Match,"{}");
    }

    public void SendMove(int index)
    {
        string json=JsonUtility.ToJson(new MakeMovePayload{index=index});
        SendJson(MessageConstants.Make_Move,json);
    }

    public void SendForfeit()
    {
        Debug.Log("Forfeit");
        SendJson(MessageConstants.Forfeit,"{}");
    }

    async void SendJson(string type, string payload)
    {
        if(ws.State !=WebSocketState.Open) return;
        var msg=new Message{type=type,payload=payload};
        byte[] bytes=Encoding.UTF8.GetBytes(JsonUtility.ToJson(msg));
        await ws.SendAsync(new ArraySegment<byte>(bytes),WebSocketMessageType.Text,true,CancellationToken.None);
    }

    public async void Disconnect()
    {
        if (ws.State == WebSocketState.Open)
        {
            try
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,"user returned to menu",CancellationToken.None);
                Debug.Log("Disconnected");
            }
            catch(Exception e)
            {
                Debug.LogError($"Disconnect error : {e}");
            }
        }
        OnGameStart=null;
        OnOppMove=null;
        OnGameEnd=null;
    }

    public bool IsConnected()
    {
        return ws!=null && ws.State==WebSocketState.Open;
    }
    async Task ListenForMessages()
    {
        try
        {    
            byte[] buffer =new byte[1024];
            while (ws.State == WebSocketState.Open)
            {
                var res=await ws.ReceiveAsync(new ArraySegment<byte>(buffer),CancellationToken.None);
                
                if (res.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("Connection terminated gracefully");

                    if (ws.State == WebSocketState.CloseReceived)
                    {
                        await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure,"Acknowledged",CancellationToken.None);
                    }
                    break;
                }
                string json = Encoding.UTF8.GetString(buffer,0,res.Count);
                mainThreadQueue.Enqueue(()=> ProcessMessage(json));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"exception {e}");
        }
    }

    void ProcessMessage(string json)
    {
        Message msg=JsonUtility.FromJson<Message>(json);
        switch (msg.type)
        {
            case MessageConstants.Game_Start:
                OnGameStart?.Invoke(JsonUtility.FromJson<GameStartPayload>(msg.payload));
                break;
            case MessageConstants.Opp_Move:
                OnOppMove?.Invoke(JsonUtility.FromJson<BroadcastMovePayload>(msg.payload));
                break;
            case MessageConstants.Game_Over:
                OnGameEnd?.Invoke(JsonUtility.FromJson<GameOverPayload>(msg.payload));
                break;
            default:
                Debug.LogWarning($"Unknown message type: {msg.type}");
                break;
        }
    }

    void OnDestroy()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "App closing", CancellationToken.None);
        }
        ws?.Dispose();
    }
}
