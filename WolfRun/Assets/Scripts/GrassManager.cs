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

        y = GetY();
        x = GetX();

        if ( y > map.GetLength( 0 ) || x > map.GetLength( 1 ) )
        {
            return false;
        }

        return ( map[ y, x ] != 0 );
    }

    public bool DeepInGrass()
    {
        int y, x;

        y = GetY();
        x = GetX();

        if (y > map.GetLength(0) || x > map.GetLength(1))
        {
            return false;
        }

        return !MoreZeroesIn9PixelNeighborhood(x, y);
    }


    public Vector3 GetNearestBarePoint( )
    {
        int iX, iY;

        int y, x, nX = -1, nY = -1;

        float nDist, sDist = Mathf.Infinity;

        y = GetY();
        x = GetX();

        for (iY = Mathf.Max(y - 9, 0); iY < Mathf.Min(map.GetLength(0) - 1, y + 9); iY++)
        {
            for (iX = Mathf.Max(x - 9, 0); iX < Mathf.Min(map.GetLength(1) - 1, x + 9); iX++)
            {
                if( iX == x && iY == y)
                {
                    continue;
                }

                nDist = Vector2.Distance(new Vector2(x, y), new Vector2(iX, iY));

                if( sDist > nDist)
                {
                    sDist = nDist;

                    nX = iX;

                    nY = iY;
                }


            }
        }

        if( nX <= -1 || nY <= -1)
        {
            return new Vector3(-1, -1, -1);
        }

        x = (int)((nX / detailWidth) * size.x);
        y = (int)((nY / detailHeight) * size.z);

        return new Vector3(x, transform.position.y, y );
    }


    private int GetX()
    {
        return (int)(detailWidth * ((1.0f / size.x) * transform.position.x));
    }

    private int GetY()
    {
        return (int)(detailHeight * ((1.0f / size.z) * transform.position.z));
    }

    

    private bool MoreZeroesIn9PixelNeighborhood( int x, int y)
    {
        int iX, iY, zeroCount = 0, itCount = 0;

        for( iY = Mathf.Max( y - 1, 0 ); iY < Mathf.Min(map.GetLength(0) -1, y + 1); iY++ )
        {
            for (iX = Mathf.Max(x - 1, 0); iX < Mathf.Min(map.GetLength(1) - 1, x + 1); iX++)
            {
                if( map[ iY, iX ] == 0)
                {
                    zeroCount++;
                }

                itCount++;
            }
        }

        return ( zeroCount > ( itCount / 2 ) );
    }
}
