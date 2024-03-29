using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JigglePhysics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Naelstrof.Easing;

namespace Naelstrof.Inflatable {
    [System.Serializable]
    public class InflatableBlendShape : InflatableListener {
        [SerializeField]
        private string blendShapeName;
        [SerializeField]
        private List<SkinnedMeshRenderer> skinnedMeshRenderers;
        [SerializeField]
        private AnimationCurve curve = AnimationCurve.Linear(0f,0f,1f,1f);
        
        private List<int> blendshapeIDs;
        public override void OnEnable() {
            blendshapeIDs = new List<int>();
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers) {
                blendshapeIDs.Add(renderer.sharedMesh.GetBlendShapeIndex(blendShapeName));
            }
        }

        public void AddTargetRenderer(SkinnedMeshRenderer renderer) {
            if (skinnedMeshRenderers.Contains(renderer)) {
                return;
            }
            skinnedMeshRenderers.Add(renderer);
            blendshapeIDs.Add(renderer.sharedMesh.GetBlendShapeIndex(blendShapeName));
        }
        
        public bool ContainsTargetRenderer(SkinnedMeshRenderer renderer) {
            return skinnedMeshRenderers.Contains(renderer);
        }

        public void RemoveTargetRenderer(SkinnedMeshRenderer renderer) {
            int index = skinnedMeshRenderers.IndexOf(renderer);
            if (index == -1) {
                return;
            }
            skinnedMeshRenderers.RemoveAt(index);
            blendshapeIDs.RemoveAt(index);
        }

        public override void OnSizeChanged(float newSize) {
            float t = curve.Evaluate(newSize);
            for (int i = 0; i < skinnedMeshRenderers.Count; i++) {
                skinnedMeshRenderers[i].SetBlendShapeWeight(blendshapeIDs[i], t*100f);
            }
        }
    }
}
