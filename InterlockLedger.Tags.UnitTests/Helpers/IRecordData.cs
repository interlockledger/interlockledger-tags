/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;

namespace InterlockLedger.Tags
{
    public interface IRecordData<T> : IVersion where T : IRecordData<T>, new()
    {
        object AsJson { get; }
        Payload<T> AsPayload { get; }

        T FromStream(Stream s);

        void ToStream(Stream s);
    }
}
