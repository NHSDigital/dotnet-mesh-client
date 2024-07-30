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
}
