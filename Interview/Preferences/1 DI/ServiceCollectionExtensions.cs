/*
    🟢🟢🟢
    Есть некоторый набор типов:
        GrainProvider,
        ToppingDeliveryService,
        IBreakfastFactory,
        PorridgeFactory.
    Поставлена задача: 
        добавить возможность выполнять инъекцию этих типов с помощью используемого DI-контейнера.
 
    🔻🔻🔻
    Какие плюсы и минусы у представленных способов регистрации?
*/

namespace Preferences._1_DI;

public sealed class GrainProvider;

public sealed class ToppingDeliveryService;

public interface IBreakfastFactory;

public sealed class PorridgeFactory(
    GrainProvider grainProvider,
    ToppingDeliveryService toppingDeliveryService) : IBreakfastFactory;

public static class ServiceCollectionExtensions
{
    public static void AddPorridgeFactoryV1(this IServiceCollection services)
    {
        services
            .AddSingleton<GrainProvider>()
            .AddSingleton<ToppingDeliveryService>()
            .AddSingleton<PorridgeFactory>()
            .AddSingleton<IBreakfastFactory, PorridgeFactory>(
                static p => p.GetRequiredService<PorridgeFactory>());
    }
    
    public static void AddPorridgeFactoryV2(this IServiceCollection services)
    {
        services
            .AddSingleton(static _ => new GrainProvider())
            .AddSingleton(static _ => new ToppingDeliveryService())
            .AddSingleton<PorridgeFactory>(
                static p => new PorridgeFactory(
                    p.GetRequiredService<GrainProvider>(),
                    p.GetRequiredService<ToppingDeliveryService>()))
            .AddSingleton<IBreakfastFactory>(
                static p => p.GetRequiredService<PorridgeFactory>());
    }

    public static void AddPorridgeFactoryV3(this IServiceCollection services)
    {
        var grainProvider = new GrainProvider();
        var toppingDeliveryService = new ToppingDeliveryService();
        var porridgeFactory = new PorridgeFactory(grainProvider, toppingDeliveryService);
        
        services
            .AddSingleton(grainProvider)
            .AddSingleton(toppingDeliveryService)
            .AddSingleton(porridgeFactory)
            .AddSingleton<IBreakfastFactory, PorridgeFactory>(
                static p => p.GetRequiredService<PorridgeFactory>());
    }
}