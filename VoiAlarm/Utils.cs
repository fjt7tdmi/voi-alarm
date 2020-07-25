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

using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using VoiAlarm.Models;

namespace VoiAlarm
{
    internal class Utils
    {
        // internalName => displayName
        private static readonly IDictionary<string, string> playerTypeDisplayNames = new Dictionary<string, string>();

        static Utils()
        {
            playerTypeDisplayNames.Add("Cevio", "CeVIO");
            playerTypeDisplayNames.Add("BouyomiChan", "棒読みちゃん");
        }

        public static string GetDisplayNameForVoice(string playerType, string voiceType)
        {
            if (playerType == null || !playerTypeDisplayNames.ContainsKey(playerType))
            {
                return string.Empty;
            }

            return $"{playerTypeDisplayNames[playerType]} - {voiceType}";
        }

        public static string GetDisplayName(FilePlayMode mode)
        {
            switch (mode)
            {
                case FilePlayMode.None:
                    return "ファイル再生なし";
                case FilePlayMode.BeforeVoice:
                    return "ボイスの前";
                default:
                    return string.Empty;
            }
        }

        public static FilePlayMode GetFilePlayMode(string displayName)
        {
            if (displayName == GetDisplayName(FilePlayMode.BeforeVoice))
            {
                return FilePlayMode.BeforeVoice;
            }
            else
            {
                return FilePlayMode.None;
            }
        }
    }
}
