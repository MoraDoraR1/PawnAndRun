using System.Collections;
using UnityEngine;

namespace PawnAndRun.Game
{
    public enum ToneWave
    {
        Sine,
        Square,
        Sawtooth,
        Triangle
    }

    /// <summary>
    /// Procedurally synthesizes the short tones web-prototype/index.html plays via WebAudio
    /// oscillators (sfx.move/capture/blocked/tick/start/gameOver/newBest), so no audio assets
    /// are needed. Volume follows SettingsModalController's SFX slider.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        private const int SampleRate = 44100;

        [SerializeField] private AudioSource source;
        [Range(0f, 1f)] public float MasterVolume = 1f;

        private void Awake()
        {
            if (source == null) source = GetComponent<AudioSource>();
            source.playOnAwake = false;
        }

        private static float Waveform(ToneWave wave, float freq, float t)
        {
            float x = freq * t;
            float frac = x - Mathf.Floor(x);
            switch (wave)
            {
                case ToneWave.Square:
                    return Mathf.Sin(2f * Mathf.PI * x) >= 0f ? 1f : -1f;
                case ToneWave.Sawtooth:
                    return 2f * frac - 1f;
                case ToneWave.Triangle:
                    return frac < 0.5f ? (4f * frac - 1f) : (3f - 4f * frac);
                default:
                    return Mathf.Sin(2f * Mathf.PI * x);
            }
        }

        private AudioClip BuildTone(float freq, float duration, ToneWave wave, float gain)
        {
            int samples = Mathf.Max(1, Mathf.RoundToInt(SampleRate * duration));
            var data = new float[samples];
            float attackSamples = SampleRate * 0.01f;
            float releaseStart = samples * 0.35f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float raw = Waveform(wave, freq, t);

                float env = Mathf.Min(1f, i / attackSamples);
                if (i > releaseStart)
                {
                    float rt = (i - releaseStart) / (samples - releaseStart);
                    env *= Mathf.Pow(1f - rt, 2f);
                }
                data[i] = raw * gain * env;
            }
            var clip = AudioClip.Create("sfx_tone", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private void PlayTone(float freq, float duration, ToneWave wave, float gain, float delay = 0f)
        {
            var clip = BuildTone(freq, duration, wave, gain);
            if (delay <= 0f) source.PlayOneShot(clip, MasterVolume);
            else StartCoroutine(PlayDelayed(clip, delay));
        }

        private IEnumerator PlayDelayed(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.PlayOneShot(clip, MasterVolume);
        }

        public void Move() => PlayTone(520, 0.05f, ToneWave.Sine, 0.5f);

        public void Capture()
        {
            PlayTone(720, 0.05f, ToneWave.Square, 0.45f);
            PlayTone(1080, 0.07f, ToneWave.Square, 0.4f, 0.05f);
        }

        public void Blocked() => PlayTone(150, 0.09f, ToneWave.Sawtooth, 0.4f);

        public void Tick() => PlayTone(880, 0.03f, ToneWave.Sine, 0.35f);

        public void PlayStart()
        {
            PlayTone(660, 0.06f, ToneWave.Sine, 0.45f);
            PlayTone(880, 0.08f, ToneWave.Sine, 0.45f, 0.07f);
        }

        public void GameOver()
        {
            PlayTone(400, 0.18f, ToneWave.Sine, 0.5f);
            PlayTone(260, 0.22f, ToneWave.Sine, 0.45f, 0.12f);
        }

        public void NewBest()
        {
            PlayTone(660, 0.08f, ToneWave.Triangle, 0.5f);
            PlayTone(880, 0.08f, ToneWave.Triangle, 0.5f, 0.08f);
            PlayTone(1100, 0.12f, ToneWave.Triangle, 0.5f, 0.16f);
        }
    }
}
