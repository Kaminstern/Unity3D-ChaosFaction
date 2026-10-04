using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    // 定义套接字
    static Socket socket;
    // 接收缓冲区
    static byte[] readBuff = new byte[1024];
    // 委托类型
    public delegate void MsgListener(String msg);
    // 监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    // 消息列表
    static List<string> msgList = new List<string>();

    // 跨 Receive 的残留缓冲
    static string buffer = "";

    // 添加监听
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    // 获取描述
    public static string GetDesc()
    {
        if (socket == null) { return ""; }
        if (!socket.Connected) { return ""; }
        return socket.LocalEndPoint.ToString();
    }

    // 连接
    public static void Connect(string ip, int port)
    {
        // Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Connect（用同步方法简化代码）
        socket.Connect(ip, port);
        // BeginReceive
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    // Receive回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket s = (Socket)ar.AsyncState;
            int count = s.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            buffer += recvStr;

            // 循环切出完整消息
            while (true)
            {
                int firstPipe = buffer.IndexOf('|');
                if (firstPipe < 0) break;
                int secondPipe = buffer.IndexOf('|', firstPipe + 1);
                if (secondPipe < 0) break;

                string one = buffer.Substring(0, secondPipe + 1); // 含尾部 |，方便 Update 里 split
                buffer = buffer.Substring(secondPipe + 1);
                msgList.Add(one);
            }

            s.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, s);
        }
        catch (SocketException ex)
        {
            Debug.Log($"Socket Receive fail {ex.Message}");
        }
    }

    // 发送
    public static void Send(string sendStr)
    {
        if (socket == null || !socket.Connected) { return; }
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    // Update，需要外面调用来驱动
    public static void Update()
    {
        // 每帧把所有已到达的完整消息处理完，而不是只处理一条
        while (msgList.Count > 0)
        {
            string msgStr = msgList[0];
            msgList.RemoveAt(0);

            // msgStr 形如 "Enter|127.0.0.1:55273,4,0,-2,0|"
            string[] split = msgStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];

            // 监听回调
            if (listeners.ContainsKey(msgName))
            {
                listeners[msgName](msgArgs);
            }
        }
    }
}
