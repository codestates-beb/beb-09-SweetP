using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : LivingEntity
{
    private static Player _instance;
    public static Player instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<Player>();
            }
            return _instance;
        }
    }

    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;
    // Start is called before the first frame update
    private void Awake()
    {
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        //healthSlider
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    public void ChangeHealthWithWeapon()
    {
        health = WeaponManager.instance.curruentWeaponData.weapon_hp;
        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);

        healthSlider.value = health;
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        if (!dead)
        {
            //hit sound 
            playerAudioPlayer.PlayOneShot(hitClip);
            playerAnimator.SetTrigger("GetHit");
        }
        base.OnDamage(damage, hitPoint, hitDirection);

        healthSlider.value = health;
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();


        healthSlider.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
        playerAudioPlayer.PlayOneShot(deathClip);

        //animator trigger
        playerAnimator.SetTrigger("Die");
    }

}
