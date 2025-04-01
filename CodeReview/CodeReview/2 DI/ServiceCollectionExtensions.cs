/*
    🔻🔻🔻
    Какие плюсы и минусы у представленных способов регистрации IBreakfastFactory?
*/

namespace CodeReview._2_DI;

public static class ServiceCollectionExtensions
{
    public static void AddPorridgeFactoryV1(this IServiceCollection services)
    {
        services
            .AddSingleton<GrainProvider>()
            .AddSingleton<ToppingDeliveryService>()
            .AddSingleton<IBreakfastFactory, PorridgeFactory>();
    }
    
    public static void AddPorridgeFactoryV2(this IServiceCollection services)
    {
        services.AddSingleton<IBreakfastFactory>(
            new PorridgeFactory(
                new GrainProvider(),
                new ToppingDeliveryService()));
    }
    
    public static void AddPorridgeFactoryV3(this IServiceCollection services)
    {
        services
            .AddSingleton(static _ => new GrainProvider())
            .AddSingleton(static _ => new ToppingDeliveryService())
            .AddSingleton<IBreakfastFactory>(
                static p => new PorridgeFactory(
                    p.GetRequiredService<GrainProvider>(),
                    p.GetRequiredService<ToppingDeliveryService>()));
    }

    public static void AddPorridgeFactoryV4(this IServiceCollection services)
    {
        var grainProvider = new GrainProvider();
        var toppingDeliveryService = new ToppingDeliveryService();
        services.AddSingleton<IBreakfastFactory>(
            _ => new PorridgeFactory(
                grainProvider,
                toppingDeliveryService));
    }
}