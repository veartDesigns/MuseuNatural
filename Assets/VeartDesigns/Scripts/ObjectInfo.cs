using UnityEngine;
using System.Collections;

public class ObjectInfo : MonoBehaviour
{
    public ObjectType ObjectType;
}

public enum ObjectType
{
    Lechuza,
    MandibulaRaton,
    MandibulaLiron,
    MandibulaMusaraña,
    LironCareto,
    Egagropila,
    Musaraña,
    Raton,
    OtrosHuesos
}