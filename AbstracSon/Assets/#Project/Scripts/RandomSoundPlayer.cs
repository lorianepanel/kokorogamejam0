using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Référence à l'audio source
    public AudioClip[] audioClips; // Tableau d'audio clips

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Jouer un son aléatoire au début
        PlayRandomSound();
    }

    public void PlayRandomSound()
    {
        if (audioClips.Length > 0)
        {
            // Sélectionne un index aléatoire dans le tableau d'audio clips
            int randomIndex = Random.Range(0, audioClips.Length);
            AudioClip randomClip = audioClips[randomIndex];

            // Joue l'audio clip sélectionné
            audioSource.clip = randomClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Aucun audio clip trouvé dans le tableau!");
        }
    }

    // Tu peux aussi appeler cette fonction pour jouer un son aléatoire sur un événement particulier
    void Update()
    {

        PlayRandomSound();
        
    }
}
