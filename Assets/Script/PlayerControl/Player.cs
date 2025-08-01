using Script.PlayerControl.PlayerState;
using Script.StateMachine;
using Unity.VisualScripting;

namespace Script.PlayerControl
{
    /// <summary>
    /// Player实例脚本
    /// </summary>
    public class Player : Entity
    {
        #region 状态机和状态
        
        public Machine machine { get; private set; }
        public PlayerIdle idleState { get; private set; }
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            machine = new Machine();
            idleState = new PlayerIdle(this, machine, "Idle", this);
        }

        protected override void Start()
        {
            base.Start();
            
            //切入初始状态
            machine.Initialize(idleState);
        }

        protected override void Update()
        {
            base.Update();
            
            //状态的帧执行
            machine.currentState.OnUpdate();
        }
    }
}