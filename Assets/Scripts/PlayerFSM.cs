using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace  PlayerState
{
    
    public class PlayerFSM: MonoBehaviour
    {
        public BaseState state;
        
        public void ChangeState(BaseState nextState)
        {
            if (state != null)
            {
                state.OnStateExit();
            }

            state = nextState;
            state.OnStateEnter();
        }

        private void Update()
        {
            if (state != null)
            {
                state.OnStateUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (state != null)
            {
                state.OnStateFixedUpdate();
            }
        }

        public PlayerState GetState()
        {
            return state.GetState();
        }
    }

    public abstract class BaseState
    {
        protected Player _player;

        protected BaseState(Player player, params int[] args)
        {
            _player = player;
        }
        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();
        public abstract void OnStateFixedUpdate();
        public abstract PlayerState GetState();
    }

    
    public class IdleState : BaseState
    {
        public IdleState(Player player) : base(player){}

        public override void OnStateEnter()
        {
            _player.animator.CrossFade("Idle",0.2f);
        }

        public override void OnStateUpdate()
        {
            //공격 버튼
            _player.HandleAttack(2);
            //점프 버튼
            _player.HandleJump();
            //스킬?

            _player.HandleDash();
        }

        public override void OnStateFixedUpdate()
        {
            //이동
            _player.HandleMovement();
        }

        public override void OnStateExit()
        {
            
        }

        public override PlayerState GetState()
        {
            return PlayerState.Idle;
        }
    }
    
    public class MovingState : BaseState
    {
        public MovingState(Player player) : base(player){}

        public override void OnStateEnter()
        {
            _player.animator.CrossFade("Move",0.2f);
        }

        public override void OnStateUpdate()
        {
            //공격 버튼
            _player.HandleAttack(2);
            //점프 버튼
            _player.HandleJump();
            //스킬?
            
            _player.HandleDash();
        }
        
        public override void OnStateFixedUpdate()
        {
            //이동
            _player.HandleMovement();
        }

        public override void OnStateExit()
        {
            _player.Stop();
        }

        public override PlayerState GetState()
        {
            return PlayerState.Moving;
        }
    }
    
    public class AttackingState : BaseState
    {
        private int attackCombo = 0;
        private int animationHash = 0;
        public AttackingState(Player player, int attackCombo) : base(player)
        {
            this.attackCombo = attackCombo;
        }

        public override void OnStateEnter()
        {
            animationHash = Animator.StringToHash("Attack" + attackCombo);
            _player.animator.Play(animationHash);
            _player.SetAttackAble(false);
        }

        public override void OnStateUpdate()
        {
            _player.HandleAttack(attackCombo);

            if (_player.IsAnimationEnd(animationHash))
            {
                
                _player.animator.SetTrigger("AnimationEnd");
                _player.ChangeState(new IdleState(_player));
            }

            _player.HandleDash();
            _player.Testvalue = _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

        public override void OnStateExit()
        {
            _player.SetAttackAble();
            _player.TrailOff();
        }

        public override void OnStateFixedUpdate()
        {
        }

        public override PlayerState GetState()
        {
            return PlayerState.Attacking;
        }
    }

    public class SkillState : BaseState
    {
        private int skillArgument = 0;
        private int animationHash = 0;

        public SkillState(Player player, int skillArgument) : base(player, skillArgument)
        {
            this.skillArgument = skillArgument;
        }

        public override void OnStateEnter()
        {
            Debug.Log("SkillOn");
            
            animationHash = Animator.StringToHash("Skill" + skillArgument);
            _player.animator.Play(animationHash);
        }

        public override void OnStateUpdate()
        {
            if (_player.IsAnimationEnd(animationHash))
            {
                _player.ChangeState(new IdleState(_player));
            }
        }

        public override void OnStateExit()
        {
        }

        public override void OnStateFixedUpdate()
        {
        }

        public override PlayerState GetState()
        {
            return PlayerState.Skill;
        }
    }
    
    
    public class JumpState : BaseState
    {
        private int animationHash = 0;

        public JumpState(Player player) : base(player)
        { }

        public override void OnStateEnter()
        {
            Debug.Log("JumpStart");
            animationHash = Animator.StringToHash("Jump");
            _player.animator.CrossFade(animationHash,0.1f);
            _player.Jump();
        }

        public override void OnStateUpdate()
        {
            if (_player.IsAnimationEnd(animationHash))
            {
                _player.ChangeState(new IdleState(_player));
            }
            
            _player.HandleDash();
        }

        public override void OnStateExit()
        {
            
        }

        public override void OnStateFixedUpdate()
        {
            //땅에 닿을경우 idle로
            if (_player.rb.velocity.y < -0.1f && _player.CheckGrounded())
            {
                Debug.Log("Grounded");
                _player.ChangeState(new IdleState(_player));
            }
            
            //이동
            _player.HandleMovement();
        }

        public override PlayerState GetState()
        {
            return PlayerState.Jumping;
        }
    }
    
    public class ShieldDashState : BaseState
    {
        private int animationHash = 0;

        public ShieldDashState(Player player) : base(player)
        { }

        public override void OnStateEnter()
        {
            _player.ChangeRotationImmediatelyByKey();
            Debug.Log("ShieldDash");
            animationHash = Animator.StringToHash("ShieldDash");
            _player.animator.Play(animationHash);
        }

        public override void OnStateUpdate()
        {
            if (_player.IsAnimationEnd(animationHash))
            {
                _player.ChangeState(new IdleState(_player));
            }
        }

        public override void OnStateExit()
        {
            
        }

        public override void OnStateFixedUpdate()
        {
        }

        public override PlayerState GetState()
        {
            return PlayerState.ShieldDash;
        }
    }

    public enum PlayerState
    {
        Idle,
        Attacking,
        Moving,
        Jumping,
        Skill,
        JumpingAtk,
        ShieldDash
    }
}
