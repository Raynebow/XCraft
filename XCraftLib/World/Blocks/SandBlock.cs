﻿namespace XCraftLib.World.Blocks
{
    public class SandBlock : Block
    {
        public override byte ID
        {
            get { return 12; }
        }

        public override byte Fallback
        {
            get { return 12; }
        }

        public override bool CPE
        {
            get { return false; }
        }

        public override bool Flammable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "sand"; }
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
            get { return 500f; }
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
