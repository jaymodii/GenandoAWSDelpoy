using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class notification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clinicalProcessStatus",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint", nullable: false),
                    title = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalProcessStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clinicalQuestion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalQuestion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gender",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint", nullable: false),
                    title = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "testDetail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    abbreviation = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true),
                    price = table.Column<double>(type: "float", nullable: false),
                    duration = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testDetail", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "userRole",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint", nullable: false),
                    title = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRole", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "userStatus",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint", nullable: false),
                    title = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "faq",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question = table.Column<string>(type: "varchar(124)", maxLength: 124, nullable: false),
                    answer = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false),
                    for_whom = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_FAQ_UserRoles",
                        column: x => x.for_whom,
                        principalTable: "userRole",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    last_name = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    email = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    headline = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    phone_number = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false),
                    address = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    dob = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    role = table.Column<byte>(type: "tinyint", nullable: false),
                    doctor_id = table.Column<long>(type: "bigint", nullable: true),
                    lab_id = table.Column<long>(type: "bigint", nullable: true),
                    gender = table.Column<byte>(type: "tinyint", nullable: true),
                    avatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    otp = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true),
                    expiry_time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    consultation_status = table.Column<int>(type: "int", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getutcdate())"),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_gender_gender",
                        column: x => x.gender,
                        principalTable: "gender",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_user_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_user_lab_id",
                        column: x => x.lab_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_userRole_role",
                        column: x => x.role,
                        principalTable: "userRole",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_userStatus_status",
                        column: x => x.status,
                        principalTable: "userStatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clinicalDetail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    answer = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalDetail", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicalDetail_clinicalQuestion_question_id",
                        column: x => x.question_id,
                        principalTable: "clinicalQuestion",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalDetail_user_patient_id",
                        column: x => x.patient_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "clinicalEnhancement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<long>(type: "bigint", nullable: true),
                    doctor_id = table.Column<long>(type: "bigint", nullable: true),
                    level_of_question = table.Column<byte>(type: "tinyint", nullable: false),
                    question = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    type_of_question = table.Column<byte>(type: "tinyint", nullable: false),
                    options = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    isQuestionMandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalEnhancement", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicalEnhancement_user_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalEnhancement_user_patient_id",
                        column: x => x.patient_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "clinicalProcess",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<long>(type: "bigint", nullable: false),
                    external_link = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    next_step = table.Column<byte>(type: "tinyint", nullable: false),
                    deadline = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    expected_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalProcess", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicalProcess_clinicalProcessStatus_next_step",
                        column: x => x.next_step,
                        principalTable: "clinicalProcessStatus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalProcess_clinicalProcessStatus_status",
                        column: x => x.status,
                        principalTable: "clinicalProcessStatus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalProcess_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalProcess_user_patient_id",
                        column: x => x.patient_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clinicalProcess_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sent_by = table.Column<long>(type: "bigint", nullable: false),
                    send_to = table.Column<long>(type: "bigint", nullable: false),
                    notification_message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    has_read = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_temp_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_User_SendToUser",
                        column: x => x.send_to,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_User_SentByUser",
                        column: x => x.sent_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "requestMoreInfoQuestion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<long>(type: "bigint", nullable: false),
                    question = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    answer = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestMoreInfoQuestion", x => x.id);
                    table.ForeignKey(
                        name: "FK_requestMoreInfoQuestion_user_patient_id",
                        column: x => x.patient_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "swipeActionSetting",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_Id = table.Column<long>(type: "bigint", nullable: false),
                    swipe_left_action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "Delete"),
                    swipe_right_action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "Read")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swipeActionSetting", x => x.id);
                    table.ForeignKey(
                        name: "FK_User_UserId",
                        column: x => x.user_Id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "userRefreshToken",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRefreshToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_userRefreshToken_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_userRefreshToken_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "clinicalEnhancementAnswer",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    answer = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalEnhancementAnswer", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicalEnhancementAnswer_clinicalEnhancement_question_id",
                        column: x => x.question_id,
                        principalTable: "clinicalEnhancement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clinicalEnhancementAnswer_user_patient_id",
                        column: x => x.patient_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clinicalProcessTest",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clinical_process_id = table.Column<long>(type: "bigint", nullable: false),
                    test_id = table.Column<long>(type: "bigint", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalProcessTest", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicalProcessTest_clinicalProcess_clinical_process_id",
                        column: x => x.clinical_process_id,
                        principalTable: "clinicalProcess",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clinicalProcessTest_testDetail_test_id",
                        column: x => x.test_id,
                        principalTable: "testDetail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clinicalProcessTest_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_clinicalProcessTest_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "testResult",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clinical_process_test_id = table.Column<long>(type: "bigint", nullable: false),
                    report_attachment_title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    report_attachment = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    doctor_notes = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    lab_notes = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testResult", x => x.id);
                    table.ForeignKey(
                        name: "FK_testResult_clinicalProcessTest_clinical_process_test_id",
                        column: x => x.clinical_process_test_id,
                        principalTable: "clinicalProcessTest",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_testResult_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_testResult_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "clinicalEnhancement",
                columns: new[] { "id", "created_on", "doctor_id", "isQuestionMandatory", "level_of_question", "options", "patient_id", "question", "type_of_question", "updated_on" },
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(381), new TimeSpan(0, 0, 0, 0, 0)), null, true, (byte)1, "Yes,No", null, "Do you have any existing medical conditions or chronic illnesses?", (byte)3, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(382), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "clinicalEnhancement",
                columns: new[] { "id", "created_on", "doctor_id", "level_of_question", "options", "patient_id", "question", "type_of_question", "updated_on" },
                values: new object[,]
                {
                    { 2L, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(388), new TimeSpan(0, 0, 0, 0, 0)), null, (byte)1, null, null, "Have you ever been hospitalized for any reason?", (byte)1, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(388), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3L, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(390), new TimeSpan(0, 0, 0, 0, 0)), null, (byte)1, "Blood Pressure,Indigestion,Skin Problem,Headache,Isomania", null, "What particular issues you are facing right now?", (byte)2, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(390), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "clinicalEnhancement",
                columns: new[] { "id", "created_on", "doctor_id", "isQuestionMandatory", "level_of_question", "options", "patient_id", "question", "type_of_question", "updated_on" },
                values: new object[] { 4L, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(391), new TimeSpan(0, 0, 0, 0, 0)), null, true, (byte)1, "Child,Teenager,Adult,Senior Citizen", null, "What is your age group?", (byte)4, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(392), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "clinicalEnhancement",
                columns: new[] { "id", "created_on", "doctor_id", "level_of_question", "options", "patient_id", "question", "type_of_question", "updated_on" },
                values: new object[] { 5L, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(393), new TimeSpan(0, 0, 0, 0, 0)), null, (byte)1, null, null, "Have you ever been diagnosed with mental health conditions, such as depression or anxiety?", (byte)1, new DateTimeOffset(new DateTime(2023, 10, 26, 4, 17, 49, 771, DateTimeKind.Unspecified).AddTicks(393), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "clinicalProcessStatus",
                columns: new[] { "id", "title" },
                values: new object[,]
                {
                    { (byte)1, "Initial" },
                    { (byte)2, "Clinical Path" },
                    { (byte)3, "Prescribe Test" },
                    { (byte)4, "Collect Sample" },
                    { (byte)5, "Ship Sample" },
                    { (byte)6, "Receive Sample" },
                    { (byte)7, "Sample Analysis" },
                    { (byte)8, "Send Lab Results" },
                    { (byte)9, "Publish Report" },
                    { (byte)10, "Complete" }
                });

            migrationBuilder.InsertData(
                table: "clinicalQuestion",
                columns: new[] { "id", "question" },
                values: new object[,]
                {
                    { 1, "Do you have any existing medical conditions or chronic illnesses?" },
                    { 2, "Have you ever been hospitalized for any reason? If yes, please provide details" },
                    { 3, "Are you currently taking any medications, either prescription or over-the-counter?" },
                    { 4, "Do you have any known allergies to medications, foods, or other substances?" },
                    { 5, "Have you ever been diagnosed with mental health conditions, such as depression or anxiety?" },
                    { 6, "Have you ever had any surgeries or major medical procedures in the past?" },
                    { 7, "Do you smoke or have a history of smoking? If yes, how many cigarettes per day and for how long ? " },
                    { 8, "Do you consume alcohol? If yes, how often and how much?" },
                    { 9, "Are you physically active, and do you exercise regularly ?" },
                    { 10, "Have you experienced any recent changes in weight, appetite, or energy levels ?" }
                });

            migrationBuilder.InsertData(
                table: "gender",
                columns: new[] { "id", "title" },
                values: new object[,]
                {
                    { (byte)1, "Male" },
                    { (byte)2, "Female" }
                });

            migrationBuilder.InsertData(
                table: "testDetail",
                columns: new[] { "id", "abbreviation", "description", "duration", "price", "title" },
                values: new object[,]
                {
                    { 1L, "WES", "Detailed description of the sequencing method & relevant gene coverage. Report of rare causative variants described in the literature with pathogenetic significance. Report of rare variants with frequency in the genral population (Minor Allele Frequency - MAF) < 1%. Clear variants of the results identified on the basis of international guidlines on 'best practices'. Please Note: In this case the report will include all the rare variants of the subject without any clinical interpretation, so will not be given any reference clinical & scientific literature, it will be up to the doctor who will be sent the report.'", (byte)4, 100.0, "Esoma Clinico (WES) 100X" },
                    { 2L, "WESR", "Comprehensive description of the sequencing methodology and pertinent gene coverage. In-depth analysis of rare causative variants documented in medical literature, emphasizing their pathogenic significance. Identification of infrequent variants prevalent in the general population (Minor Allele Frequency - MAF) < 1%. Definitive classification of significant variants based on globally recognized 'best practice' guidelines. Please note: The report will encompass all uncommon variants present in the individual, devoid of clinical interpretation. Subsequent evaluation based on clinical and scientific literature will be left to the healthcare professional receiving the report.", (byte)5, 150.0, "Esoma Clinico (WES) 100X e relativa interpretazione clinica" },
                    { 3L, "CGP", "Thorough elucidation of the genomic profiling methodology, encompassing a wide array of genes. Examination of somatic variants associated with cancer, including actionable mutations and potential therapeutic targets. Analysis of tumor mutational burden (TMB) and microsatellite instability (MSI) status. Comprehensive reporting of findings aligned with current clinical evidence and guidelines. Please Note: The provided report will furnish an extensive overview of genomic alterations without specific clinical correlations. Subsequent clinical interpretation is at the discretion of the healthcare provider.", (byte)3, 250.0, "Comprehensive Genomic Profiling (CGP) 200X" },
                    { 4L, "EMT", "A comprehensive overview of the exome mutation testing panel methodology, detailing the coverage of key genetic regions. Emphasis on identifying rare causative variants with significant clinical implications. Examination of low-frequency variants prevalent in the general population (Minor Allele Frequency - MAF) < 0.5%. Precise classification of identified variants following international 'best practice' guidelines. Please note: The report will encompass all rare variants identified in the individual without explicit clinical interpretation. Clinical and scientific referencing will be the responsibility of the healthcare professional receiving the report.", (byte)5, 180.0, "Exome Mutation Testing (EMT) Panel" },
                    { 5L, "PGX", "In-depth elucidation of the personalized genomic analysis panel methodology, encompassing key genetic markers. Focus on identifying genetic variants linked to drug metabolism and treatment response. Assessment of pharmacogenetic variants influencing medication efficacy and adverse reactions. Detailed reporting aligned with current clinical guidelines for personalized medicine. Note: The report provided will offer an extensive view of pharmacogenetic variants without specific clinical recommendations. Subsequent interpretation and medical decision-making rest with the healthcare provider.", (byte)4, 120.0, "Personalized Genomic Analysis (PGX) Panel" },
                    { 6L, "CEN", "Detailed overview of the Cardiovascular Exome Nexus (CEN) study methodology, covering a wide range of genes related to heart health. Identification of rare variants with potential implications for cardiovascular diseases. Analysis of low-frequency genetic variants with a focus on their impact on heart health (Minor Allele Frequency - MAF < 1%). Comprehensive classification of identified variants following established best practices. Please note: The report will encompass all rare variants found in the individual, without specific clinical interpretation. Clinical correlation and decision-making should be carried out by the healthcare professional receiving the report.", (byte)1, 300.0, "Cardiovascular Exome Nexus (CEN) Study" }
                });

            migrationBuilder.InsertData(
                table: "userRole",
                columns: new[] { "id", "title" },
                values: new object[,]
                {
                    { (byte)1, "Doctor" },
                    { (byte)2, "Patient" },
                    { (byte)3, "Lab" }
                });

            migrationBuilder.InsertData(
                table: "userStatus",
                columns: new[] { "id", "title" },
                values: new object[,]
                {
                    { (byte)1, "Active" },
                    { (byte)2, "Inactive" },
                    { (byte)3, "Deleted" }
                });

            migrationBuilder.InsertData(
                table: "faq",
                columns: new[] { "id", "answer", "for_whom", "question" },
                values: new object[,]
                {
                    { 1, "Genetic testing offers valuable insights into patients' genetic makeup, aiding in the identification of potential disease risks, hereditary conditions, and treatment responses. It enables personalized medical decisions and empowers patients to make informed choices about their health. Interpreting genetic test results involves analyzing genetic variants for their clinical significance and relevance to patients' health. Our comprehensive genetic tests provide detailed information about genetic variants, enabling tailored healthcare plans. Please note: While genetic testing provides crucial information, clinical expertise is essential for translating these insights into effective patient care.", (byte)1, "What are the key benefits of genetic testing for my patients?" },
                    { 2, "Interpreting genetic test results involves analyzing genetic variants for their clinical significance and relevance to patients' health. We provide clear reports outlining identified variants and their potential implications. However, it's crucial to consider these results within the broader clinical context. Genetic counselors and specialists can assist in understanding the implications and guiding treatment decisions. Please Note: Genetic test results provide valuable data, but they should be integrated with clinical judgment and expert advice.", (byte)1, "How do I interpret genetic test results for my patients?" },
                    { 3, "Yes, we offer genetic tests designed to assess cancer risk by identifying mutations associated with various types of cancer. These tests analyze genes linked to hereditary cancer syndromes. Detected mutations can inform early detection strategies and guide preventative measures. However, it's important to remember that genetic risk is just one factor, and regular screenings and consultations with oncologists are crucial. Please note: Genetic tests provide risk information, but comprehensive cancer care involves collaboration with specialists.", (byte)1, "Are there specific genetic tests for cancer risk assessment?" },
                    { 4, "Pharmacogenetic testing examines genetic variations influencing how patients metabolize medications. This information helps tailor medication selection and dosage for improved efficacy and reduced risk of adverse reactions. However, while these insights are valuable, they should be used alongside clinical judgment and patient history. Pharmacogenetic testing enhances personalized medicine, but ongoing monitoring and adjustments are vital. Please Note: Genetic testing informs medication decisions, but individual responses can vary and require medical expertise.", (byte)1, "How can pharmacogenetic testing improve patient medication management?" },
                    { 5, "Genetic testing provides insights into your unique genetic makeup, helping identify potential risks for certain health conditions and guiding personalized healthcare decisions. Understanding your genetic predispositions empowers you to adopt preventive measures and make informed lifestyle choices. However, it's important to remember that genetics is just one factor influencing your health. Genetic testing offers valuable information, but medical advice from healthcare professionals remains essential for holistic well-being.", (byte)2, "How can genetic testing benefit my health?" },
                    { 6, "Yes, pharmacogenetic testing examines genetic variants affecting how your body processes medications. This insight can help healthcare providers tailor medication choices and dosages for better treatment outcomes and reduced risks of adverse reactions. Nevertheless, while genetic testing offers valuable guidance, individual responses can vary. Genetic insights complement medical care, but ongoing communication with healthcare professionals is crucial for optimal medication management.", (byte)2, "Can genetic testing predict my response to medications?" },
                    { 7, "Genetic testing can indicate certain genetic predispositions to diseases, but it's important to note that genetics is just one part of the equation. Lifestyle factors, family history, and other environmental influences also play significant roles. Understanding your genetic risk empowers you to take proactive steps toward prevention, but it's essential to consult healthcare providers for comprehensive health assessments and personalized guidance.", (byte)2, "Will genetic testing tell me if I'm likely to develop a certain disease?" },
                    { 8, "Yes, genetic testing can help diagnose certain rare genetic conditions by identifying specific genetic mutations associated with those conditions. However, a confirmed diagnosis often involves a combination of genetic testing, clinical evaluations, and medical history analysis. Genetic testing provides valuable information, but the interpretation of results and subsequent medical decisions should be discussed with healthcare professionals who can guide you through the process.", (byte)2, "Can genetic testing diagnose rare conditions I might have?" },
                    { 9, "Our lab provides a diverse range of genetic tests, including panels for hereditary conditions, pharmacogenetics, and disease risk assessment. We offer comprehensive sequencing and analysis of relevant genes, empowering healthcare providers with valuable patient insights. Please note that while our tests provide essential genetic information, clinical expertise is vital for translating these findings into effective patient care.", (byte)3, "What types of genetic tests does our lab offer?" },
                    { 10, "We prioritize accuracy through stringent quality control measures, employing advanced sequencing technologies and validated protocols. Our lab's team of skilled professionals ensures that tests are conducted with precision and attention to detail. While we strive for accurate results, the interpretation and clinical application of genetic findings require the expertise of healthcare professionals. Genetic testing offers valuable insights, but the collaboration with medical experts is essential for patient care.", (byte)3, "How can our lab ensure the accuracy of genetic test results?" },
                    { 11, "While our lab provides comprehensive genetic testing and reporting, the interpretation of results within a clinical context is best undertaken by healthcare providers. We offer clear and detailed reports outlining identified variants, but these findings should be correlated with patients' medical history and clinical presentation. Genetic testing is a valuable tool, but medical expertise is essential for translating genetic insights into actionable care decisions.", (byte)3, "Can our lab assist in the interpretation of genetic test results?" },
                    { 12, "Our lab plays a pivotal role in personalized medicine by providing genetic data that can inform tailored healthcare strategies. Genetic insights offer valuable information about disease risks, treatment responses, and medication management. However, it's important to remember that genetics is just one aspect of patient care. Genetic testing contributes to personalized medicine, but its successful implementation requires collaborative efforts between lab professionals and healthcare providers.", (byte)3, "What role does our lab play in advancing personalized medicine?" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_clinicalDetail_patient_id",
                table: "clinicalDetail",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalDetail_question_id",
                table: "clinicalDetail",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalEnhancement_doctor_id",
                table: "clinicalEnhancement",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalEnhancement_patient_id",
                table: "clinicalEnhancement",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalEnhancementAnswer_patient_id",
                table: "clinicalEnhancementAnswer",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalEnhancementAnswer_question_id",
                table: "clinicalEnhancementAnswer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcess_created_by",
                table: "clinicalProcess",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcess_next_step",
                table: "clinicalProcess",
                column: "next_step");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcess_patient_id",
                table: "clinicalProcess",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcess_status",
                table: "clinicalProcess",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcess_updated_by",
                table: "clinicalProcess",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessStatus_title",
                table: "clinicalProcessStatus",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessTest_clinical_process_id",
                table: "clinicalProcessTest",
                column: "clinical_process_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessTest_created_by",
                table: "clinicalProcessTest",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessTest_id",
                table: "clinicalProcessTest",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessTest_test_id",
                table: "clinicalProcessTest",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalProcessTest_updated_by",
                table: "clinicalProcessTest",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_clinicalQuestion_question",
                table: "clinicalQuestion",
                column: "question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_faq_for_whom",
                table: "faq",
                column: "for_whom");

            migrationBuilder.CreateIndex(
                name: "IX_faq_question",
                table: "faq",
                column: "question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gender_title",
                table: "gender",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_send_to",
                table: "notification",
                column: "send_to");

            migrationBuilder.CreateIndex(
                name: "IX_notification_sent_by",
                table: "notification",
                column: "sent_by");

            migrationBuilder.CreateIndex(
                name: "IX_requestMoreInfoQuestion_patient_id",
                table: "requestMoreInfoQuestion",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_swipeActionSetting_user_Id",
                table: "swipeActionSetting",
                column: "user_Id");

            migrationBuilder.CreateIndex(
                name: "IX_testResult_clinical_process_test_id",
                table: "testResult",
                column: "clinical_process_test_id");

            migrationBuilder.CreateIndex(
                name: "IX_testResult_created_by",
                table: "testResult",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_testResult_updated_by",
                table: "testResult",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_created_by",
                table: "user",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_doctor_id",
                table: "user",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_gender",
                table: "user",
                column: "gender");

            migrationBuilder.CreateIndex(
                name: "IX_user_lab_id",
                table: "user",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role",
                table: "user",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_user_status",
                table: "user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_user_updated_by",
                table: "user",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_userRefreshToken_created_by",
                table: "userRefreshToken",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_userRefreshToken_id",
                table: "userRefreshToken",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_userRefreshToken_updated_by",
                table: "userRefreshToken",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_userRole_title",
                table: "userRole",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userStatus_title",
                table: "userStatus",
                column: "title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clinicalDetail");

            migrationBuilder.DropTable(
                name: "clinicalEnhancementAnswer");

            migrationBuilder.DropTable(
                name: "faq");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "requestMoreInfoQuestion");

            migrationBuilder.DropTable(
                name: "swipeActionSetting");

            migrationBuilder.DropTable(
                name: "testResult");

            migrationBuilder.DropTable(
                name: "userRefreshToken");

            migrationBuilder.DropTable(
                name: "clinicalQuestion");

            migrationBuilder.DropTable(
                name: "clinicalEnhancement");

            migrationBuilder.DropTable(
                name: "clinicalProcessTest");

            migrationBuilder.DropTable(
                name: "clinicalProcess");

            migrationBuilder.DropTable(
                name: "testDetail");

            migrationBuilder.DropTable(
                name: "clinicalProcessStatus");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "gender");

            migrationBuilder.DropTable(
                name: "userRole");

            migrationBuilder.DropTable(
                name: "userStatus");
        }
    }
}
