﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SampleApp.Data;

#nullable disable

namespace SandboxCmd.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250628184053_SourceContextUserName")]
    partial class SourceContextUserName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SerilogViewer.Abstractions.IndexedLogContext.ExceptionInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ExceptionTemplateId")
                        .HasColumnType("int");

                    b.Property<int>("LogId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ExceptionTemplateId");

                    b.ToTable("ExceptionInstances");
                });

            modelBuilder.Entity("SerilogViewer.Abstractions.IndexedLogContext.ExceptionTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ErrorId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceContext")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("StackTraceData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("ExceptionTemplates");
                });

            modelBuilder.Entity("SerilogViewer.Abstractions.IndexedLogContext.SerilogTableMarker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LogId")
                        .HasColumnType("int");

                    b.Property<string>("SchemaName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("SchemaName", "TableName")
                        .IsUnique();

                    b.ToTable("SerilogTableMarkers");
                });

            modelBuilder.Entity("SerilogViewer.Abstractions.SourceContextView.SourceContextView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("SourceContext")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("SourceContext", "Level", "UserName")
                        .IsUnique();

                    b.ToTable("SourceContexts");
                });

            modelBuilder.Entity("SerilogViewer.Abstractions.IndexedLogContext.ExceptionInstance", b =>
                {
                    b.HasOne("SerilogViewer.Abstractions.IndexedLogContext.ExceptionTemplate", "ExceptionTemplate")
                        .WithMany("Instances")
                        .HasForeignKey("ExceptionTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExceptionTemplate");
                });

            modelBuilder.Entity("SerilogViewer.Abstractions.IndexedLogContext.ExceptionTemplate", b =>
                {
                    b.Navigation("Instances");
                });
#pragma warning restore 612, 618
        }
    }
}
