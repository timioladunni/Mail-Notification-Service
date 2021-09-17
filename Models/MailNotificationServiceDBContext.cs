using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MailNotificationService.Models
{
    public partial class MailNotificationServiceDBContext : DbContext
    {
        public MailNotificationServiceDBContext()
        {
        }

        public MailNotificationServiceDBContext(DbContextOptions<MailNotificationServiceDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BudgetNotificationMail> BudgetNotificationMails { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<MailSendingStatus> MailSendingStatuses { get; set; }
        public virtual DbSet<WelcomeMail> WelcomeMails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conString = ConfigurationManager.AppSettings["conString"];
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(conString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BudgetNotificationMail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BudgetNotificationMail");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.InstitutionName).IsUnicode(false);

                entity.Property(e => e.RecepientMail).IsUnicode(false);

                entity.Property(e => e.RepaymentAccount).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<Email>(entity =>
            {
               // entity.HasNoKey();

                entity.ToTable("Email");

                entity.Property(e => e.Body).IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.MessageType).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);
            });

            modelBuilder.Entity<MailSendingStatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MailSendingStatus");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.MessageType).IsUnicode(false);

                entity.Property(e => e.SendingStatus).IsUnicode(false);
            });

            modelBuilder.Entity<WelcomeMail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("WelcomeMail");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.RecipientEmail).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
