using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TestRole: IdentityRole
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public TestRole():base()
        { }

        public TestRole(string name):base(name)
        { }

        public override bool Equals(object obj)
        {
            if (obj is IdentityRole<string> other)
            {
                return other.Id == Id
                    && other.Name == Name;
            }

            return false;
        }
    }
}
