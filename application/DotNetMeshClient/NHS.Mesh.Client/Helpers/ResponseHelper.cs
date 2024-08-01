using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Helpers;

public static class ResponseHelper
{
    public static async Task<MeshResponse<TSuccess>> CreateMeshResponse<TSuccess>(HttpResponseMessage httpResponseMessage, Func<HttpResponseMessage,Task<TSuccess>> mappingFunction )
    {
        if(httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return new MeshResponse<TSuccess>
            {
                IsSuccessful = true,
                Response = await mappingFunction(httpResponseMessage)
            };
        }
        else
        {
            return new MeshResponse<TSuccess>
            {
                IsSuccessful = false,
                Error = await MapErrorResponse(httpResponseMessage)
            };
        }

    }

    public static string GetHeaderItemValue(this HttpResponseHeaders headers, string key)
    {
        return headers.FirstOrDefault(h => h.Key == key).Value.FirstOrDefault();
    }

    private static async Task<APIErrorResponse> MapErrorResponse(HttpResponseMessage httpResponseMessage)
    {
        var errorString = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<APIErrorResponse>(errorString);
    }

}
