#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PolyNav
{

    [CustomEditor(typeof(PolyNav2D))]
    public class PolyNav2DInspector : UnityEditor.Editor
    {

        private PolyNav2D polyNav {
            get { return target as PolyNav2D; }
        }

        public override void OnInspectorGUI() {

            base.OnInspectorGUI();

            if ( Application.isPlaying ) {
                EditorGUILayout.LabelField("Nodes Count", polyNav.nodesCount.ToString());
            }
        }
    }
}

#endif