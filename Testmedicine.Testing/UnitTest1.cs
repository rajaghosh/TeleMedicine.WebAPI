using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.authorize.sample;

namespace Testmedicine.Testing
{
    [TestClass]
    public class UnitTest1: SampleCode
    {
        [TestMethod]
        public void TestMethod1()
        {
            RunMethod("ChargeCreditCard");
        }
    }
}
