using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._4_Person;

[Table("persons")]
public sealed class Person
{
    private static IServiceProvider _services;

    private static IServiceProvider Services => _services ?? throw new InvalidOperationException();

    public static void Initialize(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
    }

    public static async Task<Person> CreateAsync(string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(name);

        var result = new Person();

        result.Name = name;

        await result.SaveAsync(cancellationToken);

        return result;
    }

    public static Task<Person?> LoadAsync(long id, CancellationToken cancellationToken)
    {
        var dbContext = CreateDbContext();
        return dbContext.Set<Person>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    // For Entity framework.
    [EditorBrowsable(EditorBrowsableState.Never)]
    private Person()
    {
    }

    public long Id { get; private set; }

    public string Name { get; set; }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        var dbContext = CreateDbContext();
        dbContext.Add(this);
        if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
        {
            Logger.LogDebug($"Error saving document {Name}");
        }
    }

    private static ILogger<Person> Logger => Services.GetRequiredService<ILogger<Person>>();

    private static DbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DbContext>();
    }
}