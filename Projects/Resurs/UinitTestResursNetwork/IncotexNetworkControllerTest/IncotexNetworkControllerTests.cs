using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.NetworkControllers.DataLinkLayer;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.Model;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transaction;
using Moq;

namespace UinitTestResursNetwork.IncotexNetworkControllerTest
{
    [TestClass]
    public class IncotexNetworkControllerTests
    {
        [TestMethod]
        //[ExpectedException]
        public void TestMethod1()
        {
            // Arrange
            int amount = 1;

            var msg = new DataMessage(new byte[] { 0, 0, 0, 2 })
            {
                Address = 1,
                CmdCode = 0x20,
                MessageType = MessageType.IncomingMessage
            };

            Mock<IDataLinkPort> comPort = new Mock<IDataLinkPort>();
            //comPort.Setup(p => p.GetType()).Returns(typeof(IncotexNetworkController));
            comPort.Setup(p => p.IsOpen).Returns(true);
            comPort.Setup(p => p.MessagesToRead)
                .Returns(() => amount)
                .Callback(() => amount--);
            
            comPort.Setup(p => p.Write(It.IsAny<IMessage>()))
                .Raises(m => m.MessageReceived += null, new EventArgs());
            comPort.Setup(p => p.Read()).Returns(msg);

            var controller = new IncotexNetworkController();
            controller.Connection = comPort.Object;
            controller.Start();
            //controller.Stop();

            var device = new Mercury203()
            {
                Address = 1,
            };

            controller.Devices.Add(device);

            // Act
            var trans = device.ReadGroupAddress();
            controller.Write(trans);
            do
            {

            }
            while (trans.Status != TransactionStatus.Completed);

            // Assert
            return;
        }

    }
}
