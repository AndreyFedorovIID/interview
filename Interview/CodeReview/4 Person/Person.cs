/*
    🟢🟢🟢
    При разработке нового приложения с нуля возникла необходимость смоделировать физическое лицо.
    Один из разработчиков предложил следующий вариант модели
    и предлагает придерживаться такого шаблона в будущем.

    🔻🔻🔻
    Необходимо выполнить ревью нового класса Person, размышляя вслух.
    Следует стараться упомянуть как можно больше возможных проблем.
    Объяснять их не надо, просто сказать об их наличии.

    После ревью нужно выбрать несколько понравившиеся проблем и рассказать про них подробнее.
    Написать комментарий для автора текстом.
*/

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._4_Person;

[Table("persons")]
public sealed class Person
{
    private static IServiceProvider _services;

    private static IServiceProvider Services => _services ?? throw new InvalidOperationException();

    public static void InitializeContext(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
    }

    public static async Task<Person> CreateAsync(string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(name);

        var result = new Person();

        result.PersonName = name;

        await result.SaveAsync(cancellationToken);

        return result;
    }

    public static Task<Person?> LoadAsync(long id, CancellationToken cancellationToken)
    {
        return CreateDbContext()
            .Set<Person>()
            .FirstOrDefaultAsync(x => x.PersonId == id, cancellationToken);
    }

    // For Entity framework.
    [EditorBrowsable(EditorBrowsableState.Never)]
    private Person()
    {
    }

    public long PersonId { get; private set; }

    public string PersonName { get; set; }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        var dbContext = CreateDbContext();
        dbContext.Add(this);
        if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
        {
            Logger.LogDebug($"Error saving document {PersonName}");
        }
    }

    private static ILogger<Person> Logger => Services.GetRequiredService<ILogger<Person>>();

    private static DbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DbContext>();
    }
}