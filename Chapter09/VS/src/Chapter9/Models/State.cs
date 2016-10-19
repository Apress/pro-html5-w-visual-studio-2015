using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Entity;

namespace Chapter9.Models
{
    public class State
    {
        [Key] public int Id { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string Path { get; set; }
    }

    public class StateDbContext : ApplicationDbContext
    {
        public DbSet<State> States { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Startup.Configuration
                .Get("Data:DefaultConnection:ConnectionString"));
            base.OnConfiguring(options);
        }
    }

}
