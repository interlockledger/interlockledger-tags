/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IKeyParameters
    {
        TagPubKey PublicKey { get; }
        byte[] EncodedBytes { get; }
    }
}