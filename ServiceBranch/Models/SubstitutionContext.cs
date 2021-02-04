using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBranch.Models
{
    public class SubstitutionContext : IdentityDbContext
    {
        public SubstitutionContext(DbContextOptions<SubstitutionContext> options)
        : base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Substitute> Substitutes { get; set; }
    }
}
