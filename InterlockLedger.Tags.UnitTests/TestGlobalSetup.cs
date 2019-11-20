/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [SetUpFixture]
    public class TestGlobalSetup
    {
        [OneTimeSetUp]
        public void OneTimeSetup() => BaseKeyId.RegisterKeyIdTypes();
    }
}