using System;

namespace XCraftLib.Entity
{
    public abstract class Entity
    {
        /// <summary>
        /// Walks to a specified point with a given speed
        /// </summary>
        public abstract void Walk(short x, short y, short z, float speed);

        /// <summary>
        /// Teleports an entity to a point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public abstract void Teleport(short x, short y, short z);

        /// <summary>
        /// The attack AI of the entity
        /// </summary>
        public abstract void Attack(Entity target);

        /// <summary>
        /// The unique ID of the entity
        /// </summary>
        public abstract int ID { get; set; }

        /// <summary>
        /// The name of the entity
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// The skin of the entity
        /// </summary>
        public abstract string Skin { get; set; }

        /// <summary>
        /// The model of the entity;
        /// </summary>
        public abstract string Model { get; set; }

        /// <summary>
        /// Determines whether the entity is an npc or not
        /// </summary>
        public abstract bool NPC { get; }
    }
}
