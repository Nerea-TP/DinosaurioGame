using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    private MenuController menuController;

    void Start()
    {
        menuController = FindFirstObjectByType<MenuController>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    GameObject touchedObject = hit.collider.gameObject;
                    menuController.HandleTouch(touchedObject);
                }
            }
        }
    }
}
