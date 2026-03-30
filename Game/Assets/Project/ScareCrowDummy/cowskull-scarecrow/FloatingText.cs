using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private Camera playerMainCamera;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, 0); 
    private Transform textFollowObject;

    private void Start()
    {
        playerMainCamera = Camera.main; 
        textFollowObject = transform.parent;     
    } 

    private void LateUpdate()
    {
        if (textFollowObject == null || playerMainCamera == null)
            return;

        transform.position = textFollowObject.position + _offset;

        Vector3 directionToCamera = playerMainCamera.transform.position - transform.position;
         
        directionToCamera.y = 0; 

        if (directionToCamera != Vector3.zero)
        { 
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }
}
