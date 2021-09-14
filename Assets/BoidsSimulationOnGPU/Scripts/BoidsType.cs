using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class BoidsType : ScriptableObject
{
    public List<BoidType> boidTypesList = new List<BoidType>();
}
[System.Serializable]
public class BoidType
{


    [SerializeField]
    float cohesionNeighbourhoodRadius = 9;

    [SerializeField]
    float alignmentNeighbourhoodRadius = 2;

    [SerializeField]
    float separateNeighbourhoodRadius = 2;

    [SerializeField]
    float maxSpeed = 6;

    [SerializeField]
    float maxSteerForce = 0.5f;

    [SerializeField]
    float cohesionWeight = 1;

    [SerializeField]
    float alignmentWeight = 1;

    [SerializeField]
    float separateWeight = 3;

    //getMethods



    public float CohesionNeighbourhoodRadius
    {
        get
        {
            return cohesionNeighbourhoodRadius;
        }
    }

    public float AlignmentNeighbourhoodRadius
    {
        get
        {
            return alignmentNeighbourhoodRadius;
        }
    }

    public float SeparateNeighbourhoodRadius
    {
        get
        {
            return separateNeighbourhoodRadius;
        }
    }

    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    public float MaxSteerForce
    {
        get
        {
            return maxSteerForce;
        }
    }


    public float CohesionWeight
    {
        get
        {
            return cohesionWeight;
        }
    }

    public float AlignmentWeight
    {
        get
        {
            return alignmentWeight;
        }
    }

    public float SeparateWeight
    {
        get
        {
            return separateWeight;
        }
    }

}
