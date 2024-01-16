using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTrail : MonoBehaviour
{
    [SerializeField] private float startWidth = 0.2f;
    [SerializeField] private float endWidth = 0f;
    [SerializeField] private float trailTime = 0.1f;
    [SerializeField] private Material trailMaterial;

    Transform trailTransform;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;

        GameObject trailObj = new GameObject("Mouse Trail");
        trailTransform = trailObj.transform;
        TrailRenderer trail = trailObj.AddComponent<TrailRenderer>();
        MoveTrailToCursor(Input.mousePosition);
        trail.time = trailTime;
        trail.startWidth = startWidth;
        trail.endWidth = endWidth;
        trail.numCapVertices = 6;
        trail.sharedMaterial = trailMaterial;
        trail.sortingOrder = 1;
    }

    void Update()
    {
        MoveTrailToCursor(GameInput.Instance.GetMousePosition());
    }

    void MoveTrailToCursor(Vector3 screenPosition)
    {
        trailTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 1));
    }
}