using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Okapi
{

  [CustomEditor(typeof(OkOkapi))]
  internal class OkOkapiEditor : Editor
  {
    public void OnInspectorGUI()
    {
      EditorGUILayout.IntField(0);
    }
  }

}