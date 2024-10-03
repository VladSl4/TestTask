using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using ServerApp.Enums;
using EnumsNET;
using System.ComponentModel;

using System.Reflection;

namespace ServerApp.Configurations
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.Property(cs => cs.Name)
                .HasConversion<string>();
            builder.HasData(Enum.GetValues(typeof(StatusEnum))
                .Cast<StatusEnum>()
            .Select(cs => new Status { Id = (int)cs, Name = cs, Description = GetEnumDescription(cs)  ?? string.Empty }));
        }
        public static string? GetEnumDescription<T>(T value) where T : Enum
        {
            var field = value.GetType()
                .GetTypeInfo()
                .DeclaredMembers
                .FirstOrDefault(n => n.Name == value.ToString())
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;

            return field ?? string.Empty;
        }
    }
}
