using System.Collections.Generic;
using com.morln.game.gd.command;
using UnityEngine;

namespace Dmm.UserTask
{
    public class UserTaskData
    {
        /// <summary>
        /// 任务按钮图片
        /// </summary>
        public Sprite BtnSprite;

        /// <summary>
        /// 任务选中图片
        /// </summary>
        public Sprite BtnSelectedSprite;

        /// <summary>
        /// 任务类型
        /// </summary>
        public int TaskType;

        /// <summary>
        /// 是否有奖励可领取
        /// </summary>
        public bool CanGetAward;

        /// <summary>
        /// 这个按钮对应的任务列表
        /// </summary>
        public List<UserTaskState> TaskList;
    }
}