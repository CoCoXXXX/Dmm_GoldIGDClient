using System;
using Dmm.Base;
using Dmm.Common;
using Dmm.MoreFunction;
using Dmm.Util;
using UnityEditor;
using UnityEngine;

namespace Dmm.Game
{
	public class RankMeTestWindow : EditorWindow
	{

		public void OnEnable()
		{
			title = "RankMeDialog";
		}

		private float _days = 0;

		public void OnGUI()
		{
			if (GUILayout.Button("清空记录")) 
			{
				PrefsUtil.DeleteKey(RankMeDialog.RankMeShowKey);
				PrefsUtil.DeleteKey(RankMeDialog.RankMeShowTimeKey);
				PrefsUtil.Flush();
			}

			var shown = PrefsUtil.GetBool(RankMeDialog.RankMeShowKey, false);
			var newValue = EditorGUILayout.Toggle("已显示", shown);
			if (newValue != shown)
			{
				PrefsUtil.SetBool(RankMeDialog.RankMeShowKey, newValue);
				PrefsUtil.Flush();
			}

			var time = PrefsUtil.GetLong(RankMeDialog.RankMeShowTimeKey, 0);
			EditorGUILayout.LabelField("显示时间: " + time);
			if (GUILayout.Button("设置为当前时间")) 
			{
				PrefsUtil.SetLong(RankMeDialog.RankMeShowTimeKey, DateTime.Now.CurrentTimeMillis());
				PrefsUtil.Flush();
			}

			_days = EditorGUILayout.FloatField("天数变化", _days);
			if (GUILayout.Button("设置天数变化")) {
				time = PrefsUtil.GetLong(RankMeDialog.RankMeShowTimeKey, 0);
				PrefsUtil.SetLong(RankMeDialog.RankMeShowTimeKey, time + (long)(1000*3600*24*_days));
				PrefsUtil.Flush();
			}
		}

	}
}
