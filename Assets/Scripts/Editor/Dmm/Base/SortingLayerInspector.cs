using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace Dmm.Base
{
    [CustomEditor(typeof(SortingLayerController))]
    public class SortingLayerInspector : UnityEditor.Editor
    {

        private SortingLayerController _slc;

        private string[] _sortingLayers;

        private int _selectedSortingLayer;

        public void OnEnable()
        {
            _slc = target as SortingLayerController;
            _sortingLayers = GetSortingLayerNames();

            for (int i = 0; i < _sortingLayers.Length; i++)
            {
                if (_sortingLayers[i] == _slc.SortingLayer)
                {
                    _selectedSortingLayer = i;
                    break;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            _selectedSortingLayer = EditorGUILayout.Popup("SortingLayer", _selectedSortingLayer, _sortingLayers);
            _slc.SortingLayer = _sortingLayers[_selectedSortingLayer];

            _slc.SortingOrder = EditorGUILayout.IntField("SortingOrder", _slc.SortingOrder);
        }

        public string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }
}
