using System;
using System.Collections.Generic;

namespace Ako
{
    [Serializable]
    public class AkoSaveData
    {
        public int currentLevel = 0;
        public int personalBestLevel = 0;
        public int coins = 10; // 사용 안 함 (코인은 ICoinService가 관리)
        public int protectionTickets = 0;
        public List<string> ownedItems = new List<string>();
    }
}
