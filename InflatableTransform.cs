using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naelstrof.Inflatable {
    [System.Serializable]
    public class InflatableTransform : InflatableListener {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f,0f,1f,1f);
        
        private Vector3 startScale;

        public void SetTransform(Transform newTargetTransform) {
            targetTransform = newTargetTransform;
        }

        public override void OnEnable() {
            startScale = targetTransform.localScale;
        }
        public override void OnSizeChanged(float newSize) {
            targetTransform.localScale = startScale*Mathf.Max(curve.Evaluate(newSize),0.05f);
        }
    }
}
