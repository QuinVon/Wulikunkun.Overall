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

}


