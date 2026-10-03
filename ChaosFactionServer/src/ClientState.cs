using System.Net;
using System.Net.Sockets;

namespace ChaosFaction
{
    /// <summary>
    /// 客户端状态结构
    /// </summary>
    class ClientState
    {
        // TCP连接所需Socket
        public Socket socket;
        // 填充BeginReceive参数的读缓冲区
        public byte[] readBuff = new byte[1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;
    }
}