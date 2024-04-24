using UnityEngine;

namespace Demo.CharacterControllerFSM.CharacterControllerFSMStates
{
    public class CharacterControllerFSMGroundedState : CharacterControllerFSMBaseState
    {
        #region Fields
        #endregion
        
        
        
        #region Properties
        #endregion
        
        
        
        #region Constructor
        
        public CharacterControllerFSMGroundedState(CharacterControllerFSM currentContext, CharacterControllerFSMStateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }
        
        #endregion
        
        
        
        #region Unity LifeCycle
        #endregion



        #region Methods

        public override void UpdateState()
        {
            Move();
            CheckSwitchState();

            HandleJumpAndGravity();
        }
        
        public override void CheckSwitchState()
        {
            // Jump
            if (_context.Input.jump && _context.JumpTimeoutDelta <= 0.0f)
            {
                SwitchState(_stateFactory.GetState(CharacterControllerFSMStateType.JUMP));
            }
        }
        
        public override void EnterState()
        {
            base.EnterState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        #endregion
        
        
        
        #region Methods for IMovingCharacter

        public override void HandleJumpAndGravity()
        {
            // reset the fall timeout timer
            _context.FallTimeoutDelta = _context.FallTimeout;

            // update animator if using character
            _context.MyAnimator.SetBool(_context.AnimIDJump, false);
            _context.MyAnimator.SetBool(_context.AnimIDFreeFall, false);
                
            // stop our velocity dropping infinitely when grounded
            if (_context.VerticalVelocity < 0.0f)
            {
                _context.VerticalVelocity = -2f;
            }

            // jump timeout
            if (_context.JumpTimeoutDelta >= 0.0f)
            {
                _context.JumpTimeoutDelta -= Time.deltaTime;
            }
        }

        public override void Move()
        {
            _context.Move();
        }
        
        #endregion
        
    }
}