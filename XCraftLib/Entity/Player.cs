using System.Collections.Generic;

namespace XCraftLib.Entity
{
    public sealed partial class Player : Entity
    {
        public static List<Player> players = new List<Player>();

        public override string Name { get; set; }
        public override string Skin { get; set; }
        public override string Model { get; set; }
        public override bool NPC
        {
            get { return false; }
        }

        public override void Attack(Entity target) {
            throw new System.NotImplementedException();
        }

        public override void Teleport(short x, short y, short z) {
            throw new System.NotImplementedException();
        }

        public override void Walk(short x, short y, short z, float speed) {
            throw new System.NotImplementedException();
        }
    }
}