using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

namespace Naelstrof.Inflatable {
    [System.Serializable]
    public class Inflatable {
        private float currentSize;
        private float targetSize;

        // Tweeming
        private MonoBehaviour tweenOwner;
        private Coroutine routine;

        //[SerializeField] private AnimationCurve bounceCurve;
        //[SerializeField] private float bounceDuration;
        [SerializeField] private InflatableCurve bounce;

        public delegate void ChangedEvent(float newValue);

        public event ChangedEvent changed;

        [SerializeReference, SubclassSelector]
        public List<InflatableListener> listeners = new List<InflatableListener>();

        public ReadOnlyCollection<InflatableListener> readOnlyListeners;
        public ReadOnlyCollection<InflatableListener> GetInflatableListeners() {
            return readOnlyListeners ??= listeners.AsReadOnly();
        }

        private bool initialized = false;

        private bool SetSize(float newSize, MonoBehaviour owner, out IEnumerator tween) {
            targetSize = newSize;
            if (routine == null) {
                tween = TweenToNewSize(owner);
                return true;
            }
            tween = null;
            return false;
        }

        public void SetSize(float newSize, MonoBehaviour tweener) {
            if (!initialized) {
                Debug.LogError("Inflatable wasn't initialized.", tweener);
                throw new UnityException("Inflatable wasn't initialized ");
            }

            if (bounce == null) {
                return;
            }

            if (tweener.isActiveAndEnabled) {
                if (SetSize(newSize, tweener, out IEnumerator tween)) {
                    routine = tweener.StartCoroutine(tween);
                    tweenOwner = tweener;
                }
            } else {
                SetSizeInstant(newSize);
            }
        }

        public void SetSizeInstant(float newSize) {
            foreach (InflatableListener listener in listeners) {
                listener.OnSizeChanged(newSize);
            }
        }

        public void OnEnable() {
            if (initialized == false) {
                foreach (var listener in listeners) {
                    listener.OnEnable();
                }
                initialized = true;
            }
            if (routine != null) {
                tweenOwner.StopCoroutine(routine);
                routine = null;
            }
        }

        public float GetSize() {
            return targetSize;
        }

        private IEnumerator TweenToNewSize(MonoBehaviour owner) {
            try {
                float startSize = currentSize;
                float startTime = Time.time;
                float endTime = Time.time + bounce.GetBounceDuration();
                while (Time.time < endTime && owner.isActiveAndEnabled) {
                    float t = (Time.time - startTime) / bounce.GetBounceDuration();
                    currentSize = Mathf.LerpUnclamped(startSize, targetSize, bounce.EvaluateCurve(t));
                    foreach (InflatableListener listener in listeners) {
                        listener.OnSizeChanged(currentSize);
                    }
                    changed?.Invoke(currentSize);
                    yield return null;
                }
            } finally {
                currentSize = targetSize;
                foreach (InflatableListener listener in listeners) {
                    listener.OnSizeChanged(currentSize);
                }
                changed?.Invoke(currentSize);

                if (owner == tweenOwner) {
                    routine = null;
                }
            }
        }
        public void AddListener(InflatableListener listener) {
            if (initialized) {
                listener.OnEnable();
                listener.OnSizeChanged(currentSize);
            }

            listeners.Add(listener);
        }
    }
}
