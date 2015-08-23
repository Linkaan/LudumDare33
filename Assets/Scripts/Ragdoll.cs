using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {

    public Ragdoll[] children;

    public void InstantiateRagdoll(Rigidbody parent)
    {
        Rigidbody rigidbody = this.gameObject.AddComponent<Rigidbody>() as Rigidbody;
        rigidbody.mass = 0.25f;
        if (parent != null)
        {
            (this.gameObject.AddComponent<CharacterJoint>() as CharacterJoint).connectedBody = parent;
        }

        if (children.Length > 0)
        {
            foreach (Ragdoll ragdoll in children)
            {
                if(ragdoll != null) ragdoll.InstantiateRagdoll(rigidbody);
            }
        }
    }
}
