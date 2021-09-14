using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getMousePos : MonoBehaviour
{

    public GameObject sphere;
    private Camera mainCamera;
    private Vector3 currentPosition = Vector3.zero; //(0,0,0)



    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mainCamera = Camera.main;
        Debug.Log("sphere position: "+ sphere.transform.position);// (0, 0, 0)

        //Vector3 spherePos = new Vector3(sphere.transform.position.x, sphere.transform.position.y, mainCamera.nearClipPlane);
        //Debug.Log("spherePos"+spherePos);
        


    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 worldToCameraMatrix = mainCamera.worldToCameraMatrix;
        Matrix4x4 projectionMatrix = mainCamera.projectionMatrix;
        Matrix4x4 matrix = projectionMatrix * worldToCameraMatrix;
        Vector3 screenPos = matrix.MultiplyPoint(sphere.transform.position);
        // (-1, 1)'s clip => (0 ,1)'s viewport 
        //screenPos = new Vector3(screenPos.x + 1f, screenPos.y + 1f, 0) / 2f;
        Debug.Log("screenPos" + screenPos);
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane);
        //Debug.Log(mousePosition);

        if (Input.GetMouseButton(0))
        {

            currentPosition = mainCamera.ScreenToViewportPoint(mousePosition);
            //Debug.Log(currentPosition);

        }

    }

    void OnDrawGizmos()
    {
        if(currentPosition != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentPosition, 0.5f);

        }
    }
}
