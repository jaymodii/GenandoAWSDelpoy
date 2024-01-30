using Common.Constants;

namespace Common.Exceptions;
public class DbOperationFailedException : Exception
{
    public DbOperationFailedException()
        : base(MessageConstants.DB_OPERATION_FAILED)
    { }
}
