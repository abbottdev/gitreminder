using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Splat
{
    public static class MutableDependencyResolverMixins
    {

        public static void Bind<T, TImplementation>(this IMutableDependencyResolver resolver) where TImplementation : new()
        {
            resolver.Register(() => new TImplementation(), typeof(T));
        }

        public static void Bind<T, TImplementation>(this IMutableDependencyResolver resolver, Func<TImplementation> factory)
        {
            resolver.Register(() => factory(), typeof(T));
        }

        public static void BindWithConstructors<T, TImplementation>(this IMutableDependencyResolver resolver) 
        {
            BindingFlags flags = System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public;

            var constructors = typeof(TImplementation).GetConstructors();

            var ordered = constructors.OrderBy(cti => cti.GetParameters().Count()).ToList();

            for (int i = ordered.Count - 1; i >= 0; i--)
            {
                if (CanResolveConstructorParameters(resolver, ordered[i].GetParameters()))
                {
                    resolver.Register(() => InvokeConstructorWithParameters(resolver, ordered[i]), typeof(T));
                    break;
                }
            }

        }

        private static object InvokeConstructorWithParameters(IMutableDependencyResolver resolver, ConstructorInfo constructorInfo)
        {
            var populatedParameters = constructorInfo
                .GetParameters()
                .Select(paramInfo => ResolveParameter(resolver, paramInfo.ParameterType))
                .ToArray();

            return constructorInfo.Invoke(populatedParameters);
        }

        private static bool CanResolveConstructorParameters(IMutableDependencyResolver resolver, ParameterInfo[] parameterInfo)
        {
            if (parameterInfo.Length == 0)
                return true;

            return parameterInfo.AsEnumerable().All(pi => ResolveParameter(resolver, pi.ParameterType) != null);
        }

        private static object ResolveParameter(IMutableDependencyResolver resolver, Type type)
        {
            return resolver.GetService(type);
        }

         

    }
}
