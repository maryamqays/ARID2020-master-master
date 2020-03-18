using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public static class Common
    {
        public static string ARIDFormat(int arid)
        {
            string ss = arid.ToString("D8");
            ss = string.Format("{0}-{1}", ss.Substring(0, 4), ss.Substring(4, 4));
            return ss;
        }

        public static int ARIDUnformat(string arid)
        {
            string ss = arid.Substring(0, 4) + arid.Substring(5, 4);
            int ufarid = 0;
            try
            {
                ufarid = Convert.ToInt32(ss);
            }
            catch
            {
                ufarid = 0;
            }
            return ufarid;
        }


    }

    public enum BalanceType
    {
        [Display(Name = "الرصيد المؤقت")]
        holdingbalance = 1,
        [Display(Name = "الرصيد الحالي")]
        currentbalance = 2,
    };

    public enum PricingType
    {
        [Display(Name = "5usd")]
        Fiver = 1,
        [Display(Name = "10usd")]
        Tenth = 2,
        [Display(Name = "15usd")]
        Fifteenth = 3,
        [Display(Name = "20usd")]
        Twenty = 4,
        [Display(Name = "25usd")]
        TwenyFive = 5,
    };

    

    public class DataTableProperties
    {
        public static DataTableProperties GetDataTableProperties(HttpRequest request)
        {
            DataTableProperties DTP = new DataTableProperties
            {
                Draw = request.Form["draw"].FirstOrDefault(),

                // Skiping number of Rows count  
                Start = request.Form["start"].FirstOrDefault(),

                // Paging Length 10,20  
                Length = request.Form["length"].FirstOrDefault(),

                // Sort Column Name
                SortColumn = request.Form["columns[" +
                request.Form["order[0][column]"].FirstOrDefault() +
                "][data]"].FirstOrDefault(),

                // Sort Column Direction ( asc ,desc)  
                SortColumnDirection = request.Form["order[0][dir]"].FirstOrDefault(),

                // Search Value from (Search box)  
                SearchValue = request.Form["search[value]"].FirstOrDefault()
            };

            // Paging Size (10,20,50,100)  
            DTP.PageSize = DTP.Length != null ? Convert.ToInt32(DTP.Length) : 0;

            DTP.Skip = DTP.Start != null ? Convert.ToInt32(DTP.Start) : 0;

            DTP.RecordsTotal = 0;

            return DTP;
        }

        public string Draw { get; set; }
        public string Start { get; set; }
        public string Length { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public string SearchValue { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int RecordsTotal { get; set; }
    }

    public enum ProjectState
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "LookingForParticipants")]
        LookingForParticipants = 1,
        [Display(Name = "InProgress")]
        InProgress = 2,
        [Display(Name = "Stopped")]
        Stopped = 3,
        [Display(Name = "Suspended")]
        Suspended = 4,
        [Display(Name = "Succeeded")]
        Succeeded = 5,
        [Display(Name = "Failed")]
        Failed = 6
    };

    public enum MembershipType
    {
        [Display(Name = "الهوية البرونزية")]
        Bronze = 1,
        [Display(Name = "الهوية الفضية")]
        Silver = 2,
        [Display(Name = "الهوية الذهبية")]
        Golden = 3,
      
    };

    public enum GenderType
    {
        [Display(Name = "Unknown")]
        Unknown = 0,
        [Display(Name = "Male")]
        Male = 1,
        [Display(Name = "Female")]
        Female = 2
    };

    public enum Certificate
    {
        [Display(Name = "لا توجد")]
        None = 0,
        [Display(Name = "شهادة مدفوعة الثمن")]
        Paid = 1,
        [Display(Name = "شهادة مجانية")]
        Free = 2,
    };

    //public enum CommunityScore
    //{
    //    [Display(Name = "Login")]
    //    Login = 1,
    //    [Display(Name = "PostVote")]
    //    PostVote = 2,
    //    [Display(Name = "PostVoted")]
    //    PostVoted =5 ,
    //    [Display(Name = "CommentVote")]
    //    CommentVote = 1,
    //    [Display(Name = "Answer")]
    //    Answer = 15,
    //    [Display(Name = "Post")]
    //    Post = 5,
    //    [Display(Name = "NewBlog")]
    //    NewBlog = 15,
    //    [Display(Name = "EditAccepted")]
    //    EditAccepted = 3,
    //    [Display(Name = "AcceptBestAnswer")]
    //    AcceptBestAnswer = 2,
    //    [Display(Name = "Comment")]
    //    Comment = 3,
    //    [Display(Name = "VotedDownQuestion")]
    //    VotedDownQuestion = -2,
    //    [Display(Name = "VotedDownComment")]
    //    VotedDownComment = -1,
    //    [Display(Name = "Reported5Times")]
    //    Reported5Times = -25,
    //    [Display(Name = "MaximumDailyPoints")]
    //    MaximumDailyPoints = 200
    //        //https://stackoverflow.com/help/whats-reputation
    //};


    public enum StatusType
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "Active")]
        Active = 1,
        [Display(Name = "Inactive")]
        Inactive = 2,
        [Display(Name = "Suspended")]
        Suspended = 3,
        [Display(Name = "Suspicious")]
        Suspicious = 4
    };
    public enum GiftType
    {
        [Display(Name = "قهوة 3$")]
        Coffee = 0,
        [Display(Name = "عصير 6$")]
        juice = 1,
        [Display(Name = "علبة ماكنتوش 9$")]
        Bounty = 2
    };

    public enum JournalStatus
    {
        [Display(Name = "تحت المراجعة")]
        UnderReview = 0,
        [Display(Name = "جديدة")]
        New = 1,
        [Display(Name = "تحت المراجعة من قبل سكوباس")]
        UnderScopusReview = 2,
        [Display(Name = "تخضع لمعايير سكوباس")]
        UnderScopus = 3,
        [Display(Name = "فاعلة")]
        Active = 4,
        [Display(Name = "غير فاعلة")]
        InActive = 5
    };

    public enum ProfileType
    {
        [Display(Name = "GoogleScholar")]
        GoogleScholar = 0,

        [Display(Name = "ResearchGate")]
        ResearchGate = 1,

        [Display(Name = "ResearcherID")]
        ResearcherID = 2,

        [Display(Name = "Academia")]
        Academia = 3,

        [Display(Name = "Mendeley")]
        Mendeley = 4,

        [Display(Name = "Linkedin")]
        Linkedin = 5,

        [Display(Name = "Scopus")]
        Scopus = 6,

        [Display(Name = "ORCID")]
        ORCID = 7,

        [Display(Name = "Facebook")]
        Facebook = 8,

        [Display(Name = "Twitter")]
        Twitter = 9,

        [Display(Name = "Skype")]
        Skype = 10,

        [Display(Name = "SlideShare")]
        SlideShare = 11,

        [Display(Name = "Youtube")]
        Youtube = 12,

        [Display(Name = "Others")]
        Others = 100
    };

    public enum AccessType
    {
        [Display(Name = "ForPublic")]
        ForPublic = 0,
        [Display(Name = "OnlyMe")]
        OnlyMe = 1
    };
    public enum AuthorAcceptedRevisionsStatus
    {
        [Display(Name = "قيد الانتظار")]
        Pending = 1,
        [Display(Name = "تمت الموافقة")]
        Accepted = 2,
        [Display(Name = "رفض")]
        Reject = 3,
    };
    public enum AchievementType
    {
        [Display(Name = "أجتماع")]
        Meeting = 0,
        [Display(Name = "مقابلة")]
        Review = 1,
        [Display(Name = "تنظيم")]
        Organize = 2,
        [Display(Name = "تنفيذ")]
        Execution = 3,
        [Display(Name = "حضور حدث")]
        EventAttendance = 4
    };




    public enum UserRoles
    {
        [Display(Name = "مدير")]
        Admins = 0,
        [Display(Name = "عضو")]
        Members = 1,
        [Display(Name = "إستشاري")]
        Consultants = 2,
        [Display(Name = "محرر")]
        Editors = 3,
        [Display(Name = "مراجع علمي")]
        Reviewers = 4,
        [Display(Name = "مستقل")]
        Freelancers = 5,
        [Display(Name = "أكاديمي")]
        Academicians = 6,
        [Display(Name = "مراجع لغوي")]
        Proofreaders = 7
    };

    public enum PublicationType
    {
        [Display(Name = "Journal")]
        Journal = 0,
        [Display(Name = "Conference")]
        Conference = 1,
        [Display(Name = "ScientificArticle")]
        Letter = 2,
        [Display(Name = "Patent")]
        Patent = 3,
        [Display(Name = "Standard")]
        Standard = 4,
        [Display(Name = "MasterThesis")]
        MasterThesis = 5,
        [Display(Name = "PhDThesis")]
        PhDThesis = 6,
        [Display(Name = "ChapterinBook")]
        ChapterinBook = 7,
        [Display(Name = "Book")]
        Book = 8,
        [Display(Name = "ScientificReport")]
        ScientificReport = 9,
        [Display(Name = "Dataset")]
        Dataset = 10,
        [Display(Name = "Other")]
        Other = 100
    };

    public enum PublicationTypes
    {
        [Display(Name = " Journal")]
        Journal = 0,
        [Display(Name = "ConferenceProceeding")]
        ConferenceProceeding = 1,
        [Display(Name = "Transactions")]
        Transactions = 2,
        [Display(Name = "Communications")]
        Communications = 3,
        [Display(Name = "Magazine")]
        Magazine = 4
    };

    public enum RelationType
    {
        [Display(Name = "Alone")]
        Alone = 0,
        [Display(Name = "InGroup")]
        InGroup = 1,
        [Display(Name = "TeamLeader")]
        TeamLeader = 2
    };

    public enum SubmissionStatus
    {
        [Display(Name = "Not Submitted")]
        NotSubmitted = 0,
        [Display(Name = "Submitted")]
        Submitted = 1,
        //[Display(Name = "ReSubmitted")]
        //ReSubmitted = 5,
        [Display(Name = "Returned Back to Author")]
        Returned = 2,
        [Display(Name = "Rejected")]
        Rejected = 3,
        [Display(Name = "NeglectedSubmission")]
        NeglectedSubmission = 4
    };

    public enum EICDecision
    {
        [Display(Name = "قبول البحث")]
        Accept = 1,
        [Display(Name = "تعديلات طفيفة")]
        Minor = 2,
        [Display(Name = "تعديلات كبيرة")]
        Major = 3,
        [Display(Name = "مراجعة لتحرير اللغة")]
        ReviseForLanguageEditing = 4,
        [Display(Name = "رفض و إعادة رفع")]
        RejectWithResubmit = 5,
        [Display(Name = "رفض")]
        Reject = 6,
        [Display(Name = "رفض مع خيار الإعتراض")]
        RejectWithAppeal = 7,
        [Display(Name = "رفض و نقل إالى مجلة أخرى في أريد")]
        RejectTransfer = 8
    };

    public enum SubmissionType
    {
        [Display(Name = "Research paper | مقالات بحثية")]
        ResearchPaper = 1,
        [Display(Name = "Short Communications | ابحاث قصيرة ")]
        ShortCommunication = 2,
        [Display(Name = "Reviews | الابحاث الاستعراضية")]
        Reviews = 3,
        [Display(Name = "Letter to the Editor | رسالة إلى المحرر")]
        Letter = 4
    };

    public enum ManuscriptCurrentStatus
    {
        //NewSubmission, EditorInvited, EditorAssigned, ReviewersInvited, UnderReview, 
        //MajorRevision, MinorRevision, Accepted, Rejected, OverDueDate, Withdrawn
        [Display(Name = "المقالات التي لم تكتمل")]
        InComplete = 1,
        [Display(Name = "المقالات المرسلة/ قيد الدراسة")]
        Submitted = 2,
        [Display(Name = " مقالات قيد المراجعة")]
        UnderReview = 3,
        [Display(Name = "المقالات التي تم إعادتها للمؤلف")]
        SubmissionSentBackToAuthor = 4,
        [Display(Name = " المقالات التي تحتاج لمراجعة")]
        SubmissionsNeedingRevision = 5,
        [Display(Name = " مراجعات مرفوضة (قرر المؤلف إلغاء كل المقالات) ")]
        AuthorRejectedEditings = 6,
        [Display(Name = "بحوث تحت المراجعة بعد التسليم")]
        SubmissionsBeingProcessed = 7,
        [Display(Name = "بحوث تحت المراجعة")]
        RevisionsBeingProcessed = 8,
        [Display(Name = "حوث تم ارسالها للدفع")]
        ManuscriptSenttoAuthorforPayment = 9,
        [Display(Name = " نسخة ما قبل الطباعة النهائية")] //نسخة ما قبل الطباعة النهائية
        GalleyProof = 10,
        [Display(Name = " المقالات التي تم دراستها واتخذ القرار بنشرها")]
        Published = 11,
        [Display(Name = "بحوث تم رفضها")]
        Declined = 12,
        [Display(Name = "بحوث تمت مراجعتها")]
        ReviewCompleted = 13,
        [Display(Name = " مقالات التی ساهمت کمؤلف المشارك")]
        CoAuthor = 14
    };


    public enum CommunityType
    {
        [Display(Name = "Blog")]
        Personal = 0,
        [Display(Name = "Community")]
        Community = 1,
        [Display(Name = "Group")]
        Group = 2,
        [Display(Name = "AridBlod")]
        AridBlod = 3,
        [Display(Name = "UniversityBlog")]
        UniversityBlog = 4
    };
    public enum ReportType
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "NotRelated")]
        NotRelated = 1,
        [Display(Name = "Violation")]
        Violation = 2,
        [Display(Name = "HateSpeech")]
        HateSpeech = 3

    };
    public enum SecurityLevel
    {
        [Display(Name = "مخفية")]
        RequiresAdminApproval = 1,
        [Display(Name = "مرئية--الانظمام بموافقة المدير")]
        Open = 2,
    };

    public enum GroupPostType
    {
        [Display(Name = "Opprotunity")]
        Opprotunity = 0,
        [Display(Name = "QA")]
        QA = 1,
        [Display(Name = "Article")]
        Article = 2
    };

    public enum BlogPostType
    {
        [Display(Name = "اصدارات")]
        Publications = 3,
        [Display(Name = "انشطة علمية")]
        ScientificActivites = 4,
        [Display(Name = "مقالات علمية")]
        Article = 5,
        [Display(Name = "وقائع وتوجيهات")]
        Guidance = 6,
        [Display(Name = "الانجازات العلمية")]
        ScientificAchievements = 7,
        [Display(Name = "تقارير")]
        Reports = 8
    };


    public enum AridBlogPostType
    {
        [Display(Name = "أريد في وسائل الاعلام")]
        AridMediaCoverage = 9,

        [Display(Name = "تقارير ومقالات")]
        Articles = 10,

        [Display(Name = "مؤتمرات وندوات وملتقيات")]
        Gatherings = 11,

        [Display(Name = "اخبار المنصة")]
        AridNews = 12,

        [Display(Name = "التكريم")]
        AridHonoring = 13,

        [Display(Name = "المحفل العلمي الدولي")]
        ISF = 14,
                [Display(Name = "الملفات المساعدة")]
        HelpFiles = 15,
        [Display(Name = "يوم الباحث العربي")]
        ArabicResearcherDay = 16,
               [Display(Name = " عالم فوق المنصة")]
        ScolaronArid = 17,
        [Display(Name = " مسابقات المنصة")]
        AridCompetition = 18
               };

    public enum UniversityBlogPostType
    {
        [Display(Name = "اخبار الجامعة")]
        Publications = 19,
        [Display(Name = "مؤتمرات وندوات")]
        ScientificActivites = 20,
        [Display(Name = "دورات وورش عمل")]
        Article = 21
         };

    public enum EventType
    {
        [Display(Name = "مؤتمر علمي")]
        Conference = 0,
        [Display(Name = "ندوة")]
        Siminar = 1,
        [Display(Name = "جائزة")]
        Award = 2,
        [Display(Name = "ورشة عمل")]
        WorkShop = 3,
        [Display(Name = "دورة تدريبية")]
        TraininCourse = 4
    };

    public enum ContentType
    {
        [Display(Name = "مقطع فيديو (فيميو) ")]
        vimeo = 0,
        [Display(Name = "مقطع فيديو (يوتيوب)")]
        youtube = 1,
        [Display(Name = "ملف")]
        file = 2,
        [Display(Name = "رابط")]
        website = 3,
    };
    //ResearchLength(Appropriate-Long-notenough)


    public enum ResearchLength
    {
        [Display(Name = "مناسب")]
        Appropriate = 1,
        [Display(Name = "طويل")]
        Long = 2,
        [Display(Name = "غير كافي")]
        NotEnough = 3,
    }; /*ReviewCompleted*/    //TheoreticalResults(enough-Appropriate-notenough)


    public enum TheoreticalResults
    {
        [Display(Name = "كافية")]
        enough = 1,
        [Display(Name = "مناسبة")]
        Appropriate = 2,
        [Display(Name = "غير كافي")]
        NotEnough = 3,
    };
    //Method(New-PreviouslyApproved-UnAcceptable)


    public enum Method
    {
        [Display(Name = "جديدة")]
        New = 1,
        [Display(Name = "معتمدة سابقاً")]
        PreviouslyApproved = 2,
        [Display(Name = "غير مقبولة")]
        UnAcceptable = 3
    };
    //IsCommitted(Yes-To some extent-No)
    public enum IsCommitted
    {
        [Display(Name = "نعم")]
        Yes = 1,
        [Display(Name = "إلى حد ما")]
        ToSomeExtent = 2,
        [Display(Name = "كلا")]
        No = 3,
    };
    //IsMatchTitleContent
    //(Yes-To some extent-No)

    public enum IsMatchTitleContent
    {
        [Display(Name = "نعم")]
        Yes = 1,
        [Display(Name = "إلى حد ما")]
        ToSomeExtent = 2,
        [Display(Name = "كلا")]
        No = 3,
    };

    public enum IsModernSourcesAdopted
    {
        [Display(Name = "نعم")]
        Yes = 1,
        [Display(Name = "إلى حد ما")]
        ToSomeExtent = 2,
        [Display(Name = "كلا")]
        No = 3,
    };

    // RecommendationToPublish(Valid for publication-inValid for publication-withediting)
    public enum RecommendationToPublish
    {
        [Display(Name = "صالح للنشر")]
        ValidForPublication = 1,
        [Display(Name = "صالح للنشر بعد اجراء تعديلات جوهرية")]
        withMajorediting = 2,
        [Display(Name = "صالح للنشر بعد اجراء تعديلات طفيفة")]
        withMinorediting = 3,
        [Display(Name = "غير صالح للنشر اطلاقا")]
        NotForPublication = 4,
    };

    public enum ReviewerDecisionToReview
    {
        [Display(Name = "بانتظار قبول الدعوة")]
        WaitingInvitationAcceptance = 0,
        [Display(Name = "الموافقة")]
        AcceptedToReview = 1,
        [Display(Name = "الرفض")]
        DeclinedToReview = 2

    };




    public enum Evaluation
    {
        [Display(Name = "بالمستوى المطلوب")]
        Acceptable = 1,
        [Display(Name = "أقل من المستوى المطلوب")]
        Disappointing = 2,
    };


    public enum AdsPositionType
    {
        [Display(Name = "Top")]
        Top = 1,

        [Display(Name = "Right")]
        Right = 2,

        [Display(Name = "Left")]
        Left = 3,

        [Display(Name = "Bottom")]
        Bottom = 4
    }

    public enum TargetType
    {
        [Display(Name = "_blank")]
        blank = 1,

        [Display(Name = "_parent")]
        parent = 2,

        [Display(Name = "_self")]
        self = 3,

        [Display(Name = "_top")]
        top = 4
    }

    public enum SkillCategoryType
    {
        [Display(Name = "أعمال، محاسبة، موارد بشرية وأمور قانونية")]
        Law = 1,

        [Display(Name = "ادخال بيانات ومساعدة ادارية")]
        DataEntry = 2,

        [Display(Name = "تصميم وأعمال ابداعية وفنيّة")]
        Design = 3,

        [Display(Name = "هندسة، علوم وأبحاث")]
        Engineering = 4,

        [Display(Name = "حواسيب وأجهزة جوال ولوحية")]
        Computer = 5,

        [Display(Name = "تسويق ومبيعات")]
        SalesMarketing = 6,

        [Display(Name = "ترجمة ولغات")]
        Translation = 7,

        [Display(Name = "تقنية، مواقع وبرامج")]
        Technology = 8,
        [Display(Name = "كتابة وصناعة المحتوى")]
        Content = 9,
        [Display(Name = "اخرى")]
        Others = 10,
    }


    public enum ActivityType
    {
        [Display(Name = "Others")]
        Others = 1,

        [Display(Name = "ConferenceParticipation")]
        ConferenceParticipation = 2,

        [Display(Name = "SymposiumParticipation")]
        SymposiumParticipation = 3,

        [Display(Name = "CourseParticipation")]
        CourseParticipation = 4,

        [Display(Name = "CompititionParticipation")]
        CompititionParticipation = 5,

        [Display(Name = "PostersExhibition")]
        PostersExhibition = 6,

        [Display(Name = "GivingaCourse")]
        GivingaCourse = 7,

        [Display(Name = "GivingaWorkshop")]
        GivingaWorkshop = 8,

        [Display(Name = "KeynoteSpeech")]
        KeynoteSpeech = 9,

        [Display(Name = "ConferencePaper")]
        ConferencePaper = 10,

        [Display(Name = "JournalPaper")]
        JournalPaper = 11,

        [Display(Name = "Patent")]
        Patent = 12,

        [Display(Name = "CopyRight")]
        CopyRight = 13,

        [Display(Name = "Trademark")]
        Trademark = 14,

        [Display(Name = "Book")]
        Book = 15,

        [Display(Name = "BookChapter")]
        BookChapter = 16,

        [Display(Name = "BookTranslation")]
        BookTranslation = 17,

        [Display(Name = "JournalReviewer")]
        JournalReviewer = 18,

        [Display(Name = "ConferenceReviewer")]
        ConferenceReviewer = 19,

        [Display(Name = "EditorinChief")]
        EditorinChief = 20,

        [Display(Name = "Editor")]
        Editor = 21,

        [Display(Name = "CEO")]
        CEO = 22,

        [Display(Name = "ProjectManager")]
        ProjectManager = 23,

        [Display(Name = "TeamLeader")]
        TeamLeader = 24,

        [Display(Name = "VoluntaryWork")]
        VoluntaryWork = 25,

        [Display(Name = "Presentation")]
        Presentation = 26,

        [Display(Name = "ConferencePresident")]
        ConferencePresident = 27,

        [Display(Name = "ConferenceOrganizer")]
        ConferenceOrganizer = 28,

        [Display(Name = "TrainingDirector")]
        TrainingDirector = 29,

        [Display(Name = "Consultant")]
        Consultant = 30,

        [Display(Name = "ScientificCommitteeMember")]
        ScientificCommitteeMember = 31,

        [Display(Name = "ScientificCommitteeChairman")]
        ScientificCommitteeChairman = 32,

        [Display(Name = "InternationalArbitrator")]
        InternationalArbitrator = 33,

        [Display(Name = "MediaActivity")]
        MediaActivity = 34,

        [Display(Name = "SupervisingBachelorStudents")]
        SupervisingBachelorStudents = 35,

        [Display(Name = "SupervisingMasterStudents")]
        SupervisingMasterStudents = 36,

        [Display(Name = "SupervisingPhDStudents")]
        SupervisingPhDStudents = 37,

        [Display(Name = "DataCollection")]
        DataCollection = 38
    };

    public enum RecruiterProjectType
    {
        [Display(Name = "إضافة موكل")]
        Add = 1,

        [Display(Name = "عدم إضافة موكل")]
        DontAdd = 2,

    }

    public enum RecruiterStatus
    {
        [Display(Name = "need edit")]
        Self = 1,

        [Display(Name = "need edit")]
        Expert = 2,

        [Display(Name = "need edit")]
        Free = 3,
    }

    public enum FreelancerProjectStatus
    {
        [Display(Name = "under review")]
        underreview = 1,

        [Display(Name = "Accepting offers")]
        Acceptingoffers = 2,

        [Display(Name = "Execution")]
        Execution = 3,

        [Display(Name = "تم التسليم")]
        Delivered = 4,

        [Display(Name = "مكتمل")]
        Completed = 5,

        [Display(Name = "ملغاة")]
        Deleted = 6,

        [Display(Name = "المغلقة")]
        Closed = 7,

        [Display(Name = "المرفوضة")]
        Rejected = 7,

        [Display(Name = "خدمات جاهزة")]
        FreelancerReadyServices = 8,

        [Display(Name = "وظفني")]
        HireMe = 9,
    }

    //public enum SkillCategoryType
    //{
    //    [Display(Name = "أعمال، محاسبة، موارد بشرية وأمور قانونية")]
    //    Law = 15,

    //    [Display(Name = "ادخال بيانات ومساعدة ادارية")]
    //    DataEntry = 6,

    //    [Display(Name = "تصميم وأعمال ابداعية وفنيّة")]
    //    Design = 13,

    //    [Display(Name = "هندسة، علوم وأبحاث")]
    //    Engineering = 3,

    //    [Display(Name = "حواسيب وأجهزة جوال ولوحية")]
    //    Computer = 7,

    //    [Display(Name = "تسويق ومبيعات")]
    //    SalesMarketing = 6,

    //    [Display(Name = "ترجمة ولغات")]
    //    Translation = 12,

    //    [Display(Name = "تقنية، مواقع وبرامج")]
    //    Technology = 7,
    //    [Display(Name = "كتابة وصناعة المحتوى")]
    //    Content = 8,
    //    [Display(Name = "اخرى")]
    //    Others = 1
    //}

    public enum PurposeType//need editing
    {
        [Display(Name = "نشر علمي")]
        Publication = 1,

        [Display(Name = "مشروع بحثي")]
        ResearchProject = 2,

        [Display(Name = "مشروع شخصي")]
        PersonalProject = 3,

        [Display(Name = "اخرى")]
        other = 4
    }

    public enum ProposalStatus
    {
        [Display(Name = "بإنتظار الموافقة")]
        Pending = 1,

        [Display(Name = "مستبعد")]
        Execluded = 2,

        [Display(Name = "مكتمل")]
        Completed = 3,

        [Display(Name = "قيد التنفيذ")]
        Execution = 4,

        [Display(Name = "تم التسليم")]
        Delivered = 5
    }

    public enum BugetType
    {
        [Display(Name = "10-30 $")]
        قليل= 1,

        [Display(Name = "30-100 $")]
        متوسط = 2,

        [Display(Name = "100-300 $")]
        عالي = 3,


    }

    public enum ReviewerStatus
    {
        [Display(Name = "مؤقت")]
        Temporary = 1,
        [Display(Name = "معتمد")]
        Permenant = 2,
    }

    public enum SuggestionStatus
    {
        [Display(Name = "ترشيح")]
        Temporary = 1,
        [Display(Name = "استبعاد")]
        Permenant = 2,
    }

    public enum Role
    {
        [Display(Name = "مساعد علمي")]
        ScientificEditor = 1,
        [Display(Name = "مساعد تقني")]
        TechnicalEditor = 2,
    }

    public enum RequiredDays
    {
        [Display(Name = "لا تحتاج إالى وقت إضافي")]
        NotNeed = 1,
        [Display(Name = "يوم واحد")]
        Oneday = 2,
        [Display(Name = "يومين")]
        TwoDays = 3,
        [Display(Name = "ثلاثة أيام")]
        ThreeDays =4,
        [Display(Name = "أربعة أبام")]
        FourDays = 5,
        [Display(Name = "خمسة أيام")]
        FiveDays = 6,
        [Display(Name = "ستة أيام")]
        SixDays = 7,
        [Display(Name = "اسبوع")]
        OneWeek = 8,
        [Display(Name = "إسبوعين")]
        TwoWeeks = 9,
        [Display(Name = "ثلاثة أسابيع")]
        ThreeWeeks = 10,
        [Display(Name = "شهر")]
        OneMonth = 11,
    }



    public enum Language
    {
        [Display(Name = "اللغة العربية")]
        Arabic = 0,
        [Display(Name = "اللغة الإنجليزية")]
        English = 1,
        Mandarin,
        Spanish,
        German,
        Dutch,
        French,
        Italian,
        Turkish,
        Kurdish,
        Persian,
        Polish,
        Malay,
        Hindi,
        Portuguese,
        Romanian,
        Russian,
        Ukrainian,
        SerboCroatian,
        Uzbek,
        Japanese,
        Vietnamese,
        Korean,
        Punjabi,
        Tamil,
        Urdu,
        Thai,
        Bengali,
        Javanese,
        Telugu,
        Marathi,
        Gujarati,
        Jin,
        Pashto,
        Kannada,
        Malayalam,
        Sundanese,
        Hausa,
        Odia,
        Burmese,
        Hakka,
        Bhojpuri,
        Tagalog,
        Yoruba,
        Maithili,
        Sindhi,
        Amharic,
        Fula,
        Oromo,
        Igbo,
        Azerbaijani,
        Awadhi,
        GanChinese,
        Cebuano,
        Malagasy,
        Saraiki,
        Nepali,
        Sinhalese,
        Chittagonian,
        Zhuang,
        Khmer,
        Turkmen,
        Assamese,
        Madurese,
        Somali,
        Marwari,
        Magahi,
        Haryanvi,
        Hungarian,
        Chhattisgarhi,
        Greek,
        Chewa,
        Deccan,
        Akan,
        Kazakh,
        NorthernMin,
        Sylheti,
        Zulu,
        Czech,
        Kinyarwanda,
        Dhundhari,
        HaitianCreole,
        Ilocano,
        Quechua,
        Kirundi,
        Swedish,
        Hmong,
        Shona,
        Uyghur,
        Mossi,
        Xhosa,
        Belarusian,
        Balochi,
        Konkani
    };
}