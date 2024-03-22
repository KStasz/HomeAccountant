using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeAccountant.Core.Services.Tests
{
    public class FriendsRealTimeServiceTests
    {
        private readonly Mock<ISignalRHubConnection> _signalRHubConnectionMock;
        private readonly FriendsRealTimeService _friendsRealTimeService;

        public FriendsRealTimeServiceTests()
        {
            _signalRHubConnectionMock = new Mock<ISignalRHubConnection>();

            _friendsRealTimeService = new FriendsRealTimeService(_signalRHubConnectionMock.Object);
        }

        [Fact()]
        public async Task FriendshipCreatedAsync_ShouldCallSendAsyncMethodOnlyOnce()
        {
            _signalRHubConnectionMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _friendsRealTimeService.FriendshipCreatedAsync();

            _signalRHubConnectionMock.Verify(
                x => x.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task InitializeAsync_ShouldCallOnAndStartAsyncMethodOnlyOnce()
        {
            await _friendsRealTimeService.InitializeAsync();

            _signalRHubConnectionMock.Verify(x => x.On(It.IsAny<string>(), It.IsAny<Func<It.IsAnyType>>()), Times.Once);
            _signalRHubConnectionMock.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}