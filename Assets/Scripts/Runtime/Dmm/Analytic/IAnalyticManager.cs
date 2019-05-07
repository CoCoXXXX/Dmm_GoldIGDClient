using System.Collections.Generic;

namespace Dmm.Analytic
{
    public interface IAnalyticManager
    {
        void Init();
        void StartLevel(string level);
        void FailLevel(string level);
        void FinishLevel(string level);
        void Pay(float money, float coin, int payChannel);
        void Buy(string item, int number, float price);
        void Use(string item, int number, float price);
        void Bonus(float price, int type);
        void Bonus(string item, int number, float price, int trigger);
        void SignIn(string provider, string id);
        void SignIn(string id);
        void Event(string eventId);
        void Event(string eventId, Dictionary<string, string> attrs);
        void EventValue(string eventId, Dictionary<string, string> attrs, int value);
        void PageStart(string page);
        void PageEnd(string page);
    }
}