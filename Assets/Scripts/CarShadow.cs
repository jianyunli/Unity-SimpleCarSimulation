using UnityEngine;
using System.Collections;

public class CarShadow : MonoBehaviour
{
    #region Public Members
    public GameObject playerRef;//取得车子
    public float shadowVerticalOffset = -0.5f;
    public float terrainHeight = 0.0f;
    public Vector3 baseOffset;
    #endregion


    void Start()
    {
        baseOffset = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float currOffset = playerRef.transform.position.y - terrainHeight - shadowVerticalOffset;
        gameObject.transform.localPosition = new Vector3(baseOffset.x, -currOffset, baseOffset.z);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, playerRef.transform.rotation.eulerAngles.y, 0.0f);
    }

}