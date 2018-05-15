using System;
using System.Collections.Generic;
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

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate {AgentId = expectedResult},
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

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate {CreateDate = expectedResult},
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

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate {CreateDate = createDate},
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

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate {CreateDate = createDate},
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

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate {CreateDate = createDate},
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
        public void Should_SetNotReportedErrorsForSpecificAgent_When_AgentIsActive()
        {
            // Arrange
            const int agentId = 1;

            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate
                {
                    AgentId = agentId,
                    CreateDate = createDate
                },
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            var errors1 = new[]
            {
                new Error {Message = "error11"},
                new Error {Message = "error12"},
            };

            var agentState1 = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors1
            };

            var errors2 = new[]
            {
                new Error {Message = "error21"},
                new Error {Message = "error22"},
            };

            var agentState2 = new AgentState
            {
                AgentId = agentId,
                Errors = errors2
            };

            var errors3 = new[]
            {
                new Error {Message = "error31"},
                new Error {Message = "error32"},
            };

            var agentState3 = new AgentState
            {
                AgentId = 2,
                Errors = errors3
            };

            var errors4 = new[]
            {
                new Error {Message = "error41"},
                new Error {Message = "error42"},
            };

            var agentState4 = new AgentState
            {
                AgentId = agentId,
                Errors = errors4,
                ReportDate = new DateTime()
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.Get(It.IsAny<Expression<Func<AgentState, bool>>>(), null,
                    MonitoringContext.ErrorsProperty))
                .Returns((Expression<Func<AgentState, bool>> filter,
                        Func<IQueryable<AgentState>, IOrderedQueryable<AgentState>> orderBy,
                        string includeProperties) =>
                        new[] {agentState1, agentState2, agentState3, agentState4}.Where(filter.Compile()));

            // Act
            var actualResult = _aggregationService.Aggregate(Now).Single().Errors;

            // Assert
            CollectionAssert.AreEqual(errors1.Union(errors2).Select(x => x.Message), actualResult);
        }

        [Test]
        public void Should_UpdateAgentStateReportDate_When_AgentStateWasAggregatedAndAggregatedStateWasProcessed()
        {
            // Arrange
            const int agentId = 1;

            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate
                {
                    AgentId = agentId,
                    CreateDate = createDate
                },
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            var errors1 = new[]
            {
                new Error {Message = "error11"},
                new Error {Message = "error12"},
            };

            var agentState1 = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors1
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.Get(It.IsAny<Expression<Func<AgentState, bool>>>(), null,
                MonitoringContext.ErrorsProperty)).Returns(new[] {agentState1});

            // Act
            var result = _aggregationService.Aggregate(Now).ToArray();

            // Assert
            Assert.AreEqual(Now, agentState1.ReportDate);
        }

        [Test]
        public void Should_NotUpdateAgentStateReportDate_When_AgentStateWasAggregatedButAggregatedStateWasNotProcessed()
        {
            // Arrange
            const int agentId = 1;

            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate
                {
                    AgentId = agentId,
                    CreateDate = createDate
                },
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            var errors1 = new[]
            {
                new Error {Message = "error11"},
                new Error {Message = "error12"},
            };

            var agentState1 = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors1
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.Get(It.IsAny<Expression<Func<AgentState, bool>>>(), null,
                MonitoringContext.ErrorsProperty)).Returns(new[] {agentState1});

            // Act
            var result = _aggregationService.Aggregate(Now);

            // Assert
            Assert.IsNull(agentState1.ReportDate);
        }

        [Test]
        public void Should_SaveUpdatedAgentStateReportDate_When_AgentStateWasAggregatedAndAggregatedStateWasProcessed()
        {
            // Arrange
            const int agentId = 1;

            var createDate = Now - TimeSpan.FromMilliseconds(MonitoringSettings.AgentActivePeriod - 1000);

            var agentStateCreateDates = new[]
            {
                new AgentStateCreateDate
                {
                    AgentId = agentId,
                    CreateDate = createDate
                },
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.GetLastAgentStateCreateDates())
                .Returns(agentStateCreateDates);

            var errors1 = new[]
            {
                new Error {Message = "error11"},
                new Error {Message = "error12"},
            };

            var agentState1 = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors1
            };

            var errors2 = new[]
            {
                new Error {Message = "error21"},
                new Error {Message = "error22"},
            };

            var agentState2 = new AgentState
            {
                AgentId = agentId,
                CreateDate = createDate,
                Errors = errors2
            };

            _unitOfWork.Setup(x => x.AgentStateRepository.Get(It.IsAny<Expression<Func<AgentState, bool>>>(), null,
                MonitoringContext.ErrorsProperty)).Returns(new[] {agentState1, agentState2});

            var actualAgentStates = new List<AgentState>();
            _unitOfWork.Setup(x => x.AgentStateRepository.Update(It.IsAny<AgentState>()))
                .Callback<AgentState>(state => actualAgentStates.Add(state));

            // Act
            var result = _aggregationService.Aggregate(Now).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] {agentState1, agentState2}, actualAgentStates);
        }
    }
}