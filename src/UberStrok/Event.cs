using System;
using System.Collections.Generic;
using System.Reflection;

namespace UberStrok
{
    /* An empty abstract class, for the sake of strong typings. */
    public abstract class Event
    {
        /* 
            Maps object type to a dictionary which maps Event types to their methodinfo in
            the object type. 
         */
        private static readonly Dictionary<Type, Dictionary<Type, MethodInfo>> _cache = new Dictionary<Type, Dictionary<Type, MethodInfo>>();

        internal static Dictionary<Type, MethodInfo> GetEvents(Type type)
        {
            var onEventMethods = default(Dictionary<Type, MethodInfo>);

            /* Try to look up in the cache. */
            if (_cache.TryGetValue(type, out onEventMethods))
                return onEventMethods;

            /* Otherwise we look for them through the type instance and cache the result. */
            onEventMethods = new Dictionary<Type, MethodInfo>();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                /* 
                    Look for methods which has the name "OnEvent, returns a void 
                    (returns no value) and has 1 parameter whose type inherits from Event.
                 */
                if (method.Name != "OnEvent" || method.ReturnType != typeof(void))
                    continue;

                /* Check parameter count. */
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                /* Check parameter return type. */
                var parameter = parameters[0];
                if (!parameter.ParameterType.IsSubclassOf(typeof(Event)))
                    continue;

                onEventMethods.Add(parameter.ParameterType, method);
            }

            _cache.Add(type, onEventMethods);
            return onEventMethods;
        }
    }
}
