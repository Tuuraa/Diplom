using AssemblyAI.Transcripts;
using AssemblyAI;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Utils
{
    public class SpeechRecorder
    {
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private string outputFile = "speech.wav";

        public async Task<string> RecordAndTranscribeAsync(int seconds, string apiKey)
        {
            // Настраиваем запись
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000, 1);
            writer = new WaveFileWriter(outputFile, waveIn.WaveFormat);

            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
            };

            waveIn.StartRecording();

            await Task.Delay(seconds * 1000);

            waveIn.StopRecording();
            writer.Dispose();

            var client = new AssemblyAIClient(apiKey);
            var transcript = await client.Transcripts.TranscribeAsync(
                new FileInfo(outputFile),
                new TranscriptOptionalParams
                {
                    LanguageCode = TranscriptLanguageCode.Ru
                });


            if (transcript.Status == TranscriptStatus.Error)
                return $"Ошибка: {transcript.Error}";

            return transcript.Text;
        }
    }

}
