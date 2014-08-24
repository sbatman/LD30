using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerfCorev2.PhysicsSystem.Dynamics;
using NerfCorev2.PhysicsSystem.Dynamics.Contacts;

namespace LD30.Logic
{
    class Phys
    {
        public Fixture PhysicsFixture;

        internal Phys(Fixture fixture)
        {
            PhysicsFixture = fixture;
            PhysicsFixture.OnCollision += PerciseOnCollision;
            PhysicsFixture.OnSeparation += PerciseOnSeparation;
            PhysicsFixture.AfterCollision += AfterCollision;
        }
        internal void SetCollisionGroup(int id)
        {
            PhysicsFixture.CollidesWith = Category.None | (Category)Math.Pow(2, id - 1);
            PhysicsFixture.CollisionCategories = (Category)Math.Pow(2, id - 1);
        }

        private bool PerciseOnCollision(Fixture fp1, Fixture fp2, Contact contact)
        {
            return true;
        }
        internal static bool CollisionBetween(Fixture fp1, Fixture fp2, Type t1, Type t2)
        {

            if (fp1.UserData == null) return false;
            if (fp2.UserData == null) return false;

            if (t1.IsInstanceOfType(fp1.UserData))
            {
                if (t2.IsInstanceOfType(fp2.UserData))
                {
                    return true;
                }
            }
            if (t1.IsInstanceOfType(fp2.UserData))
            {
                if (t2.IsInstanceOfType(fp1.UserData))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool CollisionBetween(Fixture fp1, Fixture fp2, Type t1, IEnumerable<Type> t2S)
        {

            if (fp1.UserData == null) return false;
            if (fp2.UserData == null) return false;

            if (t1.IsInstanceOfType(fp1.UserData))
            {
                if (t2S.Any(t2 => t2.IsInstanceOfType(fp2.UserData)))
                {
                    return true;
                }
            }
            if (t1.IsInstanceOfType(fp2.UserData))
            {
                return t2S.Any(t2 => t2.IsInstanceOfType(fp1.UserData));
            }
            return false;
        }

        private void AfterCollision(Fixture fp1, Fixture fp2, Contact contact)
        {

        }

        private void PerciseOnSeparation(Fixture fp1, Fixture fp2)
        {

        }
    }
}
