using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Author: Vincent Versnel

public static class TransformExtensions
{
    /// <summary>
    /// Returns the parent highest in the hierarchy.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform GetHighestParent(this Transform transform)
    {
        Transform highest = transform;
        while (highest.parent != null)
            highest = highest.parent;
        return highest;
    }

    /// <summary>
    /// Destroys all the children attached to this transform.
    /// </summary>
    /// <param name="transform"></param>
    public static void DestroyChildren(this Transform transform)
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            children[i] = transform.GetChild(i);
        foreach (var child in children)
#if UNITY_EDITOR
            Object.DestroyImmediate(child.gameObject);
#else
            Object.Destroy(child.gameObject);
#endif
    }

    /// <summary>
    /// Finds the child object with the given name.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindInChildren(this Transform trans, string name)
    {
        if (trans == null)
            return null;

        if (trans.name == name || trans.name == name)
            return trans;
        else
        {
            Transform found;

            for (int i = 0; i < trans.childCount; i++)
            {
                found = FindInChildren(trans.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

}
