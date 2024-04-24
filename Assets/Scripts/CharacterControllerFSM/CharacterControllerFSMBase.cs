using Demo.CharacterControllerFSM.CharacterControllerFSMStates;
using UnityEngine;

namespace Demo.CharacterControllerFSM
{
    public abstract class CharacterControllerFSMBase : MonoBehaviour
    {
        #region Fields

        protected CharacterControllerFSMBaseState _currentState;

        protected CharacterControllerFSMStateFactory _stateFactory;
        
        #endregion
        
        
        
        #region Properties
        
        public CharacterControllerFSMBaseState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        #endregion
        
        
        
        #region Methods
        
        #endregion
        
    }
}
