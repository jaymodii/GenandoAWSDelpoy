namespace Common.Constants;
public class SystemConstants
{
    private SystemConstants() { }

    #region Basic Configuration

    public static readonly string CorsPolicy = "GenandoCors";

    public static readonly int MaxPageSizeResponse = 50;

    public static readonly string DefaultAvatarPath = "assets/images/profile.png";

    public const string DEFAULT_DATETIME = "(getutcdate())";

    public const string CONNECTION_STRING_NAME = "DefaultConnection";

    public static readonly int PasswordIteration = 5;

    #endregion Besic Configuration

    #region Policy Attribute

    public const string DoctorPolicy = "Doctor";

    public const string PatientPolicy = "Patient";

    public const string LabUserPolicy = "LabUser";

    public const string AllUserPolicy = "AllUser";

    #endregion Policy Attribute

    #region Gender

    public const string Male = "Male";

    public const string Female = "Female";

    #endregion


    #region Session Constant

    public const string LoggedUser = "LoggedUser";

    public const string Bearer = "Bearer ";

    public const string RememeberMeCookieKey = "rememberMe";

    public const string TrueString = "True";

    #endregion Session Constant

    #region Claim Type

    public const string UserIdClaim = "UserId";

    public const string LabIdClaim = "LabId";

    public const string AvatarClaim = "Avatar";

    #endregion Claim Type

    #region Role Strings
    public static readonly string PatientString = "Patient";

    public static readonly string LabString = "Lab";

    public static readonly string DoctorString = "Doctor";
    #endregion Role Strings

    #region Clinical Process status cycle

    public static readonly byte DefaultClinicalStatus = 0;

    public static readonly byte InitialStatus = 1;

    public static readonly byte ClinicalPathStatus = 2;

    public static readonly byte PrescribeTestStatus = 3;

    public static readonly byte CollectSampleStatus = 4;

    public static readonly byte ShipSampleStatus = 5;

    public static readonly byte RecieveSampleStatus = 6;

    public static readonly byte SampleAnalysisStatus = 7;

    public static readonly byte SendLabResultStatus = 8;

    public static readonly byte PublishReportStatus = 9;

    public static readonly byte CompleteStatus = 10;

    #endregion Clinical Process status cycle

    #region Otp

    public const string AuthenticationOtp = "AuthenticationOtp";

    public const string ProfileUpdateOtp = "ProfileUpdateOtp";

    #endregion

    #region Others

    public const string ZeroString = "0";

    public static readonly int NumberOfStaticQuestions = 10;

    #endregion Others
}