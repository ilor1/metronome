using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class Metronome : MonoBehaviour
{
    [SerializeField] private Intiface _intiface;
    [SerializeField] private AudioClip _sfxTok;
    [SerializeField] private AudioClip _sfxDing;

    [Tooltip("Beats per Minute(bpm)")] public float BeatsPerMinute { get; set; }
    [Tooltip("Bell sound every Nth")] public float DingEveryNth { get; set; }

    [Tooltip("Alternate sounds on left/right")]
    public bool _alternateEars { get; set; }

    [SerializeField] private AudioSource _audioSourceCenter;
    [SerializeField] private AudioSource _audioSourceLeft;
    [SerializeField] private AudioSource _audioSourceRight;


    [SerializeField] private float _hapticDuration = 0.1f;

    private bool _playOnleft = true;
    private float _timeSinceLastPlay;
    private int _toksPlayed = 0;


    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        BeatsPerMinute = 60;
        DingEveryNth = 3;
    }

    private void Update()
    {
        float timeInterval = 60f / BeatsPerMinute;

        _timeSinceLastPlay += Time.deltaTime;

        if (_timeSinceLastPlay >= timeInterval)
        {
            _timeSinceLastPlay = 0.0f;
            PlaySound();
            SetHaptic(1.0f);
        }
        else if (_timeSinceLastPlay > _hapticDuration)
        {
            // Reduce haptic to 0, between beats
            float value = timeInterval * 0.5f - _timeSinceLastPlay;
            value = math.clamp(value, 0, 1);
            SetHaptic(value);
        }
    }

    private void SetHaptic(float value)
    {
        _intiface.Intensity = value;
        _intiface.UpdateDevices();
    }

    private void PlaySound()
    {
        int nth = Mathf.RoundToInt(DingEveryNth);
        if (nth > 0 && _toksPlayed >= nth - 1)
        {
            if (_alternateEars)
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
            if (_alternateEars)
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