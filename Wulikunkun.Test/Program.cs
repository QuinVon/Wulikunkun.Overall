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
            // 保留一个疑问，在.NET Core框架中服务的实例是何时被请求的？
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
        Singleton
    }

    public class Foo : IFoo<Container, ServiceRegistry>
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

    // 为什么非要设计这样一个接口
    public interface IServiceProvider
    {
        object GetService(ServiceRegistry registry, Type[] arguments);
    }

    public class Container : IDisposable, IServiceProvider
    {
        // _root指向容器自身
        private Container _root;
        // 保存该容器内服务类型与注册条目映射关系的并发字典，一个类型对应一条注册条目
        public ConcurrentDictionary<Type, ServiceRegistry> Registries { get; }
        // 保存该容器内已经创建的服务实例
        public ConcurrentDictionary<ServiceRegistry, object> Services { get; }
        // 保存容器内等待释放的服务实例
        private ConcurrentBag<IDisposable> _disposables;
        // 表示容器自身是否已经释放的状态
        private bool _disposed;

        public Container()
        {
            this._root = this;
            Registries = new ConcurrentDictionary<Type, ServiceRegistry>();
            Services = new ConcurrentDictionary<ServiceRegistry, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        public Container(Container parent)
        {
            // 将此容器与父容器进行链接
            this._root = parent._root;
            // 获取父容器的服务注册关系
            Registries = parent.Registries;
            // 这里为什么没有获取父容器中已经创建的服务实例？
            Services = new ConcurrentDictionary<ServiceRegistry, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        # region 比较这两种写法的异同
        // public ConcurrentDictionary<Type, ServiceRegistry> _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
        // 通过注册类型的条目获取改类型的服务实例
        // private ConcurrentDictionary<ServiceRegistry, object> _services;
        // private ConcurrentBag<IDisposable> _disposables;
        #endregion

        // 获取一个注册类型(或者叫服务类型)的服务实例时，首先从容器中获取该注册类型对应的注册条目，再由该注册条目提供该注册类型对应的服务实例
        public object GetService<T>() => this.Services[this.Registries[typeof(T)]];
        public object GetService(Type type) => this.Services[this.Registries[type]];

        public void Register(ServiceRegistry registry)
        {
            if (Registries.TryGetValue(registry.ServiceType, out var existing))
            {
                // 这里空着，没有看懂作者的意思
            }
            else
            {
                Registries[registry.ServiceType] = registry;
            }
        }

        public object GetService(ServiceRegistry registry, Type[] arguments)
        {
            switch (registry.LifeTime)
            {
                // 如果需要的服务实例类型被注册为Singleton模式，则从根容器中获取或者创建服务实例
                case LifeTime.Singleton:
                    break;
                case LifeTime.Scope:
                    break;
                case LifeTime.Transient:
                    var service = registry.factory
                    _disposables.Add(service);
                    break;
            }

            // 这里的arguments对应的就是一个服务类型构造函数的参数列表
            object GetOrCreate(ServiceRegistry registry, Type[] arguments)
            {

            }
        }

        // 容器释放时，其创建的服务实例一同释放
        public void Dispose()
        {
            foreach (IDisposable service in _disposables)
            {
                service.Dispose();
            }
            // 如果这里是if，那么花括号中的代码只会执行一次，而while则会一直执行
            while (!_disposables.IsEmpty)
            {
                // 不知道这里的下划线表示什么
                _disposables.TryTake(out _);
            }
            Services.Clear();
            _disposed = true;
        }
    }

    public static class ContainerExtension
    {
        // 注册必须同时提供以下三个条件，服务类型，生命周期，服务实例的创建委托
        public static void Register(this Container container, Type fromType, Type toType, LifeTime lifeTime)
        {
            // 我好奇这个委托是如何被消费的？
            Func<Type, Type[], object> factory = (toType, arguments) => CreateInstance(container, toType, arguments);
            ServiceRegistry registry = new ServiceRegistry(fromType, lifeTime, factory);
            container.Register(registry);
        }

        public static void Register<FromType, ToType>(this Container container, LifeTime lifeTime)
        {
            container.Register(typeof(FromType), typeof(ToType), lifeTime);
        }

        public static bool HasRegistry<T>(this Container container) => container.Registries.ContainsKey(typeof(T));
        public static bool HasRegistry(this Container container, Type type) => container.Registries.ContainsKey(type);

        // 根据服务的类型找到其对应的注册条目(Registry)，再由其对应的注册条目创建该服务类型对应的服务实例，其实这里的创建实例我认为是最底层也是最重要的一个方法
        public static object CreateInstance(this Container container, Type type, Type[] arguments)
        {
            if (arguments.Length > 0)
            {
                // 这里的MakeGenericType也没看懂是干什么用的
                type = type.MakeGenericType(arguments);
            }

            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"当前类{type}并没有构造函数！");

            var constructorWithAttribute = constructors.FirstOrDefault(item => item.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());
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

    // 注册条目封装了注册(服务)类型，服务生命周期，以及服务实例的创建方式
    public class ServiceRegistry
    {
        // 对应注册服务的类型(From)，这个ServiceType我推断经常是一个接口
        public Type ServiceType { get; }
        // 对应服务实例的生命周期
        public LifeTime LifeTime { get; }
        // 创建服务实例的工厂(需要服务类型(To)，服务类型(To)构造函数的参数数组)
        public Func<Type, Type[], object> ServiceFactory { get; }

        public ServiceRegistry(Type serviceType, LifeTime lifeTime, Func<Type, Type[], object> func)
        {
            // 这里的意思难道是说只读访问器在类的外部是只读的，在类的内部依然可以被写？
            this.ServiceType = serviceType;
            this.LifeTime = lifeTime;
            this.ServiceFactory = func;
        }

    }
}