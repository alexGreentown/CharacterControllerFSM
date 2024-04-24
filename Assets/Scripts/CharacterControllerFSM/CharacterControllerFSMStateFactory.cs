using System.Collections.Specialized;
using Demo.CharacterControllerFSM.CharacterControllerFSMStates;

namespace Demo.CharacterControllerFSM
{
    public enum CharacterControllerFSMStateType
    {
        JUMP,
        GROUNDED,
        FLYING,
        SWIMMING
    }
    
    public class CharacterControllerFSMStateFactory
    {
        #region Constructor
        
        public CharacterControllerFSMStateFactory(CharacterControllerFSM context)
        {
            _context = context;
            
            InitializeStates();
        }
        
        #endregion
        
        
        
        #region Fields

        private CharacterControllerFSM _context;

        private ListDictionary _states = new ListDictionary();
        
        #endregion


        
        #region Methods
        
        private void InitializeStates()
        {
            _states.Add(CharacterControllerFSMStateType.GROUNDED, Grounded());
            _states.Add(CharacterControllerFSMStateType.JUMP, Jump());
            _states.Add(CharacterControllerFSMStateType.FLYING, Fly());
        }

        public CharacterControllerFSMBaseState GetState(CharacterControllerFSMStateType stateType)
        {
            return (CharacterControllerFSMBaseState)_states[stateType];
        }
        
        public CharacterControllerFSMBaseState Jump()
        {
            return new CharacterControllerFSMJumpState( _context, this);
        }
        
        public CharacterControllerFSMBaseState Grounded()
        {
            return new CharacterControllerFSMGroundedState( _context, this);
        }
        
        public CharacterControllerFSMBaseState Fly()
        {
            return new CharacterControllerFSMFlightState( _context, this);
        }
        
        #endregion
    }
}
