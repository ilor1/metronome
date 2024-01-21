using System;
using Unity.Mathematics;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField] private Intiface _intiface;
    [SerializeField] private AudioClip _sfxTok;
    [SerializeField] private AudioClip _sfxDing;

    public float BeatsPerMinute { get; set; }
    public float DingEveryNth { get; set; }

    public bool AlternateEars { get; set; }

    public float HapticDuration { get; set; }

    public bool AlternateDevices { get; set; }
    private bool _pairedDevice = false;

    [SerializeField] private AudioSource _audioSourceCenter;
    [SerializeField] private AudioSource _audioSourceLeft;
    [SerializeField] private AudioSource _audioSourceRight;


    private bool _playOnleft = true;
    private float _timeSinceLastPlay;
    private int _toksPlayed = 0;

    public float Volume { get; set; }


    private void Awake()
    {
        Volume = 1.0f;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        // Set volume
        _audioSourceCenter.volume = Volume;
        _audioSourceLeft.volume = Volume;
        _audioSourceRight.volume = Volume;
        
        float timeInterval = 60f / BeatsPerMinute;

        _timeSinceLastPlay += Time.deltaTime;

        // PlaySound at intervals
        if (_timeSinceLastPlay >= timeInterval)
        {
            _timeSinceLastPlay = 0.0f;
            PlaySound();
            _pairedDevice = !_pairedDevice;
        }

        if (_intiface.Client != null)
        {
            float intensityPaired = 0f;
            float intensityUnpaired = 0f;
            if (_timeSinceLastPlay > HapticDuration)
            {
                intensityPaired = 0f;
                intensityUnpaired = 0f;
            }
            else
            {
                intensityPaired = _pairedDevice ? 1f : 0;
                intensityUnpaired = _pairedDevice ? 0f : 1f;
            }

            for (int i = 0; i < _intiface.Client.Devices.Length; i++)
            {
                float intensity = math.max(intensityPaired, intensityUnpaired);
                if (AlternateDevices)
                {
                    intensity = i % 2 == 0 ? intensityPaired : intensityUnpaired;
                }

                var device = _intiface.Client.Devices[i];
                _intiface.UpdateDevice(device, intensity);
            }
        }
    }
    
    private void PlaySound()
    {
        int nth = Mathf.RoundToInt(DingEveryNth);
        if (nth > 0 && _toksPlayed >= nth - 1)
        {
            if (AlternateEars)
            {
                if (_playOnleft)
                {
                    _playOnleft = false;
                    _audioSourceLeft.PlayOneShot(_sfxDing);
                }
                else
                {
                    _playOnleft = true;
                    _audioSourceRight.PlayOneShot(_sfxDing);
                }
            }
            else
            {
                _audioSourceCenter.PlayOneShot(_sfxDing);
            }

            _toksPlayed = 0;
        }
        else
        {
            if (AlternateEars)
            {
                if (_playOnleft)
                {
                    _playOnleft = false;
                    _audioSourceLeft.PlayOneShot(_sfxTok);
                }
                else
                {
                    _playOnleft = true;
                    _audioSourceRight.PlayOneShot(_sfxTok);
                }
            }
            else
            {
                _audioSourceCenter.PlayOneShot(_sfxTok);
            }

            _toksPlayed++;
        }
    }
}