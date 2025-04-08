/*
    🟢🟢🟢
    Команде разработчиков потребовалось выполнить интеграцию с внешним сервисом,
    на который они не могут никак повлиять.
    У этого сервиса есть ClientSdk.dll, который содержит HttpRestApiClient.
    Разработчики заметили, что после добавления этой интеграции,
    а именно метода GetUserNames(), возникли проблемы производительности.
    Они попытались исправить их разными способами.

    🔻🔻🔻
    Необходимо выполнить ревью HttpRestApiClient и понять потенциальные проблемы его использования.
    Необходимо выполнить ревью попыток реализации метода GetUserNames(),
    и упорядочить их от худшего к лучшему.
    По возможности следует предложить свои варианты решения проблем.
*/

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._5_Optimization;

public sealed class HttpRestApiClient
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
        HttpRestApiClient client,
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
        HttpRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        return userIds
            .AsParallel()
            .Select(client.GetUserName)
            .Where(userName => Regex.IsMatch(userName, pattern));
    }
    
    public static async Task<IEnumerable<string>> GetUserNamesV3(
        HttpRestApiClient client,
        IEnumerable<string> userIds,
        string pattern)
    {
        var userNames = await Task.WhenAll(
            userIds.Select(userId => Task.Run(() => client.GetUserName(userId))));
        return userNames.Where(userName => Regex.IsMatch(userName, pattern));
    }
    
    public static Task<List<string>> GetUserNamesV4(
        HttpRestApiClient client,
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