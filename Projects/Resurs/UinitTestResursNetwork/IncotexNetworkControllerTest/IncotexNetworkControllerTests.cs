using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.NetworkControllers.DataLinkLayer;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.Models;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.Management;
using Moq;

namespace UinitTestResursNetwork.IncotexNetworkControllerTest
{
    [TestClass]
    public class IncotexNetworkControllerTests
    {
        /// <summary>
        /// Проверяет обработку выполения запроса при его неопределённом
        /// типе 
        /// </summary>
        [TestMethod]
        //[ExpectedException(typeof(InvalidOperationException), "")]
        public void TestWriteByWrongReqeustType()
        {
            // Arrange
            var msg = new DataMessage(new byte[] { 0, 0, 0, 2 })
            {
                Address = 1,
                CmdCode = 0x20,
                MessageType = MessageType.IncomingMessage
            };

            Mock<IDataLinkPort> comPort = new Mock<IDataLinkPort>();
            comPort.Setup(p => p.IsOpen).Returns(true);
            comPort.Setup(p => p.MessagesToRead).Returns(0);

            comPort.Setup(p => p.Write(It.IsAny<IMessage>()))
                .Raises(m => m.MessageReceived += null, new EventArgs());
            comPort.Setup(p => p.Read()).Returns(msg);

            var controller = new IncotexNetworkController();
            controller.Connection = comPort.Object;
            var device = new Mercury203();
            controller.Devices.Add(device);
            controller.Start();

            var request = new DataMessage()
            {
                Address = 0x1,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadGroupAddress)
            };
            var wrongTrans = new Transaction(null, TransactionType.Undefined, request) { Sender = device }; // Ошибка !!!
            var networkRequest = new NetworkRequest(wrongTrans);

            // Act
            try
            {
                controller.Write(networkRequest);

                while (networkRequest.Status != NetworkRequestStatus.Failed)
                {
                    // Ждём, должно быть исключение
                }
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            }
        }

        /// <summary>
        /// Проверяет выполение запроса при успешном ответе
        /// </summary>
        [TestMethod]
        public void TestReadGroupAddressSuccess()
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
            var result = device.ReadGroupAddress();
            //controller.Write(trans);
            do
            {
                // Ждём выполения комманды
            }
            while (!result.IsCompleted);

            // Assert
            Assert.AreEqual(TransactionStatus.Completed, result.Stack[result.Stack.Length - 1].Status, "Success");
        }

        /// <summary>
        /// Проверяет механизм отработки выполнения запроса по таймауту 
        /// </summary>
        [TestMethod]
        public void TestReadGroupAddressTimeout()
        {
            // Arrange
            var msg = new DataMessage(new byte[] { 0, 0, 0, 2 })
            {
                Address = 1,
                CmdCode = 0x20,
                MessageType = MessageType.IncomingMessage
            };

            Mock<IDataLinkPort> comPort = new Mock<IDataLinkPort>();
            comPort.Setup(p => p.IsOpen).Returns(true);
            comPort.Setup(p => p.MessagesToRead).Returns(0);

            //comPort.Setup(p => p.Write(It.IsAny<IMessage>()))
            //    .Raises(m => m.MessageReceived += null, new EventArgs());
            comPort.Setup(p => p.Read()).Returns(msg);

            var controller = new IncotexNetworkController();
            controller.Connection = comPort.Object;
            controller.Start();
            controller.TotalAttempts = 2;
            //controller.Stop();

            var device = new Mercury203()
            {
                Address = 1,
            };

            controller.Devices.Add(device);

            // Act
            var result = device.ReadGroupAddress();

            do
            {
                // Ждём выполения комманды
            }
            while (!result.IsCompleted);

            // Assert
            Assert.AreEqual(TransactionStatus.Aborted, result.Stack[result.Stack.Length - 1].Status, "TimeOut");
        }

        /// <summary>
        /// Проверяет механизм отработки выполнения запроса при 
        /// остановленном контроллере сети
        /// </summary>
        [TestMethod]
        //[ExpectedException(typeof(InvalidOperationException), "")]
        public void TestReadGroupAddressByControllerIsStopped()
        {
            // Arrange
            var msg = new DataMessage(new byte[] { 0, 0, 0, 2 })
            {
                Address = 1,
                CmdCode = 0x20,
                MessageType = MessageType.IncomingMessage
            };

            Mock<IDataLinkPort> comPort = new Mock<IDataLinkPort>();
            comPort.Setup(p => p.IsOpen).Returns(true);
            comPort.Setup(p => p.MessagesToRead).Returns(0);

            comPort.Setup(p => p.Write(It.IsAny<IMessage>()))
                .Raises(m => m.MessageReceived += null, new EventArgs());
            comPort.Setup(p => p.Read()).Returns(msg);

            var controller = new IncotexNetworkController();
            controller.Connection = comPort.Object;
            controller.Stop(); // Контроллер остановлен

            var device = new Mercury203()
            {
                Address = 1,
            };

            controller.Devices.Add(device);

            Type type = null;

            // Act
            try
            {
                var result = device.ReadGroupAddress();
            }
            catch (Exception ex)
            {
                type = ex.GetType();
            }

            // Assert
            Assert.AreEqual(typeof(InvalidOperationException), type);

            // Assert
            //while(true)
            //{
                // Ждём должно быть исключение
            //}
        }

        /// <summary>
        /// Проверяет механизм отработки выполнения запроса при отсутствии
        /// контроллера
        /// </summary>
        [TestMethod]
        public void TestReadGroupAddressByIsNotController()
        {
            // Arrange
            var device = new Mercury203()
            {
                Address = 1,
            };

            // Act
            var result = device.ReadGroupAddress();
            do {}
            while(!result.IsCompleted);

            // Assert
            Assert.AreEqual(TransactionStatus.Aborted, result.Stack[result.Stack.Length - 1].Status);
        }
    }
}
