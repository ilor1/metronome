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

        // Texts
        private const string TITLE = "Metronome";
        private const string BEATS_PER_MINUTE = "beats/min";
        private const string METRONOME = "<b>METRONOME</b>";
        private const string HAPTICS = "<b>HAPTICS</b>";
        private const string HAPTICS_TOGGLE = "Toggle haptics";
        private const string ALTERNATE_HAPTICS = "Alternate devices";
        private const string MAX_STRENGTH = "Max strength";
        private const string DURATION = "Duration";
        private const string ALTERNATE_SIDES = "Alternate left/right";


        private VisualElement _hapticsStrengthSetting;
        private VisualElement _hapticsDurationSlider;
        private VisualElement _hapticsAlternateDevices;
        private bool _devicesChanged;

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

            // var header = Create(HEADER);
            // var title = Create<Label>(TITLE);
            // title.text = TITLE;
            // header.Add(title);
            // root.Add(header);
            
            var content = Create<ScrollView>(CONTENT);
            root.Add(content);

            var metronomeLabel = Create<Label>();
            metronomeLabel.text = METRONOME;
            content.Add(metronomeLabel);

            var beatsContainer = Create(SETTINGS_CONTAINER);
            CreateBPMSlider(beatsContainer);
            CreateDingSlider(beatsContainer);
            CreateAlternateSidesToggle(beatsContainer);
            content.Add(beatsContainer);

            var hapticsLabel = Create<Label>();
            hapticsLabel.text = HAPTICS;
            content.Add(hapticsLabel);
            var hapticsContainer = Create(SETTINGS_CONTAINER);
            CreateHapticsToggle(hapticsContainer);
            _hapticsStrengthSetting = CreateHapticsStrengthSlider();
            _hapticsDurationSlider = CreateHapticsDurationSlider();
            _hapticsAlternateDevices = CreateAlternateDevicesToggle();


            content.Add(hapticsContainer);


            var quitButton = Create<Button>(QUIT_BUTTON);
            quitButton.text = "Exit";
            quitButton.clicked += () => { StartCoroutine(Quit()); };
            content.Add(quitButton);
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

        private void CreateBPMSlider(VisualElement content)
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
            content.Add(container);
        }

        private void CreateDingSlider(VisualElement content)
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
            content.Add(container);
        }

        private void CreateAlternateSidesToggle(VisualElement content)
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
            content.Add(container);
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
                        content.Remove(_hapticsStrengthSetting);
                        content.Remove(_hapticsDurationSlider);
                        content.Remove(_hapticsAlternateDevices);
                    }
                });

                _intiface.DevicesChanged += () => { _devicesChanged = true; };

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