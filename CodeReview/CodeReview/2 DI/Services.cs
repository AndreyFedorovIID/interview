namespace CodeReview._2_DI;

public class GrainProvider;

public class ToppingDeliveryService;

public interface IBreakfastFactory;

public class PorridgeFactory(
    GrainProvider grainProvider,
    ToppingDeliveryService toppingDeliveryService) : IBreakfastFactory;