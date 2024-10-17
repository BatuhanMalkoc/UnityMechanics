using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform camera;
    [SerializeField] private float jumpForce = 5f; // Zýplama kuvveti
    [SerializeField] private Transform groundCheck; // Zemin kontrol noktasý
    [SerializeField] private LayerMask groundMask;  // Zemin katmaný
    private Rigidbody rb;                           // Rigidbody bileþeni
    private bool isGrounded;
    float xRotation = 0f; // X eksenindeki döngü
    bool isWalking;
    bool isInTouch;
    AudioSource audioSource;
    [SerializeField] private AudioClip[] footSounds;
    private bool playSound = true;
    private int index = 0;
    public Transform blackScreen;
    Vector3 storagedLocation;
    public TextMeshProUGUI ttt;
    bool start = true;
    float number = 5;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>(); // Rigidbody'yi al
        speed = 0;
        jumpForce = 0;
        
        StartCoroutine(waitForStartGame());
    }

    // Update is called once per frame
    void Update()
    {
        MovementPlayer();
        MoveCamera();

        if (start)
        {
          
           if(number > 0)
            {
                number -= Time.deltaTime;
                Debug.Log(number);
                ttt.text =""+ Mathf.RoundToInt(number);
            }

            if (number <= 0)
            {
                start = false;
                ttt.gameObject.SetActive(false);
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    private void StepSound()
    {
     
            AudioSource.PlayClipAtPoint(footSounds[index], transform.position,25);
            index++;
            if(index == footSounds.Length-1)
        {
            index = 0;
        }
    }
 IEnumerator waitForStartGame()
    {
        yield return new  WaitForSeconds(5);
        speed = 10;
        jumpForce = 5;
        blackScreen.gameObject.SetActive(true);
    }
    private void MovementPlayer()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundMask); // Zemin kontrolü

        // Girdi al
        float moveX = Input.GetAxis("Horizontal"); // A/D tuþlarý
        float moveZ = Input.GetAxis("Vertical");   // W/S tuþlarý


         
        // Hareket yönünü belirle
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.Normalize();
        if (playSound)
        {
            StartCoroutine(waitHocam());
            playSound = false;
        }
        // Karakteri hareket ettir
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        // Zýplama kontrolü
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Zýplama kuvveti ekle
        }
    }
    IEnumerator waitHocam()
    {
        storagedLocation = transform.position;
        yield return new WaitForSeconds(0.4f);
        if (Mathf.Abs(Vector3.Magnitude(storagedLocation - transform.position)) > 1f)
        {
            StepSound();
                
        }
        playSound = true;
    }
    private void Jump()
    {


    }
    private void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // X ekseninde döndürme
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Yukarý ve aþaðý döndürme sýnýrý

        // Kamerayý döndür
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // Karakteri döndür

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Stars>(out Stars star)){
            if (star != null) {
                star.DestroyThis();
            }

        }
        if (other.gameObject.CompareTag("Engel"))
        {
            transform.position = Vector3.zero;
        }
    }
}
