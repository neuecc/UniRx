using System;

namespace RuntimeUnitTestToolkit
{
    public sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}
