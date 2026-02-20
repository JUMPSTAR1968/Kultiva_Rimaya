using UnityEngine;

public enum DuckState { Normal, Dodging, Cooldown }

public class MotherDuckAI : MonoBehaviour
{
    [Header("Detection")]
    public float circleRadius = 0.45f;
    public float lookAheadDistance = 2.5f;
    public LayerMask obstacleLayer;

    [Header("Movement & Borders")]
    public float moveSpeed = 6f;
    public float laneDistance = 1.2f;
    public float topBorder = 1.3f;
    public float bottomBorder = -1.5f; // ENSURE THIS IS NEGATIVE IN INSPECTOR

    [Header("Anti-Twitch Timing")]
    public float cooldownDuration = 0.5f;

    private DuckState currentState = DuckState.Normal;
    private float timer = 0f;
    private float targetY;
    private float fixedX;
    private Vector2 originalPos;

    void Start()
    {
        fixedX = transform.position.x;
        targetY = transform.position.y;
        originalPos = this.transform.position;
    }

    void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;

        switch (currentState)
        {
            case DuckState.Normal:
                HandleObstacleDetection();
                break;
            case DuckState.Dodging:
                if (Mathf.Abs(transform.position.y - targetY) < 0.05f)
                {
                    currentState = DuckState.Cooldown;
                    timer = cooldownDuration;
                }
                break;
            case DuckState.Cooldown:
                float SlerpOgY = Mathf.MoveTowards(transform.position.y, originalPos.y, moveSpeed * Time.deltaTime);
                this.transform.position = new Vector2(fixedX, SlerpOgY);
                if (timer <= 0 && transform.position.y == originalPos.y) 
                {
                    currentState = DuckState.Normal;
                    targetY = originalPos.y;
                };    
                
                break;
            
        }

        // Apply movement
        float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(fixedX, newY, 0);
    }

    void HandleObstacleDetection()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleRadius, Vector2.right, lookAheadDistance, obstacleLayer);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            float yDir = 0;

            // 1. Check if we are too close to the Top Border
            if (transform.position.y >= topBorder - 0.3f)
            {
                yDir = -1f; // Force Down
            }
            // 2. Check if we are too close to the Bottom Border
            else if (transform.position.y <= bottomBorder + 0.3f)
            {
                yDir = 1f; // Force Up
            }
            // 3. If in the middle, decide based on where the rock is
            else
            {
                // If hit normal is neutral, we check our Y position
                // If we are above 0, go Down. If we are below 0, go Up.
                yDir = (transform.position.y > 0) ? -1f : 1f;
            }

            targetY = transform.position.y + (yDir * laneDistance);
            // Safety Clamp: Never let targetY go outside the borders
            targetY = Mathf.Clamp(targetY, bottomBorder, topBorder);

            currentState = DuckState.Dodging;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(fixedX - 2, topBorder, 0), new Vector3(fixedX + 2, topBorder, 0));
        Gizmos.DrawLine(new Vector3(fixedX - 2, bottomBorder, 0), new Vector3(fixedX + 2, bottomBorder, 0));
    }
}