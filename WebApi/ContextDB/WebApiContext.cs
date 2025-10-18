using Microsoft.EntityFrameworkCore;
using PersistenceNet;
using PersistenceNet.Enuns;
using WebApi.Entitys;

namespace WebApi.ContextDB
{
    public class WebApiContext(DbContextOptions<WebApiContext> options) 
        : PersistenceContext(options)
    {
        public DbSet<MailInfo> MailInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MailInfo>(rt => rt.Property(rt => rt.Active)
                .HasConversion(rt => rt.ToString(), rt => Enum.Parse<ActiveEnum>(rt)));

            modelBuilder.Entity<MailInfo>(rt => rt.Property(rt => rt.SecureConnection)
                .HasConversion(rt => rt.ToString(), rt => Enum.Parse<ActiveEnum>(rt)));
        }
    }
}