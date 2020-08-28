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

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class FileBackedByteArrayTests
    {
        [Test]
        public void FromPartialFileToStream() {
            var fi = "unit".TempFileInfo();
            try {
                var arrayBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
                int bytesLength = arrayBytes.Length;
                var prefixedArrayBytes = arrayBytes.Prepend((byte)bytesLength).Prepend((byte)ILTagId.ByteArray).ToArray();
                using (var fos = fi.OpenWrite()) {
                    fos.Write(arrayBytes);
                    fos.Write(arrayBytes);
                }
                var fbba = new FileBackedByteArray(fi, bytesLength, (ulong)bytesLength);
                Assert.IsNotNull(fbba);
                Assert.AreEqual($"{fi.FullName}[{bytesLength}:{bytesLength}]", fbba.Formatted);
                Assert.AreEqual(bytesLength, fbba.Length);
                Assert.AreEqual(bytesLength, fbba.Offset);
                Assert.AreEqual(ILTagId.ByteArray, fbba.TagId);
                Assert.IsNull(fbba.Value);
                Assert.IsTrue(fbba.NoRemoval, "Backing file should not be removable");
                using var mso = SerializeInto(fbba, prefixedArrayBytes);
                mso.Position = 0;
                var tagArray = ILTag.DeserializeFrom(mso);
                Assert.IsInstanceOf<ILTagByteArray>(tagArray);
                CollectionAssert.AreEqual(prefixedArrayBytes, tagArray.EncodedBytes);
                fbba.Remove();
                Assert.IsTrue(File.Exists(fi.FullName), "Backing file should not have been deleted by calling Remove()");
            } finally {
                fi.Delete();
            }
        }

        [Test]
        public void FromStreamToStream() {
            var fi = "unit".TempFileInfo();
            try {
                var arrayBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                int bytesLength = arrayBytes.Length;
                var prefixedArrayBytes = arrayBytes.Prepend((byte)bytesLength).Prepend((byte)ILTagId.ByteArray).ToArray();
                using var ms = new MemoryStream(arrayBytes);
                var fbba = new FileBackedByteArray(fi, ms);
                Assert.IsNotNull(fbba);
                Assert.AreEqual($"{fi.FullName}[0:{bytesLength}]", fbba.Formatted);
                Assert.AreEqual(bytesLength, fbba.Length);
                Assert.AreEqual(0, fbba.Offset);
                Assert.AreEqual(ILTagId.ByteArray, fbba.TagId);
                Assert.IsFalse(fbba.NoRemoval, "Backing file should be removable");
                using var mso = SerializeInto(fbba, prefixedArrayBytes);
                mso.Position = 0;
                var tagArray = ILTag.DeserializeFrom(mso);
                Assert.IsInstanceOf<ILTagByteArray>(tagArray);
                CollectionAssert.AreEqual(prefixedArrayBytes, tagArray.EncodedBytes);
                var fbba2 = new FileBackedByteArray(fi);
                CollectionAssert.AreEqual(prefixedArrayBytes, fbba2.EncodedBytes);
                Assert.IsFalse(fbba2.NoRemoval, "Backing file should be removable");
                Assert.AreEqual(arrayBytes.Length, fbba2.Length);
                using (var s = fbba2.ReadingStream) {
                    var bytes = s.ReadAllBytesAsync().Result;
                    Assert.IsNotNull(bytes);
                    CollectionAssert.AreEqual(arrayBytes, bytes);
                }
                fbba2.Remove();
                Assert.IsFalse(File.Exists(fi.FullName), "Backing file should have been deleted by calling Remove()");
                Assert.AreEqual(0ul, fbba2.Length);
            } finally {
                fi.Delete();
            }
        }

        [Test]
        public void GetWritingStream() {
            var fi = "unit".TempFileInfo();
            try {
                byte[] bytes = "This is just a test".UTF8Bytes();
                FileBackedByteArray fbba = null;
                using (var fs = fi.GetWritingStream(it => fbba = new FileBackedByteArray(it))) {
                    Assert.IsNull(fbba);
                    fs.WriteBytes(bytes);
                    Assert.IsTrue(fi.Exists, "Temp file was not created");
                }
                Assert.IsNotNull(fbba);
                Assert.AreEqual(bytes.Length, fbba.Length);
                Assert.AreEqual(0L, fbba.Offset);
                using (var s = fbba.ReadingStream) {
                    var readBytes = s.ReadAllBytesAsync().Result;
                    CollectionAssert.AreEqual(bytes, readBytes);
                }
                var prefixedArrayBytes = bytes.Prepend((byte)bytes.Length).Prepend((byte)ILTagId.ByteArray).ToArray();
                using var mso = SerializeInto(fbba, prefixedArrayBytes);
            } finally {
                fi.Delete();
            }
        }

        private static MemoryStream SerializeInto(FileBackedByteArray fbba, byte[] prefixedArrayBytes) {
            var mso = new MemoryStream();
            _ = fbba.SerializeInto(mso);
            var outputBytes = mso.ToArray();
            Assert.IsNotNull(outputBytes);
            CollectionAssert.AreEqual(prefixedArrayBytes, outputBytes);
            return mso;
        }
    }
}