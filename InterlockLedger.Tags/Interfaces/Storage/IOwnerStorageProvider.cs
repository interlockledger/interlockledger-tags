/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IOwnerStorageProvider
    {
        IEnumerable<IOwnerData> Owners { get; }
        IEnumerable<Owner> ResolvedOwners { get; }

        Owner Create(string name, string password, string description, string email, Algorithm algorithm, KeyStrength strength, bool silent = true);

        Owner Resolve(string name, string password = null);

        Owner ResolveOrDummy(string name);

        Owner ResolveWithPassword(string name, ref string password);
    }
}