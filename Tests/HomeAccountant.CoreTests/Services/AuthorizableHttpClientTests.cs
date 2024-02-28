using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.CoreTests.MockServices;
using Moq;
using System.Net;
using Xunit;
using Assert = Xunit.Assert;

namespace HomeAccountant.Core.Services.Tests
{
    public class AuthorizableHttpClientTests
    {
        private Uri _baseAddress = new Uri("http://example.com");
        private MockHttpMessageHandler _httpMessageHandler;
        private HttpClient _httpClient;
        private Mock<ITokenStorageAccessor> _tokenStorageAccessor;
        private Mock<JwtAuthenticationStateProvider> _jwtAuthenticationStateProvider;
        private Mock<ITypeMapper<LoginResponseModel, TokenAuthenticationModel>> _loginResponseMapper;
        private AuthorizableHttpClient _authorizableHttpClient;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<IJwtTokenParser> _jwtTokenParserService;


        public AuthorizableHttpClientTests()
        {
            _httpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_httpMessageHandler)
            {
                BaseAddress = _baseAddress
            };
            _tokenStorageAccessor = new Mock<ITokenStorageAccessor>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _jwtTokenParserService = new Mock<IJwtTokenParser>();
            _jwtAuthenticationStateProvider = new Mock<JwtAuthenticationStateProvider>(
                () => new JwtAuthenticationStateProvider(
                    It.IsAny<ITokenStorageAccessor>(),
                    It.IsAny<IJwtTokenParser>()));
            _loginResponseMapper = new Mock<ITypeMapper<LoginResponseModel, TokenAuthenticationModel>>();

            _authorizableHttpClient = new AuthorizableHttpClient(
                _httpClient,
                _tokenStorageAccessor.Object,
                _authenticationServiceMock.Object,
                _jwtTokenParserService.Object,
                _jwtAuthenticationStateProvider.Object,
                _loginResponseMapper.Object);
        }

        //PostAsJsonAsync Tests

        [Fact()]
        public async Task PostAsJsonAsync_ShouldSendRequestIfTokenIsNotExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }
        
        [Fact()]
        public async Task PostAsJsonAsync_ShouldReceiveUnauthorizedIfReadingStoredTokenReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PostAsJsonAsync_ShouldReceiveUnauthorizedIfParsingExpiryDateReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);

            var response = await _authorizableHttpClient.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }
        
        [Fact()]
        public async Task PostAsJsonAsync_ShouldReceiveUnauthorizedIfRefreshTokenFailed()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);
            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<TokenAuthenticationModel?>(false));

            var response = await _authorizableHttpClient.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PostAsJsonAsync_ShouldRefreshTokenIfTokenIsExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.SetupSequence(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(DateTime.UtcNow.AddDays(-1))
                .Returns(DateTime.UtcNow.AddDays(1));

            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<TokenAuthenticationModel?>(
                    new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>())));

            var response = await _authorizableHttpClient.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        //PutAsJsonAsync

        [Fact()]
        public async Task PutAsJsonAsync_ShouldSendRequestIfTokenIsNotExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PutAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsJsonAsync_ShouldReceiveUnauthorizedIfReadingStoredTokenReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PutAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsJsonAsync_ShouldReceiveUnauthorizedIfParsingExpiryDateReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);

            var response = await _authorizableHttpClient.PutAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsJsonAsync_ShouldReceiveUnauthorizedIfRefreshTokenFailed()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);
            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<TokenAuthenticationModel?>(false));

            var response = await _authorizableHttpClient.PutAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsJsonAsync_ShouldRefreshTokenIfTokenIsExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.SetupSequence(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(DateTime.UtcNow.AddDays(-1))
                .Returns(DateTime.UtcNow.AddDays(1));

            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<TokenAuthenticationModel?>(
                    new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>())));

            var response = await _authorizableHttpClient.PutAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        //PutAsync Tests

        [Fact()]
        public async Task PutAsync_ShouldSendRequestIfTokenIsNotExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PutAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsync_ShouldReceiveUnauthorizedIfReadingStoredTokenReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.PutAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsync_ShouldReceiveUnauthorizedIfParsingExpiryDateReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);

            var response = await _authorizableHttpClient.PutAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsync_ShouldReceiveUnauthorizedIfRefreshTokenFailed()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);
            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<TokenAuthenticationModel?>(false));

            var response = await _authorizableHttpClient.PutAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task PutAsync_ShouldRefreshTokenIfTokenIsExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.SetupSequence(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(DateTime.UtcNow.AddDays(-1))
                .Returns(DateTime.UtcNow.AddDays(1));

            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<TokenAuthenticationModel?>(
                    new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>())));

            var response = await _authorizableHttpClient.PutAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        //DeleteAsync Tests

        [Fact()]
        public async Task DeleteAsync_ShouldSendRequestIfTokenIsNotExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task DeleteAsync_ShouldReceiveUnauthorizedIfReadingStoredTokenReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task DeleteAsync_ShouldReceiveUnauthorizedIfParsingExpiryDateReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);

            var response = await _authorizableHttpClient.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task DeleteAsync_ShouldReceiveUnauthorizedIfRefreshTokenFailed()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);
            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<TokenAuthenticationModel?>(false));

            var response = await _authorizableHttpClient.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task DeleteAsync_ShouldRefreshTokenIfTokenIsExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.SetupSequence(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(DateTime.UtcNow.AddDays(-1))
                .Returns(DateTime.UtcNow.AddDays(1));

            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<TokenAuthenticationModel?>(
                    new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>())));

            var response = await _authorizableHttpClient.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }
        
        //GetAsync Tests

        [Fact()]
        public async Task GetAsync_ShouldSendRequestIfTokenIsNotExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task GetAsync_ShouldReceiveUnauthorizedIfReadingStoredTokenReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(1);

            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);

            var response = await _authorizableHttpClient.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task GetAsync_ShouldReceiveUnauthorizedIfParsingExpiryDateReturnsNull()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);

            var response = await _authorizableHttpClient.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task GetAsync_ShouldReceiveUnauthorizedIfRefreshTokenFailed()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.Setup(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(expectedExpirationDate);
            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<TokenAuthenticationModel?>(false));

            var response = await _authorizableHttpClient.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }

        [Fact()]
        public async Task GetAsync_ShouldRefreshTokenIfTokenIsExpired()
        {
            var expectedTokenAuthenticationModel = new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>());
            var expectedExpirationDate = DateTime.UtcNow.AddDays(-1);

            _tokenStorageAccessor.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenAuthenticationModel);
            _jwtTokenParserService.SetupSequence(x => x.GetTokenExpirationDate(expectedTokenAuthenticationModel.Token))
                .Returns(DateTime.UtcNow.AddDays(-1))
                .Returns(DateTime.UtcNow.AddDays(1));

            _authenticationServiceMock.Setup(
                x => x.RefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<TokenAuthenticationModel?>(
                    new TokenAuthenticationModel(It.IsAny<string>(), It.IsAny<string>())));

            var response = await _authorizableHttpClient.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _httpMessageHandler.NumberOfCalls);
        }
    }
}