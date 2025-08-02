using Script.StateMachine;
using UnityEngine;

namespace Script.PlayerControl.PlayerState
{
    public class PlayerIdle : EntityState
    {
        private Player player;
        
        public PlayerIdle(Entity _entityBase, StateMachine.Machine _machine, string _animBoolName,Player _entity) : base(_entityBase, _machine, _animBoolName)
        {
            player = _entity;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            player.SetZeroVelocity();
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