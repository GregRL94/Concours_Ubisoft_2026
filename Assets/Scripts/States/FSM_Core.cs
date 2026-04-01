using UnityEngine;
// Contrat de base pour chaque etat
public abstract class EnemyState
{
    protected EnemyAI enemy;
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyAI enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter(){}
    public virtual void Update(){}
    public virtual void LogicUpdate(){}
    public virtual void FixedUpdate(){}
    public virtual void Exit(){}
    
    
    // Le cerveau qui gere le switch d'etats
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }

        public void Initialize(EnemyState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(EnemyState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
