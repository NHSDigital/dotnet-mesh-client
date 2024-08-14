using System.Net;
using System.Text.Json;
using NHS.MESH.Client.Models;

public static class UnitTestHelpers
{
    public static string CreateMeshErrorResponseJsonString(string? errorEvent, string? errorCode, string errorDescription)
    {
        var errorObject = new APIErrorResponse
        {
            ErrorEvent = errorEvent ?? "",
            ErrorCode = errorCode ?? "",
            ErrorDescription = errorDescription
        };
        string jsonString = JsonSerializer.Serialize(errorObject);
        return jsonString;
    }

    public static HttpResponseMessage CreateMockHttpResponseMessage<T>(T data, HttpStatusCode httpStatusCode, Dictionary<string, string> headers = null)
    {
        string jsonData;
        if (typeof(T) == typeof(string))
        {
            jsonData = (string)(object)data;
        }
        else
        {
            jsonData = JsonSerializer.Serialize<T>(data);
        }
        StringContent messageContent = new StringContent(jsonData);

        HttpResponseMessage responseMessageMock = new HttpResponseMessage { StatusCode = httpStatusCode, Content = messageContent };
        if (headers != null)
        {
            foreach (var header in headers)
            {
                responseMessageMock.Headers.Add(header.Key, header.Value);
            }
        }

        return responseMessageMock;

    }

}
