using System;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class Connector : MonoBehaviour
{
    [Serializable]
    public class WsMessage
    {
        public string type;
        public string data;
    }

    WebSocket _webSocket;
    static WebSocket webSocket;
    public void HandleMessage(byte[] bytes)
    {
        HandleMessage(System.Text.Encoding.UTF8.GetString(bytes));
    }
    public void HandleMessage(string messageString)
    {        
        WsMessage msg = JsonConvert.DeserializeObject<WsMessage>(messageString);
        Debug.Log("Message Type: " + msg.type);

        if (msg.type == "server")
        {
            Debug.Log(msg.data);
            return;
        }
    }

    //void CreateVehicle(FestaSimulator.Dtos.SystemState.VehicleStateDto v)
    //{
    //    if (vehicles.ContainsKey(v.Vehicle.VehicleId))
    //        return;

    //    ForkLiftController vehicle = Instantiate<ForkLiftController>(forkliftModel, snapshot.transform);
    //    vehicle.transform.position = ConvertPosition(v.X, v.Y, forkliftModel.transform.position);
    //    vehicle.transform.rotation = Quaternion.Euler(0, ConvertAngle(v.Angle), 0);
    //    vehicle.gameObject.SetActive(true);
    //    vehicle.gameObject.name = v.Vehicle.VehicleId;
    //    vehicles.Add(v.Vehicle.VehicleId, vehicle);
    //}
    async void Start()
    {
        //forkliftModel.gameObject.SetActive(false);
        //lcTrainModel.gameObject.SetActive(false);
        //snapshot = transform.Find("Snapshot").gameObject;
        //floorplan = transform.Find("Floorplan").gameObject;

#if UNITY_EDITOR
        if (_webSocket != null)
        {
            await _webSocket.Close();
        }
        ConnectDevWs();
#endif
    }

    public async void ConnectDevWs()
    {
        _webSocket = new WebSocket("ws://localhost:3000");

        _webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            webSocket = _webSocket;
        };

        _webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        _webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        _webSocket.OnMessage += HandleMessage;

        await _webSocket.Connect();
    }


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (_webSocket != null)
        {
            _webSocket.DispatchMessageQueue();
        }
        else
        {
            Start();
        }
#endif
    }

    public async static void SendMessage(string msg)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.SendText(msg);
        }
    }

    private async void OnApplicationQuit()
    {
        if (_webSocket != null)
            await _webSocket.Close();
    }
}