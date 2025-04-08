/*
    🟢🟢🟢
    Задача заключалась в разработке моделей для реализации логики веб конструктора клавиатур (см. картинку).
 
    🔻🔻🔻
    Необходимо выполнить ревью представленного кода, размышляя вслух.
    Следует стараться упомянуть как можно больше возможных проблем.
    Объяснять их не надо, просто сказать об их наличии.

    После ревью нужно выбрать несколько понравившиеся проблем и рассказать про них подробнее.
    Написать комментарий для автора текстом.
*/

using System.Runtime.Serialization.Formatters.Binary;

namespace CodeReview._1_Keyboard;

public class Keyboard
{
    public long KeyboardBaseCode { get; set; }

    public KeyboardLayout KeyboardLayout { get; set; }

    public KeyboardSize KeyboardSize { get; set; }

    public required List<Key> KeyboardKeys { get; set; }

    public static bool operator ==(Keyboard first, Keyboard second)
        => first.GetHashCode() == second.GetHashCode() && Equals(first, second);

    public static bool operator !=(Keyboard first, Keyboard second)
        => first.GetHashCode() != second.GetHashCode() && !Equals(first, second);

    public bool Equals(Keyboard other) => KeyboardBaseCode == other.KeyboardBaseCode;

    public override int GetHashCode() => (int) KeyboardBaseCode;

    public bool DeepEquals(Keyboard other)
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
    
    public static List<Keyboard> Previous = [];

    public static void Add(Keyboard keyboard) => Previous.Add(keyboard);
    
    public static Keyboard CtrlZ()
    {
        var last = Previous.LastOrDefault();
        if (last != null)
        {
            Previous.Remove(last);
        }
        return last;
    }

    public bool IsChanged()
        => Previous.LastOrDefault()?.DeepEquals(this) ?? true;
}

public class Key
{
    public int KeyPosition { get; set; }
    public char TopLeftPrint { get; set; }
    public char BottomLeftPrint { get; set; }
    public char TopRightPrint { get; set; }
    public char BottomRightPrint { get; set; }
}

public enum KeyboardLayout
{
    QWERTY,
    AZERTY
}

public enum KeyboardSize
{
    Compact,
    Tenkeyless,
    FullSize
}