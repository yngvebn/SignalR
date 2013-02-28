using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public static class TypeExtensions
    {
        public static Boolean IsEnumerable(this Type type)
        {
            return type.GetInterfaces()
                        .Where(t => t.IsGenericType)
                        .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Count() > 0;
        }

        public static Type GetEnumerableType(this Type type)
        {
            Type[] typeList = type.GetGenericArguments();

            if (typeList.Length > 0)
            {
                return type.GetGenericArguments()[0];
            }
            else if (type.IsArray)
            {
                return type.GetElementType();
            }

            return typeof(object);
        }
    }
}
