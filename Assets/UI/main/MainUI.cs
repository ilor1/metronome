using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Main
{
    // UI controls structs -> structs change and ECS systems react to it

    public class MainUI : UIBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet[] _styleSheets;

        [SerializeField] private Intiface _intiface;
        [SerializeField] private Metronome _metronome;
        [SerializeField] private SineWaveExample _binaurals;

        // Texts
        private const string TITLE = "Metronome";

        private const string METRONOME = "<b>METRONOME by ilori</b>";
        private const string BEATS_PER_MINUTE = "beats/min";
        private const string METRONOME_TOGGLE = "Toggle metronome";
        private const string ALTERNATE_SIDES = "Alternate left/right";

        private const string BINAURAL = "<b>BINAURALS</b>";
        private const string BINAURAL_TOGGLE = "Toggle binaurals";


        private const string HAPTICS = "<b>HAPTICS</b>";
        private const string HAPTICS_TOGGLE = "Toggle haptics";
        private const string ALTERNATE_HAPTICS = "Alternate devices";
        private const string MAX_STRENGTH = "Max strength";
        private const string DURATION = "Duration";

        private const string CREDITS = "<b>CREDITS</b>";

        // Metronome
        private VisualElement _metronomeVolumeSlider;
        private VisualElement _bpmSlider;
        private VisualElement _dingSlider;
        private VisualElement _sidesToggle;

        // Binaurals
        private VisualElement _binauralsVolumeSlider;
        private VisualElement _baseFrequency;
        private VisualElement _beatFrequency;
        private VisualElement _spinSpeed;

        // Haptics
        private VisualElement _hapticsStrengthSetting;
        private VisualElement _hapticsDurationSlider;
        private VisualElement _hapticsAlternateDevices;

        private void Start()
        {
            StartCoroutine(Generate());
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            // Create Root
            yield return null;
            var root = _document.rootVisualElement;
            root.AddToClassList(ROOT);
            root.Clear();

            foreach (var styleSheet in _styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }

            var content = Create<ScrollView>(CONTENT);
            root.Add(content);

            // Metronome
            var metronomeLabel = Create<Label>();
            metronomeLabel.text = METRONOME;
            content.Add(metronomeLabel);
            var metronomeContainer = Create(SETTINGS_CONTAINER);
            _metronomeVolumeSlider = CreateMetronomeVolumeSlider();
            _bpmSlider = CreateBPMSlider();
            _dingSlider = CreateDingSlider();
            _sidesToggle = CreateAlternateSidesToggle();
            CreateMetronomeToggle(metronomeContainer);
            content.Add(metronomeContainer);

            // Binaurals
            var binauralLabel = Create<Label>();
            binauralLabel.text = BINAURAL;
            content.Add(binauralLabel);
            var binauralContainer = Create(SETTINGS_CONTAINER);
            _binauralsVolumeSlider = CreateBinauralsVolumeSlider();
            _beatFrequency = CreateBeatFrequencySlider();
            _baseFrequency = CreateBaseFrequencySlider();
            _spinSpeed = CreateSpinSpeedSlider();
            CreateBinauralsToggle(binauralContainer);
            content.Add(binauralContainer);

            // Haptics
            var hapticsLabel = Create<Label>();
            hapticsLabel.text = HAPTICS;
            content.Add(hapticsLabel);
            var hapticsContainer = Create(SETTINGS_CONTAINER);
            _hapticsStrengthSetting = CreateHapticsStrengthSlider();
            _hapticsDurationSlider = CreateHapticsDurationSlider();
            _hapticsAlternateDevices = CreateAlternateDevicesToggle();
            CreateHapticsToggle(hapticsContainer);
            content.Add(hapticsContainer);

            // Quit
            var quitButton = Create<Button>(QUIT_BUTTON);
            quitButton.text = "Exit";
            quitButton.clicked += () => { StartCoroutine(Quit()); };
            content.Add(quitButton);

            // Credits
            
            // ilori
            var supportCreator = Create<Label>("credits");
            supportCreator.text = "If you wish to support me, you can do so at <b><color=white>ko-fi.com/ilori</color></b>";
            content.Add(supportCreator);
            supportCreator.RegisterCallback<ClickEvent>(evt =>
            {
                Application.OpenURL("https://ko-fi.com/ilori");
            });

            // intiface
            var intifaceCredits = Create<Label>("credits");
            intifaceCredits.text =
                "Special thanks to <b><color=white>Intiface\u00ae Central</color></b> by <b><color=white>Nonpolynomial</color></b> for the haptics support.";
            content.Add(intifaceCredits);
        }


        private IEnumerator Quit()
        {
            _intiface.enabled = false;
            yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        private VisualElement CreateHapticsStrengthSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"{MAX_STRENGTH} 100%";

            var slider = Create<Slider>();
            slider.lowValue = 0f;
            slider.highValue = 100f;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _intiface.HapticStrength = evt.newValue / 100f;
                    label.text = $"{MAX_STRENGTH} {Mathf.RoundToInt(evt.newValue)}%";
                });
                slider.value = 100;
                _intiface.HapticStrength = 1f;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateSpinSpeedSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Spin speed 1";

            var slider = Create<Slider>();
            slider.lowValue = 0;
            slider.highValue = 5;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _binaurals.SpinSpeed = evt.newValue;
                    label.text = $"Spin speed {Mathf.RoundToInt(evt.newValue)}";
                });
                slider.value = 1;
                _binaurals.SpinSpeed = 1;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateMetronomeVolumeSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Volume 50%";

            var slider = Create<Slider>();
            slider.lowValue = 0;
            slider.highValue = 100;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _metronome.Volume = evt.newValue / 100f;
                    label.text = $"Volume {Mathf.RoundToInt(evt.newValue)}%";
                });
                slider.value = 50;
                _binaurals.Volume = 0.5f;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateBinauralsVolumeSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Volume 50%";

            var slider = Create<Slider>();
            slider.lowValue = 0;
            slider.highValue = 100;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _binaurals.Volume = evt.newValue / 100f;
                    label.text = $"Volume {Mathf.RoundToInt(evt.newValue)}%";
                });
                slider.value = 50;
                _binaurals.Volume = 0.5f;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateBaseFrequencySlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Base frequency 100Hz";

            var slider = Create<Slider>();
            slider.lowValue = 1;
            slider.highValue = 250;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _binaurals.baseFrequency = evt.newValue;
                    label.text = $"Base frequency {Mathf.RoundToInt(evt.newValue)}Hz";
                });
                slider.value = 100;
                _binaurals.baseFrequency = 100;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateBeatFrequencySlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Beat frequency 5Hz";

            var slider = Create<Slider>();
            slider.lowValue = 1;
            slider.highValue = 30;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _binaurals.beatFrequency = evt.newValue;
                    label.text = $"Beat frequency {Mathf.RoundToInt(evt.newValue)}Hz";
                });
                slider.value = 5;
                _binaurals.beatFrequency = 5;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }


        private VisualElement CreateHapticsDurationSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"{DURATION} 300ms";

            var slider = Create<Slider>();
            slider.lowValue = 200f;
            slider.highValue = 1000f;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _metronome.HapticDuration = evt.newValue / 1000f;
                    label.text = $"{DURATION} {Mathf.RoundToInt(evt.newValue)}ms";
                });
                slider.value = 300;
                _metronome.HapticDuration = 0.3f;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateBPMSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"60 {BEATS_PER_MINUTE}";

            var slider = Create<Slider>();
            slider.lowValue = 30;
            slider.highValue = 180;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    _metronome.BeatsPerMinute = evt.newValue;
                    label.text = $"{Mathf.RoundToInt(evt.newValue)} {BEATS_PER_MINUTE}";
                });
                slider.value = 60;
                _metronome.BeatsPerMinute = 60;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateDingSlider()
        {
            var container = Create(SLIDER_CONTAINER);
            var label = Create<Label>();
            label.text = $"Ding every 3rd";

            var slider = Create<Slider>();
            slider.lowValue = 0;
            slider.highValue = 5;
            if (Application.isPlaying)
            {
                slider.RegisterValueChangedCallback(evt =>
                {
                    int nth = Mathf.RoundToInt(evt.newValue);
                    slider.SetValueWithoutNotify(nth);
                    _metronome.DingEveryNth = nth;

                    switch (nth)
                    {
                        case 0:
                            label.text = "Ding disabled";
                            break;
                        case 1:
                            label.text = "Ding every time";
                            break;
                        case 2:
                            label.text = "Ding every other";
                            break;
                        case 3:
                            label.text = "Ding every 3rd";
                            break;
                        default:
                            label.text = $"Ding every {nth}th";
                            break;
                    }
                });
                slider.value = 3;
                _metronome.DingEveryNth = 3;
            }

            container.Add(label);
            container.Add(slider);
            return container;
        }

        private VisualElement CreateAlternateSidesToggle()
        {
            // Label
            var container = Create(TOGGLE_CONTAINER);
            var label = Create<Label>();
            label.text = ALTERNATE_SIDES;

            // Toggle
            var toggle = Create(TOGGLE, TOGGLE_OFF);
            var slider = Create(TOGGLE_SLIDER);
            toggle.Add(slider);

            if (Application.isPlaying)
            {
                toggle.RegisterCallback<ClickEvent>(evt =>
                {
                    bool isOn = !_metronome.AlternateEars;
                    _metronome.AlternateEars = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);
                });
            }

            container.Add(label);
            container.Add(toggle);
            return container;
        }

        private VisualElement CreateAlternateDevicesToggle()
        {
            // Label
            var container = Create(TOGGLE_CONTAINER);
            var label = Create<Label>();
            label.text = ALTERNATE_HAPTICS;

            // Toggle
            var toggle = Create(TOGGLE, TOGGLE_OFF);
            var slider = Create(TOGGLE_SLIDER);
            toggle.Add(slider);

            if (Application.isPlaying)
            {
                toggle.RegisterCallback<ClickEvent>(evt =>
                {
                    bool isOn = !_metronome.AlternateDevices;
                    _metronome.AlternateDevices = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);
                });
            }

            container.Add(label);
            container.Add(toggle);
            return container;
        }

        private void CreateMetronomeToggle(VisualElement content)
        {
            // Toggle Label
            var container = Create(TOGGLE_CONTAINER);
            var label = Create<Label>();
            label.text = METRONOME_TOGGLE;

            // Toggle
            var toggle = Create(TOGGLE, TOGGLE_OFF);
            var slider = Create(TOGGLE_SLIDER);
            toggle.Add(slider);

            if (Application.isPlaying)
            {
                toggle.RegisterCallback<ClickEvent>(evt =>
                {
                    bool isOn = !_metronome.enabled;
                    _metronome.enabled = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);

                    if (isOn)
                    {
                        content.Add(_metronomeVolumeSlider);
                        content.Add(_bpmSlider);
                        content.Add(_dingSlider);
                        content.Add(_sidesToggle);
                    }
                    else
                    {
                        if (content.Contains(_metronomeVolumeSlider))
                        {
                            content.Remove(_metronomeVolumeSlider);
                        }

                        if (content.Contains(_bpmSlider))
                        {
                            content.Remove(_bpmSlider);
                        }

                        if (content.Contains(_dingSlider))
                        {
                            content.Remove(_dingSlider);
                        }

                        if (content.Contains(_sidesToggle))
                        {
                            content.Remove(_sidesToggle);
                        }
                    }
                });
            }

            container.Add(label);
            container.Add(toggle);
            content.Add(container);

            if (_metronome.enabled)
            {
                content.Add(_metronomeVolumeSlider);
                content.Add(_bpmSlider);
                content.Add(_dingSlider);
                content.Add(_sidesToggle);
                UpdateToggleVisuals(toggle, slider, true);
            }
        }

        private void CreateBinauralsToggle(VisualElement content)
        {
            // Toggle Label
            var container = Create(TOGGLE_CONTAINER);
            var label = Create<Label>();
            label.text = BINAURAL_TOGGLE;

            // Toggle
            var toggle = Create(TOGGLE, TOGGLE_OFF);
            var slider = Create(TOGGLE_SLIDER);
            toggle.Add(slider);

            if (Application.isPlaying)
            {
                toggle.RegisterCallback<ClickEvent>(evt =>
                {
                    bool isOn = !_binaurals.enabled;
                    _binaurals.enabled = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);

                    if (isOn)
                    {
                        content.Add(_binauralsVolumeSlider);
                        content.Add(_beatFrequency);
                        content.Add(_baseFrequency);
                        content.Add(_spinSpeed);
                    }
                    else
                    {
                        if (content.Contains(_binauralsVolumeSlider))
                        {
                            content.Remove(_binauralsVolumeSlider);
                        }

                        if (content.Contains(_beatFrequency))
                        {
                            content.Remove(_beatFrequency);
                        }

                        if (content.Contains(_baseFrequency))
                        {
                            content.Remove(_baseFrequency);
                        }

                        if (content.Contains(_spinSpeed))
                        {
                            content.Remove(_spinSpeed);
                        }
                    }
                });
            }

            content.Add(container);
            container.Add(label);
            container.Add(toggle);

            if (_binaurals.enabled)
            {
                content.Add(_binauralsVolumeSlider);
                content.Add(_beatFrequency);
                content.Add(_baseFrequency);
                content.Add(_spinSpeed);
                UpdateToggleVisuals(toggle, slider, true);
            }
        }

        private void CreateHapticsToggle(VisualElement content)
        {
            // Haptics label
            var container = Create(TOGGLE_CONTAINER);
            var label = Create<Label>();
            label.text = HAPTICS_TOGGLE;

            // Haptics Toggle
            var toggle = Create(TOGGLE, TOGGLE_OFF);
            var slider = Create(TOGGLE_SLIDER);
            toggle.Add(slider);

            if (Application.isPlaying)
            {
                toggle.RegisterCallback<ClickEvent>(evt =>
                {
                    bool isOn = !_intiface.enabled;
                    _intiface.enabled = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);

                    if (isOn)
                    {
                        content.Add(_hapticsStrengthSetting);
                        content.Add(_hapticsDurationSlider);
                        content.Add(_hapticsAlternateDevices);
                    }
                    else
                    {
                        if (content.Contains(_hapticsStrengthSetting))
                        {
                            content.Remove(_hapticsStrengthSetting);
                        }

                        if (content.Contains(_hapticsDurationSlider))
                        {
                            content.Remove(_hapticsDurationSlider);
                        }

                        if (content.Contains(_hapticsAlternateDevices))
                        {
                            content.Remove(_hapticsAlternateDevices);
                        }
                    }
                });

                _intiface.IntifaceDisabled += () =>
                {
                    UpdateToggleVisuals(toggle, slider, false);
                    if (content.Contains(_hapticsStrengthSetting))
                    {
                        content.Remove(_hapticsStrengthSetting);
                    }

                    if (content.Contains(_hapticsDurationSlider))
                    {
                        content.Remove(_hapticsDurationSlider);
                    }

                    if (content.Contains(_hapticsAlternateDevices))
                    {
                        content.Remove(_hapticsAlternateDevices);
                    }
                };
            }

            container.Add(label);
            container.Add(toggle);
            content.Add(container);

            if (_intiface.enabled)
            {
                content.Add(_hapticsStrengthSetting);
                content.Add(_hapticsDurationSlider);
                content.Add(_hapticsAlternateDevices);
                UpdateToggleVisuals(toggle, slider, true);
            }
        }

        private void UpdateToggleVisuals(VisualElement toggle, VisualElement toggleSlider, bool isOn)
        {
            // update visuals
            float targetX = isOn ? toggle.resolvedStyle.width - toggleSlider.resolvedStyle.width - 4 : 0f;
            toggleSlider.style.left = targetX;

            if (isOn)
            {
                toggle.RemoveFromClassList(TOGGLE_OFF);
                toggle.AddToClassList(TOGGLE_ON);
            }
            else
            {
                toggle.RemoveFromClassList(TOGGLE_ON);
                toggle.AddToClassList(TOGGLE_OFF);
            }
        }
    }
}