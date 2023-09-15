﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Harmony.Domain.Entities;

namespace Harmony.Persistence.Configurations
{
    /// <summary>
    /// EF Core entity configuration for Cards
    /// </summary>
    public class BoardListConfiguration : IEntityTypeConfiguration<BoardList>
    {
        public void Configure(EntityTypeBuilder<BoardList> builder)
        {
            builder.ToTable("BoardLists");

            builder.Property(b => b.BoardId).IsRequired();

            builder.Property(b => b.Name).IsRequired().HasMaxLength(300);

            builder.Property(b => b.Position).IsRequired();

            // A list can have multiple cards and a card belongs to one list (1-M relationship)
            builder.HasMany(list => list.Cards)
                .WithOne(c => c.BoardList).HasForeignKey(card => card.BoardListId);
        }
    }
}
