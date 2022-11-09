using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PerlerBeadizer
{
    
    [CustomEditor(typeof(TexturePerlerBeadizer))]
    public class PerlerEditorScript : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TexturePerlerBeadizer _beadizer = (TexturePerlerBeadizer)target;

            if(GUILayout.Button("Generate"))
            {
                _beadizer.Generate();
            }

            if(!_beadizer.AutoRefresh)
            {
                if(GUILayout.Button("Refresh"))
                {
                    _beadizer.RefreshBeads();
                }
            }
        }
    }
}
