using System;
using System.Collections;
using System.Reflection;

namespace Wulikunkun.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Type type = typeof(IFoo<,>);
            Type personType = typeof(Person);
            Person person = new Person("wangkun", "24");
            ConstructorInfo[] constructors = personType.GetConstructors(BindingFlags.Instance);
            Console.WriteLine("Hello World!");
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

    public class PersonBase
    {
        public string School { get; set; }
        public string Directory { get; set; }

        public PersonBase(string school, string directory)
        {
            this.School = school;
            this.Directory = directory;
        }
    }

    public class Person : PersonBase, IBar<Container>
    {
        public string Age { get; set; }
        public string Name { get; set; }

        public Person(string name, string age) : base(name, age)
        {
            this.Age = age;
            this.Name = name;
        }
    }

    public class Container
    {
    }

    public class Registry
    {
    }
}