using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;



public class manLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    [SerializeField] private AudioSource deathSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Trap"))
        {
            deathSoundEffect.Play();
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // 立刻停住所有速度
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 直接禁用你的移动脚本，从根源禁止操控
        GetComponent<manmove>().enabled = false;

        // 播放死亡动画
        anim.SetTrigger("Death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
