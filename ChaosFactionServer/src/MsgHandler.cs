using System.Net;
using System.Net.Sockets;

namespace ChaosFaction
{
    /// <summary>
    /// 消息处理函数类
    /// </summary>
    class MsgHandler
    {
        public static void MsgEnter(ClientState state,string msgArgs)
        {
            // 解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            // 赋值
            state.hp = 100;
            state.x = x;
            state.y = y; 
            state.z = z;
            state.eulY = eulY;
            // 广播
            string sendStr = $"Enter|{msgArgs}|";       // 末尾加 |，作为消息终止符
            foreach (ClientState s in MainClass.clients.Values)
            {
                MainClass.Send(s, sendStr);
            }
            Console.WriteLine($"MsgEnter {msgArgs}");
        }

        public static void MsgList(ClientState state, string msgArgs)
        {
            Console.WriteLine($"MsgList {msgArgs}");
            string sendStr = "List|";
            foreach (ClientState cs in MainClass.clients.Values)     // 所有客户端的信息
            {
                sendStr += $"{cs.socket.RemoteEndPoint.ToString()},";
                sendStr += $"{cs.x},";
                sendStr += $"{cs.y},";
                sendStr += $"{cs.z},";
                sendStr += $"{cs.eulY},";
                sendStr += $"{cs.hp},";
            }
            sendStr += "|";                                  // 末尾加 |，作为消息终止符
            Console.WriteLine($"sendStr:{sendStr}");
            MainClass.Send(state, sendStr);                     // state是需要其它客户端信息的客户端
        }
    }
}