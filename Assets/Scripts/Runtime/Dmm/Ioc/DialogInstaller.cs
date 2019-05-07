using Dmm.Chat;
using Dmm.Checkin;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Help;
using Dmm.Race;
using Dmm.Shop;
using Dmm.UserTask;
using Dmm.Widget;
using Dmm.YuanBao;
using Zenject;

namespace Dmm.Ioc
{
    public class DialogInstaller : MonoInstaller
    {
        public DialogManager DialogManagerPrefab;

        public CheckinConditionItem CheckinConditionItemPrefab;

        public ExchangeItem ExchangeItemPrefab;

        public CommodityItem CommodityItemPrefab;

        public PrepaymentItem PrepaymentItemPrefab;

        public VipItem VipItemPrefab;

        public YuanBaoItem YuanBaoItemPrefab;

        public RaceItem RaceItemPrefab;

        public RaceHistoryRankItem RaceHistoryRankItemPrefab;

        public DisplayRankItem DisplayRankItemPrefab;

        public HistoryRecordItem HistoryRecordItemPrefab;

        public CurrencyValue CurrencyValuePrefab;

        public JianMengChatItem JianMengItemPrefab;

        public ChatBubble ChatBubblePrefab;

        public YuanBaoExchangeItem YuanBaoExchangeItemPrefab;

        public UserTaskItem UserTaskItemPrefab;

        public TaskItem TaskItemPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IDialogManager>()
                .FromComponentInNewPrefab(DialogManagerPrefab).AsSingle();

            Container.Bind<DialogManager.DialogFactory>().AsSingle();

            Container.BindFactory<ChatBubble, ChatBubble.Factory>()
                .FromComponentInNewPrefab(ChatBubblePrefab);
            Container.BindFactory<JianMengChatItem, JianMengChatItem.Factory>()
                .FromComponentInNewPrefab(JianMengItemPrefab);
            Container.BindFactory<EmojiItem, EmojiItem.Factory>();

            Container.BindFactory<CurrencyValue, CurrencyValue.Factory>()
                .FromComponentInNewPrefab(CurrencyValuePrefab);
            Container.Bind<AwardMoneyObject.Factory>().AsSingle();

            Container.BindFactory<ExchangeItem, ExchangeItem.Factory>()
                .FromComponentInNewPrefab(ExchangeItemPrefab);
            Container.BindFactory<CommodityItem, CommodityItem.Factory>()
                .FromComponentInNewPrefab(CommodityItemPrefab);
            Container.BindFactory<PrepaymentItem, PrepaymentItem.Factory>()
                .FromComponentInNewPrefab(PrepaymentItemPrefab);
            Container.BindFactory<VipItem, VipItem.Factory>()
                .FromComponentInNewPrefab(VipItemPrefab);
            Container.BindFactory<YuanBaoItem, YuanBaoItem.Factory>()
                .FromComponentInNewPrefab(YuanBaoItemPrefab);

            Container.BindFactory<RaceItem, RaceItem.Factory>()
                .FromComponentInNewPrefab(RaceItemPrefab);

            Container.BindFactory<RaceHistoryRankItem, RaceHistoryRankItem.Factory>()
                .FromComponentInNewPrefab(RaceHistoryRankItemPrefab);

            Container.BindFactory<DisplayRankItem, DisplayRankItem.Factory>()
                .FromComponentInNewPrefab(DisplayRankItemPrefab);

            Container.BindFactory<HistoryRecordItem, HistoryRecordItem.Factory>()
                .FromComponentInNewPrefab(HistoryRecordItemPrefab);

            Container.BindFactory<CheckinConditionItem, CheckinConditionItem.Factory>()
                .FromComponentInNewPrefab(CheckinConditionItemPrefab);

            Container.BindFactory<YuanBaoExchangeItem, YuanBaoExchangeItem.Factory>()
                .FromComponentInNewPrefab(YuanBaoExchangeItemPrefab);

            Container.BindFactory<UserTaskItem, UserTaskItem.Factory>()
                .FromComponentInNewPrefab(UserTaskItemPrefab);

            Container.BindFactory<TaskItem, TaskItem.Factory>()
                .FromComponentInNewPrefab(TaskItemPrefab);
        }
    }
}