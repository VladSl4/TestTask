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
            builder.HasData(Enum.GetValues(typeof(Enums.StatusEnum))
                .Cast<Enums.StatusEnum>()
            .Select(cs => new Status { Id = (int)cs, Name = cs, Description = GetEnumDescription(cs)  ?? string.Empty })); // тут надо вытянуть значение с атрибута Description    }}
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return descriptionAttribute?.Description;
        }
    }
}
