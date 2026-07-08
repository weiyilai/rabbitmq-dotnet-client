// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 2.0.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2026 Broadcom. All Rights Reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v2.0:
//
//---------------------------------------------------------------------------
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
//  Copyright (c) 2007-2026 Broadcom. All Rights Reserved.
//---------------------------------------------------------------------------

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Xunit;

namespace Test.Unit
{
    public class TestWriteShortstr
    {
        [Fact]
        public void TestWriteShortstrAtMaxLengthSucceeds()
        {
            string val = new string('a', byte.MaxValue);
            byte[] bytes = new byte[byte.MaxValue + 1];

            int written = WireFormatting.WriteShortstr(ref bytes.GetStart(), val);

            Assert.Equal(byte.MaxValue + 1, written);
            int read = WireFormatting.ReadShortstr(bytes, out string actual);
            Assert.Equal(byte.MaxValue + 1, read);
            Assert.Equal(val, actual);
        }

        [Fact]
        public void TestWriteShortstrTooLongThrowsClearException()
        {
            string val = new string('a', byte.MaxValue + 1);
            byte[] bytes = new byte[byte.MaxValue + 2];

            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => WireFormatting.WriteShortstr(ref bytes.GetStart(), val));

            Assert.Contains("maximum allowed length of 255 bytes", ex.Message);
            Assert.Contains("256", ex.Message);
        }

        [Fact]
        public void TestWriteShortstrCountsBytesNotChars()
        {
            // Each of these characters encodes to two UTF-8 bytes, so 200 characters
            // is 400 bytes, which exceeds the shortstr maximum even though the string
            // is well under 255 characters long.
            string val = new string('é', 200);
            byte[] bytes = new byte[512];

            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => WireFormatting.WriteShortstr(ref bytes.GetStart(), val));

            Assert.Contains("maximum allowed length of 255 bytes", ex.Message);
            Assert.Contains("400", ex.Message);
        }
    }
}
