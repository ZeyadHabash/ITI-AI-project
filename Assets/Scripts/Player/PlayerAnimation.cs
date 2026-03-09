using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
            UnityEngine.Debug.Log("Attack");
        }

        // if (Input.GetMouseButtonDown(1))
        // {
        //     anim.SetTrigger("Spell");
        //     UnityEngine.Debug.Log("Spell");
        // }
    }
}
