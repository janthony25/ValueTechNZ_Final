using Microsoft.AspNetCore.Identity;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _data;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UnitOfWork(ApplicationDbContext data, ILoggerFactory loggerFactory, IWebHostEnvironment environment,
                          UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _data = data;
            _environment = environment;
            _loggerFactory = loggerFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            Products = new ProductRepository(_data, _loggerFactory, _environment);
            Categories = new CategoryRepository(_data, _loggerFactory);
            Store = new StoreRepository(_data, _loggerFactory);
            Users = new UserRepository(_userManager, _roleManager, _loggerFactory);
        }
        public IProductRepository Products { get; private set; }

        public ICategoryRepository Categories { get; private set; }

        public IStoreRepository Store { get; private set; }

        public IUserRepository Users { get; private set; }
    }
}
