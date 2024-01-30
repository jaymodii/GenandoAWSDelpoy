using Common.Constants;

namespace Common.Enums
{
    public class EntityStatus
    {
        public enum UserStatusTypes
        {
            ACTIVE = EntityStatusConstants.ACTIVE,
            INACTIVE = EntityStatusConstants.INACTIVE,
            DELETED = EntityStatusConstants.DELETED
        }
    }
}
