using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using phase_2_back;

namespace phase_2_back.Data
{
    public class phase_2_backContext : DbContext
    {
        public phase_2_backContext (DbContextOptions<phase_2_backContext> options)
            : base(options)
        {
        }

        public DbSet<phase_2_back.personInfo> personInfo { get; set; } = default!;
    }
}
