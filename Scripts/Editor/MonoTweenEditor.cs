using Tween;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Tween
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoTweener))]
    public class MonoTweenEditor : Editor
    {
        Vector2 tweenListPos;
        AnimType addTweenType = AnimType.Position;

        private bool isPlaying = false;
        private float playTime;

        Texture2D positionIcon;
        Texture2D rotateIcon;
        Texture2D scaleIcon;
        Texture2D colorIcon;
        Texture2D alphaIcon;
        Texture2D fieldOfViewIcon;
        Texture2D pathIcon;
        Texture2D anchorIcon;

        public static Texture2D FindTexture2D(string name)
        {
            var files = AssetDatabase.FindAssets(name);
            if (files.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(files[0]));
            return null;
        }

        private void InitIcon()
        {
            if (positionIcon == null)
            {
                positionIcon = FindTexture2D("_Jtween_Position");
                rotateIcon = FindTexture2D("_Jtween_Rotate");
                scaleIcon = FindTexture2D("_Jtween_Scale");
                colorIcon = FindTexture2D("_Jtween_Color");
                alphaIcon = FindTexture2D("_Jtween_Alpha");
                fieldOfViewIcon = FindTexture2D("_Jtween_FieldOfView");
                pathIcon = FindTexture2D("_Jtween_Path");
                anchorIcon = FindTexture2D("_Jtween_Anchor");
            }
        }

        private Texture2D GetCustomTnIcon(AnimType type)
        {
            InitIcon();
            switch (type)
            {
                case AnimType.Position:
                    return positionIcon;
                case AnimType.Rotate:
                    return rotateIcon;
                case AnimType.Scale:
                    return scaleIcon;
                case AnimType.Color:
                    return colorIcon;
                case AnimType.Alpha:
                    return alphaIcon;
//                case AnimType.FieldOfViewTween:
//                    return fieldOfViewIcon;
//                case AnimType.:
//                    return pathIcon;
                case AnimType.UiSize:
                case AnimType.UiAnchoredPosition:
                    return anchorIcon;
                default:
                    return positionIcon;
            }
        }

        /// <summary>
        /// UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            MonoTweener tar = (MonoTweener) target;
            serializedObject.Update();

            SerializedProperty playType = serializedObject.FindProperty("playType");
            SerializedProperty animationTime = serializedObject.FindProperty("animationTime");
            SerializedProperty delay = serializedObject.FindProperty("delay");
            SerializedProperty playOnAwake = serializedObject.FindProperty("playOnAwake");
            SerializedProperty IgnoreTimeScale = serializedObject.FindProperty("ignoreTimeScale");

            SerializedProperty m_onFinish = serializedObject.FindProperty("m_onFinish");

            SerializedProperty tweens = serializedObject.FindProperty("tweens");

            GUILayout.BeginVertical(GUILayout.MinHeight(100));
            EditorGUILayout.PropertyField(playType);
            EditorGUILayout.PropertyField(animationTime);
            EditorGUILayout.PropertyField(delay);
            EditorGUILayout.PropertyField(playOnAwake);
            EditorGUILayout.PropertyField(IgnoreTimeScale);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (Application.isPlaying)
                GUI.enabled = false;
            GUILayout.Label("Preview");

            GUILayout.BeginHorizontal();
            bool play = GUILayout.Toggle(isPlaying, "▶", EditorStyles.miniButton, GUILayout.Width(20),
                GUILayout.Height(20));
            if (play != isPlaying)
            {
                playTime = Time.realtimeSinceStartup;
                isPlaying = play;
            }

            if (isPlaying)
            {
                float t = Time.realtimeSinceStartup - playTime;
                if (t > 1)
                {
                    playTime = Time.realtimeSinceStartup;
                    t = 0;
                }

                tar.previewValue = t;
                EditorGUILayout.Slider(tar.previewValue, 0f, 1f);
            }
            else
            {
                tar.previewValue = EditorGUILayout.Slider(tar.previewValue, 0f, 1f);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.enabled = true;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Tweener List");

            GUILayout.BeginHorizontal();
            addTweenType = (AnimType) EditorGUILayout.EnumPopup(addTweenType);

            if (GUILayout.Button("Add Tween", EditorStyles.miniButton))
            {
                AddTween(tweens);
            }

            GUILayout.EndHorizontal();

            // draw items
            for (int i = 0; i < tweens.arraySize; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SerializedProperty thisTween = tweens.GetArrayElementAtIndex(i);
                SerializedProperty thisTween_TweenType = thisTween.FindPropertyRelative("animType");

                bool deleteElement = false;

                string thisTweenName = "Tween" + (i + 1) + ":" +
                                       thisTween_TweenType.enumNames[thisTween_TweenType.enumValueIndex];
                var tweenType = (AnimType) thisTween_TweenType.enumValueIndex;
                Texture2D tweenIcon = GetCustomTnIcon(tweenType);

                GUILayout.BeginHorizontal();

                GUILayout.BeginHorizontal();
                if (tweenIcon != null)
                    GUILayout.Button(tweenIcon, EditorStyles.label, GUILayout.Height(20), GUILayout.Width(20));
                GUILayout.Label(thisTweenName);
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete Tween?", "Delete this Tween?", "Yes", "No"))
                    {
                        tweens.DeleteArrayElementAtIndex(i);
                        deleteElement = true;
                    }
                }

                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                if (!deleteElement)
                {
                    SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative("curve");
                    SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative("isLocal");
                    SerializedProperty thisTween_EaseType = thisTween.FindPropertyRelative("easeType");

                    GUILayout.TextArea("", GUILayout.Height(2));
                    if (tweenType == AnimType.Position || tweenType == AnimType.Rotate)
                    {
                        EditorGUILayout.PropertyField(thisTween_IsLocal);
                    }

                    EditorGUILayout.PropertyField(thisTween_EaseType);
                    var easeType = (Ease) thisTween_EaseType.enumValueIndex;
                    if (easeType == Ease.Default)
                    {
                        EditorGUILayout.PropertyField(thisTween_Curve);
                    }

                    GUILayout.TextArea("", GUILayout.Height(2));

                    DrawCustomTn(thisTween, tweenType);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();

            EditorGUILayout.PropertyField(m_onFinish);
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCustomTn(SerializedProperty thisTween, AnimType animType)
        {
            switch (animType)
            {
                case AnimType.Position:
                case AnimType.Rotate:
                case AnimType.Scale:
                case AnimType.UiAnchoredPosition:
                    SerializedProperty thisFromVector = thisTween.FindPropertyRelative("fromV3");
                    SerializedProperty thisToVector = thisTween.FindPropertyRelative("toV3");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("SetFromValue", EditorStyles.miniButtonLeft))
                    {
                        SetCurrentValueCustomTn(thisTween, true);
                    }

                    if (GUILayout.Button("SetToValue", EditorStyles.miniButtonRight))
                    {
                        SetCurrentValueCustomTn(thisTween, false);
                    }

                    GUILayout.EndHorizontal();
                    Vector3 fromVector =
                        EditorGUILayout.Vector3Field("From Vector", thisFromVector.vector3Value);
                    Vector3 toVecor = EditorGUILayout.Vector3Field("To Vector", thisToVector.vector3Value);

                    thisFromVector.vector3Value = fromVector;
                    thisToVector.vector3Value = toVecor;
                    break;
                case AnimType.UiSize:
                    SerializedProperty thisFromVector2 = thisTween.FindPropertyRelative("fromV2");
                    SerializedProperty thisToVector2 = thisTween.FindPropertyRelative("toV2");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("SetFromValue", EditorStyles.miniButtonLeft))
                    {
                        SetCurrentValueCustomTn(thisTween, true);
                    }

                    if (GUILayout.Button("SetToValue", EditorStyles.miniButtonRight))
                    {
                        SetCurrentValueCustomTn(thisTween, false);
                    }

                    GUILayout.EndHorizontal();
                    Vector2 fromVector2 =
                        EditorGUILayout.Vector2Field("From Vector", thisFromVector2.vector2Value);
                    Vector2 toVector2 = EditorGUILayout.Vector2Field("To Vector", thisToVector2.vector2Value);

                    thisFromVector2.vector2Value = fromVector2;
                    thisToVector2.vector2Value = toVector2;
                    break;
                case AnimType.Color:
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("SetFromValue", EditorStyles.miniButtonLeft))
                    {
                        SetCurrentValueCustomTn(thisTween, true);
                    }

                    if (GUILayout.Button("SetToValue", EditorStyles.miniButtonRight))
                    {
                        SetCurrentValueCustomTn(thisTween, false);
                    }

                    GUILayout.EndHorizontal();
                    SerializedProperty fromColor = thisTween.FindPropertyRelative("fromColor");
                    SerializedProperty toColor = thisTween.FindPropertyRelative("toColor");
                    EditorGUILayout.PropertyField(fromColor);
                    EditorGUILayout.PropertyField(toColor);
                    break;

                case AnimType.Alpha:
//                            case AnimType.FieldOfViewTween:
                    SerializedProperty fromAlpha = thisTween.FindPropertyRelative("fromFloat");
                    SerializedProperty toAlpha = thisTween.FindPropertyRelative("toFloat");
                    EditorGUILayout.PropertyField(fromAlpha);
                    EditorGUILayout.PropertyField(toAlpha);
                    break;
//                case AnimType.CustomFloat:
//                    SerializedProperty floatMethod = thisTween.FindPropertyRelative("customMethodFloat");
//                    EditorGUILayout.PropertyField(floatMethod);
//                    break;
                default:
                    EditorGUILayout.LabelField("todo" + animType);
                    break;
//                            case AnimType.BezierCurve:
//                                SerializedProperty bezierCurve = thisTween.FindPropertyRelative("_bezierCurve");
//
//                                GUILayout.BeginHorizontal();
//                                EditorGUILayout.PropertyField(bezierCurve, new GUIContent("Curve:"));
//
//                                if (bezierCurve.objectReferenceValue == null)
//                                {
//                                    if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.Width(60)))
//                                    {
//                                        GameObject newObj = new GameObject();
//                                        newObj.name = "Path";
//                                        newObj.transform.position = tar.transform.position;
//                                        bezierCurve.objectReferenceValue = newObj.AddComponent<JBezierCurve>();
//                                        Selection.activeGameObject = newObj;
//                                    }
//                                }
//                                GUILayout.EndHorizontal();
//                                break;
            }
        }

        void SetCurrentValueCustomTn(SerializedProperty thisTween, bool isFromValue)
        {
            SerializedProperty tweenObject = thisTween.FindPropertyRelative("animGameObject");
            SerializedProperty isLocal = thisTween.FindPropertyRelative("isLocal");
            SerializedProperty type = thisTween.FindPropertyRelative("animType");

            SerializedProperty fromVector = thisTween.FindPropertyRelative("fromV3");
            SerializedProperty toVector = thisTween.FindPropertyRelative("toV3");

            SerializedProperty fromVector2 = thisTween.FindPropertyRelative("fromV2");
            SerializedProperty toVector2 = thisTween.FindPropertyRelative("toV2");

            SerializedProperty fromColor = thisTween.FindPropertyRelative("fromColor");
            SerializedProperty toColor = thisTween.FindPropertyRelative("toColor");

            SerializedProperty fromFloat = thisTween.FindPropertyRelative("fromFloat");
            SerializedProperty toFloat = thisTween.FindPropertyRelative("toFloat");

            Vector3 vectorValue = Vector3.zero;
            Color colorValue = Color.white;
            var animType = (AnimType) type.enumValueIndex;
            if (tweenObject.objectReferenceValue != null)
            {
                Transform tweenObj = ((GameObject) (tweenObject.objectReferenceValue)).transform;
                switch (animType)
                {
                    case AnimType.Position:
                        vectorValue = isLocal.boolValue ? tweenObj.localPosition : tweenObj.position;
                        break;
                    case AnimType.Rotate:
                        vectorValue = isLocal.boolValue ? tweenObj.localEulerAngles : tweenObj.eulerAngles;
                        break;
                    case AnimType.Scale:
                        vectorValue = tweenObj.localScale;
                        break;
                    case AnimType.Alpha:
                        fromFloat.floatValue = 0;
                        toFloat.floatValue = 1;
                        break;
                    case AnimType.UiAnchoredPosition:
                        vectorValue = tweenObj.GetComponent<RectTransform>().anchoredPosition;
                        break;
                    case AnimType.UiSize:
                        vectorValue = tweenObj.GetComponent<RectTransform>().sizeDelta;
                        break;
                    case AnimType.Color:
                        var maskableGraphic = tweenObj.GetComponent<MaskableGraphic>();
                        var render = tweenObj.GetComponent<Renderer>();
                        var spriteRender = tweenObj.GetComponent<SpriteRenderer>();
                        var particleSystem = tweenObj.GetComponent<ParticleSystem>();

                        if (maskableGraphic != null)
                            colorValue = maskableGraphic.color;
                        if (spriteRender != null)
                            colorValue = spriteRender.color;
                        else if (render != null)
                        {
                            if (render.sharedMaterial != null)
                                colorValue = render.sharedMaterial.color;
                        }

                        if (particleSystem != null)
                        {
                            var particleMain = particleSystem.main;
                            colorValue = particleMain.startColor.color;
                        }

                        break;
//                    case AnimType.FieldOfViewTween:
//                        Transform cameraTran = (Transform) tweenObject.objectReferenceValue;
//                        Camera camera = cameraTran.GetComponent<Camera>();
//                        if (camera != null)
//                        {
//                            fromFloat.floatValue = camera.fieldOfView;
//                            toFloat.floatValue = camera.fieldOfView;
//                        }
//
//                        break;
//                    case AnimType.BezierCurve:
//                        break;
//                    case AnimType.UIAnchorPosition:
//                        vectorValue = tweenObj.GetComponent<RectTransform>().anchoredPosition;
//                        break;
//                    
//                    case AnimType.UISizeDelta:
//                        vectorValue = tweenObj.GetComponent<RectTransform>().sizeDelta;
//                        break;
                    default:
                        Debug.Log("setValue 未处理的类型");
                        break;
                }
            }

            if (isFromValue)
            {
                fromVector.vector3Value = vectorValue;
                fromVector2.vector2Value = vectorValue;
                if (animType == AnimType.Color)
                {
                    fromColor.colorValue = colorValue;
                }
            }
            else
            {
                toVector.vector3Value = vectorValue;
                toVector2.vector2Value = vectorValue;
                if (animType == AnimType.Color)
                {
                    toColor.colorValue = colorValue;
                }
            }
        }

        void AddTween(SerializedProperty tweens)
        {
            if (addTweenType == AnimType.CustomFloat || addTweenType == AnimType.CustomVector2 ||
                addTweenType == AnimType.CustomVector3 || addTweenType == AnimType.Blink)
            {
                Debug.LogError("暂不支持该类型.请直接使用脚本执行该类型动画.");
                return;
            }

            MonoTweener tar = (MonoTweener) target;

            if (addTweenType == AnimType.UiSize || addTweenType == AnimType.UiAnchoredPosition)
            {
                if (tar.GetComponent<RectTransform>() == null)
                {
                    Debug.LogError("当前物体不是UI,不能添加UI动画");
                    return;
                }
            }

            tweens.arraySize += 1;
            SerializedProperty thisTween = tweens.GetArrayElementAtIndex(tweens.arraySize - 1);
            SerializedProperty tweenObject = thisTween.FindPropertyRelative("animGameObject");
            SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative("isLocal");
            SerializedProperty thisTweenType = thisTween.FindPropertyRelative("animType");
            SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative("curve");


            thisTweenType.enumValueIndex = (int) addTweenType;
            thisTween_IsLocal.boolValue = true;

            thisTween_Curve.animationCurveValue = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

            tweenObject.objectReferenceValue = tar.gameObject;

            SetCurrentValueCustomTn(thisTween, false);
            SetCurrentValueCustomTn(thisTween, true);
        }
    }
}