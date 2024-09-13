using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiceSound : NetworkBehaviour {
    private void OnCollisionEnter(Collision other) {
        // Debug.Log(other.impulse.magnitude);
        
        float volume = Mathf.InverseLerp(0, 200, other.impulse.magnitude);
        
        if (SoundHandler.Instance == null)
            return;
        
        if (other.gameObject.TryGetComponent(out Dice dice)) {
            // SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.DiceHit, transform.position, volume);
            PlayDiceHitSoundClientRpc(volume);
        }
        else {
            // SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.TableHit, transform.position, volume);
            PlayTableHitSoundClientRpc(volume);
        }
    }
    
    [ClientRpc]
    private void PlayDiceHitSoundClientRpc(float volume = 1f) {
        if (SoundHandler.Instance == null)
            return;
        
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.DiceHit, transform.position, volume);
    }
    
    [ClientRpc]
    private void PlayTableHitSoundClientRpc(float volume = 1f) {
        if (SoundHandler.Instance == null)
            return;
        
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.TableHit, transform.position, volume);
    }
}
