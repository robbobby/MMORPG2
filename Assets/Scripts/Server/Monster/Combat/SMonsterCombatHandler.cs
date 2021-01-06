using System;
using System.Collections;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using NUnit.Framework.Constraints;
using Server.Player;
using Server.PlayerMonster;
using Shared;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Monster.Combat {
    [RequireComponent(typeof(SMonsterFlags))]
    public class SMonsterCombatHandler : NetworkBehaviour {
        private static readonly int AttackAnimation = Animator.StringToHash("AttackAnimation");
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        private static readonly int Attack = Animator.StringToHash("attack");
        private MonsterState m_monsterState;
        private GameObject[] m_players;
        private SMonsterStats m_stats;
        private SMonsterFlags m_flags;
        private SMovement m_movement;
        private Animator m_animator;
        private GameObject m_target;
        private Vector3 m_lastKnowLocation;
        private bool m_resetBackToStartPosition = false;
        private Vector3 m_guardPosition;
        private bool m_isAttackingAnimation;
        private PlayerStateScheduler m_targetState;
        private void Start() {
            m_guardPosition = this.transform.position;
            m_flags = GetComponent<SMonsterFlags>();
            m_stats = GetComponent<SMonsterStats>();
            m_movement = GetComponent<SMovement>();
            m_animator = GetComponent<Animator>();
            StartCoroutine(StateCoroutine());
            m_monsterState = MonsterState.Idle;
            m_lastKnowLocation = default;
        }
        [Server] private IEnumerator StateCoroutine() {
            while(m_flags.IsAlive) {
                float waitTime = 0.1f;
                switch (m_monsterState) {
                    case MonsterState.Idle:
                        yield return IdleStateHandler();
                        break;
                    case MonsterState.Patrol:       // Search for targets // Move around to new positions within area
                        break;
                    case MonsterState.Suspicious:   // After target disappears move to last known location // If player not in sight, wait 2 seconds then set idle state
                        yield return SuspiciousStateHandler();
                        break;
                    case MonsterState.MovingToTarget:
                        yield return MovingToTargetHandler();
                        break;
                    case MonsterState.Attacking:    // Attacking the entity
                        yield return AttackingStateHandler();
                        break;
                    case MonsterState.Dead:         // Set IsAlive to false and target component, remove all controls from monster
                        yield return new WaitForSeconds(waitTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private YieldInstruction AttackingStateHandler() {
            float waitTime = 0.1f;
            if (m_isAttackingAnimation) return new WaitForSeconds(1);
            if (!m_targetState.IsAlive) {
                m_target = null;
                m_monsterState = MonsterState.Suspicious;
            }
            else if (InAttackRange(m_target.transform.position))
                AttackTarget(m_lastKnowLocation);
            else if (IsInSightRange(m_target.transform.position))
                m_monsterState = MonsterState.MovingToTarget;
            else
                m_monsterState = MonsterState.Idle;
            return new WaitForSeconds(waitTime);
        }
        [ClientRpc] private void AttackTarget(Vector3 targetPosition) {
            m_isAttackingAnimation = true;
            m_animator.SetFloat(AttackAnimation, UnityEngine.Random.Range(0.01f, 0.99f));
            this.transform.LookAt(targetPosition);
            m_animator.SetTrigger(Attack);
        }
        [Server] private YieldInstruction MovingToTargetHandler() {
            float waitTime = 0.1f;
            m_lastKnowLocation = m_target.transform.position;
            if (IsInSightRange(m_target.transform.position) && !InAttackRange(m_lastKnowLocation)) {
                waitTime = 1f;
                MoveToTarget(m_target.transform.position);
            }

            if (InAttackRange(m_lastKnowLocation)) m_monsterState = MonsterState.Attacking;
            if (!IsInSightRange(m_target.transform.position)) m_monsterState = MonsterState.Suspicious;
            return new WaitForSeconds(waitTime);
        }
        [Server] private YieldInstruction IdleStateHandler() {
            float waitTime = 0.1f;
            if (SearchForTarget()) { // Remove when UseIdleAiSet() is implemented
                m_lastKnowLocation = m_target.transform.position;
                m_targetState = m_target.GetComponent<PlayerStateScheduler>();
                m_monsterState = MonsterState.MovingToTarget;
                return new WaitForSeconds(waitTime);
            }
            else {
                return new WaitForSeconds(waitTime = 1.5f);
            }
        }
        [Server] private YieldInstruction SuspiciousStateHandler() {
            float waitTime = 0.1f;
            if (m_resetBackToStartPosition && InAttackRange(m_lastKnowLocation)) {
                MoveToTarget(m_guardPosition);
                m_monsterState = MonsterState.Idle;
                return new WaitForSeconds(2.5f);
            }
            if (InAttackRange(m_lastKnowLocation)) {
                StopMovement();
                waitTime = 1.5f;
                m_resetBackToStartPosition = true;
            } else {
                m_resetBackToStartPosition = false;
                MoveToTarget(m_lastKnowLocation);
                waitTime = 1.5f;
            }
            return new WaitForSeconds(waitTime);
        }
        [Server] private bool InAttackRange(Vector3 playerPosition) =>
            Vector3.Distance(playerPosition, this.transform.position) <= 4f;
        [Server] private bool SearchForTarget() {
            m_players = GameObject.FindGameObjectsWithTag("Player");
            return m_players.Length != 0 && CheckIfShouldAttack();
        }
        [Server] private bool CheckIfShouldAttack() {
            foreach (GameObject player in m_players) {
                var targetState = player.GetComponent<PlayerStateScheduler>();
                if (targetState.IsAlive && IsInSightRange(player.transform.position)) {
                    m_target = player;
                    m_monsterState = MonsterState.MovingToTarget;
                    return true;
                }
                // Collecting targets here, returns after 1 target
                // Get all targets later and make new function to decide which to attack
            }
            return false;
        }
        [Server] private bool IsInSightRange(Vector3 playerPosition) =>
            Vector3.Distance(playerPosition, transform.position) < m_stats.sightDistance;
        [Server] private void AE_EndAnimation() {
            m_isAttackingAnimation = false;
        }
        [Server] private void MoveToTarget(Vector3 playerPosition) {
            m_movement.RpcSetMove(playerPosition);
        }
        [Server] private void StopMovement() {
            m_movement.RpcStopMovement();
        }
        [Server] private void AE_Hit() {
            SPlayerCombatHandler combatHandler = GetComponent<SPlayerCombatHandler>();
            Target target = m_target.GetComponent<Target>();
            combatHandler.RpcTakeDamage(target , 5);
        }
    }
}
