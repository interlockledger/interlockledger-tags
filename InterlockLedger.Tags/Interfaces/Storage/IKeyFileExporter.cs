/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IKeyFileExporter
    {
        ExportedKeyFile ExportKeyFile(string name);
    }
}