using UnityEngine;
using UnityEngine.Serialization;

public class SineWaveExample : MonoBehaviour
{
    [Range(0, 1)] public float Volume = 0.5f;
    [Range(1, 250)] public float baseFrequency = 100; // Base frequency for both ears
    [Range(1, 30)] public float beatFrequency = 5; // Frequency difference (binaural beat)
    public float SpinSpeed = 1.0f; // Speed of the audio spin

    private float stereoPosition;
    private const float sampleRate = 44100;
    private AudioSource audioSource;
    private float phaseLeft = 0;
    private float phaseRight = 0;

    private float _time;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1;
        audioSource.Stop();
    }

    void Update()
    {
        // Modulate stereo position based on spinSpeed
        stereoPosition = Mathf.Sin(Time.time * SpinSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            // Update phases for left and right channels separately
            phaseLeft += 2 * Mathf.PI * (baseFrequency - beatFrequency / 2) / sampleRate;
            phaseRight += 2 * Mathf.PI * (baseFrequency + beatFrequency / 2) / sampleRate;

            // Calculate the sine wave for left and right channels
            float sampleLeft = Mathf.Sin(phaseLeft) * Volume;
            float sampleRight = Mathf.Sin(phaseRight) * Volume;

            // Modulate stereo position based on spinSpeed
            sampleLeft *= (1 - stereoPosition) / 2; // Adjust for stereo position
            sampleRight *= (1 + stereoPosition) / 2; // Adjust for stereo position

            // Adjust for volume
            sampleLeft *= Volume;
            sampleRight *= Volume;

            // Assign the samples to their respective channels
            for (int channel = 0; channel < channels; channel++)
            {
                if (channel == 0)
                    data[i + channel] = sampleLeft; // Left channel
                else if (channel == 1)
                    data[i + channel] = sampleRight; // Right channel
            }

            // Wrap phases
            if (phaseLeft >= 2 * Mathf.PI)
            {
                phaseLeft -= 2 * Mathf.PI;
            }

            if (phaseRight >= 2 * Mathf.PI)
            {
                phaseRight -= 2 * Mathf.PI;
            }
        }
    }
}