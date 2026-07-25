using UnityEngine;

namespace Tether.Audio
{
    public enum WaveType { Sine, Square, Triangle, Saw, Noise }

    /// <summary>
    /// Phase 3 procedural SFX generator. Renders short AudioClips at runtime
    /// from wave + envelope parameters — zero external audio files.
    /// </summary>
    public static class ProceduralAudio
    {
        public const int SampleRate = 44100;

        /// <summary>
        /// Build a mono AudioClip with a frequency sweep and exponential decay.
        /// </summary>
        public static AudioClip Tone(
            string name,
            float freqStart, float freqEnd,
            float duration,
            WaveType wave = WaveType.Sine,
            float volume = 0.5f,
            float decayExponent = 1.5f,
            float attack = 0.005f)
        {
            int samples = Mathf.Max(2, Mathf.RoundToInt(SampleRate * duration));
            var data = new float[samples];
            double phase = 0.0;

            for (int i = 0; i < samples; i++)
            {
                float t01 = (float)i / samples;
                float freq = Mathf.Lerp(freqStart, freqEnd, t01);
                phase += freq / SampleRate;

                float sample = SampleWave(wave, (float)phase);

                // Attack ramp + exponential decay envelope
                float attackFrac = attack / duration;
                float att = attackFrac > 0f ? Mathf.Clamp01(t01 / attackFrac) : 1f;
                float dec = Mathf.Pow(1f - t01, decayExponent);
                data[i] = sample * att * dec * volume;
            }

            var clip = AudioClip.Create(name, samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Mix two tones into one clip (for chord-like or noise+tone effects).
        /// </summary>
        public static AudioClip Mix(string name, AudioClip a, AudioClip b, float aGain = 1f, float bGain = 1f)
        {
            int len = Mathf.Max(a.samples, b.samples);
            var da = new float[a.samples]; a.GetData(da, 0);
            var db = new float[b.samples]; b.GetData(db, 0);
            var mix = new float[len];
            for (int i = 0; i < len; i++)
            {
                float va = i < a.samples ? da[i] * aGain : 0f;
                float vb = i < b.samples ? db[i] * bGain : 0f;
                mix[i] = Mathf.Clamp(va + vb, -1f, 1f);
            }
            var clip = AudioClip.Create(name, len, 1, SampleRate, false);
            clip.SetData(mix, 0);
            return clip;
        }

        /// <summary>
        /// Sequence of tones back-to-back (arpeggio / stinger).
        /// </summary>
        public static AudioClip Sequence(string name, params AudioClip[] parts)
        {
            int total = 0;
            foreach (var p in parts) total += p.samples;
            var buf = new float[total];
            int offset = 0;
            foreach (var p in parts)
            {
                var d = new float[p.samples];
                p.GetData(d, 0);
                System.Array.Copy(d, 0, buf, offset, p.samples);
                offset += p.samples;
            }
            var clip = AudioClip.Create(name, total, 1, SampleRate, false);
            clip.SetData(buf, 0);
            return clip;
        }

        private static float SampleWave(WaveType wave, float phase)
        {
            switch (wave)
            {
                case WaveType.Sine:     return Mathf.Sin(phase * Mathf.PI * 2f);
                case WaveType.Square:   return Mathf.Sign(Mathf.Sin(phase * Mathf.PI * 2f));
                case WaveType.Triangle: return 4f * Mathf.Abs(phase - Mathf.Floor(phase + 0.5f)) - 1f;
                case WaveType.Saw:      return 2f * (phase - Mathf.Floor(phase + 0.5f));
                case WaveType.Noise:    return Random.Range(-1f, 1f);
                default: return 0f;
            }
        }
    }
}
