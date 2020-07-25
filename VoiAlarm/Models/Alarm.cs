/*
 * Copyright (c) Akifumi Fujita
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Xml.Serialization;

namespace VoiAlarm.Models
{
    public class Alarm
    {
        [XmlElement("time")]
        public string Time { get; set; }

        [XmlElement("monday")]
        public bool IsEnabledMonday { get; set; }

        [XmlElement("tuesday")]
        public bool IsEnabledTuesday { get; set; }

        [XmlElement("wednesday")]
        public bool IsEnabledWednesday { get; set; }

        [XmlElement("thursday")]
        public bool IsEnabledThursday { get; set; }

        [XmlElement("friday")]
        public bool IsEnabledFriday { get; set; }

        [XmlElement("saturday")]
        public bool IsEnabledSaturday { get; set; }

        [XmlElement("sunday")]
        public bool IsEnabledSunday { get; set; }

        [XmlElement("playerType")]
        public string PlayerType { get; set; }

        [XmlElement("voiceType")]
        public string VoiceType { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("playFileMode")]
        public FilePlayMode FilePlayMode { get; set; }

        [XmlElement("playFilePath")]
        public string FilePath { get; set; }

        public Alarm Clone()
        {
            return new Alarm()
            {
                Time = Time,
                IsEnabledMonday = IsEnabledMonday,
                IsEnabledTuesday = IsEnabledTuesday,
                IsEnabledWednesday = IsEnabledWednesday,
                IsEnabledThursday = IsEnabledThursday,
                IsEnabledFriday = IsEnabledFriday,
                IsEnabledSaturday = IsEnabledSaturday,
                IsEnabledSunday = IsEnabledSunday,
                PlayerType = PlayerType,
                VoiceType = VoiceType,
                Message = Message,
                FilePlayMode = FilePlayMode,
                FilePath = FilePath,
            };
        }
    }
}
