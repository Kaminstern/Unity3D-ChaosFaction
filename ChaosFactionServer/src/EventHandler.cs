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
        }

        public static void MsgList(ClientState state, string msgArgs)
        {
            Console.WriteLine($"MsgList {msgArgs}");
        }
    }
}