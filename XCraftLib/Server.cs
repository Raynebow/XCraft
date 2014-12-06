using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using XCraftLib.Entity;
using XCraftLib.Networking;
using XCraftLib.World;

namespace XCraftLib
{
    public static class Server {
        public static bool DebugMode { get; private set; }
        public static List<Level> levels = new List<Level>();
        private static ClientListener listener;

        
        #region SETUP

        public static void Start(bool DEBUG, bool GUI) {
            DebugMode = DEBUG;
            Log("Starting server...", LogMessage.INFO); 
            InitialiseListener();
            if (!listener.Run()) {
                Console.ForegroundColor = ConsoleColor.Red;
                Server.Log("Server shut down, press any key to continue...", LogMessage.ERROR);
                Console.ReadKey();
                return;
            }
            InitialiseDebugSettings();
            CreateDirectories();
            LoadMainLevel();

            Console.ReadKey();
        }

        private static void InitialiseListener() {
            listener = ClientListener.Create(Port);
            listener.OnConnection += AcceptPlayerConnection;
        }

        private static void CreateDirectories() {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            Server.Log("Set up directories!");
        }

        private static void LoadMainLevel() {
            if (!File.Exists("levels/main.cw")) {
                MainLevel = new Level("main", 128, 128, 128);
                MainLevel.Save();
                Server.Log("Main level not found, creating new one!");
            }
            MainLevel = Level.Load("main", LevelFormat.ClassicWorld);
            Server.Log("Loaded main level!");
            levels.Add(MainLevel);
        }

        private static void InitialiseDebugSettings() {
            if (!DebugMode)
                return;
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) => {
                Log(e.Exception.Message, LogMessage.FIRSTCHANCE);
            };
        }

        private static void AcceptPlayerConnection(TcpClient client) {
            Player player = new Player(client);
        }

        #endregion


        public static void Log(string message, LogMessage type = LogMessage.MESSAGE) {
            if (type == LogMessage.FIRSTCHANCE && !DebugMode)
                return;
            Console.WriteLine("[{0}]: {1}", type.ToString(), message);
        }

        #region == PROPERTIES ==

        public static string Name = "XCraft 1.0";
        public static string MOTD = "Crafting all the way";
        public static int MaxClients = 20;
        public static int Port = 25566;

        public static Level MainLevel;

        #endregion
    }
}
