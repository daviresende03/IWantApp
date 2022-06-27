using IWantApp.Domain.Orders;

namespace IWantApp.Infra.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); //Chamada ao OnModelCreating da classe Pai (IdentityDbContext)

            builder.Ignore<Notification>();//Ignorar a Classe Notification de Flunt

            builder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(200);
            builder.Entity<Product>()
                .Property(p => p.Name).IsRequired();
            builder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Entity<Category>()
                .Property(p => p.Name).IsRequired();

            builder.Entity<Order>()
                .Property(p => p.ClientId).IsRequired();
            builder.Entity<Order>()
                .Property(p => p.DeliveryAddress).IsRequired();
            builder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithMany(p => p.Order)
                .UsingEntity(x => x.ToTable("OrderProducts"));
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder config)
        {
            config.Properties<string>()
                .HaveMaxLength(100); //TODOS ATRIBUTOS DO TIPO STRING TERÃO NO MÁXIMO 100 CARAC.
        }
    }
}
