using UnityEngine;

namespace Kweek
{
    public interface IState
    {
        void StateUpdate();

        void Enter();
        void Exit();
    }

    public abstract class IAbstractState : MonoBehaviour, IState
    {
        public abstract void StateUpdate();

        public abstract void Enter();
        public abstract void Exit();
    }

    public abstract class IAbstractTargetState : IAbstractState
    {
        public abstract void SetTarget(IDamageableObject target);
    }
}