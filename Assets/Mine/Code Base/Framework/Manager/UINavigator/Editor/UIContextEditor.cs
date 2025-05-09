﻿using Mine.CodeBase.Framework.Manager.UINavigator.Runtime;
using Mine.CodeBase.Framework.Manager.UINavigator.Runtime.Animation;
using Mine.CodeBase.Framework.Util.Reflection;
using System;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mine.CodeBase.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(UIContext), true)]
    public class UIContextEditor : UnityEditor.Editor
    {
        #region Fields

        private SerializedProperty _script;
        private SerializedProperty _animationSetting;
        private SerializedProperty _showAnimation;
        private SerializedProperty _hideAnimation;

        private int _settingSelectionValue;

        private readonly string[] _animationTabArray = { "Move", "Rotate", "Scale", "Fade" };
        private int _animationSelectionValue;

        #endregion


        #region Properties

        UIContext Target => target as UIContext;

        protected virtual string[] PropertyToExclude() => new[]
        {
            "m_Script",
            "_showAnimation",
            "_hideAnimation",
            "_animationSetting"
        };

        private SerializedProperty ShowAnimation
        {
            get
            {
                SerializedProperty targetAnimation;
                if (_animationSetting.enumValueIndex == 0)
                {
                    var parentContainer = Target.GetComponentInParent<UIContainer>();
                    if (parentContainer)
                    {
                        var container = new SerializedObject(parentContainer);
                        targetAnimation = container.FindProperty($"<{nameof(UIContainer.ShowAnimation)}>k__BackingField");
                    }
                    else
                        return null;
                }
                else
                    targetAnimation = _showAnimation;

                return targetAnimation;
            }
        }

        private SerializedProperty HideAnimation
        {
            get
            {
                SerializedProperty targetAnimation;
                if (_animationSetting.enumValueIndex == 0)
                {
                    var parentContainer = Target.GetComponentInParent<UIContainer>();
                    if (parentContainer)
                    {
                        var container = new SerializedObject(parentContainer);
                        targetAnimation = container.FindProperty($"<{nameof(UIContainer.HideAnimation)}>k__BackingField");
                    }
                    else return null;
                }
                else targetAnimation = _hideAnimation;

                return targetAnimation;
            }

        }

        #endregion


        #region Unity Lifecycle

        protected virtual void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _animationSetting = serializedObject.FindProperty("_animationSetting");
            _showAnimation = serializedObject.FindProperty("_showAnimation");
            _hideAnimation = serializedObject.FindProperty("_hideAnimation");
        }

        #endregion


        #region GUI Process

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;

            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Animation Setting");
                var settingTabArea = EditorGUILayout.BeginVertical();
                {
                    settingTabArea = new Rect(settingTabArea) { xMin = 18, height = 18 };
                    GUI.Box(settingTabArea, GUIContent.none, GUI.skin.window);
                    _settingSelectionValue = GUI.Toolbar(settingTabArea, _settingSelectionValue, _animationSetting.enumNames);
                    _animationSetting.enumValueIndex = _settingSelectionValue;
                    EditorGUILayout.Space(settingTabArea.height);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);

                var animationTabArea = EditorGUILayout.BeginVertical();
                {
                    animationTabArea = new Rect(animationTabArea) { xMin = 18, height = 24 };
                    GUI.Box(animationTabArea, GUIContent.none, GUI.skin.window);
                    _animationSelectionValue = GUI.Toolbar(animationTabArea, _animationSelectionValue, _animationTabArray);
                    EditorGUILayout.Space(animationTabArea.height);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);

                switch (_animationSelectionValue)
                {
                    case 0:
                        DrawAnimationSetting<MoveShowAnimation, MoveHideAnimation>("_moveAnimation");
                        break;
                    case 1:
                        DrawAnimationSetting<RotateShowAnimation, RotateHideAnimation>("_rotateAnimation");
                        break;
                    case 2:
                        DrawAnimationSetting<ScaleShowAnimation, ScaleHideAnimation>("_scaleAnimation");
                        break;
                    case 3:
                        DrawAnimationSetting<FadeShowAnimation, FadeHideAnimation>("_fadeAnimation");
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(9);

            AdditionalGUIProcess();

            DrawPropertiesExcluding(serializedObject, PropertyToExclude());
            serializedObject.ApplyModifiedProperties();
        }

        #endregion


        #region Private Methods

        private void DrawAnimationSetting<TShow, THide>(string propertyRelative)
            where TShow : class
            where THide : class
        {
            var showArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(showArea, GUIContent.none);
                DrawReferenceField<TShow>(ShowAnimation?.FindPropertyRelative(propertyRelative), "Show Animation");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(9);
            var hideArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(hideArea, GUIContent.none);
                DrawReferenceField<THide>(HideAnimation?.FindPropertyRelative(propertyRelative), "Hide Animation");
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawReferenceField<T>(SerializedProperty target, string title = null)
            where T : class
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);

                if(target != null)
                {
                    EditorGUILayout.Space(6);
                    EditorGUI.indentLevel++;
                    {
                        target.isExpanded = true;
                        EditorGUILayout.PropertyField(target, GUIContent.none, true);
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.Space(27);
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.LabelField(new GUIContent("Failed to find parent container"), new GUIStyle(GUI.skin.label)
                        { alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(9);
                }

                if (title != null)
                    DrawTitleField(title, new Rect(area) { yMin = area.yMin, height = 24 });

                // Dropdown handling
                if (target != null)
                {
                    var dropdownArea = new Rect(GUILayoutUtility.GetLastRect()) { xMin = 18, height = 24 };
                    GUI.Box(dropdownArea, GUIContent.none, GUI.skin.window);

                    EditorGUILayout.Space(1);
                    var options = ReflectionUtility.GetDerivedTypes<T>()
                        .Select(x => x.Name)
                        .Prepend("None")
                        .ToArray();

                    int currentIndex = target.managedReferenceValue != null ? Array.FindIndex(options, option => option ==  target.managedReferenceValue.GetType().Name) : 0;
                    int selectedIndex = EditorGUILayout.Popup(currentIndex, options);
                    if (currentIndex != selectedIndex)
                    {
                        target.managedReferenceValue = selectedIndex == 0 
                            ? null 
                            : Activator.CreateInstance(ReflectionUtility.GetDerivedTypes<T>()[selectedIndex - 1]) as T;

                        serializedObject.ApplyModifiedProperties();
                    }
                }

            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawTitleField(string title, Rect rect = default)
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(rect == default ? new Rect(area) { xMin = 18 } : rect, GUIContent.none, GUI.skin.window);
                var targetStyle = new GUIStyle();
                targetStyle.fontSize = 15;
                targetStyle.padding.left = 10;
                targetStyle.normal.textColor = Color.white;
                targetStyle.alignment = TextAnchor.MiddleLeft;
                targetStyle.fontStyle = FontStyle.Bold;
                if (rect == default)
                    EditorGUILayout.LabelField(title, targetStyle, GUILayout.Height(25));
                else
                    EditorGUI.LabelField(rect, title, targetStyle);
            }
            EditorGUILayout.EndVertical();
        }

        #endregion


        #region Virtual Methods

        protected virtual void AdditionalGUIProcess()
        {
        }

        #endregion
    }
}
