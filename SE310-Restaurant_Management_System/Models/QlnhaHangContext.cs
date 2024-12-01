using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SE310_Restaurant_Management_System.Models;

public partial class QlnhaHangContext : DbContext
{
    public QlnhaHangContext()
    {
    }

    public QlnhaHangContext(DbContextOptions<QlnhaHangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingOrder> BookingOrders { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboItem> ComboItems { get; set; }

    public virtual DbSet<EntryDetail> EntryDetails { get; set; }

    public virtual DbSet<ExitDetail> ExitDetails { get; set; }

    public virtual DbSet<FoodCategory> FoodCategories { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientType> IngredientTypes { get; set; }

    public virtual DbSet<InventoryEntry> InventoryEntries { get; set; }

    public virtual DbSet<InventoryExit> InventoryExits { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<TableStatus> TableStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-JRRAO9EA\\DOHUY;Initial Catalog=QLNhaHang;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingOrder>(entity =>
        {
            entity.HasKey(e => e.BookingOrderId).HasName("PK__BookingO__72C389F9BB582574");

            entity.Property(e => e.BookingOrderId).HasColumnName("BookingOrderID");
            entity.Property(e => e.BookingTime).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.HasOne(d => d.Table).WithMany(p => p.BookingOrders)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK__BookingOr__Table__693CA210");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combos__DD42580EE5DC420F");

            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.ComboName).HasMaxLength(100);
            entity.Property(e => e.ComboPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Image).HasMaxLength(255);
        });

        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.HasKey(e => e.ComboItemId).HasName("PK__ComboIte__EE32F8E5D6876E28");

            entity.Property(e => e.ComboItemId).HasColumnName("ComboItemID");
            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__ComboItem__Combo__656C112C");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK__ComboItem__MenuI__66603565");
        });

        modelBuilder.Entity<EntryDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__EntryDet__135C314D1A692F8F");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.EntryId).HasColumnName("EntryID");
            entity.Property(e => e.ImportPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Entry).WithMany(p => p.EntryDetails)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntryDeta__Entry__7A672E12");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.EntryDetails)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntryDeta__Ingre__7B5B524B");
        });

        modelBuilder.Entity<ExitDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__ExitDeta__135C314D33EB7FA0");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.ExitId).HasColumnName("ExitID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Exit).WithMany(p => p.ExitDetails)
                .HasForeignKey(d => d.ExitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExitDetai__ExitI__00200768");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.ExitDetails)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExitDetai__Ingre__01142BA1");
        });

        modelBuilder.Entity<FoodCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__FoodCate__19093A2BAEB5BE12");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27AC9003AEE");

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Type).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ingredien__TypeI__75A278F5");
        });

        modelBuilder.Entity<IngredientType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Ingredie__516F03959D9F4A51");

            entity.HasIndex(e => e.TypeName, "UQ__Ingredie__D4E7DFA864555E73").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<InventoryEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId).HasName("PK__Inventor__F57BD2D748748C9E");

            entity.Property(e => e.EntryId).HasColumnName("EntryID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<InventoryExit>(entity =>
        {
            entity.HasKey(e => e.ExitId).HasName("PK__Inventor__26D64E98FCF60CFF");

            entity.Property(e => e.ExitId).HasColumnName("ExitID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RecipientName).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD54A2963E4");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Table).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK__Invoices__TableI__6C190EBB");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId).HasName("PK__InvoiceI__478FE0FC0AFE7E05");

            entity.Property(e => e.InvoiceItemId).HasColumnName("InvoiceItemID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__InvoiceIt__Invoi__6EF57B66");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK__InvoiceIt__MenuI__6FE99F9F");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__MenuItem__8943F7024FD36DCF");

            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubCategoryId).HasColumnName("SubCategoryID");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.SubCategoryId)
                .HasConstraintName("FK__MenuItems__SubCa__60A75C0F");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__89B744E59F5FB178");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__2EB7B06FACD355BF").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("Permission_ID");
            entity.Property(e => e.PermissionGroup)
                .HasMaxLength(100)
                .HasColumnName("Permission_Group");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(100)
                .HasColumnName("Permission_Name");
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Restaura__7D5F018E74D3F28D");

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Status).WithMany(p => p.RestaurantTables)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Statu__59063A47");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AAF887777");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_Perm__Permi__534D60F1"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_Perm__Role___52593CB8"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__Role_Per__9091C0D5E737D2E9");
                        j.ToTable("Role_Permission");
                        j.IndexerProperty<int>("RoleId").HasColumnName("Role_ID");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("Permission_ID");
                    });
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryId).HasName("PK__SubCateg__26BE5BF92F73FB6E");

            entity.Property(e => e.SubCategoryId).HasColumnName("SubCategoryID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.SubCategoryName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__SubCatego__Categ__5DCAEF64");
        });

        modelBuilder.Entity<TableStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__TableSta__C8EE20436F3AA2D7");

            entity.ToTable("TableStatus");

            entity.HasIndex(e => e.StatusName, "UQ__TableSta__05E7698A5DABF002").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC1BFF8E74");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E426C94C53").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RoleId)
                .HasDefaultValue(2)
                .HasColumnName("Role_ID");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__Role_ID__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    private void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
    }
}