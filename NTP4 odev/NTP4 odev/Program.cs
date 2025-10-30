using System;
using System.IO;
using Vosk;
using NAudio.Wave;

class Program
{
    static void Main()
    {
        
        Vosk.Vosk.SetLogLevel(0);

        
        var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vosk-model-small-tr-0.3");
        if (!Directory.Exists(modelPath))
        {
            Console.WriteLine("Model klasörü bulunamadı: " + modelPath);
            return;
        }

       
        var model = new Model(modelPath);

        
        using var waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(16000, 1); 

        var rec = new VoskRecognizer(model, 16000.0f);

        
        waveIn.DataAvailable += (sender, e) =>
        {
            if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = rec.Result();
                var text = ExtractText(result);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine(text); 
                }
            }
        };

        waveIn.StartRecording();
        Console.WriteLine("Konuşmayı başlatın (Ctrl+C ile çıkış)...");

        
        while (true)
        {
            System.Threading.Thread.Sleep(100);
        }
    }

    
    static string ExtractText(string json)
    {
        int idx = json.IndexOf("\"text\" : \"");
        if (idx >= 0)
        {
            int start = idx + 10;
            int end = json.IndexOf("\"", start);
            if (end > start)
                return json.Substring(start, end - start);
        }
        return "";
    }
}
