﻿namespace XCraftLib.World.Blocks
{
    public class Crate : Block
    {
        public override byte ID
        {
            get { return 64; }
        }

        public override byte Fallback
        {
            get { return 64; }
        }

        public override bool CPE
        {
            get { return true; }
        }

        public override bool Flammable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "crate"; }
        }

        public override bool Opaque
        {
            get { return true; }
        }

        public override int Permission
        {
            get;
            set;
        }

        public override float Resistance
        {
            get { return 100f; }
        }

        public override bool WaterKill
        {
            get { return false; }
        }

        public override bool Walkthrough
        {
            get { return false; }
        }

        public override short TouchingRadius
        {
            get { return 1; }
        }
    }
}
