using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace ChaosFaction
{
    /// <summary>
    /// 消息处理函数类
    /// </summary>
    class MsgHandler
    {
        public static void MsgEnter(ClientState state, string msgArgs)
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

        public static void MsgMove(ClientState c, string msgArgs)
        {
            // 解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            // 赋值
            c.x = x;
            c.y = y;
            c.z = z;
            // 广播
            string sendStr = $"Move|{msgArgs}|";
            foreach(ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }

        public static void MsgAttack(ClientState c, string msgArgs)
        {
            // 广播
            string sendStr = $"Attack|{msgArgs}|";
            foreach(ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }

        public static void MsgHit(ClientState c, string msgArgs)
        {
            // 解析参数
            string[] split = msgArgs.Split(',');
            string attDesc = split[0];
            string hitDesc = split[1];
            // 找出被攻击的对象
            ClientState hitCs = null;
            foreach(ClientState cs in MainClass.clients.Values)
            {
                if(cs.socket.RemoteEndPoint.ToString() == hitDesc)
                {
                    hitCs = cs;
                    break;
                }
            }
            if(hitCs == null)
            {
                return;
            }
            // 扣血
            hitCs.hp -= 25;
            // 判定死亡
            if(hitCs.hp <= 0)
            {
                string sendStr = $"Die|{hitCs.socket.RemoteEndPoint.ToString()}";

                sendStr += "|";     // 重新定义了协议，末尾以|作为分隔，防止粘包

                foreach (ClientState cs in MainClass.clients.Values)
                {
                    MainClass.Send(cs, sendStr);
                }
            }
        }
    }
}