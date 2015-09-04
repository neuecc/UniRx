using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public static class TypeLoader
    {
        public static Type GetType(string typeName)
        {
            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                    from type in asm.GetTypes()
                    where type.Name == typeName
                    select type).FirstOrDefault();
        }
    }
}
