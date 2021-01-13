using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTableTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int secondNumber = 15;
            int firstNumber = DateTime.Now.Day;
            int result = firstNumber - secondNumber;
            
            Console.WriteLine("结果："+result.ToString());
            //decimal b = 2;
            //decimal a = Math.Ceiling(1%b);
            //Console.WriteLine(a.ToString());
        }
    }
}
