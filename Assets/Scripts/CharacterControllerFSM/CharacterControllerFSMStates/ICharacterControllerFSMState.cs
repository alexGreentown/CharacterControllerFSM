namespace Demo.CharacterControllerFSM.CharacterControllerFSMStates
{
    public interface ICharacterControllerFSMState
    {
        void EnterState();

        void ExitState();
        
        void CheckSwitchState();
    }
}
