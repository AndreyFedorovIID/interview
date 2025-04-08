/*
    🟢🟢🟢
    Разработчик проявил инициативу и попытался повысить производительность валидации.
 
    🔻🔻🔻
    Необходимо выполнить ревью нового метода ValidateFastAsync, размышляя вслух.
    Следует стараться упомянуть как можно больше возможных проблем.
    Объяснять их не надо, просто сказать об их наличии.

    После ревью нужно выбрать несколько понравившиеся проблем и рассказать про них подробнее.
    Написать комментарий для автора текстом.
*/

using System.Runtime.CompilerServices;

namespace CodeReview._2_Validator;

public interface IValidator<in T>
{
    public bool IsValid(T value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> ValidateFastAsync(T value, CancellationToken ct)
    {
        if (value == null)
        {
            throw new NullReferenceException("value is null");
        }

        bool result = false;

        Monitor.Enter(value);

        result = await Task.Run(() => IsValid(value), ct);

        Monitor.Exit(value);

        if (ct.IsCancellationRequested)
        {
            throw new OperationCanceledException();
        }

        return result;
    }
}