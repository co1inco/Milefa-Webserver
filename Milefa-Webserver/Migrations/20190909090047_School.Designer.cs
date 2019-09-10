﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Milefa_Webserver.Data;

namespace Milefa_Webserver.Migrations
{
    [DbContext(typeof(CompanyContext))]
    [Migration("20190909090047_School")]
    partial class School
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Milefa_Webserver.Models.Department", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MaxEmployes");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("Milefa_Webserver.Models.Skill", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DepartmentID");

                    b.Property<string>("Name");

                    b.Property<int?>("StudentID");

                    b.HasKey("ID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("StudentID");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("Milefa_Webserver.Models.Student", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Breakfast");

                    b.Property<int?>("Choise1ID");

                    b.Property<int?>("Choise2ID");

                    b.Property<DateTime>("DateValide");

                    b.Property<int?>("DeployedDepID");

                    b.Property<int?>("Gender");

                    b.Property<bool>("Lunch");

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<int>("PersNr");

                    b.Property<string>("School")
                        .HasMaxLength(50);

                    b.Property<string>("_Class")
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.HasIndex("Choise1ID");

                    b.HasIndex("Choise2ID");

                    b.HasIndex("DeployedDepID");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("Milefa_Webserver.Models.Skill", b =>
                {
                    b.HasOne("Milefa_Webserver.Models.Department")
                        .WithMany("RequiredSkills")
                        .HasForeignKey("DepartmentID");

                    b.HasOne("Milefa_Webserver.Models.Student")
                        .WithMany("Skills")
                        .HasForeignKey("StudentID");
                });

            modelBuilder.Entity("Milefa_Webserver.Models.Student", b =>
                {
                    b.HasOne("Milefa_Webserver.Models.Department", "Choise1")
                        .WithMany()
                        .HasForeignKey("Choise1ID");

                    b.HasOne("Milefa_Webserver.Models.Department", "Choise2")
                        .WithMany()
                        .HasForeignKey("Choise2ID");

                    b.HasOne("Milefa_Webserver.Models.Department", "DeployedDep")
                        .WithMany()
                        .HasForeignKey("DeployedDepID");
                });
#pragma warning restore 612, 618
        }
    }
}
