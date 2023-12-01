using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker : MonoBehaviour
{
    public float followSpeed = 3f;
    public float scrollSpeed = 5f;
    private GameObject selectedObject;
    private Rigidbody selectedRigidbody;
    private Vector3 targetPosition;
    private float distanceToCamera;
    private ConstantForce constantForce;
    public bool scrollEnabled = true;
    public int maxDistance = 20;
    
    public bool isHoldingObject(GameObject obj, bool anyObject)
    {
        if (anyObject)
        {
            return selectedObject != null;
        } else
        return selectedObject == obj;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Use raycastDistance as the max distance for the raycast
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.collider.gameObject.CompareTag("Pickable"))
                {
                    selectedObject = hit.collider.gameObject;
                    selectedRigidbody = selectedObject.GetComponent<Rigidbody>();
                    distanceToCamera = Vector3.Distance(selectedObject.transform.position, Camera.main.transform.position);
                    selectedRigidbody.useGravity = false;
                
                    if(selectedObject.GetComponent<ConstantForce>())
                    {
                        constantForce = selectedObject.GetComponent<ConstantForce>();
                        constantForce.enabled = true;
                    }
                }
            }
        }

        if (selectedObject != null)
        {
            if (scrollEnabled) // Only allow scrolling if scrollEnabled is true
            {
                distanceToCamera += Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;
                distanceToCamera = Mathf.Max(1.0f, distanceToCamera);
            }
        
            targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
            Vector3 moveDirection = (targetPosition - selectedObject.transform.position);
            selectedRigidbody.velocity = moveDirection * followSpeed;
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null)
        {
            selectedRigidbody.useGravity = true;
        
            if(constantForce != null)
                constantForce.enabled = false;

            selectedObject = null;
            selectedRigidbody = null;
        }
    }
}

