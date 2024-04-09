using System;
using UnityEngine;

namespace NKOA.Helpers
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        #region Events

        public event Action<AnimationEvent> OnLanded;

        public event Action<AnimationEvent> OnFootStep;

        #endregion


        #region Methods

        private void OnLand(AnimationEvent animationEvent)
        {
            OnLanded?.Invoke(animationEvent);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            OnFootStep?.Invoke(animationEvent);
        }

        #endregion
    }
}