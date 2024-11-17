﻿using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _data;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _environment;
        public UnitOfWork(ApplicationDbContext data, ILoggerFactory loggerFactory, IWebHostEnvironment environment)
        {
            _data = data;
            _environment = environment;
            _loggerFactory = loggerFactory;
            Products = new ProductRepository(_data, _loggerFactory, _environment);
            Categories = new CategoryRepository(_data, _loggerFactory);
            Store = new StoreRepository(_data, _loggerFactory);
        }
        public IProductRepository Products { get; private set; }

        public ICategoryRepository Categories { get; private set; }

        public IStoreRepository Store { get; private set; }
    }
}
