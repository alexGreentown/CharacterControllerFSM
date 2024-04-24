using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.CharacterControllerFSM
{

    public interface IMovingCharacter
    {
        void HandleJumpAndGravity();

        void Move();
    }
}
