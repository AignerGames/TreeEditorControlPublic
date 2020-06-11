using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

using TreeEditorControl.Environment;
using TreeEditorControl.Nodes;

namespace TreeEditorControl.Utility
{
    internal static class TypeUtility
    {
        /// <summary>
        /// Returns the Type.Name but with a space before every upper case char.
        /// </summary>
        /// <param name="type"></param>
        public static string GetTypeDisplayName(Type type)
        {
            var typeName = type.Name;

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(typeName[0]);

            for(var i = 1; i < typeName.Length; ++i)
            {
                var nameChar = typeName[i];

                if(char.IsUpper(nameChar))
                {
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(nameChar);
            }

            return stringBuilder.ToString();
        }

        public static IEnumerable<Type> GetAssignableTypes(Type baseType, Assembly assembly)
        {
            var types = assembly.GetTypes();

            return types.Where(t => baseType.IsAssignableFrom(t));
        }

        public static ConstructorInfo GetConstructorWithOptionalParameters(this Type type, params Type[] parameterTypes)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                if (ValidateConstructorParameterTypes(constructor, parameterTypes))
                {
                    return constructor;
                }
            }

            return null;
        }

        public static bool ValidateConstructorParameterTypes(ConstructorInfo constructorInfo, Type[] parameterTypes)
        {
            var constructorParameters = constructorInfo.GetParameters();
            if (constructorParameters == null || constructorParameters.Length < parameterTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < constructorParameters.Length; ++i)
            {
                if (i < parameterTypes.Length)
                {
                    if (!constructorParameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
                    {
                        return false;
                    }
                }
                else if (!constructorParameters[i].IsOptional)
                {
                    return false;
                }
            }

            return true;
        }

        public static Func<object[], object> GetFactoryFunction(Type type, params Type[] parameterTypes)
        {
            var constructorInfo = type.GetConstructorWithOptionalParameters(parameterTypes);
            if(constructorInfo == null)
            {
                return null;
            }

            Func<object[], object> factoryFunc;

            var constructorParameters = constructorInfo.GetParameters();
            if(constructorParameters.Length == parameterTypes.Length)
            {
                factoryFunc = (parameters) => Activator.CreateInstance(type, parameters);
            }
            else
            {
                // Handle optional parameters
                var optionalParameters = Enumerable.Repeat(Type.Missing, constructorParameters.Length - parameterTypes.Length).ToArray();

                factoryFunc = (parameters) => Activator.CreateInstance(type,                    
                    BindingFlags.CreateInstance |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.OptionalParamBinding, null, parameters.Concat(optionalParameters).ToArray(), CultureInfo.InvariantCulture);
            }

            return factoryFunc;
        }

        public static Func<object> GetEmptyConstructorFactoryFunction(Type type)
        {
            var objectFun = GetFactoryFunction(type, Type.EmptyTypes);
            if(objectFun == null)
            {
                return null;
            }

            return () => objectFun(Type.EmptyTypes);
        }

        public static Func<IEditorEnvironment, ITreeNode> GetNodeFactoryFunction(Type nodeType)
        {
            var nodeFunc = GetFactoryFunction(nodeType, typeof(IEditorEnvironment));
            if (nodeFunc != null)
            {
                return (env) => nodeFunc(new object[] { env }) as ITreeNode;
            }

            // Fallback for empty constructor
            var emptyConstructorFactoryFunction = GetEmptyConstructorFactoryFunction(nodeType);
            if (emptyConstructorFactoryFunction != null)
            {
                return (env) => emptyConstructorFactoryFunction() as ITreeNode;
            }

            return null;
        }

        public static bool CanCreateInstance(Type type, params Type[] types)
        {
            return type.IsClass && !type.IsAbstract && !type.IsGenericType && type.GetConstructorWithOptionalParameters(types) != null;
        }


        public static bool CanCreateInstanceWithEmptyConstructor(Type type)
        {
            return CanCreateInstance(type, Type.EmptyTypes);
        }

        public static bool CanCreateTreeNodeInstance(Type nodeType)
        {
            return CanCreateInstance(nodeType, typeof(IEditorEnvironment)) || CanCreateInstanceWithEmptyConstructor(nodeType);
        }

        public static T GetAttributeFromTypeOrInterface<T>(Type type) where T : Attribute
        {
            var attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }

            var interfaces = type.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                attribute = interfaceType.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;

        }
    }
}
