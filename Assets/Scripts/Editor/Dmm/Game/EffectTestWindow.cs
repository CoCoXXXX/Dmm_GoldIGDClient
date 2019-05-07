using UnityEditor;
using UnityEngine;

namespace Dmm.Game
{
    public class EffectTestWindow : EditorWindow
    {
        [MenuItem("IGD/Test/GameEffect")]
        public static void ShowMenu()
        {
            EditorWindow.CreateInstance<EffectTestWindow>().Show();
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("游戏特效测试");
        }

        public void OnGUI()
        {
            if (GUILayout.Button("大炸弹特效"))
            {
                var gameWindow = GameObject.FindObjectOfType<GameWindow>();
                if (gameWindow)
                {
                    gameWindow.ShowDaZhaDan();
                }
            }

            if (GUILayout.Button("小炸弹特效"))
            {
                var gameWindow = GameObject.FindObjectOfType<GameWindow>();
                if (gameWindow)
                {
                    gameWindow.ShowXiaoZhaDan();
                }
            }

            if (GUILayout.Button("飞机特效"))
            {
                var gameWindow = GameObject.FindObjectOfType<GameWindow>();
                if (gameWindow)
                {
                    gameWindow.ShowFeiJi();
                }
            }

            if (GUILayout.Button("同花顺特效"))
            {
                var gameWindow = GameObject.FindObjectOfType<GameWindow>();
                if (gameWindow)
                {
                    gameWindow.ShowTongHuaShun();
                }
            }

            if (GUILayout.Button("火箭特效"))
            {
                var gameWindow = GameObject.FindObjectOfType<GameWindow>();
                if (gameWindow)
                {
                    gameWindow.ShowHuoJian();
                }
            }
        }
    }
}
