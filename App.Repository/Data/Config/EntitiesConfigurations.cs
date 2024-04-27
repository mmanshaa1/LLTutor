using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Repository.Data.Config
{

    public class OTPForConfirmConfigurations : IEntityTypeConfiguration<OTPForConfirm>
    {
        public void Configure(EntityTypeBuilder<OTPForConfirm> builder)
        {
            builder.Property(M => M.LifeTime).HasColumnType("time(6)");
        }
    }
}
