using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {
        // Styles
        protected const string ROOT = "root";
        protected const string HEADER = "header";
        protected const string CONTENT = "content";

        protected const string SETTINGS_CONTAINER = "settings-container";
        
        protected const string SLIDER_CONTAINER = "slider-container";
        
        protected const string TOGGLE_CONTAINER = "toggle-container";
        protected const string TOGGLE = "toggle";
        protected const string TOGGLE_ON = "toggle-on";
        protected const string TOGGLE_OFF = "toggle-off";
        protected const string TOGGLE_SLIDER = "toggle-slider";

        protected const string QUIT_BUTTON = "quit-button";
        
        protected VisualElement Create(params string[] classNames)
        {
            return Create<VisualElement>(classNames);
        }

        protected T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            var element = new T();
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}