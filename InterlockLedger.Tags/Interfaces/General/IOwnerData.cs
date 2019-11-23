/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IOwnerData
    {
        string Description { get; }
        string Email { get; }
        string Name { get; }
        Algorithm Algorithm { get; }
        KeyStrength Strength { get; }
        BaseKeyId Id { get; }
    }
}