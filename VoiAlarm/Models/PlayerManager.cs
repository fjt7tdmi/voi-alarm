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
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;

namespace VoiAlarm.Models
{
    internal class PlayerCommand
    {
        public string VoiceType { get; }
        public string Message { get; }

        public PlayerCommand(string voiceType, string message)
        {
            VoiceType = voiceType;
            Message = message;
        }
    }

    internal class PlayerManager
    {
        private readonly CevioPlayer cevioPlayer;
        private readonly BouyomiChanPlayer bouyomiChanPlayer;

        // List of (playerType, voiceType)
        public IList<(string, string)> PlayerVoices { get; } = new List<(string, string)>();

        public PlayerManager(SettingsManager settingsManager)
        {
            lock (settingsManager.LockGuard)
            {
                cevioPlayer = new CevioPlayer(settingsManager);
                bouyomiChanPlayer = new BouyomiChanPlayer(settingsManager.Value);
            }

            // Initialize PlayerVoices
            foreach (var voiceType in cevioPlayer.VoiceTypes)
            {
                PlayerVoices.Add(("Cevio", voiceType));
            }
            PlayerVoices.Add(("BouyomiChan", "デフォルト"));
        }

        public bool IsAvailable(string playerType)
        {
            switch (playerType)
            {
                case "Cevio":
                    return cevioPlayer.IsAvailable();
                case "BouyomiChan":
                    return bouyomiChanPlayer.IsAvailable();
                default:
                    return false;
            }
        }

        public void Play(Alarm alarm)
        {
            var task = new Task(() =>
            {
                if (alarm.FilePlayMode == FilePlayMode.BeforeVoice)
                {
                    if (!string.IsNullOrWhiteSpace(alarm.FilePath))
                    {
                        PlayAudioFile(alarm.FilePath);
                    }
                }

                var command = new PlayerCommand(alarm.VoiceType, alarm.Message);

                if (string.IsNullOrWhiteSpace(alarm.PlayerType))
                {
                    MessageBox.Show($"ボイスを選択してください。");
                    return;
                }

                switch (alarm.PlayerType)
                {
                    case "Cevio":
                        cevioPlayer.Play(command);
                        break;
                    case "BouyomiChan":
                        bouyomiChanPlayer.Play(command);
                        break;
                    default:
                        MessageBox.Show($"PlayerType ({alarm.PlayerType}) には非対応です。");
                        break;
                }
            });
            task.Start();
        }

        public void PlayAudioFile(string path)
        {
            Trace.WriteLine($"{nameof(PlayAudioFile)} (path length: {path.Length})");

            using (var audioFileReader = new AudioFileReader(path))
            using (var stopEvent = new ManualResetEventSlim())
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFileReader);
                outputDevice.PlaybackStopped += (object sender, StoppedEventArgs e) =>
                {
                    stopEvent.Set();
                };
                outputDevice.Play();

                stopEvent.Wait();
            }
        }
    }
}
