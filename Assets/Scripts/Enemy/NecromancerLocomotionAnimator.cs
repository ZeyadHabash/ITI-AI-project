using UnityEngine;
using UnityEngine.AI;

public class NecromancerLocomotionAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private string movingBoolParameter = "IsMoving";
    [SerializeField] private string speedFloatParameter = "MoveSpeed";
    [SerializeField] private float moveThreshold = 0.05f;
    [SerializeField] private float speedLerp = 8f;

    private int movingHash;
    private int speedHash;
    private bool hasMovingParam;
    private bool hasSpeedParam;
    private float currentSpeedValue;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        movingHash = Animator.StringToHash(movingBoolParameter);
        speedHash = Animator.StringToHash(speedFloatParameter);

        hasMovingParam = HasParameter(animator, movingBoolParameter, AnimatorControllerParameterType.Bool);
        hasSpeedParam = HasParameter(animator, speedFloatParameter, AnimatorControllerParameterType.Float);
    }

    private void Update()
    {
        if (animator == null || agent == null)
        {
            return;
        }

        float planarSpeed = new Vector2(agent.velocity.x, agent.velocity.z).magnitude;
        bool isMoving = planarSpeed > moveThreshold && agent.enabled;

        if (hasMovingParam)
        {
            animator.SetBool(movingHash, isMoving);
        }

        if (hasSpeedParam)
        {
            float normalizedSpeed = agent.speed > 0.001f ? Mathf.Clamp01(planarSpeed / agent.speed) : 0f;
            currentSpeedValue = Mathf.Lerp(currentSpeedValue, normalizedSpeed, Time.deltaTime * Mathf.Max(0.01f, speedLerp));
            animator.SetFloat(speedHash, currentSpeedValue);
        }
    }

    private static bool HasParameter(Animator targetAnimator, string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (targetAnimator == null || string.IsNullOrEmpty(parameterName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in targetAnimator.parameters)
        {
            if (parameter.type == parameterType && parameter.name == parameterName)
            {
                return true;
            }
        }

        return false;
    }
}
