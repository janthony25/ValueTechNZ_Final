namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; } 
        ICategoryRepository Categories { get; }
        IStoreRepository Store { get; }
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IClientOrderRepository ClientOrders { get; }
    }
}
