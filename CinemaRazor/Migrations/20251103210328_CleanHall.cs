using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaRazor.Migrations
{
    /// <inheritdoc />
    public partial class CleanHall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
            {
                throw new NotSupportedException("This migration relies on SQL Server specific SQL. Switch to SQL Server or adjust the migration manually.");
            }

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Genres]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Genres]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Genres] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Genres]', N'Description') IS NULL
BEGIN
    ALTER TABLE [dbo].[Genres]
    ADD [Description] NVARCHAR(500) NULL;
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Positions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Positions]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [Title] NVARCHAR(50) NOT NULL,
        [Salary] DECIMAL(10, 2) NOT NULL,
        [Responsibilities] NVARCHAR(500) NOT NULL,
        [Requirements] NVARCHAR(500) NOT NULL,
        CONSTRAINT [PK_Positions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Positions]', N'Responsibilities') IS NULL
BEGIN
    ALTER TABLE [dbo].[Positions]
    ADD [Responsibilities] NVARCHAR(500) NOT NULL CONSTRAINT [DF_Positions_Responsibilities] DEFAULT (N'');
    ALTER TABLE [dbo].[Positions] DROP CONSTRAINT [DF_Positions_Responsibilities];
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Positions]', N'Requirements') IS NULL
BEGIN
    ALTER TABLE [dbo].[Positions]
    ADD [Requirements] NVARCHAR(500) NOT NULL CONSTRAINT [DF_Positions_Requirements] DEFAULT (N'');
    ALTER TABLE [dbo].[Positions] DROP CONSTRAINT [DF_Positions_Requirements];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Movies]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Movies]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [Title] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(1000) NOT NULL,
        [ProducerCompany] NVARCHAR(100) NOT NULL,
        [ProductionCountry] NVARCHAR(100) NOT NULL,
        [Actors] NVARCHAR(300) NOT NULL,
        [AgeRating] NVARCHAR(20) NOT NULL,
        [ReleaseDate] DATETIME2 NOT NULL,
        [DurationMinutes] INT NOT NULL,
        [GenreId] INT NOT NULL,
        CONSTRAINT [PK_Movies] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Movies_Genres_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genres] ([Id]) ON DELETE CASCADE
    );
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Movies]', N'ProducerCompany') IS NULL
BEGIN
    ALTER TABLE [dbo].[Movies]
    ADD [ProducerCompany] NVARCHAR(100) NOT NULL CONSTRAINT [DF_Movies_ProducerCompany] DEFAULT (N'');
    ALTER TABLE [dbo].[Movies] DROP CONSTRAINT [DF_Movies_ProducerCompany];
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Movies]', N'ProductionCountry') IS NULL
BEGIN
    ALTER TABLE [dbo].[Movies]
    ADD [ProductionCountry] NVARCHAR(100) NOT NULL CONSTRAINT [DF_Movies_ProductionCountry] DEFAULT (N'');
    ALTER TABLE [dbo].[Movies] DROP CONSTRAINT [DF_Movies_ProductionCountry];
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Movies]', N'Actors') IS NULL
BEGIN
    ALTER TABLE [dbo].[Movies]
    ADD [Actors] NVARCHAR(300) NOT NULL CONSTRAINT [DF_Movies_Actors] DEFAULT (N'');
    ALTER TABLE [dbo].[Movies] DROP CONSTRAINT [DF_Movies_Actors];
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Movies]', N'AgeRating') IS NULL
BEGIN
    ALTER TABLE [dbo].[Movies]
    ADD [AgeRating] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Movies_AgeRating] DEFAULT (N'');
    ALTER TABLE [dbo].[Movies] DROP CONSTRAINT [DF_Movies_AgeRating];
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Movies_Genres_GenreId')
BEGIN
    ALTER TABLE [dbo].[Movies] WITH CHECK ADD CONSTRAINT [FK_Movies_Genres_GenreId]
    FOREIGN KEY([GenreId]) REFERENCES [dbo].[Genres]([Id]) ON DELETE CASCADE;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Movies_GenreId' AND object_id = OBJECT_ID(N'[dbo].[Movies]'))
