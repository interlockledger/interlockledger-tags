/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;

namespace InterlockLedger.Tags
{
    public interface IStreamSerializer<T>
    {
        T Deserialize(Stream s);

        Stream Serialize(Stream s, T value);
    }
}