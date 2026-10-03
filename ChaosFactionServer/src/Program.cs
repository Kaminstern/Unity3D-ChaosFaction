using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ChaosFaction
{
    class MainClass
    {
        static Socket listenfd;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        public static void Main(string[] args)
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp);

            // Listen
            listenfd.Listen(0);     // 参数backlog表示队列中最多可容纳等待接受的连接数，0表示不限制
            Console.WriteLine("[服务器] 启动成功");

            // Accept
            listenfd.BeginAccept(AcceptCallback, listenfd);          // 阻塞式，客户端如果没有发消息过来，就一直卡在这

            // 等待
            Console.ReadLine();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[服务器] Accept");
                Socket listenfd = (Socket)ar.AsyncState!;
                Socket clientfd = listenfd.EndAccept(ar);

                // client列表
                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);
                // 接收数据
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

                // 继续Accept
                listenfd.BeginAccept(AcceptCallback, listenfd);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Accept fail {ex.Message}");
            }
        }

        // Recevie回调
        public static void ReceiveCallback(IAsyncResult ar)
        {
            ClientState state = (ClientState)ar.AsyncState;
            Socket clientfd = state.socket;
            try
            {
                int count = clientfd.EndReceive(ar);

                // 客户端关闭
                if (count == 0)
                {
                    MethodInfo mei = typeof(EventHandler).GetMethod("Disconnect");
                    object[] ob = { state };
                    mei.Invoke(null, ob);

                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket close");
                    return;
                }

                string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
                string[] splits = recvStr.Split('|');
                Console.WriteLine($"Receive {recvStr}");
                string msgName = splits[0];
                string msgArgs = splits[1];
                string funName = $"Msg{msgName}";
                MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
                object[] o = { state, msgArgs };
                mi.Invoke(null, o);

                // 广播
                string sendStr = recvStr;
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
                foreach (ClientState cs in clients.Values)
                {
                    cs.socket.Send(sendBytes);
                }
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("Disconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);
                Console.WriteLine($"Socket Receive fail {ex.Message}");
            }
        }

        public static void Send(ClientState sc, string sendStr)
        {
            Socket socket = sc.socket;
            if (socket == null || !socket.Connected) { return; }
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            socket.Send(sendBytes);
        }
    }
}