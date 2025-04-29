using Mine.CodeBase.Framework.Manager.UINavigator.Runtime;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Sheet;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(SheetContainer))]
    public class SheetContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty _instantiateType;
        SerializedProperty _registerSheetsByPrefab;
        SerializedProperty _registerSheetsByAddressable;
        SerializedProperty _hasDefault;
        readonly string[] _toggleArray = { "On", "Off" };

        #endregion


        #region Properties

        SheetContainer Target => target as SheetContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(SheetContainer.InstantiateType)}>k__BackingField",
            $"<{nameof(SheetContainer.RegisterSheetsByPrefab)}>k__BackingField",
#if ADDRESSABLE_SUPPORT
            $"<{nameof(SheetContainer.RegisterSheetsByAddressable)}>k__BackingField",
#endif

            $"<{nameof(SheetContainer.HasDefault)}>k__BackingField"
        }).ToArray();

        #endregion


        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            _instantiateType = serializedObject.FindProperty($"<{nameof(SheetContainer.InstantiateType)}>k__BackingField");
            _registerSheetsByPrefab = serializedObject.FindProperty($"<{nameof(SheetContainer.RegisterSheetsByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            _registerSheetsByAddressable = serializedObject.FindProperty($"<{nameof(SheetContainer.RegisterSheetsByAddressable)}>k__BackingField");
#endif

            _hasDefault = serializedObject.FindProperty($"<{nameof(SheetContainer.HasDefault)}>k__BackingField");
        }

        #endregion


        #region GUI Process

        protected override void AdditionalGUIProcess()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Initialize Setting");
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(_instantiateType, GUIContent.none);
                    switch (Target.InstantiateType)
                    {
                        case EInstantiateType.InstantiateByPrefab:
                            EditorGUILayout.PropertyField(_registerSheetsByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case EInstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(_registerSheetsByAddressable);
                            break;
#endif
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("Has Default"));
                        var select = GUILayout.Toolbar(_hasDefault.boolValue ? 0 : 1, _toggleArray);
                        _hasDefault.boolValue = select == 0;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
