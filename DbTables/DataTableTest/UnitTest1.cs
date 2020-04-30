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
            decimal b = 4;
            decimal a = Math.Ceiling(7/b);
            Console.WriteLine(a.ToString());
        }
    }
}
