using UnityEngine;

public class BackgroundFitter : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // 1. Calculate the exact height and width of the Camera view
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        // 2. Calculate the exact height and width of your background image
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        // 3. Figure out how much we need to scale to fit the screen
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        // 4. Pick the LARGER scale factor. 
        // This ensures the screen is 100% covered without squashing the art!
        float finalScale = Mathf.Max(scaleX, scaleY);

        // 5. Apply the perfect scale
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}