
using Amazon.Runtime.Internal;
using Domain.Entities;
using Domain.SharedKernel;
using Domain.Users;
using Domain.Users.ValueObjects;
using Infrastructure.Identity;
using Infrastructure.Presistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Presistence.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Document> Documents { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<User> Users {  get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Ignore<DomainEvent>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
