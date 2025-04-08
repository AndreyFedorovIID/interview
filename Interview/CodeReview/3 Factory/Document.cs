/*
    🟢🟢🟢
    Разработчик решил сделать удобный интерфейс для чтения документа, представленного файлом,
    добавив именованный конструктор в существующий класс Document.
    Дополнительно, он решил повысить производительность, так как знал,
    что некоторый файл используется значительно чаще других.
 
    🔻🔻🔻
    Необходимо выполнить ревью нового метода Instance, размышляя вслух.
    Следует стараться упомянуть как можно больше возможных проблем.
    Объяснять их не надо, просто сказать об их наличии.

    После ревью нужно выбрать несколько понравившиеся проблем и рассказать про них подробнее.
    Написать комментарий для автора текстом.
*/

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