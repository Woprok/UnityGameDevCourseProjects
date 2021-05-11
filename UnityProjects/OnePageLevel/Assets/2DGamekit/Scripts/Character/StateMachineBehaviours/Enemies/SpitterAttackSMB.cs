using UnityEngine;

namespace Gamekit2D
{
    public class SpitterAttackSMB : SceneLinkedSMB<EnemyBehaviour>
    {
        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            m_MonoBehaviour.SetHorizontalSpeed(0);
        }
    }
}