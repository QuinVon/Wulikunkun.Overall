using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Wulikunkun.Test
{
    #region 文件系统设计

    internal class Program
    {
        private static void Main(string[] args)
        {
            Random random = new Random();
            random.Next(1, 10);

            int i = 0;

            Thread thread = new Thread(() =>
            {
                i++;
                Thread.Sleep(TimeSpan.FromSeconds(5));
            });
            thread.Start();
            i++;
            Console.WriteLine(i);
            Console.Read();




            Console.WriteLine("当前主线程的线程id为：" + Thread.CurrentThread.ManagedThreadId);
            Task task = ConsoleInfoAsync();
            Console.WriteLine("异步任务创建完毕 ，当前主线程的线程id为：" + Thread.CurrentThread.ManagedThreadId);
            Console.ReadLine();

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();


            #region CancellationToken和CancellationTokenSource的基本用法
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            cancellationToken.Register(() => Console.WriteLine("好人王昆"));
            Console.WriteLine(cancellationToken.IsCancellationRequested);
            cancellationTokenSource.Cancel();
            Console.WriteLine(cancellationToken.IsCancellationRequested);
            Console.ReadLine();
            #endregion

            //总结Action和Func在声明和调用时的区别
            //object obj = new object();
            //cancellationToken.Register((_) => Console.WriteLine(nameof(_)), obj);
            //cancellationTokenSource.Cancel();
            //Console.ReadLine();

            File.WriteAllText(@"C:\Users\wulikunkun\Desktop\IO\IO.txt", "我是好人wnagkun");
            Console.Read();

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Console.WriteLine("好人王昆");
            }
        }

        public static async Task ConsoleInfoAsync()
        {
            Console.WriteLine("进入Async方法，但尚未执行await方法，当前线程id为：" + Thread.CurrentThread.ManagedThreadId);
            await new Task(() => Console.WriteLine("第一个await任务创建完毕，当前线程id为：" + Thread.CurrentThread.ManagedThreadId));
            Console.WriteLine("第一个await任务创建完毕，当前线程id为：" + Thread.CurrentThread.ManagedThreadId);
            await new Task(() => Console.WriteLine("第二个await任务创建完毕，当前线程id为：" + Thread.CurrentThread.ManagedThreadId));
            Console.WriteLine("第二个await任务创建完毕，当前线程id为：" + Thread.CurrentThread.ManagedThreadId);
            //await Task.Run(() => Console.WriteLine("开始执行第一个await方法，当前线程id为：" + Thread.CurrentThread.ManagedThreadId));
            //Thread.Sleep(2000);
            //await Task.Run(() => Console.WriteLine("开始执行第二个await方法，当前线程id为：" + Thread.CurrentThread.ManagedThreadId));
            //Console.WriteLine("异步方法的await执行完毕，当前线程id为：" + Thread.CurrentThread.ManagedThreadId);
        }
    }



    public interface IChangeToken
    {
        bool HasChanged { get; }
        bool ActiveChangeCallbacks { get; }
        //注意，这里声明一个不需要参数的Action不能使用尖括号
        IDisposable RegisterChangeCallback(Action callback);
    }


    ////该类主要封装了一个系统提供的结构类型CancellationToken，通过系统提供的CancellationToken类来实现我们请求取消某一步操作时执行的委托操作
    public class CancellationChangeToken : IChangeToken
    {
        private readonly CancellationToken _token;
        public CancellationChangeToken(CancellationToken token) => this._token = token;

        #region IChangeToken成员区域

        //获取监控数据的状态是否发生了改变
        public bool HasChanged => _token.IsCancellationRequested;
        //在默认情况下当IChangeToken监控的数据发生变化时就会执行我们注册的回调操作
        public bool ActiveChangeCallbacks => true;
        public IDisposable RegisterChangeCallback(Action callback) => _token.Register(callback);

        #endregion
    }

    ////总结IReadOnlyList,IReadOnlyCollection,IReadOnlyDictionary
    public class CompositeChangeToken : IChangeToken
    {
        private IReadOnlyList<IChangeToken> _changeTokens;
        public CompositeChangeToken(IReadOnlyList<IChangeToken> changeTokens) => this._changeTokens = changeTokens;


        #region IChangeToken成员区域
        public bool HasChanged { get; }
        public bool ActiveChangeCallbacks => throw new NotImplementedException();
        public IDisposable RegisterChangeCallback(Action callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public interface IFileInfo
    {
        bool Exisits { get; }
        bool IsDirectory { get; }
        string Name { get; }
        string PhysicalPath { get; }
        DateTimeOffset LastModified { get; }
        long Length { get; }
        Stream CreateReadStream();
    }

    //接口的每一个成员在继承类中必须实现，但是在继承接口中却不需要这样
    public interface IDirectoryContents : IEnumerable<IFileInfo>, IEnumerable
    {
        bool Exisits { get; set; }
    }

    //尽管IDirectoryContents这个接口继承了 IEnumerable<IFileInfo>, IEnumerable这两个接口，但是我在IDirectoryContents的继承类中没有实现IEnumerable<IFileInfo>, IEnumerable这两个接口居然可以编译通过
    //public class CompositeDirectoryContents : IDirectoryContents
    //{
    //    readonly string _subPath;

    //    public bool Exisits { get; set; }

    //    public IEnumerator<IFileInfo> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //如果和父接口中有重名的成员，需要在重名的成员前加上new
    //public interface IDirectoryContents : IFileInfo
    //{
    //    new bool Exisits { get; }
    //}

    #endregion

    #region 对Async和await的研究
    //public static async Task ConsoleInfoAsync()
    //{
    //    Console.WriteLine("开始异步方法的执行，当前执行线程为：" + Thread.CurrentThread.ManagedThreadId);
    //    await Task.Run(ConsoleName);//一个异步方法内的多个Task.Run()方法并不一定是由同一个线程执行的=>那这样一个await方法的阻塞应该也不会影响到另外一个await方法？但是经过我的测试结果却不是这样，尽管一个异步方法内部的多个await方法可能是由不同的线程执行的，但是其中一个await方法的阻塞也会造成另外一个await方法的阻塞=>那这样的话一个async方法的内部不是只需要一个await方法就行了？
    //                                //Thread.Sleep(TimeSpan.FromSeconds(3));
    //    await Task.Run(ConsoleAge);
    //    //await Task.Run(() => ConsoleAge());
    //    Console.WriteLine("异步方法执行结束，当前线程为：" + Thread.CurrentThread.ManagedThreadId);
    //    //考虑下这种在异步方法内部调用其它异步方法的执行流程
    //    //await ConsoleNameAsync();
    //}

    //public static void ConsoleName()
    //{
    //    //Thread.Sleep(TimeSpan.FromSeconds(3));
    //    Console.WriteLine("进入打印名字方法的线程，当前线程ID为：" + Thread.CurrentThread.ManagedThreadId);
    //}

    //public static void ConsoleAge()
    //{
    //    Thread.Sleep(TimeSpan.FromSeconds(10));
    //    Console.WriteLine("进入打印年龄方法的线程，当前线程ID为：" + Thread.CurrentThread.ManagedThreadId);
    //}
    //public static async Task ConsoleNameAsync() => await Task.Run(() =>
    //{
    //    //Thread.Sleep(1000);
    //    Console.WriteLine("进入打印姓名方法线程，当前线程ID是：" + Thread.CurrentThread.ManagedThreadId);
    //});

    //public static async Task ConsoleAgeAsync() => await Task.Run(() =>
    //{
    //    Thread.Sleep(2000);
    //    Console.WriteLine("进入打印年龄方法线程，当前线程ID是：" + Thread.CurrentThread.ManagedThreadId);
    //});
    //public static async Task<string> ReturnGender() => await new Task<string>(() => "Man");
    //public static async Task ConsoleNameAsync() => await new Task(() => Console.WriteLine("进入打印姓名方法线程，当前线程ID是：" + Thread.CurrentThread.ManagedThreadId));
    //public static async Task ConsoleAgeAsync() => await new Task(() => Console.WriteLine("进入打印年龄方法线程，当前线程ID是：" + Thread.CurrentThread.ManagedThreadId));
    //public static async Task<string> ReturnGender() => await new Task<string>(() => "Man");
    #endregion

    #region 获取环境变量方法
    //private static void Main(string[] args)
    //{
    //    string sqlConnString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.User);
    //    Console.Read();
    //}
    #endregion

    #region 对Func的研究
    //private static void Main(string[] args)
    //{
    //    PersonCollection personCollection = new PersonCollection();
    //    Person wangkun = (Person)personCollection.Select(item => item.Name == "wangkun");
    //    Person wanglei = (Person)personCollection.Select(item => item.Name == "wanglei" && item.Age == 18);
    //    Console.WriteLine(wangkun.Age);
    //    Console.WriteLine(wanglei.Gender);

    //    Console.Read();
    //}

    //public class PersonCollection
    //{
    //    public Person[] personArray = new Person[] { new Person("wangkun", 24, "man"), new Person("wangzhen", 20, "man"), new Person("wanglei", 18, "man") };
    //    public object Select(Func<Person, bool> func)
    //    {
    //        for (int i = 0; i < personArray.Length; i++)
    //        {
    //            if (func(personArray[i])) return personArray[i];
    //        }
    //        return null;
    //    }
    //}

    //public class Person
    //{
    //    public string Name { get; }
    //    public int Age { get; }
    //    public string Gender { get; }
    //    public Person(string name, int age, string gender)
    //    {
    //        this.Name = name;
    //        this.Age = age;
    //        this.Gender = gender;
    //    }
    //}
    #endregion

    #region 二叉树的研究

    //internal class Program
    //{
    //    private static void Main(string[] args)
    //    {
    //        #region 对二叉树的测试
    //        //Node rootNode = new Node(34);
    //        //int[] intArray = new int[] { 23, 12, 8, 14, 9, 40, 11, 5, 27, 88, 120, 37, 84, 72, 33, 57, 91, 10, 81 };
    //        //rootNode.AddPatchNode(intArray);
    //        //Console.WriteLine(rootNode.FindNode(81));
    //        Console.Read();
    //        #endregion
    //    }
    //}

    //public class Node
    //{
    //    private int _val;
    //    public Node LeftNode { get; set; }
    //    public Node RightNode { get; set; }
    //    public int Val
    //    {
    //        get
    //        {
    //            Console.WriteLine($"当前节点有被访问到，该节点的值是：{_val}");
    //            return _val; ;
    //        }
    //        set
    //        {
    //            this._val = value;
    //        }
    //    }
    //    public Node(int val)
    //    {
    //        _val = val;
    //    }
    //    public void AddNode(int val)
    //    {
    //        if (this._val < val && this.RightNode == null)
    //        {
    //            Node newNode = new Node(val);
    //            RightNode = newNode;
    //        }
    //        else if (this._val < val && this.RightNode != null) this.RightNode.AddNode(val);
    //        else if (this._val > val && this.LeftNode == null)
    //        {
    //            Node newNode = new Node(val);
    //            LeftNode = newNode;
    //        }
    //        else if (this._val > val && this.LeftNode != null) this.LeftNode.AddNode(val);
    //        else throw new InvalidOperationException("当前元素已存在于二叉树中");
    //    }
    //    public void AddPatchNode(params int[] intArray)
    //    {
    //        for (int i = 0; i < intArray.Length; i++)
    //        {
    //            this.AddNode(intArray[i]);
    //        }
    //    }
    //    public int FindNode(int targetValue)
    //    {
    //        if (targetValue < this.Val && this.LeftNode != null) return this.LeftNode.FindNode(targetValue);
    //        else if (targetValue < this.Val && this.LeftNode == null) throw new Exception("当前树中不存在该元素!");
    //        else if (targetValue > this.Val && this.RightNode != null) return this.RightNode.FindNode(targetValue);
    //        else if (targetValue > this.Val && this.RightNode == null) throw new Exception("当前树中不存在该元素!");
    //        else return this._val;
    //    }
    //}

    #endregion

    #region 依赖注入框架设计

    //public void LoadRegisterServices()
    //{
    //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
    //    var registeredTypes = from type in currentAssembly.GetExportedTypes()
    //                          let attribute = type.GetCustomAttribute<MapToAttribute>()
    //                          where attribute != null
    //                          select new { AchievedType = type, RegisterAttribute = attribute };
    //    Container container = new Container();
    //    foreach (var item in registeredTypes)
    //    {
    //        Type achievedType = item.AchievedType, registerType = item.RegisterAttribute.RegisterType;

    //        container.Register(registerType, achievedType, item.RegisterAttribute.LifeTime);
    //    }
    //    var serviceOne = container.GetService(typeof(ITest));
    //    var serviceTwo = container.GetService(typeof(ITest));
    //    // 保留一个疑问，在.NET Core框架中服务的实例是何时被请求的？
    //    // object obj = container.GetOrCreate<Person>();
    //}

    //public interface ITest
    //{
    //    void WriteTypeName();
    //}

    //[MapTo(typeof(ITest), LifeTime.Singleton)]
    //public class Test : ITest
    //{
    //    public Test()
    //    {
    //        Console.WriteLine(this.GetType().Name + "的实例创建了");
    //    }
    //    public void WriteTypeName()
    //    {
    //        Console.WriteLine(this.GetType().Name);
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Constructor)]
    //public class InjectionAttribute : Attribute
    //{
    //}

    //// 声明一个用于批量注册的特性
    //[AttributeUsage(AttributeTargets.Class)]
    //public class MapToAttribute : Attribute
    //{
    //    public Type RegisterType { get; }
    //    public LifeTime LifeTime { get; }

    //    public MapToAttribute(Type registerType, LifeTime lifeTime)
    //    {
    //        RegisterType = registerType;
    //        LifeTime = lifeTime;
    //    }
    //}

    //// 声明注册服务的声明周期
    //public enum LifeTime
    //{
    //    Transient,
    //    Scope,
    //    Singleton
    //}

    //// 为什么非要设计这样一个接口
    //public interface IServiceProvider
    //{
    //    object GetServiceCore(ServiceRegistry registry, Type[] genericArguments);
    //}

    //public class Container : IDisposable, IServiceProvider
    //{
    //    // 保存容器内等待释放的服务实例
    //    private readonly ConcurrentBag<IDisposable> _disposables;

    //    // 保存该容器内服务类型与注册条目映射关系的并发字典，一个类型对应一条注册条目
    //    public ConcurrentDictionary<Type, ServiceRegistry> Registries { get; }

    //    // 保存该容器内已经创建的服务实例（一个注册条目对应一个服务实例）
    //    public ConcurrentDictionary<ServiceRegistry, object> Services { get; }

    //    // 表示容器自身是否已经释放的状态
    //    private bool _disposed;

    //    // _root指向容器自身
    //    private readonly Container _root;

    //    public Container()
    //    {
    //        _root = this;
    //        Registries = new ConcurrentDictionary<Type, ServiceRegistry>();
    //        Services = new ConcurrentDictionary<ServiceRegistry, object>();
    //        _disposables = new ConcurrentBag<IDisposable>();
    //    }

    //    public Container(Container parent)
    //    {
    //        // 将此容器与父容器进行链接
    //        _root = parent._root;
    //        // 获取父容器的服务注册关系
    //        Registries = parent.Registries;
    //        // 这里为什么没有获取父容器中已经创建的服务实例？
    //        Services = new ConcurrentDictionary<ServiceRegistry, object>();
    //        _disposables = new ConcurrentBag<IDisposable>();
    //    }

    //    // 容器释放时，其创建的服务实例一同释放
    //    public void Dispose()
    //    {
    //        foreach (var service in _disposables) service.Dispose();
    //        // 如果这里是if，那么花括号中的代码只会执行一次，而while则会一直执行
    //        while (!_disposables.IsEmpty)
    //            // 不知道这里的下划线表示什么
    //            _disposables.TryTake(out _);
    //        Services.Clear();
    //        _disposed = true;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="registry"></param>
    //    /// <param name="arguments"></param>
    //    /// <returns></returns>
    //    public object GetServiceCore(ServiceRegistry registry, Type[] genericArguments)
    //    {
    //        switch (registry.LifeTime)
    //        {
    //            // 如果需要的服务实例类型被注册为Singleton模式，则从根容器中获取或者创建服务实例
    //            case LifeTime.Singleton:
    //                return GetOrCreate(registry, genericArguments);
    //            case LifeTime.Scope:
    //                return GetOrCreate(registry, genericArguments);
    //            default:
    //                var transientServiceInstance = registry.ServiceFactory(genericArguments);
    //                if (transientServiceInstance is IDisposable disposable) _disposables.Add(disposable);
    //                return transientServiceInstance;
    //        }

    //        // 这里的arguments对应的就是一个服务类型构造函数的参数列表，如果当前容器中已经存在该注册条目对应的服务实例，则返回该服务实例，否则，创建该注册条目对应的服务实例
    //        object GetOrCreate(ServiceRegistry registry, Type[] genericArguments)
    //        {
    //            if (Services.ContainsKey(registry)) return Services[registry];
    //            var serviceInstance = registry.ServiceFactory(genericArguments);
    //            Services[registry] = serviceInstance;
    //            if (serviceInstance is IDisposable disposable) _disposables.Add(disposable);
    //            return serviceInstance;
    //        }
    //    }

    //    // 获取一个注册类型(或者叫服务类型)的服务实例时，首先从容器中获取该注册类型对应的注册条目，再由该注册条目提供该注册类型对应的服务实例

    //    public object GetService(Type type)
    //    {
    //        if (type == typeof(Container) || type == typeof(IServiceProvider)) return this;
    //        else if (Registries.TryGetValue(type, out ServiceRegistry registryItem)) return GetServiceCore(registryItem, type.GetGenericArguments());
    //        else return null;
    //    }

    //    public void Register(ServiceRegistry registry)
    //    {
    //        if (Registries.TryGetValue(registry.ServiceType, out var existing))
    //        {
    //            // 这里空着，没有看懂作者的意思
    //        }
    //        else
    //        {
    //            Registries[registry.ServiceType] = registry;
    //        }
    //    }

    //    #region 比较这两种写法的异同

    //    // public ConcurrentDictionary<Type, ServiceRegistry> _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
    //    // 通过注册类型的条目获取改类型的服务实例
    //    // private ConcurrentDictionary<ServiceRegistry, object> _services;
    //    // private ConcurrentBag<IDisposable> _disposables;

    //    #endregion
    //}

    //public static class ContainerExtension
    //{
    //    // 注册必须同时提供以下三个条件，服务类型，生命周期，服务实例的创建委托，注册就是将一个被注册类型(Type)绑定一个注册条目(Registry)
    //    public static void Register(this Container container, Type fromType, Type toType, LifeTime lifeTime)
    //    {
    //        // 我好奇这个委托是如何被消费的？
    //        Func<Type[], object> factory = (arguments) => CreateInstance(container, toType, arguments);
    //        var registry = new ServiceRegistry(fromType, lifeTime, factory);
    //        container.Register(registry);
    //    }

    //    public static void Register<FromType, ToType>(this Container container, LifeTime lifeTime)
    //    {
    //        container.Register(typeof(FromType), typeof(ToType), lifeTime);
    //    }

    //    public static bool HasRegistry<T>(this Container container)
    //    {
    //        return container.Registries.ContainsKey(typeof(T));
    //    }

    //    public static bool HasRegistry(this Container container, Type type)
    //    {
    //        return container.Registries.ContainsKey(type);
    //    }

    //    // 根据服务的类型找到其对应的注册条目(Registry)，再由其对应的注册条目创建该服务类型对应的服务实例，其实这里的创建实例我认为是最底层也是最重要的一个方法
    //    public static object CreateInstance(this Container container, Type type, Type[] arguments)
    //    {
    //        if (arguments.Length > 0)
    //            // 这里的MakeGenericType也没看懂是干什么用的
    //            type = type.MakeGenericType(arguments);
    //        Type type1 = typeof(Test);
    //        var constructors = type.GetConstructors();
    //        if (constructors.Length == 0)
    //            throw new InvalidOperationException($"当前类{type}并没有构造函数！");

    //        ConstructorInfo constructorWithAttribute = constructors.FirstOrDefault(item => item.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());
    //        constructorWithAttribute ??= constructors.First();
    //        ParameterInfo[] constructorParams = constructorWithAttribute.GetParameters();
    //        if (constructorParams.Length == 0) return Activator.CreateInstance(type);
    //        var constructorActualParams = new object[constructorParams.Length];
    //        for (var i = 0; i < constructorParams.Length; i++)
    //        {
    //            var parameter = constructorParams[i];
    //            var parameterType = parameter.ParameterType;
    //            if (container.HasRegistry(parameterType))
    //            {
    //                constructorActualParams[i] = container.GetService(parameterType);
    //            }
    //            else if (parameter.HasDefaultValue)
    //            {
    //                constructorActualParams[i] = parameter.DefaultValue;
    //            }
    //            else
    //            {
    //                throw new InvalidOperationException($"无法创建{type}类型的实例，因为其构造参数类型未注册！");
    //            }
    //        }
    //        return Activator.CreateInstance(type, constructorParams);
    //    }
    //}

    //// 注册条目封装了注册(服务)类型，服务生命周期，以及服务实例的创建方式
    //public class ServiceRegistry : IEquatable<ServiceRegistry>
    //{
    //    // 对应注册服务的类型(From)，这个ServiceType我推断经常是一个接口
    //    public Type ServiceType { get; }

    //    // 对应服务实例的生命周期
    //    public LifeTime LifeTime { get; }

    //    // 创建服务实例的工厂(需要服务类型(To)，服务类型(To)构造函数的参数数组)
    //    public Func<Type[], object> ServiceFactory { get; }

    //    public ServiceRegistry(Type serviceType, LifeTime lifeTime, Func<Type[], object> func)
    //    {
    //        // 这里的意思难道是说只读访问器在类的外部是只读的，在类的内部依然可以被写？
    //        ServiceType = serviceType;
    //        LifeTime = lifeTime;
    //        ServiceFactory = func;
    //    }
    //    public bool Equals([AllowNull] ServiceRegistry other)
    //    {
    //        if (ServiceType.GetHashCode() == other.ServiceType.GetHashCode()) return true;
    //        else return false;
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        if (obj is ServiceRegistry serviceRegistry) return Equals(obj);
    //        else return false;
    //    }
    //    public override int GetHashCode()
    //    {
    //        return ServiceType.GetHashCode();
    //    }
    //    public static bool operator ==(ServiceRegistry selfRegistry, object otherRegistry)
    //    {
    //        if (otherRegistry == null) return Object.Equals(selfRegistry, otherRegistry);
    //        else return selfRegistry.Equals(otherRegistry);
    //    }
    //    public static bool operator !=(ServiceRegistry selfRegistry, object otherRegistry)
    //    {
    //        if (otherRegistry == null) return Object.Equals(selfRegistry, otherRegistry);
    //        else return selfRegistry.Equals(otherRegistry);
    //    }
    //}
    #endregion
}


