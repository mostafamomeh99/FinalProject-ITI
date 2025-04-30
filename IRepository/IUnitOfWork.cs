using Microsoft.EntityFrameworkCore.Storage;
using Models;
using Models.StoredprocMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositoryService
{
    public interface IUnitOfWork
    {
        public IAccountRepository Accounts { get; }
        public IDatabaseRepository<Trip> Trips { get; }
        public IDatabaseRepository<TripSites> TripSites { get; }
        public IDatabaseRepository<Driver> Drivers { get; }
        public IDatabaseRepository<Book> Books { get; }
        public IDatabaseRepository<Rating> Ratings { get; }
        public IDatabaseRepository<Government> Governments { get; }
        public IDatabaseRepository<Site> Sites { get; }
        public IDatabaseRepository<Transportation> Transportations { get; }
        public IDatabaseRepository<SiteImage> SiteImages { get; }
        public IDatabaseRepository<TripExcluded> TripExcludeds { get; }
        public IDatabaseRepository<TripIncluded> TripIncludeds { get; }
        public ISqlProcedureService<TripSiteDetailDto> TripSiteDetails { get;}
        public IGrokService GrokService { get; }
        Task CommitTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task RollbackTransactionAsync();
        Task DisposeAsync();
        void Dispose();
        int Compelet();
    }
}