BEGIN
    CREATE INDEX [IX_Movies_GenreId] ON [dbo].[Movies]([GenreId]);
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Employees]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Employees]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [FullName] NVARCHAR(100) NOT NULL,
        [Age] INT NOT NULL,
        [Gender] NVARCHAR(10) NOT NULL,
        [Address] NVARCHAR(150) NOT NULL,
        [Phone] NVARCHAR(MAX) NOT NULL,
        [PositionId] INT NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Employees_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Positions] ([Id]) ON DELETE CASCADE
    );
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Employees_Positions_PositionId')
BEGIN
    ALTER TABLE [dbo].[Employees] WITH CHECK ADD CONSTRAINT [FK_Employees_Positions_PositionId]
    FOREIGN KEY([PositionId]) REFERENCES [dbo].[Positions]([Id]) ON DELETE CASCADE;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employees_PositionId' AND object_id = OBJECT_ID(N'[dbo].[Employees]'))
BEGIN
    CREATE INDEX [IX_Employees_PositionId] ON [dbo].[Employees]([PositionId]);
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Sessions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Sessions]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [StartTime] DATETIME2 NOT NULL,
        [EndTime] DATETIME2 NOT NULL,
        [Price] DECIMAL(10, 2) NOT NULL,
        [MovieId] INT NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Sessions_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id]) ON DELETE CASCADE
    );
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Sessions_Movies_MovieId')
BEGIN
    ALTER TABLE [dbo].[Sessions] WITH CHECK ADD CONSTRAINT [FK_Sessions_Movies_MovieId]
    FOREIGN KEY([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Sessions_MovieId' AND object_id = OBJECT_ID(N'[dbo].[Sessions]'))
BEGIN
    CREATE INDEX [IX_Sessions_MovieId] ON [dbo].[Sessions]([MovieId]);
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Seats]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Seats]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [RowNumber] INT NOT NULL,
        [SeatNumber] INT NOT NULL,
        [SessionId] INT NOT NULL,
        [IsOccupied] BIT NOT NULL,
        CONSTRAINT [PK_Seats] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Seats_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;
""");

            migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[Seats]', N'IsOccupied') IS NULL
BEGIN
    ALTER TABLE [dbo].[Seats]
    ADD [IsOccupied] BIT NOT NULL CONSTRAINT [DF_Seats_IsOccupied] DEFAULT (0);
    ALTER TABLE [dbo].[Seats] DROP CONSTRAINT [DF_Seats_IsOccupied];
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Seats_Sessions_SessionId')
BEGIN
    ALTER TABLE [dbo].[Seats] WITH CHECK ADD CONSTRAINT [FK_Seats_Sessions_SessionId]
    FOREIGN KEY([SessionId]) REFERENCES [dbo].[Sessions]([Id]) ON DELETE CASCADE;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Seats_SessionId' AND object_id = OBJECT_ID(N'[dbo].[Seats]'))
BEGIN
    CREATE INDEX [IX_Seats_SessionId] ON [dbo].[Seats]([SessionId]);
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Tickets]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Tickets]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [SessionId] INT NOT NULL,
        [SeatId] INT NOT NULL,
        [Price] DECIMAL(10, 2) NOT NULL,
        [PurchaseDate] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Tickets_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [dbo].[Seats] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Tickets_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]) ON DELETE NO ACTION
    );
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Tickets_Seats_SeatId')
BEGIN
    ALTER TABLE [dbo].[Tickets] WITH CHECK ADD CONSTRAINT [FK_Tickets_Seats_SeatId]
    FOREIGN KEY([SeatId]) REFERENCES [dbo].[Seats]([Id]) ON DELETE CASCADE;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Tickets_Sessions_SessionId')
BEGIN
    ALTER TABLE [dbo].[Tickets] WITH CHECK ADD CONSTRAINT [FK_Tickets_Sessions_SessionId]
    FOREIGN KEY([SessionId]) REFERENCES [dbo].[Sessions]([Id]) ON DELETE NO ACTION;
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Tickets_SeatId' AND object_id = OBJECT_ID(N'[dbo].[Tickets]'))
BEGIN
    CREATE INDEX [IX_Tickets_SeatId] ON [dbo].[Tickets]([SeatId]);
END;
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Tickets_SessionId' AND object_id = OBJECT_ID(N'[dbo].[Tickets]'))
BEGIN
    CREATE INDEX [IX_Tickets_SessionId] ON [dbo].[Tickets]([SessionId]);
END;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
            {
                throw new NotSupportedException("This migration relies on SQL Server specific SQL. Switch to SQL Server or adjust the migration manually.");
            }

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Tickets]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Tickets];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Seats]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Seats];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Employees]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Employees];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Sessions]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Sessions];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Movies]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Movies];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Positions]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Positions];
END;
""");

            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Genres]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Genres];
END;
""");
        }
    }
}
