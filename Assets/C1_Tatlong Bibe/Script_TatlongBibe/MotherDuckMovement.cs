using UnityEngine;

public class MotherDuckAI : MonoBehaviour
{
    [Header("Detection")]
    public Vector2 boxSize = new Vector2(1f, 1f); // Narrow vision is key!
    public float lookAheadDistance = 4f; // Don't look too far ahead
    public LayerMask obstacleLayer;
    public float moveSpeed = 10f; // Faster move prevents getting 'caught'

    [Header("River Setup")]
    public float riverTopY = 1.32f;
    public float riverBottomY = -2.6f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float fixedX;

    void Start()
    {
        fixedX = transform.position.x;
        targetPosition = transform.position;
    }

    void Update()
    {
        // 1. ARRIVAL CHECK
        // If we are close to the target, stop moving and allow new detections
        if (Mathf.Abs(transform.position.y - targetPosition.y) < 0.05f)
        {
            isMoving = false;
        }

        // 2. DETECTION (Only if NOT currently moving)
        if (!isMoving)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.right, lookAheadDistance, obstacleLayer);

            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                isMoving = true; // Lock into a move

                // Switch to the OTHER lane and STAY THERE
                float newY = (transform.position.y > (riverTopY + riverBottomY) / 2) ? riverBottomY : riverTopY;
                targetPosition = new Vector3(fixedX, newY, 0);
            }
        }

        // 3. ACTUAL MOVEMENT
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // REMEMBER: ONLY ONE OF THESE!
    void OnDrawGizmos()
    {
        Gizmos.color = isMoving ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * lookAheadDistance);
    }
}