namespace Common.Constants;

public class MessageConstants
{
    private MessageConstants()
    { }

    #region Success Message

    public const string GlobalSuccess = "Success";

    public const string GlobalCreated = "Resource created successfully.";

    public static readonly string AccountCreated = "Account created successfully.";

    public static readonly string LoginSuccess = "You are logged in !!";

    public static readonly string MailSent = "Mail has been sent to your registered email.";

    public static readonly string ProfileUpdated = "Profile updated successfully.";

    public static readonly string ProfileDeleted = "Profile deleted successfully.";

    public static readonly string PasswordReset = "Your Password reset successfully.";

    public static readonly string QuestionSavedAsDraft = "Changes are saved successfully.";

    public static readonly string QuestionSendToPatient = "Questions are sent to patient.";

    public static readonly string InfoSentToPatient = "Requested information is sent to your doctor.";

    public static readonly string DoctorResult = "Result Published Successfully.";

     public static readonly string PrescribeTest = "Test prescribed Successfully.";

     public static readonly string PostQuestion = "Questions Posted Successfully.";

    public static readonly string AnswerUpdated = "Answer updated successfully.";

    public static readonly string CollectSample = "Sample has been collected.";

    public static readonly string ShipSample = "Sample shipped.";

    public static readonly string ReceiveSample = "Sample Received.";
    
    public static readonly string ResultUpload = "Results upload successfully.";

    public static readonly string OtpVerified = "OTP verified successfully.";

    #endregion Success Message

    #region Exception Messages

    public static readonly string DEFAULT_MODELSTATE = "Model state is invalid!";

    public static readonly string DB_OPERATION_FAILED = "Something went wrong during db operation!";

    public static readonly string VALIDATION_ERROR = "One or more validation failures have occured!";

    public const string RESOURCE_NOT_FOUND = "Resource not found!";

    public const string CLINICAL_PROFILE_NOT_FOUND = "Patient Clinical Profile Not Found!";

    public const string UNAUTHERIZE = "Unautherize access!";

    public const string TOKEN_EXPIRE = "Your session has been expired!";

    public const string INVALID_TOKEN = "Invalid Token!";

    public const string INVALID_ATTEMPT = "Invalid Attempt!";

    public static readonly string USER_RESOURCE_NOT_FOUND = "User with ID # not found!";

    public static readonly string RECORD_NOT_FOUND = "Record not found!";

    public static readonly string RECORD_ALREADY_EXIST = "Record already exist!";

    public static readonly string FORBID_USER_DELETE = "User cannot be deleted.";

    public static readonly string MINIMUM_TWO_OPTIONS_REQUIRED = "Mininum 2 options are required";

    public static readonly string PATIENT_ID_REQUIRED = "Patient Id cannot be null or 0!";

    public static readonly string INVALID_QUESTION = "Question is Invalid!";

    public static readonly string MANDATORY_ANSWER = "Mandatory question requires an answer!";
    
    public static readonly string TEST_ALREADY_PRESCRIBED = "Test already prescribed!";

    public static readonly string SAMPLE_NOT_RECEIVED = "Sample not received!";

    public static readonly string PATIENT_ANSWERED = "Patient has already completed clinical path , please go for request more info.";

    public static readonly string CANT_GIVE_ANS_TEST_ALREADY_PRESCRIBE = "You can't submit answer now because doctor has already prescribe test.";
    
    public static readonly string USER_NOT_FOUND = "User not found.";

    #endregion Exception Messages

    #region Validation Messages


    public static readonly string PasswordRegExFailed
        = "Password should have at least 8 character, 1 uppercase, 1 lowercase and 1 symbol.";

    public static readonly string LinkRegExFailed = "Invalid http link.";

    public static readonly string EmailRegExFailed = "Invalid email address.";

    public static readonly string EmailAlreadyExists = "User already exists. Please try with other email.";

    public static readonly string InvalidLoginCredential = "Invalid email/username or password.";

    public static readonly string Invalidotp = "Invalid OTP";

    public static readonly string NameRegExFailed = "should contain alphabets only. No space or special characters are allowed.";

    public static readonly string PhoneNumberLength = $"Phone Number should be of {ValidationConstants.PhoneNumberLength} digit.";

    public static readonly string PhoneNumberRegExFailed = "Phone number should only contain digits.";

    public static readonly string LessThanCurrentDate = "should be smaller than today date.";

    public static readonly string InvalidId = "Invalid # ID.";

    public static readonly string PasswordMissMatch = "Password and confirmPassword is not same";
    
    public static readonly string DeleteUserError
        = "Cannot delete user since user in clinical process.";

    public static readonly string DeleteLabError
        = "Cannot delete lab user since it is associated with patients.";

    public static readonly string PatientQuestionStatus = "Invalid question status type.";

    public static readonly string IdValueError = "Invalid ID value.";

    public static readonly string PublishMoreInfoError = "All answers must be provided before publish.";

    public static readonly string DraftMoreInfoError = "Please provide answer to draft.";

    public static readonly string NoQuestionsProvided = "Please provide at least 1 Question!";
    
    public static readonly string LabUserNotFound = "Please register lab user to proceed.";

    public static readonly string InvalidDateOfBirth = "Invalid Date Of Birth.";

    #endregion Validation Messages

    #region Notification Message

    public static readonly string ProfileUodatedNoti = "Your Profile has been updated!";

    public static readonly string DeleteQuestion = "Question has been deleted!";

    #endregion Notification Message

    #region Mail Subjects

    public static readonly string NewAccountNotification = "Welcome To Genando";

    #endregion
}

public class MailConstants
{
    public MailConstants()
    { }

    #region Subject

    public const string GenericSubject = "Genando || noreply email";

    public const string OtpSubject = "Genando || Verification OTP || No Reply";

    public const string ResetPasswordSubject = "Genando || ResetPassword || No Reply";

    public const string TestReportMailSubject = "Genando || Test Report || No Reply";

    public const string UpdatedPatientDetailsMailSubject = "Genando || Updated Personal Information || No Reply";

    #endregion Subject
}