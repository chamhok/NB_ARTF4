using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Scripts.Scene;

public class KillZone : MonoBehaviour
{
    public static event Action OnDeath;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
			//col.gameObject.GetComponent<CharacterControls>().LoadCheckPoint();
            col.gameObject.GetComponent<PlayerController>().LoadCheckPoint();
            OnDeath?.Invoke();
        }
	}
}
