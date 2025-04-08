using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._5_Optimization;

public interface IRestApiClient
{
    public string GetUserName(string userId);
}

public sealed class HttpRestApiClient : IRestApiClient
{
    public string Uri { get; set; }

    public string GetUserName(string userId)
        => new HttpClient()
            .Send(new HttpRequestMessage(HttpMethod.Get, Uri + $"/users/{userId}"))
            .Content
            .ReadAsStringAsync()
            .Result;
}

public static class RestApiClientHelper
{
    public static IEnumerable<string> GetUserNamesV1(
        IRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        foreach (var userId in userIds)
        {
            var userName = client.GetUserName(userId);
            if (Regex.IsMatch(userName, pattern) )
            {
                yield return userName;
            }
        }
    }
    
    public static IEnumerable<string> GetUserNamesV2(
        IRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        return userIds
            .AsParallel()
            .Select(client.GetUserName)
            .Where(userName => Regex.IsMatch(userName, pattern));
    }
    
    public static async Task<IEnumerable<string>> GetUserNamesV3(
        IRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        var userNames = await Task.WhenAll(
            userIds.Select(userId => Task.Run(() => client.GetUserName(userId))));
        return userNames.Where(userName => Regex.IsMatch(userName, pattern));
    }
    
    public static Task<List<string>> GetUserNamesV4(
        IRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        return userIds
            .Select(client.GetUserName)
            .AsQueryable()
            .Where(userName => Regex.IsMatch(userName, pattern))
            .ToListAsync();
    }
}