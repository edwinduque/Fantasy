using System.Net;

namespace Fantasy.Frontend.Repositories;

public class HttpResponseWrapper<T>
{
    public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
    {
        Response = response;
        Error = error;
        HttpResponseMessage = httpResponseMessage;
    }

    public T? Response { get; set; }
    public bool Error { get; set; }
    public HttpResponseMessage HttpResponseMessage { get; }

    public async Task<string?> GetErrorMessageAsync()
    {
        if (Response == null || HttpResponseMessage == null)
        {
            return null;
        }

        var statusCode = HttpResponseMessage.StatusCode;
        if(statusCode == HttpStatusCode.NotFound)
        {
            return "Not Found";
        }
        if (statusCode == HttpStatusCode.BadRequest)
        {
            return await HttpResponseMessage.Content.ReadAsStringAsync();
        }
        if(statusCode == HttpStatusCode.Unauthorized)
        {
            return "Unauthorized";
        }
        if (statusCode == HttpStatusCode.Forbidden)
        {
            return "Forbidden";
        }

        return "Unexpected error";
    }
}