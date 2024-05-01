using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace App.Repository.Data.Contexts
{
    public class MainContext : IdentityDbContext<Account, IdentityRole, string>
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Configure auth scheme
            modelBuilder.Entity<Account>().ToTable("Accounts", schema: "auth");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", schema: "auth");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema: "auth");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema: "auth");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema: "auth");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema: "auth");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema: "auth");
            #endregion

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #region DbSets
        public DbSet<Account> Accounts { get; set; }
        public DbSet<OTPForConfirm> OTPs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ChatHistory> Chats { get; set; }
        #endregion

        #region Drop All Tables
        public async Task DropAllTablesAsync()
        {
            // Execute the SQL script for dropping all tables
            try
            {
                await Database.ExecuteSqlRawAsync(@"
                    EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

                    DECLARE @schemaName NVARCHAR(MAX);
                    DECLARE @tableName NVARCHAR(MAX);
                    DECLARE schema_cursor CURSOR FOR
                    SELECT s.name AS schemaName, t.name AS tableName
                    FROM sys.tables t
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id;

                    OPEN schema_cursor;

                    FETCH NEXT FROM schema_cursor INTO @schemaName, @tableName;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        DECLARE @dropTableSQL NVARCHAR(MAX);
                        SET @dropTableSQL = 'DROP TABLE ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName);
                        EXEC sp_executesql @dropTableSQL;
                        FETCH NEXT FROM schema_cursor INTO @schemaName, @tableName;
                    END;

                    CLOSE schema_cursor;
                    DEALLOCATE schema_cursor;

                    EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
                ");
            }
            catch (Exception)
            {
                Console.WriteLine("Can't drop tables");
            }
        }
        #endregion
    }

}
