using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKOA.CharacterControllerFSM
{

    public interface IMovingCharacter
    {
        void HandleJumpAndGravity();

        void Move();
    }
}
