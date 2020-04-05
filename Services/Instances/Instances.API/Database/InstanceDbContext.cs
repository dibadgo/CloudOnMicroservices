using Instances.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Database
{
    public class InstanceDbContext : DbContext
    {
        public InstanceDbContext(DbContextOptions options) : base(options)
        {
        }

        DbSet<Instance> Instances { get; set; }

        DbSet<Volume> Volumes { get; set; }
    }
}
