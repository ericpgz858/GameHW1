using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip attackAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public int currentHealth = 100;
        public bool controlEnabled;

        public LayerMask enemyLayer;
        public int attackValue;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;
        // added 
        public GameObject Game_Cont;
        [SerializeField]
        public Transform Center;
        [SerializeField]
        public Transform spawnPoint;
        public GameObject HeartCanvas;
        public float hurtTime;
        public float attackTime;
        public bool hasFlower = false;
        private bool isHurt = false;
        private bool timer = false;
        private float startTime;
        private float duration;
        private bool isAttack = false;
        private int attackState = 1;
        private float attackSource_x;
        private int FacingDirection = 1;
        private bool isDead = false;
        private bool recover = false;

        void Awake()
        {
            controlEnabled = false;
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled && !timer)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                {
                    audioSource.PlayOneShot(jumpAudio);
                    jumpState = JumpState.PrepareToJump;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                else if (Input.GetMouseButtonDown(0) && jumpState == JumpState.Grounded)
                {
                    Attack();
                }

                if (move.x > 0.01f && FacingDirection == -1)
                {
                    FacingDirection *= -1;
                    transform.Rotate(0f, 180f, 0f);
                }
                else if (move.x < -0.01f && FacingDirection == 1)
                {
                    FacingDirection *= -1;
                    transform.Rotate(0f, 180f, 0f);
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if(isDead)
            {
                move.x = 0;
                if (Time.time >= startTime + duration && timer)
                {
                    timer = false;
                    Respawn(recover);
                }
            }
            else if (isHurt)
            {
                if ((Time.time - startTime) <= duration/4)
                {
                    move.x = 0.6f;
                    velocity.y = 1 * model.jumpModifier;
                }
                else if((Time.time - startTime) > duration/4 && Time.time < (startTime + duration))
                {
                    move.x = 0.2f;
                    if (velocity.y > 0)
                    {
                        velocity.y = velocity.y * model.jumpDeceleration;
                    }
                }
                else
                {
                    if (velocity.y > 0)
                    {
                        velocity.y = velocity.y * model.jumpDeceleration;
                    }
                    timer = false;
                    isHurt = false;
                    move.x = 0;
                }
                if (attackSource_x >= Center.position.x)
                {
                    move.x *= -1;
                }
            }
            else if (isAttack)
            {
                move.x = 0;
                if (Time.time >= startTime + duration)
                {
                    timer = false;
                    isAttack = false;
                }
            }
            else
            {
                //animator.SetBool("grounded", IsGrounded);
                animator.SetFloat("velocityY", velocity.y / jumpTakeOffSpeed);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            }
            

            targetVelocity = move * maxSpeed;
        }
        private void Attack()
        {
            if (!timer)
            {
                audioSource.PlayOneShot(attackAudio);
                timer = true;
                if (attackState == 1)
                {
                    animator.SetTrigger("attack1");
                    attackState *= -1;
                }
                else
                {
                    animator.SetTrigger("attack2");
                    attackState *= -1;
                }
                isAttack = true;
                SetTimerOn(attackTime);
                /*Collider2D[] hitenemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider2D collider in hitenemy)
                {
                    //Debug.Log("test");
                    //Schedule<EnemyDeath>().enemy = collider.gameObject.GetComponent<EnemyController>();
                    var enemycontroller_Melee = collider.gameObject.GetComponent<MeleeController>();
                    var enemycontroller_archer = collider.gameObject.GetComponent<ArcherController>();
                    if (enemycontroller_Melee != null)
                    {
                        enemycontroller_Melee.Hurt(attackValue);
                    }

                    if (enemycontroller_archer != null)
                    {
                        enemycontroller_archer.Hurt(attackValue);
                    }
                    Debug.Log("We hit " + collider.gameObject.name);
                }*/
            }
        }
        private void SetTimerOn(float t)
        {
            startTime = Time.time;
            duration = t;
            timer = true;
        }
        public void Hurt(float source_x, int value = 10)
        {
            // add timer hurtBool animation
            if(!isHurt)
            {
                audioSource.PlayOneShot(ouchAudio);
                timer = false;
                isAttack = false;
                isHurt = true;
                attackSource_x = source_x;
                SetTimerOn(hurtTime);
                animator.SetTrigger("TakeHit");
                currentHealth -= value;
                HeartCanvas.GetComponent<HeartUIController>().UpHeart(currentHealth);
                if (currentHealth <= 0)
                {
                    Dead(true);
                }
            }
        }
        public void Heal(int value = 10)
        {
            currentHealth += value;
            if (currentHealth > 100)
                currentHealth = 100;
            HeartCanvas.GetComponent<HeartUIController>().UpHeart(currentHealth);
        }
        public void Respawn(bool recoverHealth) 
        {
            if(recoverHealth)
            {
                currentHealth = 100;
                HeartCanvas.GetComponent<HeartUIController>().UpHeart(currentHealth);
            }
            isDead = false;
            Teleport(spawnPoint.position);
            animator.SetBool("Alive", true);
        }
        public void Dead(bool recoverHealth = false)
        {
            if (!isDead)
            {
                animator.SetTrigger("Dead");
                animator.SetBool("Alive", false);
                isDead = true;
                HeartCanvas.GetComponent<HeartUIController>().UpHeart(currentHealth);
                recover = recoverHealth;
                timer = false;
                if (recoverHealth)
                {
                    Game_Cont.GetComponent<GameController>().GameOver();
                }
                else
                {
                    SetTimerOn(2f);
                }
            }        
        }
        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}