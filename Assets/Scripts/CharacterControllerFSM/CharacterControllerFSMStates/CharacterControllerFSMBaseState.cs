using System;
using UnityEngine;

namespace NKOA.CharacterControllerFSM.CharacterControllerFSMStates
{
    public abstract class CharacterControllerFSMBaseState : ICharacterControllerFSMState, IMovingCharacter
    {
        #region Fields

        protected CharacterControllerFSM _context;

        protected CharacterControllerFSMStateFactory _stateFactory;
        
        #endregion
        
        
        
        #region Unity LifeCycle

        public CharacterControllerFSMBaseState(CharacterControllerFSM currentContext, CharacterControllerFSMStateFactory stateFactory)
        {
            _context = currentContext;
            _stateFactory = stateFactory;
        }

        public virtual void UpdateState()
        { }

        #endregion

        

        #region Methods for ICharacterControllerFSMState
        
        public virtual void EnterState()
        { }
        
        
        public abstract void ExitState();

        public virtual void SwitchState(CharacterControllerFSMBaseState newState)
        {
            ExitState();
            newState.EnterState();
            
            _context.CurrentState = newState;
        }

        public abstract void CheckSwitchState();
        
        #endregion
        
        
        
        #region Methods for IMovingCharacter

        public abstract void HandleJumpAndGravity();

        public abstract void Move();


        #endregion

    }
    
}