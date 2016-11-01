using UnityEngine;
using System.Collections;

[System.Serializable]
public struct SimpleTransform
{
    public SimpleVector3 position;
    public SimpleQuaternion rotation;
    public SimpleVector3 scale;

    public SimpleTransform(Transform copyFrom)
    {
        position = copyFrom.position;
        rotation = copyFrom.rotation;
        scale = copyFrom.localScale;
    }

    public override string ToString()
    {
        return string.Format("position : {0} rotation : {1} scale {2}", position.ToString(), rotation.ToString(), scale.ToString());
    }
}

[System.Serializable]
public struct SimpleVector3
{
    public float x;
    public float y;
    public float z;

    public Vector3 Vector3Value
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    static public implicit operator SimpleVector3(Vector3 value)
    {
        return new SimpleVector3(value);
    }

    static public implicit operator Vector3(SimpleVector3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    public SimpleVector3(Vector3 copyFrom)
    {
        x = copyFrom.x;
        y = copyFrom.y;
        z = copyFrom.z;
    }

    public override string ToString()
    {
        return Vector3Value.ToString();
    }
}

[System.Serializable]
public struct SimpleQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Quaternion QuaternionValue
    {
        get
        {
            return new Quaternion(x, y, z, w);
        }
    }

    static public implicit operator SimpleQuaternion(Quaternion value)
    {
        return new SimpleQuaternion(value);
    }

    static public implicit operator Quaternion(SimpleQuaternion value)
    {
        return new Quaternion(value.x, value.y, value.z, value.w);
    }

    public SimpleQuaternion(Quaternion copyFrom)
    {
        x = copyFrom.x;
        y = copyFrom.y;
        z = copyFrom.z;
        w = copyFrom.w;
    }

    public override string ToString()
    {
        return QuaternionValue.ToString();
    }
}

[System.Serializable]
public struct SimpleColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public Color ColorValue
    {
        get
        {
            return new Color(r, g, b, a);
        }
    }

    static public implicit operator SimpleColor(Color value)
    {
        return new SimpleColor(value);
    }

    static public implicit operator Color(SimpleColor value)
    {
        return new Color(value.r, value.g, value.b, value.a);
    }

    public SimpleColor(Color copyFrom)
    {
        r = copyFrom.r;
        g = copyFrom.g;
        b = copyFrom.b;
        a = copyFrom.a;
    }

    public override string ToString()
    {
        return ColorValue.ToString();
    }
}


public class CoroutineHelper : MonoBehaviour
{
    public static void ShowDebug(string message)
    {
        Debug.Log(message);
    }
}

class WaitForRealSeconds : CustomYieldInstruction
{
    readonly float time;
    readonly float entryTime;

    public override bool keepWaiting { get { return Time.unscaledTime - entryTime < time; } }

    public WaitForRealSeconds(float f)
    {
        time = f;
        entryTime = Time.unscaledTime;
    }
}

[System.Serializable]
public struct InputResponse
{
    public bool Response;
}

[System.Serializable]
public class UnityEventBool : UnityEngine.Events.UnityEvent<bool>
{

}
