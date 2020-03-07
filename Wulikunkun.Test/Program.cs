using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

namespace Wulikunkun.Test
{
    class Program
    {
        // 设计理念，注册就是将一个被注册类型(Type)绑定一个注册条目(Registry)
        static void Main(string[] args)
        {
            Container container = new Container();
            object obj = container.GetOrCreate<Person>();
            Console.Read();
        }
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionAttribute : Attribute
    {
    }

    public enum LifeTime
    {
        Transient,
        Scope,
        Root
    }

    public class Foo : IFoo<Container, Registry>
    {
    }

    public interface IFoo<T1, T2>
    {
    }

    public interface IBar<T>
    {
    }

    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }

        public Person(string name)
        {
            this.Name = name;
        }

        [Injection]
        public Person()
        {
        }

        public Person(int age = 0)
        {
            this.Age = age;
        }
    }

    public class Container
    {
        private Container _root;

        // 一个类型对应一条注册条目
        public ConcurrentDictionary<Type, Registry> _registries = new ConcurrentDictionary<Type, Registry>();

        // 通过注册类型的条目获取改类型的服务实例
        private ConcurrentDictionary<Registry, object> _services;
        private ConcurrentBag<IDisposable> _disposables;

        // 获取一个注册类型(或者叫服务类型)的服务实例时，首先从容器中获取该注册类型对应的注册条目，再由该注册条目提供该注册类型对应的服务实例
        public object GetService<T>() => this._services[this._registries[typeof(T)]];
        public object GetService(Type type) => this._services[this._registries[type]];
    }

    public static class ContainerExtension
    {
        public static bool HasRegistry<T>(this Container container) => container._registries.ContainsKey(typeof(T));
        public static bool HasRegistry(this Container container, Type type) => container._registries.ContainsKey(type);

        // 根据服务的类型找到其对应的注册条目(Registry)，再由其对应的注册条目创建该服务类型对应的服务实例
        public static object GetOrCreate<T>(this Container container)
        {
            Type type = typeof(T);
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"当前类{type}并没有构造函数！");

            var constructorWithAttribute = constructors.FirstOrDefault(item =>
                item.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());
            constructorWithAttribute ??= constructors.First();
            ParameterInfo[] constructorParams = constructorWithAttribute.GetParameters();
            if (constructorParams.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            object[] actualConstructorParams = new object[constructorParams.Length];
            for (int i = 0; i < constructorParams.Length; i++)
            {
                var parameter = constructorParams[i];
                var parameterType = parameter.ParameterType;
                if (container.HasRegistry(parameterType))
                {
                    actualConstructorParams[i] = container.GetService(parameterType);
                }
                else if (parameter.HasDefaultValue)
                {
                    actualConstructorParams[i] = parameter.DefaultValue;
                }
                else
                {
                    throw new InvalidOperationException($"无法创建{type}类型的实例，因为其构造参数类型未注册！");
                }
            }

            return Activator.CreateInstance(type, constructorParams);
        }
    }

    public class Registry
    {
        private LifeTime _lifeTime;
        private Type _registryType;
    }
}