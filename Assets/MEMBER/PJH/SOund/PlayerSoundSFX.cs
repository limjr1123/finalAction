using UnityEngine;

public class PlayerSoundSFX : MonoBehaviour
{
    [Header("오디오 클립")]
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip dodgeClip;
    [SerializeField] private AudioClip damagedClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip leftfootstepClip;
    [SerializeField] private AudioClip rightfootstepClip;
    [SerializeField] private AudioClip LevelUpClip; 

    public void PlayAttackSound(int comboCount)
    {
        int index = comboCount - 1;

        if (index >= 0 && index < attackClips.Length && attackClips[index] != null)
        {
            SoundManager.Instance.PlaySkillSFX(attackClips[index]);
        }
    }

    public void PlayJumpSound()
    {
        SoundManager.Instance.PlaySkillSFX(jumpClip);
    }

    public void PlayDodgeSound()
    {
        SoundManager.Instance.PlaySkillSFX(dodgeClip);
    }

    public void PlayDamagedSound()
    {   
        SoundManager.Instance.PlaySkillSFX(damagedClip);
    }

    public void PlayDeathSound()
    {
        SoundManager.Instance.PlaySkillSFX(deathClip);
    }

    public void PlayLeftFootstepSound()
    {
        SoundManager.Instance.PlaySkillSFX(leftfootstepClip);
    }
    public void PlayRightFootstepSound()
    {
        SoundManager.Instance.PlaySkillSFX(rightfootstepClip);
    }

    public void PlayLevelUpSound()
    {
        SoundManager.Instance.PlaySkillSFX(LevelUpClip);
    }
}
