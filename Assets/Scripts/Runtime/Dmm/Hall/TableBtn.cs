using com.morln.game.gd.command;
using Dmm.Widget;
using UnityEngine.UI;

namespace Dmm.Hall
{
    public class TableBtn : Item<Table>
    {
        public Button Button;

        public Text TableNumber;

        public Text Player1;

        public Text Player2;

        public Text Player3;

        public Text Player4;

        private Table _data;

        public override Table GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, Table data)
        {
            _data = data;

            if (TableNumber)
            {
                TableNumber.text = "";
            }
            if (Player1)
            {
                Player1.text = "";
            }
            if (Player2)
            {
                Player2.text = "";
            }
            if (Player3)
            {
                Player3.text = "";
            }
            if (Player4)
            {
                Player4.text = "";
            }

            if (data != null)
            {
                if (TableNumber)
                {
                    TableNumber.text = "" + (data.table_id + 1);
                }
                if (Player1)
                {
                    Player1.text = data.user1 != null ? data.user1.nickname : "";
                }
                if (Player2)
                {
                    Player2.text = data.user2 != null ? data.user2.nickname : "";
                }
                if (Player3)
                {
                    Player3.text = data.user3 != null ? data.user3.nickname : "";
                }
                if (Player4)
                {
                    Player4.text = data.user4 != null ? data.user4.nickname : "";
                }
            }
        }

        public override void Reset(int currentIndex)
        {
            _data = null;

            if (TableNumber)
            {
                TableNumber.text = "";
            }
            if (Player1)
            {
                Player1.text = "";
            }
            if (Player2)
            {
                Player2.text = "";
            }
            if (Player3)
            {
                Player3.text = "";
            }
            if (Player4)
            {
                Player4.text = "";
            }
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }
    }
}