using Disks.gRPC.Service;
using Disks.gRPC.Service.Data;
using Disks.gRPC.Service.Repos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Disks.gRPC.Tests
{
    [TestClass]
    public class VolumeRepositoryTests
    {
        private IVolumeDataSource volumeDataSource;
        private Mock<IDatabase> databaseMock;

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<IRedisConnectionService> redisServiceMock = new Mock<IRedisConnectionService>();
            Mock<ILogger<VolumesRepository>> loggerMock = new Mock<ILogger<VolumesRepository>>();

            Mock<IConnectionMultiplexer> connectionMock = new Mock<IConnectionMultiplexer>();
            redisServiceMock.Setup(d => d.Connect()).Returns(connectionMock.Object);

            databaseMock = new Mock<IDatabase>();
            connectionMock.Setup(d => d.GetDatabase(-1, null)).Returns(databaseMock.Object);                       

            volumeDataSource = new VolumesRepository(redisServiceMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public async Task TestCreateVolume()
        {
            Mock<ITransaction> transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(d => d.ExecuteAsync(CommandFlags.None)).Returns(Task.FromResult(true));

            databaseMock.Setup(d => d.CreateTransaction(null)).Returns(transactionMock.Object);

            CreateVolumeRequest createVolumeRequest = new CreateVolumeRequest()
            {
                Name = "TestVolumeName",
                OsType = "Windows",
                SizeGb = 3
            };
            createVolumeRequest.MountPints.Add("c:");
           
            var voluem = await volumeDataSource.Create(createVolumeRequest, "userId");

            Assert.IsTrue(voluem.Id.Contains("vol"));
            Assert.IsTrue(voluem.SizeGb == createVolumeRequest.SizeGb);
            Assert.IsTrue(voluem.Name == createVolumeRequest.Name);
        }
        [TestMethod]
        [ExpectedException(typeof(VolumeException))]
        public async Task TestCreateVolumeWhenTransactionFailed()
        {
            Mock<ITransaction> transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(d => d.ExecuteAsync(CommandFlags.None)).Returns(Task.FromResult(false));

            databaseMock.Setup(d => d.CreateTransaction(null)).Returns(transactionMock.Object);

            CreateVolumeRequest createVolumeRequest = new CreateVolumeRequest()
            {
                Name = "TestVolumeName",
                OsType = "Windows",
                SizeGb = 3
            };
            createVolumeRequest.MountPints.Add("c:");

            var voluem = await volumeDataSource.Create(createVolumeRequest, "userId");

            Assert.IsFalse(true, "This method must throw an exception, when a transaction fails");
        }
        [TestMethod]
        public async Task TestGetVolume()
        {
            VolumeReply volumeReply = new VolumeReply()
            {
                Id = "vol-0000000",
                Name = "TestVolumeName",
                SizeGb = 3
            };
            volumeReply.MountPoints.Add("c:");

            Mock<IDatabase> databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(d => d.KeyExists(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(true);
            databaseMock.Setup(d => d.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None))
                .Returns(async (RedisKey userId, RedisValue key, CommandFlags flags) =>
                {
                    if (key == "Id")
                        return volumeReply.Id;
                    if (key == "Name")
                        return volumeReply.Name;
                    else if (key == "Size")
                        return volumeReply.SizeGb;
                    else if (key == "MountPoints")
                        return "c:";
                    else
                        throw new Exception();
                });

            Mock<IConnectionMultiplexer> connectionMock = new Mock<IConnectionMultiplexer>();
            connectionMock.Setup(d => d.GetDatabase(-1, null)).Returns(databaseMock.Object);

            Mock<IRedisConnectionService> redisServiceMock = new Mock<IRedisConnectionService>();
            redisServiceMock.Setup(d => d.Connect()).Returns(connectionMock.Object);

            Mock<ILogger<VolumesRepository>> loggerMock = new Mock<ILogger<VolumesRepository>>();
            VolumesRepository volumeDataSource = new VolumesRepository(redisServiceMock.Object, loggerMock.Object);

            VolumeReply volume = await volumeDataSource.Get("key", "userId");

            Assert.IsTrue(volume.Id.Contains("vol"));
            Assert.IsTrue(volume.SizeGb == volumeReply.SizeGb);
            Assert.IsTrue(volume.Name == volumeReply.Name);
        }
        [TestMethod]
        public async Task TestListVolumes()
        {
            VolumeReply volumeReply = new VolumeReply()
            {
                Id = "vol-0000000",
                Name = "TestVolumeName",
                SizeGb = 3
            };
            volumeReply.MountPoints.Add("c:");

            Mock<IDatabase> databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(d => d.KeyExists(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(true);
            databaseMock.Setup(d => d.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None))
                .Returns(async (RedisKey userId, RedisValue key, CommandFlags flags) =>
                {
                    if (key == "Id")
                        return volumeReply.Id;
                    if (key == "Name")
                        return volumeReply.Name;
                    else if (key == "Size")
                        return volumeReply.SizeGb;
                    else if (key == "MountPoints")
                        return "c:";
                    else
                        throw new Exception();
                });

            Mock<IConnectionMultiplexer> connectionMock = new Mock<IConnectionMultiplexer>();
            connectionMock.Setup(d => d.GetDatabase(-1, null)).Returns(databaseMock.Object);
            Mock<EndPoint> endPointMock = new Mock<EndPoint>();
            connectionMock.Setup(d => d.GetEndPoints(false)).Returns(new EndPoint[] { endPointMock.Object });
            connectionMock
                .Setup(d => d.GetServer(It.IsAny<EndPoint>(), null).Keys(0, It.IsAny<RedisValue>(), 250, 0, 0, CommandFlags.None))
                .Returns(new RedisKey[] { "userId:vol-0000000" });

            Mock<IRedisConnectionService> redisServiceMock = new Mock<IRedisConnectionService>();
            redisServiceMock.Setup(d => d.Connect()).Returns(connectionMock.Object);

            Mock<ILogger<VolumesRepository>> loggerMock = new Mock<ILogger<VolumesRepository>>();
            VolumesRepository volumeDataSource = new VolumesRepository(redisServiceMock.Object, loggerMock.Object);

            IEnumerable<VolumeReply> volumes = await volumeDataSource.List("userId");
            foreach(VolumeReply volume in volumes)
            {
                Assert.IsTrue(volume.Id.Contains("vol"));
                Assert.IsTrue(volume.SizeGb == volumeReply.SizeGb);
                Assert.IsTrue(volume.Name == volumeReply.Name);
            }
        }
    }
}
