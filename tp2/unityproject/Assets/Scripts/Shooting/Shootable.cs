using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shootable : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public bool recoversHealth = false;
    public float healthRecoveryRate = 5f;
    public float recoveryCooldown = 4f;
    public float currentRecoveryTime = 0f;
    public GameObject bloodPanel;
    private Image bloodImage;
    private float bloodAlpha = 0f;

    [Header("UI")]

    public bool showInUI = false;
    public ProgressBar healthBar;

    [Header("Animations")]

    private bool isDead = false;
    private Animator characterAnimator;
    public ParticleSystem bloodParticles;

    [Header("Sounds")]

    public bool playGlobalSound = false;
    private AudioManager audioManager;

    [Header("Events")]
    public bool emitPlayerDeath = false;
    public bool emitEnemyDeath = false;

    void Awake()
    {
        this.bloodParticles.Stop();
        this.characterAnimator = GetComponent<Animator>();
        this.audioManager = GetComponent<AudioManager>();
        // In case we show health in UI
        if (this.showInUI)
        {
            this.healthBar.SetMaxValue(this.currentHealth);
        }
        if (this.recoversHealth)
        {
            this.currentRecoveryTime = this.recoveryCooldown;
            this.bloodImage = bloodPanel.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (!this.isDead)
        {
            if (this.recoversHealth)
            {
                this.currentRecoveryTime += Time.deltaTime;
                this.RecoverHealth();
            }
        }
    }

    public void ApplyDamage(int amount)
    {
        if (!this.isDead)
        {
            // Play sounds
            this.SetShotSounds();
            // Remove damage amount from health
            this.currentHealth = Mathf.Ceil(this.currentHealth - amount);
            this.bloodParticles.Play();
            // Check if alter UI
            if (this.showInUI)
            {
                this.healthBar.SetValue(this.currentHealth);
                // Set the blood level
                this.SetBloodLevel();
            }
            // Check if health has fallen below zero
            if (this.currentHealth <= 0)
            {
                // Play animation before marking to avoid race condition errors
                this.SetDeath();
                this.isDead = true;
            }
            // Mark the hit if it recovers health
            this.currentRecoveryTime = 0f;
        }
    }

    private void SetDeath()
    {
        if (this.characterAnimator != null) this.characterAnimator.SetBool("Death_b", true);
        if (this.emitPlayerDeath) FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnPlayerDeath.notifier);
        if (this.emitEnemyDeath) FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnEnemyDeath.notifier);
        this.SetDeadSound();
    }

    public bool IsDead()
    {
        return this.isDead;
    }

    private void RecoverHealth()
    {
        if (this.currentRecoveryTime >= this.recoveryCooldown)
        {
            this.currentHealth = Mathf.Clamp(this.currentHealth + (Time.deltaTime * this.healthRecoveryRate), 0, this.maxHealth);
            // Check if alter UI
            if (this.showInUI)
            {
                this.healthBar.SetValue(this.currentHealth);
                // Set the blood level
                this.SetBloodLevel();
            }
        }
    }

    private void SetBloodLevel()
    {
        // Set the blood level
        if (this.bloodImage != null)
        {
            // this.bloodAlpha = Mathf.Clamp((Mathf.Log10((-1 * this.currentHealth) + this.maxHealth))/2f, 0f, 1f);
            this.bloodAlpha = Mathf.Clamp(1 - (this.currentHealth / this.maxHealth), 0f, 1f);
            this.bloodImage.color = new Color(this.bloodImage.color.r, this.bloodImage.color.g, this.bloodImage.color.b, this.bloodAlpha);
        }
    }

    private void SetShotSounds()
    {
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Play(Sounds.AUDIO_TYPE.ENTITY_GRUNT);
        }
        else
        {
            this.audioManager.Play(Sounds.AUDIO_TYPE.ENTITY_GRUNT);
        }
    }

    private void SetDeadSound()
    {
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_WALK);
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_RUN);
        }
        else
        {
            this.audioManager.Stop(Sounds.AUDIO_TYPE.ENTITY_WALK);
        }
    }
}
