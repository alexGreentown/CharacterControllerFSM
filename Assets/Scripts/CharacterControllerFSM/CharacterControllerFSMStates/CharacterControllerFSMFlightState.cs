using UnityEngine;

namespace Demo.CharacterControllerFSM.CharacterControllerFSMStates
{
    public class CharacterControllerFSMFlightState : CharacterControllerFSMBaseState
    {
        #region Fields
        #endregion
        
        
        
        #region Properties
        #endregion
        
        
        #region Constructor
        
        public CharacterControllerFSMFlightState(CharacterControllerFSM currentContext, CharacterControllerFSMStateFactory stateFactory) : base(currentContext, stateFactory)
        { }
        
        #endregion
        
        
        
        #region Unity LifeCycle
        #endregion



        #region Methods for ICharacterControllerFSMState

        public override void UpdateState()
        {
            CheckSwitchState();
            Move();
            HandleJumpAndGravity();
        }

        public override void CheckSwitchState()
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                SwitchState(_stateFactory.GetState(CharacterControllerFSMStateType.JUMP));
            }
        }

        public override void EnterState()
        {
            base.EnterState();
            
            _context.MyAnimator.SetBool(_context.AnimIDFlying, true);
        }

        public override void ExitState()
        {
            base.ExitState();
            
            _context.MyAnimator.SetBool(_context.AnimIDFlying, false);
        }

        #endregion
        
        
        
        #region Methods for IMovingCharacter
        
        public override void Move()
        {
            _context.Move();
        }

        public override void HandleJumpAndGravity()
        {
            _context.VerticalVelocity = 5f;
        }
        
        #endregion
    }
}