using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Models;
public class MeshResponse<TSuccess>
{
    public bool IsSuccessful;

    public TSuccess Response;

    public APIErrorResponse? Error;

}
