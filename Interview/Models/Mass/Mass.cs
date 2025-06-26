/*
    🟢🟢🟢
    Команда программистов занимается разработкой множества веб магазинов.
    Они часто встречают необходимость представления массы, так как многие товары продают на развес.
    Было принято решение смоделировать понятие веса в общей библиотеке.
    Для этого была сделана следующая заготовка.

    🔻🔻🔻
    Необходимо реализовать методы UseCases "наилучшим" образом.
    Можно вносить изменения везде, кроме объявлений методов UseCases.
*/

namespace Models.Mass;

public sealed class Mass
{
    public double Value { get; set; }

    public MassUnit Unit { get; set; }

    public override string ToString() => $"{Value} {Unit}";
}

public enum MassUnit
{
    Milligram,
    Gram,
    Kilogram,
    Megagram
}

// Некоторые сценарии, в которых нужно продемонстрировать использование модели массы.
public static class UseCases
{
    public static Mass Difference(Mass oldMass, Mass newMass)
    {
        throw new NotImplementedException();
    }

    public static Mass Sum(IEnumerable<Mass> masses)
    {
        throw new NotImplementedException();
    }
    
    public static bool InRange(Mass value, Mass minimum, Mass maximum)
    {
        throw new NotImplementedException();
    }

    public static Mass Max(IEnumerable<Mass> masses)
    {
        throw new NotImplementedException();
    }

    public static double ValueInMilligrams(Mass mass)
    {
        throw new NotImplementedException();
    }

    // Пример результата: 12,3 кг
    public static string DisplayValueInKilogramsInRussian(Mass mass)
    {
        throw new NotImplementedException();
    }

    // Пример результата: 12.3 kg 
    public static string DisplayValueInKilogramsInEnglish(Mass mass)
    {
        throw new NotImplementedException();
    }
}