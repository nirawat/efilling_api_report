using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using THD.Core.Api.Entities.Tables;
using System.Text;

namespace THD.Core.Api.Repository.DataContext
{
    public class DataContextProvider : DbContext
    {
        public DataContextProvider(DbContextOptions<DataContextProvider> options) : base(options)
        {
        }

        public DbSet<EntityRegisterUser> RegisterUsers { get; set; }


    }
}
