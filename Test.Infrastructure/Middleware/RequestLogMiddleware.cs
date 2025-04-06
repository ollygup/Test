using log4net;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace Test.Infrastructure.Middleware
{
    public class RequestLogMiddleware : IMiddleware
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RequestLogMiddleware));

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var logMessage = new StringBuilder();

            var request = context.Request;
            var apiName = request.Path.ToString();
            var timeCalled = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            logMessage.AppendLine($"API Name: {apiName}");
            logMessage.AppendLine($"Time Called: {timeCalled}");
            logMessage.AppendLine($"Request Method: {request.Method}");
            logMessage.AppendLine($"Request URL: {request.Scheme}://{request.Host}{request.Path}{request.QueryString}");

            var originalRequestBodyStream = request.Body;
            var memoryStream = new MemoryStream();

            await request.Body.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var requestBody = await new StreamReader(memoryStream).ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                try
                {
                    // Add Request Body to log
                    var jsonBody = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(requestBody);
                    EncryptFields(jsonBody);

                    var encryptedRequestBody = JsonSerializer.Serialize(jsonBody);

                    logMessage.AppendLine("Request Body:");
                    logMessage.AppendLine(encryptedRequestBody);
                }
                catch (JsonException)
                {
                    logMessage.AppendLine("Request Body: Invalid or non-JSON content");
                }
            }
            else
            {
                logMessage.AppendLine("Request Body: Empty");
            }

            // Reset response body stream position before reading
            memoryStream.Seek(0, SeekOrigin.Begin);
            request.Body = memoryStream;

            var originalResponseBodyStream = context.Response.Body;

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await next(context);

            // Read the response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            logMessage.AppendLine("Response Body:");
            logMessage.AppendLine(responseBody);

            log.Info(logMessage.ToString());

            // Reset the response body stream position before copying
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }

        private void EncryptFields(Dictionary<string, JsonElement> jsonBody)
        {
            var fieldsToEncrypt = new List<string> { "partnerPassword" };

            foreach (var field in fieldsToEncrypt)
            {
                if (jsonBody.ContainsKey(field))
                {
                    // Fake encryption
                    var encryptedValue = "ENCRYPTED";
                    using (var doc = JsonDocument.Parse($"\"{encryptedValue}\""))
                    {
                        jsonBody[field] = doc.RootElement.Clone();
                    }
                }
            }
        }
    }
}
