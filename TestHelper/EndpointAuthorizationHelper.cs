using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace TestHelper
{
    public static class EndpointAuthorizationHelper
    {
        public static void MockHttpContext(ControllerBase controller, string userId)
        {
            var claimsMock = new Mock<ClaimsPrincipal>();
            var httpContextMock = new Mock<HttpContext>();
            IEnumerable<Claim> claims = new List<Claim>()
            {
                new Claim("UserId", userId)
            }.AsEnumerable();

            claimsMock.Setup(x => x.Claims)
                .Returns(claims);

            httpContextMock.Setup(
                x => x.User)
                .Returns(claimsMock.Object);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContextMock.Object;
        }
    }
}
