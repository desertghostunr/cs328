using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{


    public Vector3 size;
    public float detailHeight;
    public float detailWidth;
    public int[,] map;

    public bool OnGrass( )
    {
        int y, x;

        y = ( int ) ( detailHeight * ( ( 1.0f / size.z ) * transform.position.z ) );
        x = ( int ) ( detailHeight * ( ( 1.0f / size.x ) * transform.position.x ) );



        if ( y > map.GetLength( 0 ) || x > map.GetLength( 1 ) )
        {
            return false;
        }

        return map[y, x] != 0;

    }
}
