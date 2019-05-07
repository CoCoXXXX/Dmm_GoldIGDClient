using UnityEditor;
using UnityEngine;

namespace Dmm.Base
{
    /// <summary>
    /// 自定义的编辑器风格变量。
    /// </summary>
    public class MyEditorStyles 
    {

        private readonly static GUIStyle Divider = new GUIStyle();

        static MyEditorStyles()
        {
            // 设置分界线。
            Divider.normal.background = EditorGUIUtility.whiteTexture;
            Divider.stretchWidth = true;
            Divider.margin = new RectOffset(0, 0, 7, 7);

            // 折叠标题。
            FoldoutTitle.normal.textColor = Color.white;
            FoldoutTitle.fontSize = 14;
            FoldoutTitle.fontStyle = UnityEngine.FontStyle.Bold;

            // 标题文字。
            TitleFont.normal.textColor = Color.white;
            TitleFont.fontSize = 14;
            TitleFont.fontStyle = UnityEngine.FontStyle.Bold;

            SaveButton.fixedHeight = 30;
            SaveButton.fontSize = 16;
            SaveButton.fontStyle = UnityEngine.FontStyle.Bold;
            SaveButton.normal.textColor = Color.yellow;
        }

        /// <summary>
        /// 小按钮。
        /// </summary>
        public readonly static GUILayoutOption MiniBtnWidth = GUILayout.Width(20);

        /// <summary>
        /// 中等大小按钮。
        /// </summary>
        public static readonly GUILayoutOption MidBtnWidth = GUILayout.Width(50);

        /// <summary>
        /// 标题文字。
        /// </summary>
        public readonly static GUIStyle TitleFont = new GUIStyle();

        /// <summary>
        /// 折叠标题栏。
        /// </summary>
        public readonly static GUIStyle FoldoutTitle = new GUIStyle(EditorStyles.foldout);

        /// <summary>
        /// 醒目的保存按钮。
        /// </summary>
        public readonly static GUIStyle SaveButton = new GUIStyle(GUI.skin.button);

        public static void Separator(Color color, float thickness = 1)
        {
            EditorGUILayout.Space();
            var position = GUILayoutUtility.GetRect(GUIContent.none, Divider, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                var restoreColor = GUI.color;
                GUI.color = color;
                Divider.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
            EditorGUILayout.Space();
        }

        public static GUIStyle FontStyle(Color color, int fontSize, bool bold)
        {
            var style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = fontSize;
            if (bold) style.fontStyle = UnityEngine.FontStyle.Bold;

            return style;
        }

    }
}