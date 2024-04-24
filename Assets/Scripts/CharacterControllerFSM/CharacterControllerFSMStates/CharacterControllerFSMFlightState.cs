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



        #region Methods

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

        public override void HandleJumpAndGravity()
        {
            _context.VerticalVelocity = 5f;
        }

        public override void Move()
        {
            _context.Move();
        }

        public override void EnterState()
        {
            Debug.Log("Enter Flight state");
        }

        public override void ExitState()
        {
            Debug.Log("Exit Flight state");
        }

        #endregion
    }
}