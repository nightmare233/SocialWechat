namespace Social.Domain.Core
{
    using Framework.Core;
    using Framework.Core.EntityFramework;
    using log4net;
    using Social.Domain.Entities;
    using Social.Infrastructure.Enum;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Interception;

    public class SiteDataContext : DataContext, ITransient
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SiteDataContext));

        public SiteDataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<SiteDataContext>(null);
            Database.Log = t => logger.Debug(t);
        }

        static SiteDataContext()
        {
            DbInterception.Add(new ShardingTableInterceptor(new[] { typeof(SiteDataContext).Assembly }));
        }

        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<ConversationField> ConversationFields { get; set; }
        public virtual DbSet<ConversationFieldOption> ConversationFieldOptions { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<SocialAccount> SocialAccounts { get; set; }
        public virtual DbSet<SocialUser> SocialUsers { get; set; }
        public virtual DbSet<TwitterAuth> TwitterAuths { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentMember> DepartmentMembers { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<CustomAwayStatus> CustomAwayStatus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>()
                .HasMany(t => t.Messages)
                .WithRequired(t => t.Conversation)
                .HasForeignKey(t => t.ConversationId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Conversation>()
                .HasMany(t => t.Logs)
                .WithRequired(t => t.Conversation)
                .HasForeignKey(t => t.ConversationId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Conversation>()
                .HasRequired(t => t.LastMessageSender)
                .WithMany()
                .HasForeignKey(t => t.LastMessageSenderId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Conversation>()
                .HasOptional(t => t.Agent)
                .WithMany()
                .HasForeignKey(t => t.AgentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Conversation>()
                .HasOptional(t => t.Department)
                .WithMany()
                .HasForeignKey(t => t.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .HasMany(t => t.Attachments)
                .WithRequired(t => t.Message)
                .HasForeignKey(t => t.MessageId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>()
                .HasRequired(t => t.Sender)
                .WithMany()
                .HasForeignKey(t => t.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>()
                .HasOptional(t => t.Receiver)
                .WithMany()
                .HasForeignKey(t => t.ReceiverId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>()
                .HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(t => t.ParentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>()
                .HasOptional(t => t.SendAgent)
                .WithMany()
                .HasForeignKey(t => t.SendAgentId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Filter>()
                .HasMany(t => t.Conditions)
                .WithRequired(t => t.Filter)
                .HasForeignKey(t => t.FilterId).WillCascadeOnDelete(true);
            modelBuilder.Entity<FilterCondition>()
                .HasRequired(t => t.Field)
                .WithMany(t => t.Conditions)
                .HasForeignKey(t => t.FieldId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ConversationField>()
                .HasMany(t => t.Options)
                .WithRequired(t => t.Field)
                .HasForeignKey(t => t.FieldId).WillCascadeOnDelete(false);

            modelBuilder.Entity<SocialUser>()
                .HasOptional(t => t.SocialAccount)
                .WithRequired(t => t.SocialUser);

            modelBuilder.Entity<Department>()
                .HasMany(t => t.Members)
                .WithRequired(t => t.Department)
                .HasForeignKey(t => t.DepartmentId);
        }
    }
}
