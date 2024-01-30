namespace Common.Constants;

public class ValidationConstants
{
    private ValidationConstants()
    { }

    #region Patterns

    public static readonly string PasswordRegEx = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{7,15}$";

    public static readonly string EmailRegEx = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,3}$";

    public static readonly string NameRegex = "^[a-zA-Z]+$";

    public static readonly string PhoneNumberRegEx = @"^[0-9]+";

    public static readonly string LinkRegEx = @"^(https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?$";


    #endregion Patterns

    #region Constants

    #region User details constants

    public static readonly int MinNameLength = 2;

    public static readonly int MaxNameLength = 16;

    public static readonly int MinEmailLength = 5;

    public static readonly int MaxEmailLength = 255;

    public static readonly int MinPasswordLength = 6;

    public static readonly int MaxPasswordLength = 20;

    public static readonly int Min3Length = 3;

    public static readonly int Max16Length = 16;

    public static readonly int PhoneNumberLength = 10;

    public static readonly int MinAddressLength = 10;

    public static readonly int MaxAddressLength = 512;

    public static readonly int MinQuestionLength = 5;

    public static readonly int MaxQuestionLength = 255;

    public static readonly int Max1024Length = 1024;
    #endregion User details constants

    #endregion Constants
}