using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    public class TableLocation : MonoBehaviour
    {
        private IDataContainer<Room> _room;

        private IDataContainer<int> _gameMode;

        [Inject]
        public void Inject(IDataRepository dataRepository)
        {
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _gameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
        }

        public Text GameModeTxt;

        public Text RoomText;

        private void Update()
        {
            if (GameModeTxt)
            {
                var mode = _gameMode.Read();
                GameModeTxt.text = GameMode.LabelOf(mode);
            }

            if (RoomText)
            {
                var room = _room.Read();
                RoomText.text = room != null ? room.room_name : "";
            }
        }
    }
}