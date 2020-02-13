/******************************************************************************************************************************
 
Copyright (c) 2018-2020 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public struct ILTagId
    {
        // Implicit
        public const ulong Null = 0;        // length in bytes = 0	    The NULL value.
        public const ulong Bool = 1;        // length in bytes = 1	    A boolean value.
        public const ulong Int8 = 2;        // length in bytes = 1	    8-bit unsigned int.
        public const ulong UInt8 = 3;       // length in bytes = 1	    8-bit signed int.
        public const ulong Int16 = 4;       // length in bytes = 2	    16-bit unsigned int.
        public const ulong UInt16 = 5;      // length in bytes = 2	    16-bit signed int.
        public const ulong Int32 = 6;       // length in bytes = 4	    32-bit unsigned int.
        public const ulong UInt32 = 7;      // length in bytes = 4	    32-bit signed int.
        public const ulong Int64 = 8;       // length in bytes = 8	    64-bit unsigned int.
        public const ulong UInt64 = 9;      // length in bytes = 8	    64-bit signed int.
        public const ulong ILInt = 10;      // length in bytes = 1-9    An ILInt value.
        public const ulong Binary32 = 11;   // length in bytes = 4	    32-bit floating point.
        public const ulong Binary64 = 12;   // length in bytes = 8	    64-bit floating point.
        public const ulong Binary128 = 13;  // length in bytes = 16     128-bit floating point.

        // Explicit
        public const ulong ByteArray = 16;          //                  A raw byte array.
        public const ulong String = 17;             //                  A UTF-8 encoded string.
        public const ulong BigInteger = 18;         //                  A big integer encoded in big endian format.
        public const ulong BigDecimal = 19;         //                  A big decimal number.
        public const ulong ILIntArray = 20;         //                  An array of ILint.
        public const ulong ILTagArray = 21;         //                  An array of some ILTag (all elements are of the same type)
        public const ulong Sequence = 22;           //                  A sequence of diverses ILTags
        public const ulong Range = 23;              //                  Range of ILTagId
        public const ulong Version = 24;            //                  Semantic version number
        public const ulong OID = 25;                //                  Reserved for OIDs implemented in C++ same as ILIntArray
        public const ulong Dictionary = 30;         //                  Dictionary<string,ILTag>
        public const ulong StringDictionary = 31;   //                  Dictionary<string,string>

        // Reserved for Core App up to 247 (ILInt of just one byte)

        // Core record parts
        public const ulong PubKey = 37;     //                          Public Key to verify the signature
        public const ulong Signature = 38;  //                          The signature data
        public const ulong Hash = 39;       //                          The hash data
        public const ulong PublicRSAParameters = 40;
        public const ulong RSAParameters = 41;
        public const ulong Encrypted = 42;
        public const ulong InterlockId = 43; //                         The interlock id
        public const ulong InterlockKey = 44; //                        Per Usage Verifiying Key
        public const ulong InterlockSigningKey = 45; //                 Per Usage Signing Key
        public const ulong InterlockUpdatableSigningKey = 46; //        Per Usage Evolving Signing Key
        public const ulong Hmac = 47;        //                         The hmac data
        public const ulong Certificate = 49;//                          X509 Certificate
        public const ulong SignedValue = 50; //                         SignedData+Signatures
        public const ulong IdentifiedSignature = 51;   //               The signature with signer identification
        public const ulong Reader = 52;  //                             Encrypted data reader
        public const ulong ReadingKey = 53;  //                         Encrypted data reader encrypted key
        public const ulong EncryptedTag = 54;
        public const ulong EncryptedBlob = 55;
        public const ulong InterlockKeyAppPermission = 56;

        // Metadata
        public const ulong DataField = 225;         //  DataField for Published Apps Records
        public const ulong DataIndex = 226;         //  DataIndex for Published Apps Records
        public const ulong DataModel = 224;         //  DataModel for Published Apps Records
    }
}