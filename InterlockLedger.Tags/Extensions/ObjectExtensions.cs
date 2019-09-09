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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace InterlockLedger.Tags
{
    public static class ObjectExtensions
    {
        public static object AsNavigable(this object value) {
            Type valueType = value.GetType();
            if (valueType.IsPrimitive || valueType == typeof(string)) {
                return value;
            }
            if (value is System.Collections.IEnumerable list) {
                var newList = new List<object>();
                foreach (object listElement in list) {
                    newList.Add(AsNavigable(listElement));
                }
                return newList;
            }
            if (value is JObject jo) {
                var jdictionary = new Dictionary<string, object>();
                foreach (var p in jo.Properties()) {
                    jdictionary[p.Name] = AsNavigable(p.Value);
                }
                return jdictionary;
            }
            var dictionary = new Dictionary<string, object>();
            foreach (var p in valueType.GetProperties()) {
                dictionary[p.Name] = AsNavigable(p.GetValue(value, null));
            }
            return dictionary;
        }

        public static string PadLeft(this object value, int totalWidth) => value.ToString().PadLeft(totalWidth);

        public static string PadRight(this object value, int totalWidth) => value.ToString().PadRight(totalWidth);

        public static string WithDefault(this object value, string @default) => StringExtensions.WithDefault(value?.ToString(), @default);
    }
}