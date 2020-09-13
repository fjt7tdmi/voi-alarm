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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CeVIO.Talk.RemoteService;

namespace VoiAlarm.Models
{
    internal class CevioPlayer
    {
        private readonly SettingsManager settingsManager;

        public IList<string> VoiceTypes { get; } = new List<string>();

        public CevioPlayer(SettingsManager settingsManager)
        {
            this.settingsManager = settingsManager;

            try
            {
                if (settingsManager.Value.AutoStartCevio)
                {
                    ServiceControl.StartHost(false);
                }
                if (IsAvailable())
                {
                    VoiceTypes = TalkerAgent.AvailableCasts;
                }
            }
            catch (Exception)
            {
                // 外部ツールの状態確認は他のビューやダイアログに任せるため、ここでは CeVIO が起動できなくても無視
            }
        }

        public bool IsAvailable()
        {
            return ServiceControl.IsHostStarted;
        }

        public void Play(PlayerCommand command)
        {
            Trace.WriteLine($"{nameof(CevioPlayer)}.{nameof(Play)} {command.VoiceType} (message length: {command.Message.Length})");

            var talker = new Talker { Cast = command.VoiceType };
            talker.Speak(command.Message);
        }
    }
}
