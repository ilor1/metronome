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
        private const string MAX_STRENGTH = "Max strength";
        private const string ALTERNATE_SIDES = "Alternate left/right";


        private VisualElement _hapticsStrengthSetting;

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
            root.Clear();
            root.AddToClassList(ROOT);
            foreach (var styleSheet in _styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }

            // var header = Create(HEADER);
            // var title = Create<Label>(TITLE);
            // title.text = TITLE;
            // header.Add(title);
            // root.Add(header);

            var content = Create(CONTENT);
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
            content.Add(hapticsContainer);
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
                    bool isOn = !_metronome._alternateEars;
                    _metronome._alternateEars = isOn;
                    UpdateToggleVisuals(toggle, slider, isOn);
                });
            }

            container.Add(label);
            container.Add(toggle);
            content.Add(container);
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
                    }
                    else if (content.Contains(_hapticsStrengthSetting))
                    {
                        content.Remove(_hapticsStrengthSetting);
                    }
                });

                _intiface.IntifaceDisabled += () =>
                {
                    UpdateToggleVisuals(toggle, slider, false);
                    if (content.Contains(_hapticsStrengthSetting))
                    {
                        content.Remove(_hapticsStrengthSetting);
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