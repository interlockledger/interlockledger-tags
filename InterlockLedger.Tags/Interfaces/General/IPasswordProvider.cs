/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IPasswordProvider
    {
        string PasswordFor(InterlockId id);
        OwnerId OwnerId { get; }
    }
}