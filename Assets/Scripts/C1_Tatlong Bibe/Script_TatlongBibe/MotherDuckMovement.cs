using UnityEngine;

public enum DuckState { Normal, Dodging, Cooldown }

public class MotherDuckAI : MonoBehaviour
{
    [Header("Detection")]
    public float circleRadius = 0.45f;
    public float lookAheadDistance = 1.5f;
    public LayerMask obstacleLayer;

    [Header("Movement & Borders")]
    public float dodgeSpeed = 8f;
    public float returnSpeed = 3f;
    public float laneDistance = 1.2f;
    public float topBorder = 1.3f;
    public float bottomBorder = -1.5f;

    [Header("Passing Clearance")]
    public float safePassDistance = 1.0f;

    private DuckState currentState = DuckState.Normal;
    private float targetY;
    private float fixedX;
    private Vector2 originalPos;

    private Transform currentObstacle;

    void Start()
    {
        fixedX = transform.position.x;
        targetY = transform.position.y;
        originalPos = this.transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case DuckState.Normal:
                HandleObstacleDetection();
                break;

            case DuckState.Dodging:
                if (Mathf.Abs(transform.position.y - targetY) < 0.05f)
                {
                    currentState = DuckState.Cooldown;
                }
                break;

            case DuckState.Cooldown:
                bool isObstacleGone = (currentObstacle == null);
                bool hasObstaclePassed = false;

                if (!isObstacleGone)
                {
                    hasObstaclePassed = currentObstacle.position.x < (transform.position.x - safePassDistance);
                }

                if (isObstacleGone || hasObstaclePassed)
                {
                    targetY = originalPos.y;

                    if (Mathf.Abs(transform.position.y - originalPos.y) < 0.05f)
                    {
                        currentState = DuckState.Normal;
                    }
                }
                break;
        }

        float currentSpeed = (targetY == originalPos.y) ? returnSpeed : dodgeSpeed;
        float newY = Mathf.MoveTowards(transform.position.y, targetY, currentSpeed * Time.deltaTime);
        transform.position = new Vector3(fixedX, newY, 0);
    }

    void HandleObstacleDetection()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleRadius, Vector2.right, lookAheadDistance, obstacleLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.CompareTag("Obstacle"))
            {
                float yDir = 0;

                // 1. If too close to top border, force DOWN
                if (transform.position.y >= topBorder - 0.3f)
                {
                    yDir = -1f;
                }
                // 2. If too close to bottom border, force UP
                else if (transform.position.y <= bottomBorder + 0.3f)
                {
                    yDir = 1f;
                }
                // 3. THE FIX: If in the safe middle area, pick UP or DOWN randomly!
                else
                {
                    // This is a 50/50 coin flip. Random.value gives a number between 0.0 and 1.0.
                    yDir = (Random.value > 0.5f) ? 1f : -1f;
                }

                targetY = transform.position.y + (yDir * laneDistance);
                targetY = Mathf.Clamp(targetY, bottomBorder, topBorder);

                currentObstacle = hit.collider.transform;
                currentState = DuckState.Dodging;
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(fixedX - 2, topBorder, 0), new Vector3(fixedX + 2, topBorder, 0));
        Gizmos.DrawLine(new Vector3(fixedX - 2, bottomBorder, 0), new Vector3(fixedX + 2, bottomBorder, 0));

        Gizmos.color = Color.red;
        float drawX = Application.isPlaying ? fixedX : transform.position.x;
        Gizmos.DrawLine(new Vector3(drawX, transform.position.y, 0), new Vector3(drawX + lookAheadDistance, transform.position.y, 0));
    }
}