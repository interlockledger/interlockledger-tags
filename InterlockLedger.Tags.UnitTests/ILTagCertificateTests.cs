/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
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

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class ILTagCertificateTests
    {
        [Test]
        public void GuaranteeBijectiveBehavior() {
            var x509 = new X509Certificate2(Convert.FromBase64String(_certificateInBase64));
            var tag = new TagCertificate(x509);
            var encodedBytes = tag.EncodedBytes;
            using var ms = new MemoryStream(encodedBytes);
            var retag = ms.Decode<TagCertificate>();
            Assert.AreEqual(x509, retag.Value);
            Assert.AreEqual(x509.ToString(), retag.Value.ToString());
        }

        private const string _certificateInBase64 = "MIIENTCCAx2gAwIBAgIJAJs7yUDZRzUKMA0GCSqGSIb3DQEBCwUAMGAxLTArBgNVBAMMJEJpdGRlZmVuZGVyIFBlcnNvbmFsIENBLmF2ZnJlZTAwMDAwMDEMMAoGA1UECwwDSURTMRQwEgYDVQQKDAtCaXRkZWZlbmRlcjELMAkGA1UEBhMCVVMwHhcNMTkwODI4MTMyNjQ2WhcNMTkxMTI2MTMyNjQ2WjAiMSAwHgYDVQQDExdpbnRlcmxvY2tsZWRnZXIubmV0d29yazCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKvxaV/dLYivduXUwAizA86l1sdoEoJarGCWzkJgseiudcFeSSd+VfuCDY2jwvLRRWs0aDWjYUtzWZi0i17L5eA7aXJHqQRnWtlfiDNTbapcdFbe4uV2pQdkUzyPI5rGrIs6oYBTg+wXF6hdk9oblNsspxWwjkPOThg74m0m+IKUvIih8vlxQIkAKExnOYtYfRdqTjDH6aUxmxc2DMXCdD5sKiwhU2QRKqgJcCl8dFQqpGfpzUTk1YjKC2edLCy9vgyf9F5sqYpwPKf6UxQ7rV2PCQUeyNjuWCG/skdJ4tmZ92MUBW+1tJGj8bbooCosda8ThBoLzDE1A97tJtS8XrcCAwEAAaOCAS4wggEqMIIBJgYDVR0RBIIBHTCCARmCJGF1dG9kaXNjb3Zlci5pbnRlcmxvY2tsZWRnZXIubmV0d29ya4IeY3BhbmVsLmludGVybG9ja2xlZGdlci5uZXR3b3JrgitpbnRlcmxvY2tsZWRnZXItbmV0d29yay5pbnRlcmxvY2tyZWNvcmQuY29tghdpbnRlcmxvY2tsZWRnZXIubmV0d29ya4IcbWFpbC5pbnRlcmxvY2tsZWRnZXIubmV0d29ya4Ifd2ViZGlzay5pbnRlcmxvY2tsZWRnZXIubmV0d29ya4Ivd3d3LmludGVybG9ja2xlZGdlci1uZXR3b3JrLmludGVybG9ja3JlY29yZC5jb22CG3d3dy5pbnRlcmxvY2tsZWRnZXIubmV0d29yazANBgkqhkiG9w0BAQsFAAOCAQEAfY0XybYZCctyDAS/MxWNH/gNtdKPHlearMQgYbbIxj0QxYPbHvrpMQjqDD5AEJpcPt0cOmEcBw0rx0qzG6lBJKKVlL3XwUpjueP5Vh83CLLqMgvrnIMEddPpIMmahe11NMYtWhxjGLkucjhRCoo0JQYVfrWVMsrcYdyafO0ThSwLIXZfC4bm9e++lSoYlblBIvucO7rIbxsfId2RY0XcyCzc96HFqUHm8nZFAYVDc3HURj+2wFtsgPmRJou0/CwJcyWPo9T8E1amzUw0myDeaztdUwczlrUBJkLs7Gstcgrpb+mLRp/mhgP1r7QsiLL+aiEFBeuPCaFljhnc/zQx3A==";
    }
}