﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Data
{
    public class ApplicationToSellThingsAPIsContext : DbContext
    {
        public ApplicationToSellThingsAPIsContext (DbContextOptions<ApplicationToSellThingsAPIsContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationToSellThings.APIs.Models.Product> Products { get; set; } = default!;
        public DbSet<ApplicationToSellThings.APIs.Models.Order> Orders { get; set; } = default!;
        public DbSet<ApplicationToSellThings.APIs.Models.OrderDetail> OrderDetails { get; set; } = default!;
    }
}
