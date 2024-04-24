using System;
using UnityEngine;

namespace Demo.CharacterControllerFSM.CharacterControllerFSMStates
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
        {
            Debug.Log($"EnterState() {GetType().Name}");
        }


        public virtual void ExitState()
        {
            Debug.Log($"ExitState() {GetType().Name}");
        }

        public virtual void SwitchState(CharacterControllerFSMBaseState newState)
        {
            Debug.Log($"SwitchState() {GetType().Name}");
            
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