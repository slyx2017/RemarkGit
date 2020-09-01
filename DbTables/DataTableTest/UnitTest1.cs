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
            decimal b = 2;
            decimal a = Math.Ceiling(1%b);
            Console.WriteLine(a.ToString());
        }
    }
}
