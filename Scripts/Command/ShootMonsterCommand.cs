using QFramework;
using UnityEngine;

namespace QFramework
{
    public class ShootMonsterCommand: AbstractCommand
    {
        private GameObject target;

        public ShootMonsterCommand(GameObject target)
        {
            this.target = target;
        }

        protected override void OnExecute()
        {
            this.GetSystem<IObjectPoolSystem>().Recovery(target);
        }
    }
}
