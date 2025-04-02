using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using System.Collections.Generic;

namespace RestAPI.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Machine> Machines { get; set; }
    }
}