using System;
using System.Collections;
using _Scripts.Cards.Sabotage;
using UnityEngine;

namespace _Scripts.Racer
{
    public class RacerBase : MonoBehaviour
    {
        protected Track track;
        private Rigidbody2D racerRigidbody2d;
        private Collider2D collider2D;
        protected float scaledFixedDeltaTime;
        private float respawnTimeRemaining;

        [Header("Card Components")]
        [SerializeField] protected CardDeck deck;
        [SerializeField] private BoostComponent boost;
        [SerializeField] private BrakeComponent brake;
        [SerializeField] private ShortcutComponent shortcut;
        [SerializeField] private JumpComponent jumpComponent;
        [SerializeField] private SabotageComponent sabotage;
        public CardBase triggerCard;

        [Header("Cards")]
        [SerializeField] private int startingHand = 4;

        [Header("Lap Progress")]
        [SerializeField] [ReadOnly] private float distanceAlongTrack;
        public float DistanceAlongTrack => distanceAlongTrack;
        protected Vector3 positionOnTrackSpline;
        protected Vector3 tangentOnTrackSpline;

        [Header("Movement")]
        [SerializeField] [ReadOnly] private float currentSpeed;
        public float CurrentSpeed => currentSpeed;
        private Vector3 currentHeading;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 1f;

        [Header("Whiskers")]
        [SerializeField] private float forwardsWhiskerLength;
        [SerializeField] private float sideWhiskerLength;
        [SerializeField] private float sideWhiskerAngle;
        [SerializeField] private float sideWhiskerTurnAnglePerSecond;
        private Vector3 whiskerFront;
        private Vector3 whiskerRight;
        private Vector3 whiskerLeft;

        [Header("Alignment")]
        [SerializeField] private float alignmentTurnAnglePerSecond = 5f;

        [Header("Respawning")]
        [ReadOnly] public bool isRespawning;
        public float respawnStartDelay;
        public float respawnDuration;

        [Header("Debug")]
        public float positionGizmoRadius = 1f;

        [Header("Audio")]
        [SerializeField] private AudioClip crashSound;
        [SerializeField] private float crashSoundVolume = 0.5f;

        protected virtual void OnEnable()
        {
            RaceManager.OnRaceStarted += BeginRace;
        }

        protected virtual void OnDisable()
        {
            RaceManager.OnRaceStarted -= BeginRace;
        }

        private void Awake()
        {
            track = FindFirstObjectByType<Track>();
            racerRigidbody2d = GetComponent<Rigidbody2D>();
            collider2D = GetComponentInChildren<Collider2D>();
        }

        private void BeginRace(int numberOfLaps)
        {
            currentHeading = transform.up;

            // Setup Racers Deck
            deck = GetComponent<CardDeck>();
            deck?.SetupDeck();
            for (int i = 0; i < startingHand; i++)
            {
                deck?.DrawCard();
            }
        }

        protected virtual void Update()
        {
            UpdateTrackPosition();

            if (!RaceManager.Instance.HasRaceStarted)
            {
                return;
            }
            if(!isRespawning && !shortcut.IsInShortcut && !jumpComponent.IsJumping && Vector2.Distance(transform.position, positionOnTrackSpline) > track.TrackWidth / 2)
            {
                // To far away from the center of the track (and not shortcutting). Lets respawn.
                StartCoroutine(Respawn());

            }
        }

        private void UpdateTrackPosition()
        {
            if (!isRespawning)
            {
                distanceAlongTrack = track.GetDistanceToSpline(transform.position, out Vector3 positionOnSpline, out Vector3 tangentOnSpline);
                positionOnTrackSpline = positionOnSpline;
                tangentOnTrackSpline = tangentOnSpline.normalized;
            }
        }

        private void FixedUpdate()
        {
            if (!RaceManager.Instance.HasRaceStarted)
            {
                return;
            }
            
            if (!shortcut.IsInShortcut && !isRespawning)
            {
                MovementUpdate();
            }
        }

        private void MovementUpdate()
        {
            scaledFixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
            
            // Whiskers
            whiskerFront = transform.up * forwardsWhiskerLength;
            // Setup Whisker Right
            Vector3 sideWhiskerDirectionRight = Vector3.RotateTowards(transform.up, transform.right, Mathf.Deg2Rad * sideWhiskerAngle, 0);
            whiskerRight = sideWhiskerDirectionRight * sideWhiskerLength;
            //Setup Whisker Left
            Vector3 sideWhiskerDirectionLeft = Vector3.RotateTowards(transform.up, transform.right * -1, Mathf.Deg2Rad * sideWhiskerAngle, 0);
            whiskerLeft = sideWhiskerDirectionLeft * sideWhiskerLength;

            CheckSideWhisker(sideWhiskerDirectionLeft, -1);
            CheckSideWhisker(sideWhiskerDirectionRight, 1);
            AlignWithTrack();
            
            if (sabotage.IsAffectingRacer && currentSpeed > sabotage.MaxSpeed)
            {
                currentSpeed -= sabotage.Deceleration * scaledFixedDeltaTime;
            }
            else if (brake.IsActive && currentSpeed > brake.MaxSpeed)
            {
                currentSpeed -= brake.Deceleration * scaledFixedDeltaTime;
            }
            else if (boost.IsFinishing) // Boost - Finished, so decelerate to max speed.
            {
                currentSpeed -= boost.Deceleration * scaledFixedDeltaTime;
                if (currentSpeed <= maxSpeed)
                {
                    boost.OverrideFinished();
                }
            }
            else // Acceleration
            {
                bool isBoosting = boost.IsActive;
                float accelerationChange = isBoosting ? boost.Acceleration : acceleration;

                if (currentSpeed <= (isBoosting ? boost.MaxSpeed : maxSpeed))
                {
                    currentSpeed += accelerationChange * scaledFixedDeltaTime;
                }
            }
            
            // Execute the move and set the cart to look in the direction of movement.
            float angle = Mathf.Atan2(currentHeading.y, currentHeading.x) * Mathf.Rad2Deg;

            racerRigidbody2d.MovePosition(transform.position + currentHeading * (currentSpeed * scaledFixedDeltaTime));
            racerRigidbody2d.MoveRotation(angle - 90f);

        }

