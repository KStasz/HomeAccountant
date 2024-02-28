using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace HomeAccountant.CoreTests.MockServices
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _response;

        public MockHttpMessageHandler()
        {
            _response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
        }

        public int NumberOfCalls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;

            if (request.Headers.Authorization == null)
                return Task.FromResult(new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized });

            return Task.FromResult(_response);
        }

        public void ConfigureResponse(HttpResponseMessage response)
        {
            _response = response;
        }
    }
}
