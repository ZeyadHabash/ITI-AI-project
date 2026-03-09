using UnityEngine;

public interface IEnemy
{
    GameObject playerTarget { get; set; }

    void Attack();


    
}
