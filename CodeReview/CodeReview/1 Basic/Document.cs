/*
    🔻🔻🔻
    Необходимо выполнить ревью представленного кода, размышляя вслух.
    Следует стараться упомянуть как можно больше проблем.
    После ревью нужно выбрать 3-4 понравившиеся проблемы и рассказать про них подробнее,
    желательно в виде комментариев для автора.

    🟢🟢🟢
    Контекст задачи.
    Нужно построить программную модель документа, представленного в виде текстового файла.
    Нужно предусмотреть:
        - возможность хранения информации о документах в базе данных;
        - оптимизиацию доступа к часто используемым документам;
        - возможность сериализации для передачи по сети;
        - удобный способ анализа содержимого документа;
        - возможность проверки наличия в документе зарпещённых строк.
*/

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._1_Basic;

[Table("doc")]
public class Document : IEnumerable<char>, IAsyncDisposable
{
    private static volatile Document _instance;

    public static Document Instance(string file, ServiceProvider serviceProvider)
    {
        if (_instance == null || _instance._documentName != file)
        {
            lock (typeof(Document))
            {
                if (_instance == null || _instance._documentName != file)
                {
                    try
                    {
                        _instance = new Document(serviceProvider)
                        {
                            DocumentName = file,
                            DocumentContent = File.ReadAllText(file)
                        };
                    }
                    catch (Exception e)
                    {
                        if (e is FileNotFoundException || e is DirectoryNotFoundException)
                            throw new ArgumentException("bad file OOF");
                        throw e;
                    }
                }
            }
        }

        return _instance;
    }

    // For Entity framework
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Document()
    {
    }

    public Document(ServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    private ServiceProvider _serviceProvider;

    private ILogger<Document> Logger => _serviceProvider.GetRequiredService<ILogger<Document>>();

    private DbContext CreateDbContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DbContext>();
    }

    public async ValueTask SaveDocument()
    {
        var dbContext = CreateDbContext();
        dbContext.Add(this);
        if (await dbContext.SaveChangesAsync() != 1)
        {
            Logger.LogDebug($"Error saving document {DocumentContent}");
        }
    }

    [JsonIgnore]
    public long DocumentDbId { get; set; }

    private string _documentName;

    public virtual required string DocumentName
    {
        get => _documentName;
        set => _documentName = value ?? throw new InvalidOperationException();
    }

    public string GetDocumentName() => Path.GetFileName(_documentName);

    [NotMapped]
    public string DocumentContent { get; set; }

    public string DocumentJson()
    {
        JsonDocument json = JsonSerializer.SerializeToDocument(this);
        return json.RootElement.GetRawText();
    }

    public IEnumerator<char> GetEnumerator()
    {
        ArgumentNullException.ThrowIfNull(DocumentContent);

        foreach (var ch in DocumentContent) yield return ch;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        ArgumentNullException.ThrowIfNull(DocumentContent);

        foreach (var ch in DocumentContent) yield return ch;
    }

    private static OperationCanceledException _cachedException = new ();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> CheckIsValid(
        CancellationToken ct,
        IReadOnlyList<string> errorStrings)
    {
        errorStrings = errorStrings == default
            ? throw new NullReferenceException("error strings")
            : errorStrings;
        if (!errorStrings.Any())
        {
            if (ct.IsCancellationRequested) throw _cachedException;

            return await ValueTask.FromResult(true).ConfigureAwait(false);
        }

        Monitor.Enter(DocumentContent);
        // offload for optimization
        var run = await Task.Run(
            async () =>
            {
                errorStrings = await errorStrings.AsQueryable().Distinct().ToListAsync(ct);

                var bad = errorStrings
                    .AsParallel()
                    .Any(_ => Regex.Count(DocumentContent, _, RegexOptions.IgnoreCase) > 0);

                if (ct.IsCancellationRequested) throw _cachedException;

                return !bad;
            });
        Monitor.Exit(DocumentContent);

        if (ct.IsCancellationRequested) throw _cachedException;

        return run;
    }

    public static bool operator ==(Document first, Document second)
        => first.GetHashCode() == second.GetHashCode() && Equals(first, second);

    public static bool operator !=(Document first, Document second)
        => first.GetHashCode() != second.GetHashCode() && !Equals(first, second);

    public bool Equals(Document other) => DocumentDbId == other.DocumentDbId;

    public bool DeepEquals(Document other)
    {
        var stream = new MemoryStream();
        var otherStream = new MemoryStream();
        // TODO: use json?
#pragma warning disable SYSLIB0011 BinaryFormatter ok for now
        new BinaryFormatter().Serialize(stream, this);
        new BinaryFormatter().Serialize(otherStream, other);

        return StringOf(stream.ToArray()) == StringOf(otherStream.ToArray());
    }

    public virtual string StringOf(in byte[] data) => BitConverter.ToString(data).Replace("-", "");

    public override int GetHashCode() => (int) DocumentDbId;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _serviceProvider.DisposeAsync();
    }

    ~Document()
    {
        _serviceProvider.Dispose();
    }
}

public static class Usage
{
    public static async Task Example()
    {
        var servicesBuilder = new ServiceCollection();
        servicesBuilder.AddLogging();
        servicesBuilder.AddDbContext<DbContext>();

        var services = servicesBuilder.BuildServiceProvider();

        await using var document = Document.Instance("path/to/file", services);

        if (await document.CheckIsValid(CancellationToken.None, ["<content>", "<html", "{0}"]))
        {
            await document.SaveDocument();

            const int dataTransferLimit = 1_000_0000;
            if (document.Count() <= dataTransferLimit)
            {
                using var httpClient = new HttpClient();
                await httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/")
                    {
                        Content = new StringContent(document.DocumentJson())
                    });
            }
        }
    }
}