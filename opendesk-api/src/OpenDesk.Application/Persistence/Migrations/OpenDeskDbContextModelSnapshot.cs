﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OpenDesk.Application.Persistence;

namespace OpenDesk.Application.Persistence.Migrations
{
    [DbContext(typeof(OpenDeskDbContext))]
    partial class OpenDeskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("claim_value");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claims_role_id");

                    b.ToTable("role_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("claim_value");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claims_user_id");

                    b.ToTable("user_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_logins_user_id");

                    b.ToTable("user_logins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("user_id");

                    b.Property<string>("RoleId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_roles");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_user_roles_role_id");

                    b.ToTable("user_roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_tokens");

                    b.ToTable("user_tokens");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Blob", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("deleted_by");

                    b.Property<DateTimeOffset>("Expiry")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiry");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uri");

                    b.HasKey("Id")
                        .HasName("pk_blobs");

                    b.ToTable("blobs");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Booking", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("deleted_by");

                    b.Property<string>("DeskId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("desk_id");

                    b.Property<DateTimeOffset>("EndDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date_time");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("OpenDeskUserId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("open_desk_user_id");

                    b.Property<DateTimeOffset>("StartDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date_time");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_bookings");

                    b.HasIndex("DeskId")
                        .HasDatabaseName("ix_bookings_desk_id");

                    b.HasIndex("OpenDeskUserId")
                        .HasDatabaseName("ix_bookings_open_desk_user_id");

                    b.ToTable("bookings");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Desk", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("deleted_by");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<string>("OfficeId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("office_id");

                    b.HasKey("Id")
                        .HasName("pk_desks");

                    b.HasIndex("OfficeId")
                        .HasDatabaseName("ix_desks_office_id");

                    b.ToTable("desks");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Office", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("deleted_by");

                    b.Property<string>("ImageId")
                        .HasColumnType("character varying(50)")
                        .HasColumnName("image_id");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<string>("SubLocation")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("sub_location");

                    b.HasKey("Id")
                        .HasName("pk_offices");

                    b.HasIndex("ImageId")
                        .HasDatabaseName("ix_offices_image_id");

                    b.ToTable("offices");
                });

            modelBuilder.Entity("OpenDesk.Application.Identity.OpenDeskRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("OpenDesk.Application.Identity.OpenDeskUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("display_name");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("OpenDesk.Application.Identity.OpenDeskRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_claims_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OpenDesk.Application.Identity.OpenDeskUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_claims_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OpenDesk.Application.Identity.OpenDeskUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_logins_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("OpenDesk.Application.Identity.OpenDeskRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_user_roles_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OpenDesk.Application.Identity.OpenDeskUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_roles_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("OpenDesk.Application.Identity.OpenDeskUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_tokens_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Booking", b =>
                {
                    b.HasOne("OpenDesk.Application.Entities.Desk", "Desk")
                        .WithMany("Bookings")
                        .HasForeignKey("DeskId")
                        .HasConstraintName("fk_bookings_desks_desk_id");

                    b.HasOne("OpenDesk.Application.Identity.OpenDeskUser", null)
                        .WithMany("Bookings")
                        .HasForeignKey("OpenDeskUserId")
                        .HasConstraintName("fk_bookings_users_open_desk_user_id");

                    b.Navigation("Desk");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Desk", b =>
                {
                    b.HasOne("OpenDesk.Application.Entities.Office", "Office")
                        .WithMany("Desks")
                        .HasForeignKey("OfficeId")
                        .HasConstraintName("fk_desks_offices_office_id");

                    b.OwnsOne("OpenDesk.Application.ValueObjects.DiagramPosition", "DiagramPosition", b1 =>
                        {
                            b1.Property<string>("DeskId")
                                .HasColumnType("character varying(50)")
                                .HasColumnName("id");

                            b1.Property<int>("X")
                                .HasColumnType("integer")
                                .HasColumnName("diagram_position_x");

                            b1.Property<int>("Y")
                                .HasColumnType("integer")
                                .HasColumnName("diagram_position_y");

                            b1.HasKey("DeskId")
                                .HasName("pk_desks");

                            b1.ToTable("desks");

                            b1.WithOwner()
                                .HasForeignKey("DeskId")
                                .HasConstraintName("fk_desks_desks_id");
                        });

                    b.Navigation("DiagramPosition");

                    b.Navigation("Office");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Office", b =>
                {
                    b.HasOne("OpenDesk.Application.Entities.Blob", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .HasConstraintName("fk_offices_blobs_image_id");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Desk", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("OpenDesk.Application.Entities.Office", b =>
                {
                    b.Navigation("Desks");
                });

            modelBuilder.Entity("OpenDesk.Application.Identity.OpenDeskUser", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}