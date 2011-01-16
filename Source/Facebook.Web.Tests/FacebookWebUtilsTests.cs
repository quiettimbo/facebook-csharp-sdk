namespace Facebook.Web.Tests
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Web;
    using Facebook.Web;
    using Moq;
    using Xunit;
    using Xunit.Extensions;

    public class FacebookWebUtilsTests
    {
        [Fact(DisplayName = "GetSessionCookieValue: Given a http request with no fb session cookie Then it should return null")]
        public void GetSessionCookieValue_GivenAHttpRequestWithNoFbSessionCookie_ThenItShouldReturnNull()
        {
            var requestParams = new NameValueCollection();
            var request = GetHttpRequest(requestParams);
            var appId = "dummy";

            var cookieValue = FacebookWebUtils.GetSessionCookieValue(appId, request);

            Assert.Null(cookieValue);
        }

        [Fact(DisplayName = "GetSessionCookieValue: Given a http request with a valid fb session cookie Then it should not return null")]
        public void GetSessionCookieValue_GivenAHttpRequestWithAValidFbSessionCookie_ThenItShouldNotReturnNull()
        {
            var requestParams = new NameValueCollection
                                    {
                                        { "fbs_dummyappid", "access_token=124973200873702%7C2.OAaqICOCk_B4sZNv59q8Yg__.3600.1295118000-100001327642026%7Cvz4H9xjlRZPfg2quCv0XOM5g9_o&expires=1295118000&secret=lddpssZCuPoEtjcDFcWtoA__&session_key=2.OAaqICOCk_B4sZNv59q8Yg__.3600.1295118000-100001327642026&sig=1d95fa4b3dfa5b26c01c8ac8676d80b8&uid=100001327642026" }
                                    };

            var request = GetHttpRequest(requestParams);
            var appId = "dummyappid";

            var cookieValue = FacebookWebUtils.GetSessionCookieValue(appId, request);

            Assert.NotNull(cookieValue);
        }

        [Fact(DisplayName = "GetSession: Given a http request with no signed request or fb session cookies Then it should return null")]
        public void GetSession_GivenAHttpRequestWithNoSignedRequestOrFbSessionCookies_ThenItShouldReturnNull()
        {
            var requestParams = new NameValueCollection();
            var request = GetHttpRequest(requestParams);
            var appId = "dummy";
            var appSecret = "dummy";

            var session = FacebookWebUtils.GetSession(appId, appSecret, request);

            Assert.Null(session);
        }

        [InlineData("543690fae0cd186965412ac4a49548b5", "Iin8a5nlQOHhlvHu_4lNhKDDvut6s__fm6-jJytkHis.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImV4cGlyZXMiOjEyODI5Mjg0MDAsIm9hdXRoX3Rva2VuIjoiMTIwNjI1NzAxMzAxMzQ3fDIuSTNXUEZuXzlrSmVnUU5EZjVLX0kyZ19fLjM2MDAuMTI4MjkyODQwMC0xNDgxMjAxN3xxcmZpT2VwYnY0ZnN3Y2RZdFJXZkFOb3I5YlEuIiwidXNlcl9pZCI6IjE0ODEyMDE3In0")]
        [Theory(DisplayName = "GetSession: Given a http request with a valid signed request and valid app secret Then it should not return null")]
        public void GetSession_GivenAHttpRequestWithAValidSignedRequestAndValidAppSecret_ThenItShouldNotReturnNull(string appSecret, string signedRequestValue)
        {
            var requestParams = new NameValueCollection { { "signed_request", signedRequestValue } };
            var request = GetHttpRequest(requestParams);
            var appId = "dummy";

            var session = FacebookWebUtils.GetSession(appId, appSecret, request);

            Assert.NotNull(session);
        }

        [InlineData("Iin8a5nlQOHhlvHu_4lNhKDDvut6s__fm6-jJytkHis.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImV4cGlyZXMiOjEyODI5Mjg0MDAsIm9hdXRoX3Rva2VuIjoiMTIwNjI1NzAxMzAxMzQ3fDIuSTNXUEZuXzlrSmVnUU5EZjVLX0kyZ19fLjM2MDAuMTI4MjkyODQwMC0xNDgxMjAxN3xxcmZpT2VwYnY0ZnN3Y2RZdFJXZkFOb3I5YlEuIiwidXNlcl9pZCI6IjE0ODEyMDE3In0")]
        [Theory(DisplayName = "GetSession: Given a http request with a valid signed request and invalid app secret Then it should throw InvalidOperationException")]
        public void GetSession_GivenAHttpRequestWithAValidSignedRequestAndInvalidAppSecret_ThenItShouldThrowInvalidOperationException(string signedRequestValue)
        {
            var requestParams = new NameValueCollection { { "signed_request", signedRequestValue } };
            var request = GetHttpRequest(requestParams);
            var appId = "dummy";
            var appSecret = "invalid_secret";

            Assert.Throws<InvalidOperationException>(() => FacebookWebUtils.GetSession(appId, appSecret, request));
        }

        [InlineData("dummyappid")]
        [Theory(DisplayName = "GetSessionCookieName: Given a app id Then it returns fbs_ contatenated with app id")]
        public void GetSessionCookieName_GivenAAppId_ThenItReturnsFbsUnderscoreContatenatedWithAppId(string appId)
        {
            var expectedCookieName = string.Concat("fbs_", appId);

            var result = FacebookWebUtils.GetSessionCookieName(appId);

            Assert.Equal(expectedCookieName, result);
        }

        [Fact(DisplayName = "GetSignedRequest: Given no signed_request HttpRequest Param Then it should return null")]
        public void GetSignedRequest_GivenNoSignedRequestHttpRequestParam_ThenItShouldReturnNull()
        {
            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(request => request.Params).Returns(new NameValueCollection());

            var signedRequest = FacebookWebUtils.GetSignedRequest("dummy_app_secret", requestMock.Object);

            Assert.Null(signedRequest);
        }

        [InlineData("543690fae0cd186965412ac4a49548b5", "Iin8a5nlQOHhlvHu_4lNhKDDvut6s__fm6-jJytkHis.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImV4cGlyZXMiOjEyODI5Mjg0MDAsIm9hdXRoX3Rva2VuIjoiMTIwNjI1NzAxMzAxMzQ3fDIuSTNXUEZuXzlrSmVnUU5EZjVLX0kyZ19fLjM2MDAuMTI4MjkyODQwMC0xNDgxMjAxN3xxcmZpT2VwYnY0ZnN3Y2RZdFJXZkFOb3I5YlEuIiwidXNlcl9pZCI6IjE0ODEyMDE3In0")]
        [Theory(DisplayName = "GetSignedRequest: Given a valid signed_request HttpRequest Param and secret Then the result should not be null")]
        public void GetSignedRequest_GivenAValidSignedRequestHttpRequestParamAndSecret_ThenTheResultShouldNotBeNull(string secret, string signedRequestValue)
        {
            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(request => request.Params).Returns(new NameValueCollection { { "signed_request", signedRequestValue } });

            var signedRequest = FacebookWebUtils.GetSignedRequest(secret, requestMock.Object);

            Assert.NotNull(signedRequest);
        }

        [InlineData("Iin8a5nlQOHhlvHu_4lNhKDDvut6s__fm6-jJytkHis.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImV4cGlyZXMiOjEyODI5Mjg0MDAsIm9hdXRoX3Rva2VuIjoiMTIwNjI1NzAxMzAxMzQ3fDIuSTNXUEZuXzlrSmVnUU5EZjVLX0kyZ19fLjM2MDAuMTI4MjkyODQwMC0xNDgxMjAxN3xxcmZpT2VwYnY0ZnN3Y2RZdFJXZkFOb3I5YlEuIiwidXNlcl9pZCI6IjE0ODEyMDE3In0")]
        [Theory(DisplayName = "GetSignedRequest: Given a valid signed_request HttpRequest Param and invalid secret Then it should throw invalid operation exception")]
        public void GetSignedRequest_GivenAValidSignedRequestHttpRequestParamAndInvalidSecret_ThenItShouldThrowInvalidOperationException(string signedRequestValue)
        {
            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(request => request.Params).Returns(new NameValueCollection { { "signed_request", signedRequestValue } });

            Assert.Throws<InvalidOperationException>(
                () => FacebookWebUtils.GetSignedRequest("wrong_secret", requestMock.Object));
        }

        private static HttpRequestBase GetHttpRequest(NameValueCollection requestParams)
        {
            Contract.Requires(requestParams != null);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(request => request.Params).Returns(requestParams);

            return requestMock.Object;
        }
    }
}