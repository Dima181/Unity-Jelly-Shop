using Mine.CodeBase.Framework.Manager.UINavigator.Runtime;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Page;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(PageContainer))]
    public class PageContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty _instantiateType;
        SerializedProperty _registerPagesByPrefab;
        SerializedProperty _registerPagesByAddressable;
        SerializedProperty _hasDefault;
        readonly string[] _toggleArray = { "On", "Off" };

        #endregion

        #region Properties

        PageContainer Target => target as PageContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(PageContainer.InstantiateType)}>k__BackingField",
            $"<{nameof(PageContainer.RegisterPagesByPrefab)}>k__BackingField",
#if ADDRESSABLE_SUPPORT
            $"<{nameof(PageContainer.RegisterPagesByAddressable)}>k__BackingField",
#endif

            $"<{nameof(PageContainer.HasDefault)}>k__BackingField"
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            _instantiateType = serializedObject.FindProperty($"<{nameof(PageContainer.InstantiateType)}>k__BackingField");
            _registerPagesByPrefab = serializedObject.FindProperty($"<{nameof(PageContainer.RegisterPagesByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            _registerPagesByAddressable = serializedObject.FindProperty($"<{nameof(PageContainer.RegisterPagesByAddressable)}>k__BackingField");
#endif

            _hasDefault = serializedObject.FindProperty($"<{nameof(PageContainer.HasDefault)}>k__BackingField");
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
                            EditorGUILayout.PropertyField(_registerPagesByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case EInstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(_registerPagesByAddressable);
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
