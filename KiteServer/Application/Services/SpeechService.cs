using Microsoft.CognitiveServices.Speech.Audio;

namespace Application.Services;

public class SpeechService
{
    /// <summary>
    /// 合成语音
    /// </summary>
    public static async Task<bool> SynthesizeSpeechAsync(SpeechConfig speechConfig, string ssmlContent, string outputFilePath, bool isSsmlContent)
    {
        try
        {
            using (var synthesizer = new SpeechSynthesizer(speechConfig, AudioConfig.FromWavFileOutput(outputFilePath)))
            {
                if (isSsmlContent)
                {
                    var result = await synthesizer.SpeakSsmlAsync(ssmlContent);
                    return result.Reason == ResultReason.SynthesizingAudioCompleted;
                }
                else
                {
                    var result = await synthesizer.SpeakTextAsync(ssmlContent);
                    return result.Reason == ResultReason.SynthesizingAudioCompleted;
                }
            }
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("合成失败");
        }
    }
}
