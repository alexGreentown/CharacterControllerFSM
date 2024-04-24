using UnityEngine;

namespace Demo.CharacterControllerFSM.CharacterControllerFSMStates
{
    public class CharacterControllerFSMJumpState : CharacterControllerFSMBaseState
    {
        #region Fields

        private int _isFlyingHash;

        private float _timerValue = 0f;
        
        #endregion
        
        
        
        #region Properties
        #endregion
        
        
        #region Constructor

        public CharacterControllerFSMJumpState(CharacterControllerFSM currentContext, CharacterControllerFSMStateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }
        
        #endregion
        
        
        
        #region Unity LifeCycle
        #endregion



        #region Methods for ICharacterControllerFSMState

        public override void UpdateState()
        {
            Move();
            HandleJumpAndGravity();
            CheckSwitchState();
        }
        
        public override void CheckSwitchState()
        {
            if (_context.Grounded && _context.JumpTimeoutDelta <= 0.0f)
            {
                SwitchState(_stateFactory.GetState(CharacterControllerFSMStateType.GROUNDED));
            }
            
            if (_context.Input.jump && _context.JumpTimeoutDelta <= 0.0f)
            {
                SwitchState(_stateFactory.GetState(CharacterControllerFSMStateType.FLYING));
            }
        }

        public override void EnterState()
        {   
            base.EnterState();
            
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _context.VerticalVelocity = Mathf.Sqrt(_context.JumpHeight * -2f * _context.Gravity);
                   
            _context.MyAnimator.SetBool(_context.AnimIDJump, true);
            
            // reset the jump timeout timer
            _context.JumpTimeoutDelta = _context.JumpTimeout;

            _context.Input.jump = false;
        }

        public override void ExitState()
        {
            base.ExitState();
            
            // reset the fall timeout timer
            _context.FallTimeoutDelta = _context.FallTimeout;

            _context.MyAnimator.SetBool(_context.AnimIDJump, false);
            _context.MyAnimator.SetBool(_context.AnimIDFreeFall, false);
            
            // stop our velocity dropping infinitely when grounded
            if (_context.VerticalVelocity < 0.0f)
            {
                _context.VerticalVelocity = -2f;
            }
        }

        #endregion
        
        
        
        #region Methods for ICharacterControllerFSMState

        public override void Move()
        {
            _context.Move();
        }
        
        public override void HandleJumpAndGravity()
        {
            // fall timeout
            if (_context.FallTimeoutDelta >= 0.0f)
            {
                _context.FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                _context.MyAnimator.SetBool(_context.AnimIDFreeFall, true);
            }

            // jump timeout
            if (_context.JumpTimeoutDelta >= 0.0f)
            {
                _context.JumpTimeoutDelta -= Time.deltaTime;
            }
        }

        #endregion
        
    }
}