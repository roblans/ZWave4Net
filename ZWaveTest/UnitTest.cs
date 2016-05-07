using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using ZWave;

namespace ZWaveTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var controller = new ZWaveController("COM3");
            controller.Open();
            var version = controller.GetVersion().Result;
            controller.Close();
        }
    }
}
