using HomeAccountant.Core.Exceptions;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;
using Assert = Xunit.Assert;

namespace HomeAccountant.Core.Services.Tests
{
    public class SignalRHubConnectionTests
    {
        private readonly Mock<HubConnection> _hubConnectionMock;
        private readonly SignalRHubConnection _signalRHubConnection;
        private readonly Mock<IHubConnectionSenderAsync> _hubConnectionSenderAsync;
        private readonly Mock<IHubConnectionStateGetter> _hubConnectionStateGetter;
        private readonly Mock<IHubConnectionConfigurator> _hubConnectionConfigurator;
        private readonly Mock<ILogger<SignalRHubConnection>> _loggerMock;
        private readonly Mock<IConnectionFactory> _connectionFactory;
        private readonly Mock<IHubProtocol> _protocol;
        private readonly Mock<EndPoint> _endPoint;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly Mock<ILoggerFactory> _loggerFactory;

        public SignalRHubConnectionTests()
        {
            _connectionFactory = new Mock<IConnectionFactory>();
            _protocol = new Mock<IHubProtocol>();
            _endPoint = new Mock<EndPoint>();
            _serviceProvider = new Mock<IServiceProvider>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _hubConnectionMock = new Mock<HubConnection>(
                _connectionFactory.Object,
                _protocol.Object,
                _endPoint.Object,
                _serviceProvider.Object,
                _loggerFactory.Object);
            _hubConnectionSenderAsync = new Mock<IHubConnectionSenderAsync>();
            _hubConnectionStateGetter = new Mock<IHubConnectionStateGetter>();
            _hubConnectionConfigurator = new Mock<IHubConnectionConfigurator>();
            _loggerMock = new Mock<ILogger<SignalRHubConnection>>();

            _signalRHubConnection = new SignalRHubConnection(
                _hubConnectionMock.Object,
                _hubConnectionSenderAsync.Object,
                _hubConnectionStateGetter.Object,
                _hubConnectionConfigurator.Object,
                _loggerMock.Object);
        }

        [Fact()]
        public async Task StartAsync_ShouldCallHubConnectionOnlyOnce()
        {
            _hubConnectionMock.Setup(x => x.StartAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _signalRHubConnection.StartAsync();
            _hubConnectionMock.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact()]
        public void On_ShouldConfigureEventWithEventName()
        {
            var exception = Record.Exception(() => _signalRHubConnection.On("EventName", () => 1));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType>>()),
                Times.Once);
            Assert.Null(exception);
        }

        [Fact()]
        public void On_ShouldThrowExceptionIfMethodNameIsNull()
        {
            var exception = Record.Exception(() => _signalRHubConnection.On(null, () => 1));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType>>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact()]
        public void On_ShouldThrowExceptionIfFuncParameterIsNull()
        {
            Func<int>? nullFunc = null;
            var exception = Record.Exception(() => _signalRHubConnection.On("MethodName", nullFunc));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType>>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact()]
        public void On_WithFuncParam_ShouldConfigureEventWithEventName()
        {
            var exception = Record.Exception(() => _signalRHubConnection.On<int, int>("EventName", (x) => 1));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType, It.IsAnyType>>()),
                Times.Once);
            Assert.Null(exception);
        }

        [Fact()]
        public void On_WithFuncParam_ShouldThrowExceptionIfMethodNameIsNull()
        {
            var exception = Record.Exception(() => _signalRHubConnection.On<int, int>(null, (x) => 1));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType, It.IsAnyType>>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact()]
        public void On_WithFuncParam_ShouldThrowExceptionIfFuncParameterIsNull()
        {
            Func<int, int>? nullFunc = null;
            var exception = Record.Exception(() => _signalRHubConnection.On("MethodName", nullFunc));

            _hubConnectionConfigurator.Verify(
                x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType, It.IsAnyType>>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact()]
        public async Task SendAsync_ShouldSuccessfullySendMethodInvocation()
        {
            _hubConnectionSenderAsync.Setup(
                x => x.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _hubConnectionStateGetter.SetupGet(x => x.State)
                .Returns(HubConnectionState.Connected);

            var exception = await Record.ExceptionAsync(() => _signalRHubConnection.SendAsync("MethodName"));

            _hubConnectionSenderAsync.Verify(
                x => x.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.Null(exception);
        }

        [Fact()]
        public async Task SendAsync_ShouldNotInvokeMethodIfConnectionWasNotEstablished()
        {
            _hubConnectionSenderAsync.Setup(
               x => x.SendAsync(
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            var exception = await Record.ExceptionAsync(() => _signalRHubConnection.SendAsync("MethodName"));

            _hubConnectionSenderAsync.Verify(
                x => x.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<SignalRHubConnectionException>(exception);
        }

        [Fact()]
        public async Task SendAsync_ShouldThrowExceptionIfMethodNameIsMissing()
        {
            _hubConnectionSenderAsync.Setup(
                x => x.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var exception = await Record.ExceptionAsync(() => _signalRHubConnection.SendAsync(null));

            _hubConnectionSenderAsync.Verify(
                x => x.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}