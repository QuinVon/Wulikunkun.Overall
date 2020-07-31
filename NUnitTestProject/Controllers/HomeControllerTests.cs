using NUnit.Framework;
using Wulikunkun.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Wulikunkun.Web.Controllers.Tests
{
    [TestFixture(Description = "关于HomeController的测试")]
    public class HomeControllerTests
    {
        [OneTimeSetUp]
        public void Init()
        {
            Console.WriteLine("这是一个在执行任何测试方法之前必须执行的方法");
        }

        [Test(Description = "关于HomeController下的Index测试"), Ignore("留待修改的测试方法"),Order(1)]
        public void IndexTest()
        {
            Console.WriteLine("关于HomeController下的Index测试");
        }

        [Test(Description = "关于HomeController下的LogIn测试"),Explicit("设置必须选中进行测试"),Order(2)]
        public void LogInTest()
        {
            Console.WriteLine("关于HomeController下的LogIn测试");
        }

        [TestCase(1,2)]
        [Test(Description = "测试加法运算"),Order(3)]
        public void AddTest(int a,int b)
        {
            Assert.IsTrue(a + b == 3);
            Assert.AreEqual(3,a+b);
        }

        [Test(Description = "测试减法运算"),Order(4)]
        public void MinusTest([Values(10,9,8)]int a,[Range(1,5,1)]  int b)
        {
            Console.WriteLine(a - b);
        }

        [Test(Description = "测试乘法运算"), Order(5)]
        public void MultiplyTest([Values(10, 9, 8)]int a, [Range(1, 5, 1)]  int b)
        {
            Console.WriteLine(a * b);
        }

        [TestCase(6,3)]
        [TestCase(4, 2)]
        [Test(Description = "测试除法运算"), Order(6)]
        public void DivideTest(int a,int b)
        {
            Console.WriteLine(a / b);
        }

        [OneTimeTearDown]
        public void End()
        {
            Console.WriteLine("这是一个在所有的测试方法执行");
        }
    }
}