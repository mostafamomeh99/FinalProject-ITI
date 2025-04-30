using DatabaseConnection;
using IRepositoryService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Models;
using Models.StoredprocMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace RepositoryFactory
{
  public  class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private readonly IEmailService emailService;
        private readonly IOtpService OtpService;
        public IGrokService GrokService { get; private set; }

        private IDbContextTransaction currentTransaction;
        public IAccountRepository Accounts { get; private set; }
        public IDatabaseRepository<Trip> Trips { get; private set; }
        public IDatabaseRepository<TripSites> TripSites { get; private set; }
        public IDatabaseRepository<Driver> Drivers { get; private set; }
        public IDatabaseRepository<Book> Books { get; private set; }
        public IDatabaseRepository<Rating> Ratings { get; private set; }
        public IDatabaseRepository<Government> Governments { get; private set; }
        public IDatabaseRepository<Site> Sites { get; private set; }
        public IDatabaseRepository<Transportation> Transportations { get; private set; }
        public IDatabaseRepository<SiteImage> SiteImages { get; private set; }
        public IDatabaseRepository<TripExcluded> TripExcludeds { get; private set; }
        public IDatabaseRepository<TripIncluded> TripIncludeds { get; private set; }

        public ISqlProcedureService<TripSiteDetailDto> TripSiteDetails { get; private set; }
        public UnitOfWork(ApplicationDbContext context, IEmailService emailService, IConfiguration configuration
  , UserManager<ApplicationUser> userManager, IOtpService otpService ,  HttpClient httpClient)
        {
            this.context = context;
            this.emailService = emailService;
            Accounts = new AccountRepository(userManager, context, configuration, emailService, otpService);
            Trips = new DatabaseRepository<Trip>(context);
            Transportations = new DatabaseRepository<Transportation>(context);
            TripSites = new DatabaseRepository<TripSites>(context);
            Governments = new DatabaseRepository<Government>(context);
            TripIncludeds = new DatabaseRepository<TripIncluded>(context);
            TripExcludeds = new DatabaseRepository<TripExcluded>(context);
            SiteImages = new DatabaseRepository<SiteImage>(context);
            Sites = new DatabaseRepository<Site>(context);
            Ratings = new DatabaseRepository<Rating>(context);
            Books = new DatabaseRepository<Book>(context);
            Drivers = new DatabaseRepository<Driver>(context);
            OtpService = otpService;
            TripSiteDetails = new SqlProcedureService<TripSiteDetailDto>(context);
            GrokService = new GrokService(httpClient, configuration);
        }
        public int Compelet()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            // Check if a transaction is already active
            if (currentTransaction == null)
            {
                currentTransaction = await context.Database.BeginTransactionAsync();
            }

            return currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (currentTransaction != null)
            {
                await currentTransaction.CommitAsync();
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (currentTransaction != null)
            {
                await currentTransaction.RollbackAsync();
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }


        public async Task DisposeAsync()
        {
            if (currentTransaction != null)
            {
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }
    }
}
