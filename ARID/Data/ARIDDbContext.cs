using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ARID.Models;
using ARID.DTOs;
using ARID.Messages.Models;

namespace ARID.Data
{
    public class ARIDDbContext : IdentityDbContext<ApplicationUser>
    {
        public ARIDDbContext(DbContextOptions<ARIDDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // This foreach is to disable cascade delete in EF7.
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        //Profiling
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ProfileLink> ProfileLinks { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<EducationalLevel> EducationalLevels { get; set; }
        public DbSet<PositionType> PositionTypes { get; set; }
        public DbSet<AcademicPosition> AcademicPositions { get; set; }
        public DbSet<TeachingExperience> TeachingExperiences { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<AcademicActivity> AcademicActivities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Expertise> Expertises { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserExpertise> UserExpertises { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<ARID.Models.Gallary> Gallaries { get; set; }
        public DbSet<ARID.DTOs.ApplicationUserDTO> ApplicationUserDTO { get; set; }
        public object ApplicationUser { get; internal set; }


        //Notifications
        public DbSet<EmailContent> EmailContents { get; set; }
        public DbSet<Sender> Senders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        //public DbSet<NotificationLog> Notificationlogs { get; set; }

        //Badges
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }



        //Community (Blog, communiy, Group)
        public DbSet<ARID.Models.Community> Communities { get; set; }
        public DbSet<ARID.Models.Post> Posts { get; set; }
        public DbSet<ARID.Models.PostComment> PostComments { get; set; }
        public DbSet<ARID.Models.CommunityFollower> CommunityFollowers { get; set; }
        public DbSet<ARID.Models.CommentMetric> CommentMetrics { get; set; }
        public DbSet<ARID.Models.PostMetric> PostMetrics { get; set; }
        public DbSet<ARID.Models.PostRevision> PostRevisions { get; set; }

        //User Score
        public DbSet<ARID.Models.ScoreValue> ScoreValues { get; set; }
        public DbSet<ARID.Models.ScoreLog> ScoreLogs { get; set; }

        //ScientificEvents
        public DbSet<ARID.Models.ScientificEvent> ScientificEvents { get; set; }
        public DbSet<ARID.Models.PublicationTitle> PublicationTitles { get; set; }
        public DbSet<ARID.Models.Publisher> Publishers { get; set; }

        //MOOC
        public DbSet<ARID.Models.MOOCProvider> MOOCProviders { get; set; }
        public DbSet<ARID.Models.MOOCProviderFollower> MOOCProviderFollowers { get; set; }
        public DbSet<ARID.Models.MOOCList> MOOCLists { get; set; }

        //Vip Declarations
        public DbSet<ARID.Models.VipDeclaration> VipDeclarations { get; set; }


        //Committee
        public DbSet<ARID.Models.Committee> Committees { get; set; }
        public DbSet<ARID.Models.CommitteeProfile> CommitteeProfiles { get; set; }
        public DbSet<ARID.Models.CommitteeAchievement> CommitteeAchievements { get; set; }


        //RegistrationForm
        public DbSet<ARID.Models.RegistrationForm> RegistrationForms { get; set; }
        public DbSet<ARID.Models.UserExpressInterest> UserExpressInterests { get; set; }
        public DbSet<ARID.Models.SentEmailRecords> SentEmailRecords { get; set; }


        //Ads
        public DbSet<ARID.Models.SideAd> SideAds { get; set; }


        //Courses
        public DbSet<ARID.Models.Course> Courses { get; set; }
        public DbSet<ARID.Models.CourseChapter> CourseChapters { get; set; }
        public DbSet<ARID.Models.CourseChapterContent> CourseChapterContents { get; set; }
        public DbSet<ARID.Models.CourseInstructor> CourseInstructors { get; set; }
        public DbSet<ARID.Models.CourseSponsor> CourseSponsors { get; set; }
        public DbSet<ARID.Models.CourseRegisteration> CourseRegisterations { get; set; }
        public DbSet<ARID.Models.ChapterStudyStatus> ChapterStudyStatuses { get; set; }
        public DbSet<ARID.Models.CourseChapterExam> CourseChapterExams { get; set; }
        public DbSet<ARID.Models.CourseChapterExamLog> ChapterExamLogs { get; set; }
        public DbSet<ARID.Models.CourseChapterChoice> CourseChapterChoices { get; set; }

        //Message
        public DbSet<ARID.Messages.Models.Message> Messages { get; set; }
        public DbSet<ARID.Models.MessageReply>  MessageReplies { get; set; }

        //Journals
        public DbSet<ARID.Models.Journal> Journals { get; set; }
        public DbSet<ARID.Models.Manuscript> Manuscripts { get; set; }
        public DbSet<ARID.Models.Volume> Volumes { get; set; }
        public DbSet<ARID.Models.Submission> Submissions { get; set; }
        public DbSet<ARID.Models.SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<ARID.Models.AreaEditor> AreaEditors { get; set; }
        public DbSet<ARID.Models.ArticleAuthorship> ArticleAuthorships { get; set; }
        public DbSet<ARID.Models.SubmissionReviewer> SubmissionReviewers { get; set; }
        public DbSet<ARID.Models.ArticleType> ArticleTypes { get; set; }
        public DbSet<ARID.Models.JournalArticleType> JournalArticleTypes { get; set; }
        public DbSet<ARID.Models.JournalFileType> JournalFileTypes { get; set; }
        public DbSet<ARID.Models.FileType> FileTypes { get; set; }
        public DbSet<ARID.Models.JournalReviewer> JournalReviewers { get; set; }
        public DbSet<ARID.Models.JournalIssue> JournalIssues { get; set; }
        public DbSet<ARID.Models.JournalPage> JournalPages { get; set; }
        public DbSet<ARID.Models.SuggestedReviewer> SuggestedReviewers { get; set; }
        public DbSet<ARID.Models.JournalSpeciality> JournalSpecialities { get; set; }

        //Freelancer
        public DbSet<ARID.Models.FreelancerComment> FreelancerComments { get; set; }
        public DbSet<ARID.Models.FreelancerPortfolio> FreelancerPortfolios { get; set; }
        public DbSet<ARID.Models.FreelancerProject> FreelancerProjects { get; set; }
        public DbSet<ARID.Models.FreelancerProposal> FreelancerProposals { get; set; }
        public DbSet<ARID.Models.FreelancerRating> FreelancerRatings { get; set; }
        public DbSet<ARID.Models.FreelancerRecruiter> FreelancerRecruiters { get; set; }
        public DbSet<ARID.Models.FreelancerSkillVerification> FreelancerSkillVerifications { get; set; }

        //Ticket
        public DbSet<ARID.Models.Ticket> Tickets { get; set; }
        public DbSet<ARID.Models.TicketReply> TicketReplies { get; set; }
        public DbSet<ARID.Models.TicketCategory> TicketCategory { get; set; }
        public DbSet<ARID.Models.Statement> Statements { get; set; }
        public DbSet<ARID.Models.Filspay> Filspays { get; set; }
        public DbSet<ARID.Models.Membership> Memberships { get; set; }
        public DbSet<ARID.Models.Address> Addresses { get; set; }
        public DbSet<ARID.Models.FreelancerReadyService> FreelancerReadyServices { get; set; }
        public DbSet<ARID.Models.FreelancerReadyServiceExtension> FreelancerReadyServiceExtensions { get; set; }


    }
}
