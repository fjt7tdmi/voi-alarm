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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiAlarm.Models
{
    internal class BouyomiChanPlayer
    {
        private readonly Settings settings;

        public BouyomiChanPlayer(Settings settings)
        {
            this.settings = settings;

            if (settings.AutoStartBouyomiChan)
            {
                // 棒読みちゃんの二重起動でエラーが表示されるのを防止するため、
                // BouyomiChan.exe の存在をチェック
                if (Process.GetProcessesByName("BouyomiChan").Length == 0)
                {
                    LaunchBouyomiChan(settings.BouyomiChanPath);
                }
            }
        }
        public bool IsAvailable()
        {
            var port = settings.BouyomiChanPort;

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(new IPEndPoint(IPAddress.Loopback, port));
                    using (var writer = new BinaryWriter(tcpClient.GetStream()))
                    {
                        // Send dummy command
                        short commandType = 1;  // talk
                        short speed = -1;   // default
                        short tone = -1;    // default
                        short volume = -1;  // default
                        short voice = 0;    // default (TODO: check command.Voice)
                        byte charCode = 0;  // UTF-8
                        int messageSize = 0;

                        writer.Write(commandType);
                        writer.Write(speed);
                        writer.Write(tone);
                        writer.Write(volume);
                        writer.Write(voice);
                        writer.Write(charCode);
                        writer.Write(messageSize);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Play(PlayerCommand command)
        {
            Trace.WriteLine($"{nameof(BouyomiChanPlayer)}.{nameof(Play)} {command.VoiceType} (message length: {command.Message.Length})");

            var port = settings.BouyomiChanPort;

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(new IPEndPoint(IPAddress.Loopback, port));
                    using (var writer = new BinaryWriter(tcpClient.GetStream()))
                    {
                        var messageBytes = Encoding.UTF8.GetBytes(command.Message);

                        // Bouyoumi Chan protocol header
                        short commandType = 1;  // talk
                        short speed = -1;   // default
                        short tone = -1;    // default
                        short volume = -1;  // default
                        short voice = 0;    // default (TODO: check command.Voice)
                        byte charCode = 0;  // UTF-8
                        int messageSize = messageBytes.Length;

                        writer.Write(commandType);
                        writer.Write(speed);
                        writer.Write(tone);
                        writer.Write(volume);
                        writer.Write(voice);
                        writer.Write(charCode);
                        writer.Write(messageSize);
                        writer.Write(messageBytes);
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show($"棒読みちゃんとの通信に失敗しました。(TCP ポート: {port})");
            }
            catch (SocketException)
            {
                MessageBox.Show($"棒読みちゃんとの通信に失敗しました。(TCP ポート: {port})");
            }
        }

        private void LaunchBouyomiChan(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show($"棒読みちゃんの起動に失敗しました。パスが設定されていません。");
                return;
            }

            if (!File.Exists(path))
            {
                MessageBox.Show($"棒読みちゃんの起動に失敗しました。指定されたパスに実行ファイルが存在しません。({settings.BouyomiChanPath})");
                return;
            }

            try
            {
                Process.Start(settings.BouyomiChanPath);
            }
            catch
            {
                MessageBox.Show($"棒読みちゃんの起動に失敗しました。({settings.BouyomiChanPath})");
                return;
            }

            // 棒読みちゃんが起動して available になるのを待つ
            var retryCount = 5;
            var retryInternalMs = 500;

            for (int i = 0; i < retryCount; i++)
            {
                if (IsAvailable())
                {
                    return;
                }

                Task.Delay(retryInternalMs).Wait();
            }
        }
    }
}
