using Xunit;
using Moq;
using Domain.Services;
using HomeAccountant.FriendsService.Model;
using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Service;
using AutoMapper;
using Assert = Xunit.Assert;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;
using TestHelper;
using Microsoft.AspNetCore.Mvc;
using Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Microsoft.Extensions.Logging;
using Domain.Dtos.IdentityPlatform;
using Domain.Dtos.FriendsService;

namespace HomeAccountant.FriendsService.Controllers.Tests
{
    public class FriendshipControllerTests
    {
        private readonly Mock<IRepository<ApplicationDbContext, Friendships>> _friendshipRepository;
        private readonly Mock<IIdentityPlatformService> _identityPlatformService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<FriendshipController>> _logger;
        private readonly FriendshipController _controller;

        public FriendshipControllerTests()
        {
            _friendshipRepository = new Mock<IRepository<ApplicationDbContext, Friendships>>();
            _identityPlatformService = new Mock<IIdentityPlatformService>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<FriendshipController>>();

            _controller = new FriendshipController(
                _friendshipRepository.Object,
                _identityPlatformService.Object,
                _mapper.Object,
                _logger.Object);
        }

        [Fact()]
        public async Task GetFriendships_ShouldReturnCollection()
        {
            EndpointAuthorizationHelper.MockHttpContext(_controller, "UserId");

            _friendshipRepository
                .Setup(x => x.GetAll(It.IsAny<Func<Friendships, bool>>()))
                .Returns(new List<Friendships>()
                {
                    new Friendships()
                    {
                        Id = It.IsAny<int>(),
                        CreatorId = It.IsAny<string>(),
                        FriendId = It.IsAny<string>(),
                        UserId = It.IsAny<string>(),
                        CreationDate = It.IsAny<DateTime>(),
                        IsAccepted = It.IsAny<bool>(),
                        IsActive = It.IsAny<bool>()
                    }
                }.AsEnumerable());

            _identityPlatformService
                .Setup(x => x.GetUsersAsync(It.IsAny<string[]>()))
                .ReturnsAsync(
                new ServiceResponse<UserModelDto[]?>(
                    new UserModelDto[]
                    {
                        new UserModelDto()
                        {
                            Id = It.IsAny<string>(),
                            Email = It.IsAny<string>(),
                            UserName = It.IsAny<string>()
                        },
                        new UserModelDto()
                        {
                            Id = It.IsAny<string>(),
                            Email = It.IsAny<string>(),
                            UserName = It.IsAny<string>()
                        },
                        new UserModelDto()
                        {
                            Id = It.IsAny<string>(),
                            Email = It.IsAny<string>(),
                            UserName = It.IsAny<string>()
                        }
                    }));

            var response = await _controller.GetFriendships();

            Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<ServiceResponse<IEnumerable<FriendshipReadDto>?>>(((OkObjectResult)response.Result).Value);
            Assert.True(((ServiceResponse<IEnumerable<FriendshipReadDto>?>?)((OkObjectResult?)response.Result)?.Value)?.Result);
            Assert.NotNull(((response.Result as OkObjectResult)?.Value as ServiceResponse<IEnumerable<FriendshipReadDto>?>)?.Value);
            Assert.NotEmpty(((response.Result as OkObjectResult)?.Value as ServiceResponse<IEnumerable<FriendshipReadDto>?>)?.Value ?? Array.Empty<FriendshipReadDto>());
        }

        [Fact()]
        public async Task CreateFriendshipRequest_ShouldCreateNotificationIfFriendshipAndNotificationDoesntExists()
        {
            EndpointAuthorizationHelper.MockHttpContext(_controller, "SampleUser");

            _identityPlatformService.Setup(x => x.GetUserIdByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync("ownerId");

            _identityPlatformService.Setup(x => x.GetEmailByUserId(It.IsAny<string>()))
                .ReturnsAsync(new Domain.Model.ServiceResponse<string?>(value: "senderEmailResponse"));

            _friendshipRepository.Setup(
                x => x.Get(It.IsAny<Func<Friendships, bool>>()))
                .Returns(value: null);

            var response = await _controller.CreateFriendshipRequest(It.IsAny<string>());

            Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<ServiceResponse>(((OkObjectResult)response.Result).Value);
            Assert.True(((ServiceResponse?)((OkObjectResult?)response.Result)?.Value)?.Result);
        }
    }
}