using System.Net;
using System.Net.Sockets;

namespace ChaosFaction
{
    /// <summary>
    /// 事件处理类
    /// </summary>
    class EventHandler
    {
        // 处理玩家下线
        public static void OnDisconnect(ClientState state)
        {
            Console.WriteLine($"OnDisconnect");
            string desc = state.socket.RemoteEndPoint.ToString();
            string sendStr = $"Leave|{desc},|";
            foreach(ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }
    }
}