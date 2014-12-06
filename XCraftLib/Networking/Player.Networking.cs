using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XCraft.Networking;
using XCraftLib.Networking;
using XCraftLib.World;

namespace XCraftLib.Entity
{
    public sealed partial class Player
    {
        private TcpClient client;
        private byte[] TempData = new byte[0xFF];
        private byte[] PartialData = new byte[0];
        private Packet Packet;

        private System.Timers.Timer PingTimer = new System.Timers.Timer(2000);

        public Level level {
            get {
                return Server.levels[LevelID];
            }
        }
        private byte LevelID = 0;

        public bool LoggedIn = false;

        public string IP  {
            get {
                if (client != null)
                    return client.Client.RemoteEndPoint.ToString().Split(':')[0];
                else return string.Empty;
            }
        }

        public NetworkStream NetworkStream {
            get {
                return client.GetStream();
            }
        }

        public Player(TcpClient client) {
            this.client = client;
            Server.Log(IP + " connected to the server.");

            NetworkStream.BeginRead(TempData, 0, TempData.Length, Read, this);
            PingTimer.Elapsed += delegate { SendPing(); };
            PingTimer.Start();
        }

        private static void Read(IAsyncResult result) {
            Player p = (Player)result.AsyncState;
            if (p == null) {
                return;
            } else {
                int read = p.NetworkStream.EndRead(result);
                if (read == 0) {
                    // Disconnected
                    return;
                }

                byte[] FullPacket = new byte[p.PartialData.Length + read];
                Buffer.BlockCopy(p.PartialData, 0, FullPacket, 0, p.PartialData.Length);
                Buffer.BlockCopy(p.TempData, 0, FullPacket, p.PartialData.Length, read);

                p.PartialData = p.ProcessData(FullPacket);
            }
        }

        private byte[] ProcessData( byte[] data ) {
            int msgID = data[0], length = 0;
            switch (msgID)  {
                case 0x00: length = 130; break;
                case 0x05: length = 8; break;
                case 0x08: length = 9; break;
                case 0x0D: length = 65; break;
                case 0x10: length = 66; break;
                case 0x11: length = 68; break;
                case 0x13: length = 1; break;
                default: break;
            }

            byte[] tmp = new byte[length];
            byte[] tmp2 = new byte[data.Length - length - 1];
            Buffer.BlockCopy(data, 1, tmp, 0, length);
            Buffer.BlockCopy(data, length + 1, tmp2, 0, data.Length - length - 1);

            switch (msgID) {
                case 0x00: ProcessLogin(tmp); break;
                case 0x05: length = 8; break;
                case 0x08: length = 9; break;
                case 0x0D: length = 65; break;
                case 0x10: length = 66; break;
                case 0x11: length = 68; break;
                case 0x13: length = 1; break;
            }

            return tmp2;
        }

        private void ProcessLogin(byte[] msg) {
            byte protocolVersion = msg[0];
            string Username = Encoding.ASCII.GetString(msg, 1, 64).Trim();
            string VerificationKey = Encoding.ASCII.GetString(msg, 65, 64).Trim();
            byte clientType = msg[129];

            SendID(Server.Name, Server.MOTD, 0x00);
            SendToCurrentLevel();
        }

        private void ProcessBlockchange(byte[] msg) {
            short x = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 0));
            short y = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 2));
            short z = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 4));
            byte mode = msg[6];
            byte type = msg[7];

            if (mode > 1) {

            }

            if (type > 66) {

            }
            level.PlayerBlockchange(this, x, y, z, type, mode);
        }

        private void ProcessMovement(byte[] msg) {
            byte PlayerID = msg[0];
            short x = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 1));
            short y = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 3));
            short z = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 5));
            byte Yaw = msg[7];
            byte Pitch = msg[8];
        }

        private void ProcessMessage(byte[] msg) {
            byte Unused = msg[0];
            string message = Encoding.ASCII.GetString(msg, 1, 64).Trim();
        }

        private void ProcessExtEntry(byte[] msg) {

        }

        private void ProcessExtInfo(byte[] msg) {

        }

        public void SendID(string ServerName, string MOTD, byte UserType) {
            if (UserType != 0x00 || UserType != 0x64)
                UserType = 0x00;
            Packet = new Packet(131);
            Packet.Write(OpCode.ServerIdentification);
            Packet.Write(0x07);
            Packet.Write(ServerName);
            Packet.Write(MOTD);
            Packet.Write(UserType);
            Send(Packet);
        }

        public void SendPing() {
            Packet = new Packet(1);
            Packet.Write(OpCode.Ping);
            Send(Packet);
        }

        public void SendLevelInitialize() {
            Packet = new Packet(1);
            Packet.Write(OpCode.LevelInitialize);
            Send(Packet);
        }

        public void SendToCurrentLevel() {
            SendLevelInitialize();

            byte[] TempData = new byte[level.BlockData.Length + 4];
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.BlockData.Length)).CopyTo(TempData, 0);
            Buffer.BlockCopy(level.BlockData, 0, TempData, 4, level.BlockData.Length);
            TempData = TempData.Compress();
            byte[] tmp;
            byte[] tmp2;
            int loops = (short)(Math.Ceiling(((double)(TempData.Length) / 1024)));

            for (int i = 1; TempData.Length > i; i++)
            {
                short length = (short)Math.Min(TempData.Length, 1024);
                tmp = new byte[length];
                tmp2 = new byte[TempData.Length - length];
                Buffer.BlockCopy(TempData, 0, tmp, 0, length);
                Buffer.BlockCopy(TempData, length, tmp2, 0, TempData.Length - length);
                TempData = tmp2;
                byte percentComplete = (byte)((i * 100 / loops));
                SendLevelDataChunk(length, tmp, percentComplete);
            }
            SendLevelFinalize(level.width, level.depth, level.height);
        }

        public void SendToLevel(Level level) {
            SendToCurrentLevel();
        }

        public void SendLevelDataChunk(short ChunkLength, byte[] ChunkData, byte PercentComplete) {
            Packet = new Packet(4 + 1024);
            Packet.Write(OpCode.LevelDataChunk);
            Packet.Write(ChunkLength);
            Packet.Write(ChunkData);
            Packet.Write(PercentComplete);
            Send(Packet);
        }

        public void SendLevelFinalize(short X, short Y, short Z) {
            Packet = new Packet(7);
            Packet.Write(OpCode.LevelFinalize);
            Packet.Write(X);
            Packet.Write(Y);
            Packet.Write(Z);
            Send(Packet);
        }

        public void SendBlockchange(short x, short y, short z, byte block) {
            Packet = new Packet(8);
            Packet.Write(OpCode.Blockchange);
            Packet.Write(x);
            Packet.Write(y);
            Packet.Write(z);
            Packet.Write(block);
            Send(Packet);
        }

        public void Send(byte[] data)  {
            try  {
                this.NetworkStream.BeginWrite(data, 0, data.Length, delegate(IAsyncResult result) { }, null);
            }
            catch {
                //Failed to send packet
                //Disconnect
            }
        }

        public void Send(Packet packet) {
            Send(packet.Data);
        }
    }
}