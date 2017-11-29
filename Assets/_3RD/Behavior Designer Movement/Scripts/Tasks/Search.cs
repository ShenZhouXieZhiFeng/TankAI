using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Search for a target by combining the wander, within hearing range, and the within seeing range tasks using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=10")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SearchIcon.png")]
    public class Search : NavMeshMovement
    {
        [Tooltip("How far ahead of the current position to look ahead for a wander")]
        public SharedFloat wanderDistance = 10;
        [Tooltip("The amount that the agent rotates direction")]
        public SharedFloat wanderRate = 1;
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 30;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("Should the search end if audio was heard?")]
        public SharedBool senseAudio = true;
        [Tooltip("How far away the unit can hear")]
        public SharedFloat hearingRadius = 30;
        [Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The further away a sound source is the less likely the agent will be able to hear it. " +
                 "Set a threshold for the the minimum audibility level that the agent can hear")]
        public SharedFloat audibilityThreshold = 0.05f;
        [Tooltip("The object that is found")]
        public SharedGameObject returnedObject;

        public override void OnStart()
        {
            base.OnStart();
            SetDestination(Target());
        }

        // Keep searching until an object is seen or heard (if senseAudio is enabled)
        public override TaskStatus OnUpdate()
        {
            SetDestination(Target());
            // Detect if any objects are within sight
            returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, ignoreLayerMask);
            // If an object was seen then return success
            if (returnedObject.Value != null) {
                return TaskStatus.Success;
            }
            // Detect if any object are within audio range (if enabled)
            if (senseAudio.Value) {
                returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
                // If an object was heard then return success
                if (returnedObject.Value != null) {
                    return TaskStatus.Success;
                }
            }

            // No object has been seen or heard so keep searching
            return TaskStatus.Running;
        }

        // Return targetPosition if targetTransform is null
        private Vector3 Target()
        {
            // point in a new random direction and then multiply that by the wander distance
            var direction = transform.forward + Random.insideUnitSphere * wanderRate.Value;
            return transform.position + direction.normalized * wanderDistance.Value;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            wanderDistance = 10;
            wanderRate = 1;
            fieldOfViewAngle = 90;
            viewDistance = 30;
            senseAudio = true;
            hearingRadius = 30;
            audibilityThreshold = 0.05f;
        }
    }
}