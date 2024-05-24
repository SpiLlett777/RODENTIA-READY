using UnityEditor;
using UnityEngine;

public class ChildMeleeAttack : MonoBehaviour
{
    private MeleeAttack meleeAttack;
    private PersonMovement personMovement;
    private void Awake()
    {
        meleeAttack = GetComponentInParent<MeleeAttack>();
        personMovement = GetComponentInParent<PersonMovement>();

    }
    public void ResetAttack()
    {
        meleeAttack.ResetAttack();
    }
    public void EnableMovement()
    {
        personMovement.EnableMovement();
    }
    public void DisableMovement()
    {
        personMovement.DisableMovement();
    }
}
