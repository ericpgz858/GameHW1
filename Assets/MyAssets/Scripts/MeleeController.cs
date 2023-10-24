using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Unity.VisualScripting;
using UnityEngine.Windows;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class MeleeController : KinematicObject
    {
        public AudioClip attackAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /*internal new*/
        private Collider2D collider2d;
        private Rigidbody2D rigidbody2d;

        /*internal new*/
        public AudioSource audioSource;
        public int  health;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        // added
        [SerializeField]
        public Transform LedgeCheck;
        [SerializeField]
        public Transform WallCheck;
        [SerializeField]
        public Transform playerCheck;
        [SerializeField]
        private Transform Player_T;
        public GameObject MG_monster;
        
        private float wallCheckDistance = 0.02f;
        private float ledgeCheckDistance = 0.4f;
        public float AttackDistance = 0.7f;
        private float playerCheckDistance = 3f;
        public LayerMask whatIsGround;
        public LayerMask whatIsPlayer;

        private int FacingDirection = 1;
        private float startTime;
        public  float attackTime = 1.9f;
        public  float idleTime = 3f;
        public  float turnBackTime = 0.5f;
        public  float hurtTime = 1.0f;
        private float duration;
        private bool timer = false;
        private bool inHurt = false;
        private bool inDead = false;
        private Vector3 spawnPosition;
        private enum State
        {
            Idle,
            Walk,
            Attack,
            Hurt,
            Dead
        }
        private State enemyState = State.Walk;

        void Awake()
        {
            spawnPosition = transform.position;
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            rigidbody2d = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if(inDead == true)
            {
                enemyState = State.Dead;
            }
            else if(inHurt)
            {
                if(timer == false)
                {
                    SetTimerOn(hurtTime);
                    enemyState = State.Hurt;
                }
                else if (Time.time >= startTime + duration)
                {
                    timer = false;
                    inHurt = false;
                    enemyState = State.Idle;
                }
            }
            else if (CheckPlayerInBack())
            {
                Flip();
                timer = false;
                SetTimerOn(turnBackTime);
                enemyState = State.Idle;
            }
            else if ((CheckWall() || !Checkedge()))
            {
                if(timer == false)
                {
                    SetTimerOn(idleTime);
                    enemyState = State.Idle;
                }
                else if(Time.time >= startTime + duration)
                {
                    timer = false;
                    Flip();
                    enemyState = State.Walk;
                }
            }
            else if (timer == true)
            {
                enemyState = State.Idle;
                if (Time.time >= startTime + duration)
                {
                    timer = false;
                    enemyState = State.Walk;
                }
            }
            else if (CheckPlayerInAttackRange())
            {
                if (timer == false)
                {
                    audioSource.PlayOneShot(attackAudio);
                    SetTimerOn(attackTime);
                    enemyState = State.Attack;
                    //Debug.Log("attack");
                }
                else if (Time.time >= startTime + idleTime)
                {
                    timer = false;
                    enemyState = State.Idle;
                }
            }
            else 
            {
                enemyState = State.Walk;
            }
            base.Update();
        }

        protected override void ComputeVelocity()
        {
            if (enemyState == State.Walk)// walk and search player
            {
                move.x = 1 * FacingDirection;
                animator.SetBool("Attack", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Walk", true);
            }else if (enemyState == State.Idle)
            {
                move = Vector3.zero;
                animator.SetBool("Attack", false);
                animator.SetBool("Walk", false);
                animator.SetBool("Idle", true);
            }
            else if(enemyState == State.Attack)
            {
                move = Vector3.zero;
                Attack();
                animator.SetTrigger("Attack");
            }
            else if (enemyState == State.Hurt)
            {
                move.x = -1 * 0.2f * FacingDirection;
            }
            else
            {
                move = Vector3.zero;
            }

            targetVelocity = move * maxSpeed;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            //Debug.Log(collision.gameObject.tag);
            if (player != null)
            {
                player.Hurt(playerCheck.position.x);
            }
        }
        private void SetTimerOn(float t)
        {
            startTime = Time.time;
            duration = t;
            timer = true;
        }
        private void Flip()
        {
            FacingDirection *= -1;
            transform.Rotate(0f, 180f, 0f);
        }
        public virtual bool CheckPlayerInAttackRange()
        {
            return Physics2D.Raycast(transform.position, transform.right, AttackDistance, whatIsPlayer);
        }
        public virtual bool CheckPlayerInBack()
        {
            if(Vector2.Distance(Player_T.position, transform.position) <= playerCheckDistance && (Player_T.position.x- transform.position.x)*FacingDirection < 0)
            {
                return true;
            }
            return false;
            //return Physics2D.Raycast(transform.position, transform.right*-1, 10, whatIsPlayer);
        }
        private bool CheckWall()
        {
            return Physics2D.Raycast(WallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        }
        private bool Checkedge()
        {
            return Physics2D.Raycast(LedgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
        }
        public void Hurt(int value)
        {
            audioSource.PlayOneShot(ouchAudio);
            timer = false;
            inHurt = true;
            health -= value;
            animator.SetTrigger("TakeHit");
            if (health <= 0)
            {
                Dead();
            }
        }
        public void respawn()
        {
            if (inDead)
            {
                animator.SetTrigger("Respawn");
                collider2d.enabled = true;
                rigidbody2d.simulated = true;
                inDead = false;
            }
            health = 100;
            transform.position = spawnPosition;
        }
        public void Dead()
        {
            animator.SetTrigger("Dead");
            collider2d.enabled = false;
            rigidbody2d.simulated = false;
            inDead = true;
            MG_monster.GetComponent<ManageMonster>().killMonster++;
        }
        private void Attack()
        {
            
        }
    }
}