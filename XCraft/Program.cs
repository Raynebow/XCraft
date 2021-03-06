﻿using System;
using System.Diagnostics;
using XCraftLib;

namespace XCraft
{
    public class Program
    {
        public static void Main(string[] args) {
            if (args.Length > 0) {
                bool debug = false;
                bool gui = false;
                try {
                    debug = bool.Parse(args[0]);
                    gui = bool.Parse(args[1]);
                }
                catch {
                    Console.WriteLine("Could not parse the arguments.");
                    Console.WriteLine("Make sure it is as followed: \"xcraft.exe [bool:debug] [bool:gui]\"");
                    Environment.Exit(0);
                }
                Server.Start(debug, gui);
            }
            else {
#if DEBUG
                Server.Start(true, false);
#else
                Server.Start(false, false);
#endif
            }
        }
    }
}
