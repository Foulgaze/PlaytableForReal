using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
public class NetworkManager : MonoBehaviour
{


    TcpClient client;
    NetworkStream rwStream;
    string messageBuffer = "";
    int bufferSize = 4096;



    // Start is called before the first frame update
    
    public int Connect(string server, string username, int port)
    {
        try
        {
            client = new TcpClient(server, port);

            rwStream = client.GetStream();

            SendMessage(username,"00","00");
            return 0;
        }
        catch (ArgumentNullException e)
        {
            Debug.LogError($"ArgumentNullException: {e}");
            return 1;
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
            return 2;
        }
    }

    public string AddMessageSize(string message)
    {
        string msgByteSize = message.Length.ToString();
        if(msgByteSize.Length > 4)
        {
            Debug.LogError("The message was too large");
        }
        while(msgByteSize.Length != 4)
        {
            msgByteSize = "0" + msgByteSize;
        }
        return msgByteSize + message;
    }

    public void SendMessage(string text, string serverOpCode, string clientOpCode) // Sends a message to the server. The send format is {uuid opcode message} The spaces are not present 
    {
        string message = $"{GameManager.Instance._uuid}|{serverOpCode}|{clientOpCode}|{text}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(AddMessageSize(message));
        rwStream.Write(data, 0, data.Length);
    }

    void Update()
    {
        messageBuffer += NetworkReceiver.ReadSocketData(rwStream,bufferSize);
        messageBuffer = NetworkCommandHandler.ParseSocketData(messageBuffer);
    }
}
