using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{


    private void OnTriggerEnter( Collider other )
    {
        PathObject pathObject = other.GetComponent<PathObject>( );

        if ( pathObject )
        {
            pathObject.IncrementTrigger( );
        }
        
    }

    private void OnTriggerExit( Collider other )
    {
        PathObject pathObject = other.GetComponent<PathObject>( );

        if ( pathObject )
        {
            pathObject.DecrementTrigger( );
        }
    }
}
