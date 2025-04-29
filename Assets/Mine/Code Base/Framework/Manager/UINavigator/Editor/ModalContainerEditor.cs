using Mine.CodeBase.Framework.Manager.UINavigator.Runtime;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Modal;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(ModalContainer))]
    public class ModalContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty _instantiateType;
        SerializedProperty _registerModalsByPrefab;
        SerializedProperty _registerModalsByAddressable;
        SerializedProperty _modalBackdrop;

        #endregion

        #region Properties

        ModalContainer Target => target as ModalContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(ModalContainer.InstantiateType)}>k__BackingField",
            $"<{nameof(ModalContainer.RegisterModalsByPrefab)}>k__BackingField",
#if ADDRESSABLE_SUPPORT
            $"<{nameof(ModalContainer.RegisterModalsByAddressable)}>k__BackingField",
#endif

            "_modalBackdrop"
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            _instantiateType = serializedObject.FindProperty($"<{nameof(ModalContainer.InstantiateType)}>k__BackingField");
            _registerModalsByPrefab = serializedObject.FindProperty($"<{nameof(ModalContainer.RegisterModalsByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            _registerModalsByAddressable = serializedObject.FindProperty($"<{nameof(ModalContainer.RegisterModalsByAddressable)}>k__BackingField");
#endif

            _modalBackdrop = serializedObject.FindProperty("_modalBackdrop");
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
                            EditorGUILayout.PropertyField(_registerModalsByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case EInstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(_registerModalsByAddressable);
                            break;
#endif

                    }

                    EditorGUILayout.PropertyField(_modalBackdrop);
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
