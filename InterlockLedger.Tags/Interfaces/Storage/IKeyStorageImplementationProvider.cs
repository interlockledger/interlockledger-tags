/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IKeyStorageImplementationProvider : IKeyStorageProvider
    {
        string Name { get; }
        bool SupportsKeyCreation { get; }
        bool SupportsCertificateImport { get; }

        bool SupportsAlgorithm(Algorithm algorithm);
    }
}