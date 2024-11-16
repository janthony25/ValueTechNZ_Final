namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; } 
        ICategoryRepository Categories { get; }
    }
}
