using Dmm.Data;
using Dmm.Msg;
using Dmm.Session;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Dmm.Game
{
    public class AwardTestWindow : EditorWindow
    {

        [Inject]
        public IMsgRepo MsgRepo;

        private string _awardCode;

        private SceneContext _context;

        public void OnEnable()
        {
            _context = FindObjectOfType<SceneContext>();
            _context.Container.Inject(this);
        }

        public void OnGUI()
        {
            _awardCode = EditorGUILayout.TextField("AwardCode", _awardCode);
            if (GUILayout.Button("请求奖励"))
            {
                var msg = CmdUtil.HU.RequestAward(_awardCode);
                MsgRepo.SendMsg(msg);
            }
        }
    }
}