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

namespace Models._1_Weight;

public sealed class Weight
{
    public double Value { get; set; }

    public WeightUnit Unit { get; set; }

    public override string ToString() => $"{Value} {Unit}";
}

public enum WeightUnit
{
    Milligram,
    Gram,
    Kilogram,
    Megagram
}

public static class UseCases
{
    public static Weight Sum(IEnumerable<Weight> weights)
    {
        throw new NotImplementedException();
    }

    public static Weight Max(IEnumerable<Weight> weights)
    {
        throw new NotImplementedException();
    }

    public static string DisplayValueInKilograms(Weight weight)
    {
        throw new NotImplementedException();
    }
}