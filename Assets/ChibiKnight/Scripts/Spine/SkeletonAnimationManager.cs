using Spine.Unity;

namespace DChild.Gameplay
{
    public class SkeletonAnimationManager : GameplayModuleManager<SpineAnimation>
    {
        public override string name => "SkeletonAnimationManager";

        public void UpdateModule(float deltaTime)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].UpdateAnimation(deltaTime);
            }
        }
    }
}

