using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ChainLink.WorldTime
{
    public class ColorTransitioner : MonoBehaviour
    {
        public float Value {
            get => _value;
            set {
                _value = Mathf.Clamp(value, 0, 1);
                _currentColor = Color.Lerp(_fromColor, _toColor, _value);
            }
        }
        public Color CurrentColor {
            get => _currentColor;
            set {
                if (_currentColor != value) {
                    _currentColor = value;
                    _onColorChanged?.Invoke(_currentColor);
                }
            }
        }

        [SerializeField]
        private Color _fromColor;
        [SerializeField]
        private Color _toColor;

        [SerializeField, ReadOnly]
        private Color _currentColor;
        [SerializeField]
        private UnityEvent<Color> _onColorChanged;

        [Range(0,1)]
        private float _value;

        public void StartTransition(Color fromColor, Color toColor)
        {
            _fromColor = fromColor;
            _toColor = toColor;
        }

        private void Update()
        {
            
        }
    }
}