        private void AlignWithTrack()
        {
            currentHeading = Vector3.RotateTowards(currentHeading, tangentOnTrackSpline,
                (Mathf.Deg2Rad * alignmentTurnAnglePerSecond) * scaledFixedDeltaTime,
                0);
        }

        private void CheckSideWhisker(Vector3 whiskerDirection, int side)
        {
            var hits = Physics2D.RaycastAll(transform.position, whiskerDirection, sideWhiskerLength);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("Track"))
                    {
                        // Collided with the track border.
                        currentHeading = Vector3.RotateTowards(currentHeading, transform.right * -side,
                            (Mathf.Deg2Rad * sideWhiskerTurnAnglePerSecond) * scaledFixedDeltaTime,
                            0);
                        return;
                    }
                }
            }
        }
        
        private bool CheckForUnobstructedSideWhisker(Vector3 whiskerDirection, int side)
        {
            bool unobstructed = true;
            var hits = Physics2D.RaycastAll(transform.position, whiskerDirection, sideWhiskerLength);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("Track")) // Potentially add tag check for RacerAi
                    {
                        // Collided with the track border.
                        unobstructed = false;
                        break;
                    }
                }
            }

            if (unobstructed)
            {
                currentHeading = Vector3.RotateTowards(currentHeading, transform.right * side,
                    (Mathf.Deg2Rad * sideWhiskerTurnAnglePerSecond) * scaledFixedDeltaTime,
                    0);
            }

            return unobstructed;

        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (!shortcut.IsInShortcut && !isRespawning && collision.transform.CompareTag("Track"))
            {
                // We've left the track. Time to respawn.
                StartCoroutine(Respawn());
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!shortcut.IsInShortcut && !isRespawning && collision.transform.CompareTag("SabotageObject"))
            {
                // We've hit a sabotage
                CollideWithSabotageObject(collision.gameObject);
            }
        }

        private void CollideWithSabotageObject(GameObject sabotageObject)
        {
            sabotage.StartOverride();
            if (crashSound != null)
            {
                AudioSystem.Instance.PlaySound(crashSound, crashSoundVolume);
            }
        }

        private IEnumerator Respawn()
        {
            isRespawning = true;

            boost.RacerRespawned();
            brake.RacerRespawned();
            
            yield return new WaitForSeconds(respawnStartDelay);

            if (positionOnTrackSpline == Vector3.zero)
            {
                UpdateTrackPosition();
            }

            transform.position = positionOnTrackSpline;

            // Face the splines tangent, and also reset the heading + speed.
            transform.up = tangentOnTrackSpline;
            racerRigidbody2d.velocity = Vector3.zero;
            racerRigidbody2d.angularVelocity = 0f;
            currentHeading = transform.up;
            currentSpeed = 0f;

            respawnTimeRemaining = respawnDuration;
            while (respawnTimeRemaining > 0)
            {
                // If the boost is used while respawning, immediately end the respawn for a recovery.
                if (boost.IsActive)
                {
                    respawnTimeRemaining = 0;
                }
                
                respawnTimeRemaining -= Time.deltaTime;
                yield return null;
            }

            respawnTimeRemaining = 0;

            isRespawning = false;
            
            yield return null;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            if (positionOnTrackSpline == Vector3.zero) positionOnTrackSpline = transform.position;
            Gizmos.DrawSphere(positionOnTrackSpline, positionGizmoRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (currentSpeed * currentHeading));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * forwardsWhiskerLength);
            Gizmos.DrawLine(transform.position , transform.position + whiskerRight);
            Gizmos.DrawLine(transform.position, transform.position + whiskerLeft);
        }

        public void UseActiveCard()
        {
            triggerCard?.UseCard();
        }

        public void DisableCollision()
        {
            racerRigidbody2d.isKinematic = true;
            collider2D.enabled = false;
        }

        public void EnableCollision()
        {
            racerRigidbody2d.isKinematic = false;
            StartCoroutine((EnableCollisionAfterDuration()));
        }

        public void ExitedShortcut(Vector2 heading)
        {
            currentHeading = heading;
            EnableCollision();
        }

        private IEnumerator EnableCollisionAfterDuration()
        {
            yield return new WaitForSeconds(1f);
            collider2D.enabled = true;
        }

        protected bool Equals(RacerBase other)
        {
            if (this.name == null || other.name == null) throw new Exception("racerNames are null");
            return this.name.Equals(other.name);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }
    }
}
