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
using System.IO;
using System.Reflection;

namespace VoiAlarm.Models
{
    internal class CevioPlayer
    {
        private const string dllName = "CeVIO.Talk.RemoteService";

        private readonly SettingsManager settingsManager;
        private readonly Assembly assembly;

        public IList<string> VoiceTypes { get; } = new List<string>();

        public CevioPlayer(SettingsManager settingsManager)
        {            
            this.settingsManager = settingsManager;

            try
            {
                assembly = Assembly.LoadWithPartialName(dllName);
            }
            catch (IOException)
            {
                // CeVIO がインストールされていない場合
                Trace.WriteLine($"Failed to load {dllName}.dll");
                return;
            }

            if (settingsManager.Value.AutoStartCevio)
            {
                CevioStartHost();
            }

            if (IsAvailable())
            {
                VoiceTypes = CevioAvailableCasts();
            }
        }

        public bool IsAvailable()
        {
            if (assembly == null)
            {
                return false;
            }

            return CevioIsHostStarted();
        }

        public void Play(PlayerCommand command)
        {
            Trace.WriteLine($"{nameof(CevioPlayer)}.{nameof(Play)} {command.VoiceType} (message length: {command.Message.Length})");

            if (assembly == null)
            {
                return;
            }

            CevioSpeak(command.VoiceType, command.Message);
        }

        private void CevioStartHost()
        {
            // CeVIO.Talk.RemoteService.ServiceControl.StartHost(false)
            var type = assembly.GetType("CeVIO.Talk.RemoteService.ServiceControl");
            var method = type.GetMethod("StartHost");

            method.Invoke(null, new object[] { false });
        }

        private IList<string> CevioAvailableCasts()
        {
            // CeVIO.Talk.RemoteService.TalkerAgent.AvailableCasts
            var type = assembly.GetType("CeVIO.Talk.RemoteService.TalkerAgent");
            var property = type.GetProperty("AvailableCasts");
            
            return (IList<string>)property.GetValue(null);
        }

        private bool CevioIsHostStarted()
        {
            // CeVIO.Talk.RemoteService.ServiceControl.IsHostStarted
            var type = assembly.GetType("CeVIO.Talk.RemoteService.ServiceControl");
            var property = type.GetProperty("IsHostStarted");

            return (bool)property.GetValue(null);
        }
        
        private void CevioSpeak(string cast, string text)
        {
            // var talker = new CeVIO.Talk.RemoteService.Talker { Cast = cast };
            // talker.Speak(text);
            var talkerType = assembly.GetType("CeVIO.Talk.RemoteService.Talker");
            var speakMethod = talkerType.GetMethod("Speak");

            var talkerInstance = Activator.CreateInstance(talkerType, new object[] { cast });
            speakMethod.Invoke(talkerInstance, new object[] { text });
        }
    }
}
