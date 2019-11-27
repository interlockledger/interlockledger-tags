/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IJsonCustom<T>
    {
        string TextualRepresentation { get; }

        T ResolveFrom(string textualRepresentation);
    }
}