namespace CodeReview._3_Factory;

public sealed class Document
{
    private static volatile Document _instance;

    public static Document Instance(string file)
    {
        if (_instance == null || _instance.DocumentName != file)
        {
            lock (typeof(Document))
            {
                if (_instance == null || _instance.DocumentName != file)
                {
                    try
                    {
                        _instance = new Document
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

    public string DocumentName { get; set; }

    public string DocumentContent { get; set; }
}