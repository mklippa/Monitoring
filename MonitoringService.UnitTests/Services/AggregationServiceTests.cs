using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MonitoringService.Models;
using MonitoringService.Models.Entities;
using MonitoringService.Repositories;
using MonitoringService.Services;
using Moq;
using NUnit.Framework;

namespace MonitoringService.UnitTests.Services
{
    [TestFixture]
    public class AggregationServiceTests
    {
        private static readonly DateTime Now = new DateTime(2018, 5, 15, 18, 0, 0);

        private static readonly MonitoringSettings MonitoringSettings = new MonitoringSettings
        {
            AgentActivePeriod = 30000
        };

        private AggregationService _aggregationService;

        private Mock<IUnitOfWork> _unitOfWork;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            var options = new Mock<IOptions<MonitoringSettings>>();
            options.Setup(x => x.Value).Returns(MonitoringSettings);

            _aggregationService = new AggregationService(_unitOfWork.Object, options.Object);
        }

        [Test]
        public void Should_SetAgentId_When_AgentStateExists()
        {
            // Arrange
            const int expectedResult = 1;

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate{AgentId = expectedResult},
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().AgentId;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Should_SetLastReportDate_When_AgentStateExists()
        {
            // Arrange
            var expectedResult = new DateTime(2018, 5, 15);

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate{CreateDate = expectedResult},
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().LastReportDate;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Should_SetIsActiveToTrue_When_AgentIsActive()
        {
            // Arrange
            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate{CreateDate = createDate},
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().IsActive;

            // Assert
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void Should_SetIsActiveToFalse_When_AgentIsInactive()
        {
            // Arrange
            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod + 1000);

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate{CreateDate = createDate},
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().IsActive;

            // Assert
            Assert.IsFalse(actualResult);
        }

        [Test]
        public void Should_SetInactivePeriod_When_AgentIsInactive()
        {
            // Arrange
            var expectedResult = TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod + 1000);

            var createDate = Now - expectedResult;

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate{CreateDate = createDate},
            };

            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            var options = new Mock<IOptions<MonitoringSettings>>();
            options.Setup(x => x.Value).Returns(MonitoringSettings);

            var service = new AggregationService(unitOfWork.Object, options.Object);


            // Act
            var actualResult = service.Aggregate(Now).Single().InactivePeriod;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Should_SetErrors_When_AgentIsActive()
        {
            // Arrange
            const int agentId = 1;

            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new []
            {
                new AgentStateCreateDate
                {
                    AgentId = agentId,
                    CreateDate = createDate
                },
            };

            var errors = new []
            {
                new Error {Message = "error1"},
                new Error {Message = "error2"},
            };

            var agentState = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            _unitOfWork.Setup(x => x.AgentStateRepository.Get(It.IsAny<Expression<Func<AgentState, bool>>>(), null,
                    MonitoringContext.ErrorsProperty))
                .Returns(new[] {agentState});

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().Errors;

            // Assert
            CollectionAssert.AreEqual(errors.Select(x=>x.Message), actualResult);
        }
    }
}
