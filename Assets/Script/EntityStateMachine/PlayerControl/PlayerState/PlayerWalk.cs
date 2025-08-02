using Script.StateMachine;
using UnityEngine;

namespace Script.PlayerControl.PlayerState
{
    public class PlayerWalk : EntityState
    {
        private Player player;
        public PlayerWalk(Entity _entityBase, Machine _machine, string _animBoolName,Player _entity) : base(_entityBase, _machine, _animBoolName)
        {
            player = _entity;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}