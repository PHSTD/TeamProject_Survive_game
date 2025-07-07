using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.WorkPlace.LHG.Scripts.EventSystem
{
    public class EC10001 : EventController
    {
        public double durabilityPlusAmount = 10;
        protected override void OnEventComplete()
        {
            // 이벤트 완료 시 추가 작업을 수행할 수 있습니다.
            base.OnEventComplete();
            ActivateEvent();
            StatusSystem.Instance.SetPlusDurability(durabilityPlusAmount);

            // 예: 데이터 저장, UI 업데이트 등
            Console.WriteLine("Event EC10001 completed successfully.");
            RefreshButton();
        }
    }
}