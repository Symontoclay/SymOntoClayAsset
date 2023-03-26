using SymOntoClay.UnityAsset.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.SymOntoClay.Editors.CustomEditorGUILayouts
{
    public class CategoriesCustomEditorGUILayout
    {
        public CategoriesCustomEditorGUILayout(Waypoint target, SerializedObject so)
        {
            _target = target;
            _so = so;
            _categoriesProperty = so.FindProperty("Categories");
        }

        private Waypoint _target;
        private SerializedObject _so;
        private SerializedProperty _categoriesProperty;

        public void DrawGUI()
        {
            _target.EnableCategories = EditorGUILayout.Toggle("Enable Categories", _target.EnableCategories);

            EditorGUILayout.PropertyField(_categoriesProperty, true);

            if (_target.Categories == null)
            {
                _target.Categories = new List<string>(_target.DefaultCategories);
            }

            if (!_target.Categories.Any())
            {
                _target.Categories.AddRange(_target.DefaultCategories);
            }
        }
    }
}
