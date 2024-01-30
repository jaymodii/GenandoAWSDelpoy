using Common.Constants;
using Common.Enums;
using Common.Utils;
using Entities.Abstract;
using Entities.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Common.Enums.EntityStatus;

namespace DataAccessLayer.Data;

public class AppDbContext : DbContext
{
    #region Constructor

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion Constructor

    #region Methods
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
                .Entries()
                .Where(e => (IsAuditableEntity(e.Entity.GetType()) || IsTimestampedEntity(e.Entity.GetType())) &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (EntityEntry entityEntry in entries)
        {
            if (IsAuditableEntity(entityEntry.Entity.GetType()))
            {
                dynamic? baseEntity = (dynamic)entityEntry.Entity;

                long userId = GetUserId();

                if (entityEntry.State == EntityState.Added)
                {
                    baseEntity.CreatedOn = DateUtil.UtcNow;
                    baseEntity.CreatedBy = userId;
                }
                if (entityEntry.State == EntityState.Modified)
                {
                    baseEntity.UpdatedOn = DateUtil.UtcNow;
                    baseEntity.UpdatedBy = userId;
                }
            }
            else if (IsTimestampedEntity(entityEntry.Entity.GetType()))
            {
                dynamic? timeStampedEntity = (dynamic)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    timeStampedEntity.CreatedOn = DateUtil.UtcNow;
                }
                if (entityEntry.State == EntityState.Modified)
                {
                    timeStampedEntity.UpdatedOn = DateUtil.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private long GetUserId()
    {
        System.Security.Claims.Claim? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        {
            return userId;
        }
        return 1; // Default value if the claim is not present or not parseable
    }


    private static bool IsTimestampedEntity(Type entityType)
    {
        Type? baseType = entityType.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(TimestampedEntity<>))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    private static bool IsAuditableEntity(Type entityType)
    {
        Type? baseType = entityType.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(AuditableEntity<>))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    #endregion

    #region DbSets

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<TestDetail> TestDetails { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<ClinicalProcessStatus> ClinicalProcessStatuses { get; set; }

    public virtual DbSet<ClinicalQuestion> ClinicalQuestions { get; set; }

    public virtual DbSet<ClinicalDetail> ClinicalDetails { get; set; }

    public virtual DbSet<ClinicalProcess> ClinicalProcesses { get; set; }

    public virtual DbSet<FAQ> FAQs { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<RequestMoreInfoQuestion> RequestMoreInfoQuestions { get; set; }
    
    public virtual DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }

    public virtual DbSet<ClinicalEnhancement> ClinicalEnhancements { get; set; }

    public virtual DbSet<ClinicalEnhancementAnswer> ClinicalEnhancementAnswers { get; set; }

    public virtual DbSet<ClinicalProcessTest> ClinicalProcessesTests { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<SwipeActionSetting> SwipeActionSettings { get; set; }

    #endregion

    #region Model_Builder

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("gender");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.HasKey(e => e.Id).HasName("PK_Gender_Id");
        });

        modelBuilder.Entity<ClinicalQuestion>(entity =>
        {
            entity.ToTable("clinicalQuestion");
            entity.Property(e => e.Question).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Question).IsUnique();
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<ClinicalProcessStatus>(entity =>
        {
            entity.ToTable("clinicalProcessStatus");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(32);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("userRole");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(16);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.ToTable("userStatus");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(32);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql(SystemConstants.DEFAULT_DATETIME);
            entity.Property(e => e.Status).HasDefaultValue(UserStatusTypes.ACTIVE);

            entity
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(a => a.UpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<FAQ>(entity =>
        {
            entity.ToTable("faq");
            entity.Property(e => e.Question).IsRequired().HasMaxLength(124);
            entity.HasIndex(e => e.Question).IsUnique();
            entity.Property(e => e.Answer).IsRequired();
            entity.Property(e => e.ForWhom);
            entity.HasKey(e => e.Id).HasName("PK_FAQ_Id");

            entity.HasOne(e => e.UserRoles)
                  .WithMany()
                  .HasForeignKey(e => e.ForWhom)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_FAQ_UserRoles");
        });

        modelBuilder.Entity<ClinicalDetail>(entity =>
        {
            entity.ToTable("clinicalDetail");
            entity.Property(e => e.PatientId).IsRequired();
            entity.Property(e => e.QuestionId);
            entity.Property(e => e.Answer);
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Users)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.ClinicalQuestions)
                  .WithMany()
                  .HasForeignKey(e => e.QuestionId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TestDetail>(entity =>
        {
            entity.ToTable("testDetail");
            entity.Property(e => e.Abbreviation).IsRequired();
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Duration).IsRequired();
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<ClinicalProcess>(entity =>
        {
            entity.ToTable("clinicalProcess");
            entity.Property(e => e.PatientId).IsRequired();
            entity.Property(e => e.ExternalLink);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.NextStep).IsRequired();
            entity.Property(e => e.Deadline);
            entity.Property(e => e.ExpectedDate);
            entity.HasKey(e => e.Id);

            entity
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(a => a.UpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.ClinicalProcessStatuses)
                  .WithMany()
                  .HasForeignKey(e => e.Status)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.NextStepClinicalProcess)
                 .WithMany()
                 .HasForeignKey(e => e.NextStep)
                 .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.ToTable("testResult");
            entity.Property(e => e.ClinicalProcessTestId).IsRequired();
            entity.Property(e => e.ReportAttachmentTitle).IsRequired();
            entity.Property(e => e.ReportAttachment).IsRequired();
            entity.Property(e => e.DoctorNotes);
            entity.Property(e => e.LabNotes);
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ClinicalProcessTests)
                .WithMany()
                .HasForeignKey(e => e.ClinicalProcessTestId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(a => a.UpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UserRefreshTokens>(entity =>
        {
            entity.ToTable("userRefreshToken");
            entity.HasIndex(e => e.Id);
        });

        modelBuilder.Entity<ClinicalEnhancement>(entity =>
        {
            entity.ToTable("clinicalEnhancement");
            entity.Property(e => e.PatientId);
            entity.Property(e => e.TypeOfQuestion);
            entity.Property(e => e.Options);
            entity.Property(e => e.IsQuestionMandatory).HasDefaultValue(false);
        });
        modelBuilder.Entity<ClinicalEnhancementAnswer>(entity =>
        {
            entity.ToTable("clinicalEnhancementAnswer");
            entity.Property(e => e.PatientId).IsRequired();
            entity.Property(e=>e.QuestionId).IsRequired();
        });

        modelBuilder.Entity<ClinicalEnhancementAnswer>(entity =>
        {
            entity.ToTable("clinicalEnhancementAnswer");
            entity.Property(e => e.PatientId).IsRequired();
            entity.Property(e=>e.QuestionId).IsRequired();
        });

        modelBuilder.Entity<ClinicalProcessTest>(entity =>
        {
            entity.ToTable("clinicalProcessTest");
            entity.HasIndex(e => e.Id);
            entity.Property(e => e.ClinicalProcessId);
            entity.Property(e => e.TestId);
        });

        modelBuilder.Entity<ClinicalProcessTest>(entity =>
        {
            entity.ToTable("clinicalProcessTest");
            entity.HasIndex(e => e.Id);
            entity.Property(e => e.ClinicalProcessId);
            entity.Property(e => e.TestId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notification");
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.SentBy).IsRequired();
            entity.Property(e => e.SendTo).IsRequired();
            entity.Property(e => e.NotificationMessage).IsRequired().HasMaxLength(255);
            entity.Property(e => e.HasRead).HasDefaultValue(false);
            entity.Property(e => e.IsTempDeleted).HasDefaultValue(false);

            entity.HasOne(e => e.SentByUser)
                  .WithMany()
                  .HasForeignKey(e => e.SentBy)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_User_SentByUser");

            entity.HasOne(e => e.SendToUser)
                  .WithMany()
                  .HasForeignKey(e => e.SendTo)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_User_SendToUser");
        });

        modelBuilder.Entity<SwipeActionSetting>(entity =>
        {
            entity.ToTable("swipeActionSetting");
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.SwipeLeftAction).HasMaxLength(100).HasDefaultValue("Delete");
            entity.Property(e => e.SwipeRightAction).HasMaxLength(100).HasDefaultValue("Read");

            entity.HasOne(e => e.user)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_User_UserId");

        });
        #region Seed_Data

        ClinicalEnhancement[] clinicalEnhancement = new ClinicalEnhancement[]
        {
            new ClinicalEnhancement 
            { 
                Id = 1,
                LevelOfQuestion = QuestionLevel.HQLevel,
                TypeOfQuestion = QuestionType.Radio,
                Question = "Do you have any existing medical conditions or chronic illnesses?",
                Options="Yes,No",
                IsQuestionMandatory = true,
            },
            new ClinicalEnhancement {
                Id = 2,
                LevelOfQuestion = QuestionLevel.HQLevel,
                TypeOfQuestion = QuestionType.TextArea,
                Question = "Have you ever been hospitalized for any reason?",            
                IsQuestionMandatory= false,
            },
            new ClinicalEnhancement {
                Id = 3,
                LevelOfQuestion = QuestionLevel.HQLevel,
                TypeOfQuestion = QuestionType.CheckBox,
                Question = "What particular issues you are facing right now?",
                Options="Blood Pressure,Indigestion,Skin Problem,Headache,Isomania",
                IsQuestionMandatory= false,
            },
            new ClinicalEnhancement {
                Id = 4,
                LevelOfQuestion = QuestionLevel.HQLevel,
                TypeOfQuestion = QuestionType.Select,
                Question = "What is your age group?",
                Options ="Child,Teenager,Adult,Senior Citizen",
                IsQuestionMandatory= true,
            },
            new ClinicalEnhancement {
                Id = 5,
                LevelOfQuestion = QuestionLevel.HQLevel,
                TypeOfQuestion = QuestionType.TextArea,
                Question = "Have you ever been diagnosed with mental health conditions, such as depression or anxiety?",
                IsQuestionMandatory= false,
            },
        };
        modelBuilder.Entity<ClinicalEnhancement>().HasData(clinicalEnhancement);

        modelBuilder.Entity<Gender>().HasData(
             new Gender { Id = 1, Title = "Male" },
             new Gender { Id = 2, Title = "Female" }
        );

        ClinicalQuestion[] clinicalQuestions = new ClinicalQuestion[]
        {
            new ClinicalQuestion { Id = 1, Question = "Do you have any existing medical conditions or chronic illnesses?"},
            new ClinicalQuestion { Id = 2, Question = "Have you ever been hospitalized for any reason? If yes, please provide details"},
            new ClinicalQuestion { Id = 3, Question = "Are you currently taking any medications, either prescription or over-the-counter?"},
            new ClinicalQuestion { Id = 4, Question = "Do you have any known allergies to medications, foods, or other substances?"},
            new ClinicalQuestion { Id = 5, Question = "Have you ever been diagnosed with mental health conditions, such as depression or anxiety?"},
            new ClinicalQuestion { Id = 6, Question = "Have you ever had any surgeries or major medical procedures in the past?"},
            new ClinicalQuestion { Id = 7, Question = "Do you smoke or have a history of smoking? If yes, how many cigarettes per day and for how long ? "},
            new ClinicalQuestion { Id = 8, Question = "Do you consume alcohol? If yes, how often and how much?"},
            new ClinicalQuestion { Id = 9, Question = "Are you physically active, and do you exercise regularly ?"},
            new ClinicalQuestion { Id = 10, Question = "Have you experienced any recent changes in weight, appetite, or energy levels ?"},
        };
        modelBuilder.Entity<ClinicalQuestion>().HasData(clinicalQuestions);

        ClinicalProcessStatus[] clinicalProcessStatus = new ClinicalProcessStatus[]
       {
            new () { Id = 1, Title = "Initial"},
            new () { Id = 2, Title = "Clinical Path"},
            new () { Id = 3, Title = "Prescribe Test"},
            new () { Id = 4, Title = "Collect Sample"},
            new () { Id = 5, Title = "Ship Sample"},
            new () { Id = 6, Title = "Receive Sample" },
            new () { Id = 7, Title = "Sample Analysis"},
            new () { Id = 8, Title = "Send Lab Results"},
            new () { Id = 9, Title = "Publish Report"},
            new () { Id = 10, Title = "Complete" }
       };
        modelBuilder.Entity<ClinicalProcessStatus>().HasData(clinicalProcessStatus);

        modelBuilder.Entity<UserRole>().HasData(
             new UserRole { Id = 1, Title = "Doctor" },
             new UserRole { Id = 2, Title = "Patient" },
             new UserRole { Id = 3, Title = "Lab" }
        );

        modelBuilder.Entity<UserStatus>().HasData(
            new UserStatus { Id = 1, Title = "Active" },
            new UserStatus { Id = 2, Title = "Inactive" },
            new UserStatus { Id = 3, Title = "Deleted" }
       );

        modelBuilder.Entity<TestDetail>().HasData(
                new TestDetail
                {
                    Id = 1,
                    Abbreviation = "WES",
                    Title = "Esoma Clinico (WES) 100X",
                    Description = "Detailed description of the sequencing method & relevant gene coverage. Report of rare causative variants described in the literature with pathogenetic significance. Report of rare variants with frequency in the genral population (Minor Allele Frequency - MAF) < 1%. Clear variants of the results identified on the basis of international guidlines on 'best practices'. Please Note: In this case the report will include all the rare variants of the subject without any clinical interpretation, so will not be given any reference clinical & scientific literature, it will be up to the doctor who will be sent the report.'",
                    Duration = 4,
                    Price = 100.00
                },
                new TestDetail
                {
                    Id = 2,
                    Abbreviation = "WESR",
                    Title = "Esoma Clinico (WES) 100X e relativa interpretazione clinica",
                    Description = "Comprehensive description of the sequencing methodology and pertinent gene coverage. In-depth analysis of rare causative variants documented in medical literature, emphasizing their pathogenic significance. Identification of infrequent variants prevalent in the general population (Minor Allele Frequency - MAF) < 1%. Definitive classification of significant variants based on globally recognized 'best practice' guidelines. Please note: The report will encompass all uncommon variants present in the individual, devoid of clinical interpretation. Subsequent evaluation based on clinical and scientific literature will be left to the healthcare professional receiving the report.",
                    Duration = 5,
                    Price = 150.00
                },
                new TestDetail
                {
                    Id = 3,
                    Abbreviation = "CGP",
                    Title = "Comprehensive Genomic Profiling (CGP) 200X",
                    Description = "Thorough elucidation of the genomic profiling methodology, encompassing a wide array of genes. Examination of somatic variants associated with cancer, including actionable mutations and potential therapeutic targets. Analysis of tumor mutational burden (TMB) and microsatellite instability (MSI) status. Comprehensive reporting of findings aligned with current clinical evidence and guidelines. Please Note: The provided report will furnish an extensive overview of genomic alterations without specific clinical correlations. Subsequent clinical interpretation is at the discretion of the healthcare provider.",
                    Duration = 3,
                    Price = 250.00
                },
                new TestDetail
                {
                    Id = 4,
                    Abbreviation = "EMT",
                    Title = "Exome Mutation Testing (EMT) Panel",
                    Description = "A comprehensive overview of the exome mutation testing panel methodology, detailing the coverage of key genetic regions. Emphasis on identifying rare causative variants with significant clinical implications. Examination of low-frequency variants prevalent in the general population (Minor Allele Frequency - MAF) < 0.5%. Precise classification of identified variants following international 'best practice' guidelines. Please note: The report will encompass all rare variants identified in the individual without explicit clinical interpretation. Clinical and scientific referencing will be the responsibility of the healthcare professional receiving the report.",
                    Duration = 5,
                    Price = 180.00
                },
                new TestDetail
                {
                    Id = 5,
                    Abbreviation = "PGX",
                    Title = "Personalized Genomic Analysis (PGX) Panel",
                    Description = "In-depth elucidation of the personalized genomic analysis panel methodology, encompassing key genetic markers. Focus on identifying genetic variants linked to drug metabolism and treatment response. Assessment of pharmacogenetic variants influencing medication efficacy and adverse reactions. Detailed reporting aligned with current clinical guidelines for personalized medicine. Note: The report provided will offer an extensive view of pharmacogenetic variants without specific clinical recommendations. Subsequent interpretation and medical decision-making rest with the healthcare provider.",
                    Duration = 4,
                    Price = 120.00
                },
                new TestDetail
                {
                    Id = 6,
                    Abbreviation = "CEN",
                    Title = "Cardiovascular Exome Nexus (CEN) Study",
                    Description = "Detailed overview of the Cardiovascular Exome Nexus (CEN) study methodology, covering a wide range of genes related to heart health. Identification of rare variants with potential implications for cardiovascular diseases. Analysis of low-frequency genetic variants with a focus on their impact on heart health (Minor Allele Frequency - MAF < 1%). Comprehensive classification of identified variants following established best practices. Please note: The report will encompass all rare variants found in the individual, without specific clinical interpretation. Clinical correlation and decision-making should be carried out by the healthcare professional receiving the report.",
                    Duration = 1,
                    Price = 300.00
                }
            );
            modelBuilder.Entity<FAQ>().HasData(
                new FAQ
                {
                    Id = 1,
                    Question = "What are the key benefits of genetic testing for my patients?",
                    Answer = "Genetic testing offers valuable insights into patients' genetic makeup, aiding in the identification of potential disease risks, hereditary conditions, and treatment responses. It enables personalized medical decisions and empowers patients to make informed choices about their health. Interpreting genetic test results involves analyzing genetic variants for their clinical significance and relevance to patients' health. Our comprehensive genetic tests provide detailed information about genetic variants, enabling tailored healthcare plans. Please note: While genetic testing provides crucial information, clinical expertise is essential for translating these insights into effective patient care.",
                    ForWhom = 1
                },
                new FAQ
                {
                    Id = 2,
                    Question = "How do I interpret genetic test results for my patients?",
                    Answer = "Interpreting genetic test results involves analyzing genetic variants for their clinical significance and relevance to patients' health. We provide clear reports outlining identified variants and their potential implications. However, it's crucial to consider these results within the broader clinical context. Genetic counselors and specialists can assist in understanding the implications and guiding treatment decisions. Please Note: Genetic test results provide valuable data, but they should be integrated with clinical judgment and expert advice.",
                    ForWhom = 1
                },
                new FAQ
                {
                    Id = 3,
                    Question = "Are there specific genetic tests for cancer risk assessment?",
                    Answer = "Yes, we offer genetic tests designed to assess cancer risk by identifying mutations associated with various types of cancer. These tests analyze genes linked to hereditary cancer syndromes. Detected mutations can inform early detection strategies and guide preventative measures. However, it's important to remember that genetic risk is just one factor, and regular screenings and consultations with oncologists are crucial. Please note: Genetic tests provide risk information, but comprehensive cancer care involves collaboration with specialists.",
                    ForWhom = 1
                },
                new FAQ
                {
                    Id = 4,
                    Question = "How can pharmacogenetic testing improve patient medication management?",
                    Answer = "Pharmacogenetic testing examines genetic variations influencing how patients metabolize medications. This information helps tailor medication selection and dosage for improved efficacy and reduced risk of adverse reactions. However, while these insights are valuable, they should be used alongside clinical judgment and patient history. Pharmacogenetic testing enhances personalized medicine, but ongoing monitoring and adjustments are vital. Please Note: Genetic testing informs medication decisions, but individual responses can vary and require medical expertise.",
                    ForWhom = 1
                },
                new FAQ
                {
                    Id = 5,
                    Question = "How can genetic testing benefit my health?",
                    Answer = "Genetic testing provides insights into your unique genetic makeup, helping identify potential risks for certain health conditions and guiding personalized healthcare decisions. Understanding your genetic predispositions empowers you to adopt preventive measures and make informed lifestyle choices. However, it's important to remember that genetics is just one factor influencing your health. Genetic testing offers valuable information, but medical advice from healthcare professionals remains essential for holistic well-being.",
                    ForWhom = 2
                },
                new FAQ
                {
                    Id = 6,
                    Question = "Can genetic testing predict my response to medications?",
                    Answer = "Yes, pharmacogenetic testing examines genetic variants affecting how your body processes medications. This insight can help healthcare providers tailor medication choices and dosages for better treatment outcomes and reduced risks of adverse reactions. Nevertheless, while genetic testing offers valuable guidance, individual responses can vary. Genetic insights complement medical care, but ongoing communication with healthcare professionals is crucial for optimal medication management.",
                    ForWhom = 2
                },
                new FAQ
                {
                    Id = 7,
                    Question = "Will genetic testing tell me if I'm likely to develop a certain disease?",
                    Answer = "Genetic testing can indicate certain genetic predispositions to diseases, but it's important to note that genetics is just one part of the equation. Lifestyle factors, family history, and other environmental influences also play significant roles. Understanding your genetic risk empowers you to take proactive steps toward prevention, but it's essential to consult healthcare providers for comprehensive health assessments and personalized guidance.",
                    ForWhom = 2
                },
                new FAQ
                {
                    Id = 8,
                    Question = "Can genetic testing diagnose rare conditions I might have?",
                    Answer = "Yes, genetic testing can help diagnose certain rare genetic conditions by identifying specific genetic mutations associated with those conditions. However, a confirmed diagnosis often involves a combination of genetic testing, clinical evaluations, and medical history analysis. Genetic testing provides valuable information, but the interpretation of results and subsequent medical decisions should be discussed with healthcare professionals who can guide you through the process.",
                    ForWhom = 2
                },
                new FAQ
                {
                    Id = 9,
                    Question = "What types of genetic tests does our lab offer?",
                    Answer = "Our lab provides a diverse range of genetic tests, including panels for hereditary conditions, pharmacogenetics, and disease risk assessment. We offer comprehensive sequencing and analysis of relevant genes, empowering healthcare providers with valuable patient insights. Please note that while our tests provide essential genetic information, clinical expertise is vital for translating these findings into effective patient care.",
                    ForWhom = 3
                },
                new FAQ
                {
                    Id = 10,
                    Question = "How can our lab ensure the accuracy of genetic test results?",
                    Answer = "We prioritize accuracy through stringent quality control measures, employing advanced sequencing technologies and validated protocols. Our lab's team of skilled professionals ensures that tests are conducted with precision and attention to detail. While we strive for accurate results, the interpretation and clinical application of genetic findings require the expertise of healthcare professionals. Genetic testing offers valuable insights, but the collaboration with medical experts is essential for patient care.",
                    ForWhom = 3
                },
                new FAQ
                {
                    Id = 11,
                    Question = "Can our lab assist in the interpretation of genetic test results?",
                    Answer = "While our lab provides comprehensive genetic testing and reporting, the interpretation of results within a clinical context is best undertaken by healthcare providers. We offer clear and detailed reports outlining identified variants, but these findings should be correlated with patients' medical history and clinical presentation. Genetic testing is a valuable tool, but medical expertise is essential for translating genetic insights into actionable care decisions.",
                    ForWhom = 3
                },
                new FAQ
                {
                    Id = 12,
                    Question = "What role does our lab play in advancing personalized medicine?",
                    Answer = "Our lab plays a pivotal role in personalized medicine by providing genetic data that can inform tailored healthcare strategies. Genetic insights offer valuable information about disease risks, treatment responses, and medication management. However, it's important to remember that genetics is just one aspect of patient care. Genetic testing contributes to personalized medicine, but its successful implementation requires collaborative efforts between lab professionals and healthcare providers.",
                    ForWhom = 3
                }
            );
        #endregion

    }
    #endregion
}